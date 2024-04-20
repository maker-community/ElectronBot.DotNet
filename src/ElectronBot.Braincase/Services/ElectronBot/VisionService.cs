using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Helpers;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;
using Microsoft.AI.MachineLearning;
using Models.ElectronBot;
using Vedure.Braincsse.WinUI.Helpers;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.FaceAnalysis;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Services.ElectronBot;
public class VisionService
{
    private CameraHelper _cameraHelper = new();

    private bool _taskRunning = false;

    private SoftwareBitmap _backBuffer;

    private readonly PoseCpuSolution _calculator = new(modelComplexity: 2, smoothLandmarks: false);

    private bool _isProcessing = false;

    public event EventHandler<VisionResult>? SoftwareBitmapFramePoseAndHandsPredictResult;

    #region 表情识别
    private LearningModel _model = null;
    private LearningModelDevice _device;
    private LearningModelSession _session;
    private TensorFeatureDescriptor _inputImageDescriptor;
    private TensorFeatureDescriptor _outputTensorDescriptor;
    private LearningModelBinding _binding;
    private FaceDetector _faceDetector;
    #endregion

    #region 手势识别
    private string _modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\MLModel1.zip";
    private HandsCpuSolution _handsCalculator = new();
    #endregion

    private static VisionService? _current;
    public static VisionService Current => _current ??= new VisionService();

    public async Task StartAsync(bool onlyLoadModel = true)
    {
        if (onlyLoadModel)
        {
            #region 初始化表情识别
            var modelLoaded = await LoadModelAsync();
            if (modelLoaded == true)
            {
                _faceDetector = await FaceDetector.CreateAsync();
            }
            #endregion
        }
        else
        {
            #region 初始化表情识别
            var modelLoaded = await LoadModelAsync();
            if (modelLoaded == true)
            {
                _faceDetector = await FaceDetector.CreateAsync();
            }
            #endregion

            var availableFrameSourceGroups = await CameraHelper.GetFrameSourceGroupsAsync();
            if (availableFrameSourceGroups != null)
            {
                _cameraHelper.FrameSourceGroup = availableFrameSourceGroups.First();

                var result = await _cameraHelper.InitializeAndStartCaptureAsync();

                // Camera Initialization succeeded
                if (result == CameraHelperResult.Success)
                {
                    // Subscribe to get frames as they arrive
                    _cameraHelper.FrameArrived += CameraHelper_FrameArrived;
                }
            }

        }

    }
    public CameraHelper CameraHelper
    {
        get => _cameraHelper;
        set => _cameraHelper = value;
    }


    public async Task StopAsync()
    {
        _cameraHelper.FrameArrived -= CameraHelper_FrameArrived;
        await _cameraHelper.CleanUpAsync();

        _current = null;
        _model.Dispose();
        _session.Dispose();
        _model = null;
        _session = null;
    }


    /// <summary>
    /// 摄像头帧触发事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void CameraHelper_FrameArrived(object? sender, FrameEventArgs e)
    {
        //Debug.WriteLine($"frame arrived--{DateTime.Now.Ticks}");
        // Gets the current video frame
        var currentVideoFrame = e.VideoFrame;

        // Gets the software bitmap image
        var softwareBitmap = currentVideoFrame.SoftwareBitmap;

        if (softwareBitmap is not null)
        {
            if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                softwareBitmap = SoftwareBitmap.Convert(
                softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }

            // Swap the processed frame to _backBuffer and dispose of the unused image.
            softwareBitmap = Interlocked.Exchange(ref _backBuffer, softwareBitmap);
            softwareBitmap?.Dispose();

            // Don't let two copies of this task run at the same time.
            if (_taskRunning)
            {
                return;
            }
            _taskRunning = true;

            // Keep draining frames from the backbuffer until the backbuffer is empty.
            SoftwareBitmap latestBitmap;
            while ((latestBitmap = Interlocked.Exchange(ref _backBuffer, null)) != null)
            {
                using IRandomAccessStream stream = new InMemoryRandomAccessStream();

                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;

                if (CameraHelper.FrameSourceGroup != null && CameraHelper.FrameSourceGroup.DisplayName.EndsWith("Cam"))
                {
                    encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise270Degrees;
                }

                // Set the software bitmap
                encoder.SetSoftwareBitmap(latestBitmap);

                await encoder.FlushAsync();

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                var softwareBitmapInput = await decoder.GetSoftwareBitmapAsync(latestBitmap.BitmapPixelFormat, latestBitmap.BitmapAlphaMode);
                ///姿态识别坐标点
                var poseOutput = await PoseAndHandsPredictResultUnUseQueueAsync(softwareBitmapInput);

                Debug.WriteLine("hands: " + poseOutput.Item2);

                //表情识别
                var emojis = await EmotionClassificationAsync(softwareBitmapInput);

                var result = new VisionResult
                {
                    PoseOutput = poseOutput.Item1,
                    HandResult = poseOutput.Item2,
                    Emoji = emojis.Item1,
                    FaceSoftwareBitmap = emojis.Item2,
                    Height = latestBitmap.PixelHeight,
                    Width = latestBitmap.PixelWidth,
                };

                if (poseOutput.Item1 != null)
                {
                    result.PoseLandmarks = poseOutput.Item1.PoseLandmarks;
                }

                SoftwareBitmapFramePoseAndHandsPredictResult?.Invoke(this, result);

                latestBitmap?.Dispose();
                softwareBitmapInput?.Dispose();
            }

            _taskRunning = false;
            currentVideoFrame?.Dispose();
        }
    }


    /// <summary>
    /// 表情分类
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public async Task<(Emoji?, SoftwareBitmap?)> EmotionClassificationAsync(SoftwareBitmap e)
    {
        if (e != null && _session != null)
        {
            BitmapPixelFormat bpf = e.BitmapPixelFormat;
            var uncroppedBitmap = SoftwareBitmap.Convert(e, BitmapPixelFormat.Nv12);
            var faces = await _faceDetector.DetectFacesAsync(uncroppedBitmap);
            if (faces.Count > 0)
            {
                //crop image to focus on face portion
                var faceBox = faces[0].FaceBox;

                VideoFrame inputFrame = VideoFrame.CreateWithSoftwareBitmap(e);

                VideoFrame tmp = null;

                tmp = new VideoFrame(e.BitmapPixelFormat, (int)(faceBox.Width + faceBox.Width % 2) - 2, (int)(faceBox.Height + faceBox.Height % 2) - 2);

                await inputFrame.CopyToAsync(tmp, new BitmapBounds(faceBox.X - 25, faceBox.Y - 25, faceBox.Width + 50, faceBox.Height + 50), null);

                //crop image to fit model input requirements
                VideoFrame croppedInputImage = new VideoFrame(BitmapPixelFormat.Gray8, (int)_inputImageDescriptor.Shape[3], (int)_inputImageDescriptor.Shape[2]);
                var srcBounds = GetCropBounds(
                    tmp.SoftwareBitmap.PixelWidth,
                    tmp.SoftwareBitmap.PixelHeight,
                    croppedInputImage.SoftwareBitmap.PixelWidth,
                    croppedInputImage.SoftwareBitmap.PixelHeight);

                await tmp.CopyToAsync(croppedInputImage, srcBounds, null);

                ImageFeatureValue imageTensor = ImageFeatureValue.CreateFromVideoFrame(croppedInputImage);

                TensorFloat outputTensor = TensorFloat.Create(_outputTensorDescriptor.Shape);

                // Bind inputs + outputs
                _binding.Bind(_inputImageDescriptor.Name, imageTensor);
                _binding.Bind(_outputTensorDescriptor.Name, outputTensor);

                // Evaluate results
                var results = await _session.EvaluateAsync(_binding, new Guid().ToString());

                Debug.WriteLine("ResultsEvaluated: " + results.ToString());

                var outputTensorList = outputTensor.GetAsVectorView();
                var resultsList = new List<float>(outputTensorList.Count);
                for (var i = 0; i < outputTensorList.Count; i++)
                {
                    resultsList.Add(outputTensorList[i]);
                }

                var softMaxexOutputs = SoftMax(resultsList);

                double maxProb = 0;
                var maxIndex = 0;

                // Comb through the evaluation results
                for (var i = 0; i < Constants.POTENTIAL_EMOJI_NAME_LIST.Count(); i++)
                {
                    // Record the dominant emotion probability & its location
                    if (softMaxexOutputs[i] > maxProb)
                    {
                        maxIndex = i;
                        maxProb = softMaxexOutputs[i];
                    }
                }

                // For evaluations run on the MainPage, update the emoji carousel
                if (maxProb >= Constants.CLASSIFICATION_CERTAINTY_THRESHOLD)
                {
                    return (CurrentEmojis._emojis.Emojis[maxIndex], tmp.SoftwareBitmap);
                }
            }
        }

        return (null, null);
    }

    /// <summary>
    /// 姿态和手势识别方法
    /// </summary>
    /// <param name="softwareBitmap"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<(PoseOutput?, string?)> PoseAndHandsPredictResultUnUseQueueAsync(
           SoftwareBitmap? softwareBitmap, CancellationToken cancellationToken = default)
    {
        if (_isProcessing)
        {
            return (null, null);
        }
        try
        {
            if (softwareBitmap != null)
            {
                _isProcessing = true;

                using IRandomAccessStream stream = new InMemoryRandomAccessStream();

                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(softwareBitmap);

                await encoder.FlushAsync();

                using var image = new Bitmap(stream.AsStream());

                var matData = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

                var dataMeta = mat2.Data;

                var length = mat2.Width * mat2.Height * mat2.Channels();

                var data = new byte[length];

                Marshal.Copy(dataMeta, data, 0, length);

                var widthStep = (int)mat2.Step();

                using var imgFrame = new Mediapipe.Net.Framework.Format.ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

                using var imageHandsFrame = new Mediapipe.Net.Framework.Format.ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

                var postOutput = _calculator.Compute(imgFrame);

                var handsOutput = _handsCalculator.Compute(imageHandsFrame);

                if (handsOutput.MultiHandLandmarks != null)
                {
                    var landmarks = handsOutput.MultiHandLandmarks[0].Landmark;

                    var result = HandDataFormatHelper.PredictResult(landmarks.ToList(), _modelPath!);

                    _isProcessing = false;
                    return (postOutput, result);
                }
                _isProcessing = false;
                return (postOutput, null);
            }
        }
        catch (Exception)
        {
            _isProcessing = false;
        }

        return (null, null);
    }


    /// <summary>
    /// 加载表情识别模型
    /// </summary>
    /// <returns></returns>
    private async Task<bool> LoadModelAsync()
    {
        var modelStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(Constants.MODEL_PATH));

        try
        {
            _model = await LearningModel.LoadFromStorageFileAsync(modelStorageFile);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        // since we do not specify the device, we are using the default CPU option
        _session = new LearningModelSession(_model);

        List<ILearningModelFeatureDescriptor> inputFeatures;
        List<ILearningModelFeatureDescriptor> outputFeatures;

        if (_model.InputFeatures == null)
        {
            return false;
        }
        else
        {
            inputFeatures = _model.InputFeatures.ToList();
        }

        if (_model.OutputFeatures == null)
        {
            return false;
        }
        else
        {
            outputFeatures = _model.OutputFeatures.ToList();
        }

        _inputImageDescriptor =
            inputFeatures.FirstOrDefault(feature => feature.Kind == LearningModelFeatureKind.Tensor) as TensorFeatureDescriptor;

        _outputTensorDescriptor =
            outputFeatures.FirstOrDefault(feature => feature.Kind == LearningModelFeatureKind.Tensor) as TensorFeatureDescriptor;

        _binding = new LearningModelBinding(_session);
        return true;
    }

    //WinML team function
    private List<float> SoftMax(List<float> inputs)
    {
        List<float> inputsExp = new List<float>();
        float inputsExpSum = 0;
        for (int i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            inputsExp.Add((float)Math.Exp(input));
            inputsExpSum += inputsExp[i];
        }
        inputsExpSum = inputsExpSum == 0 ? 1 : inputsExpSum;
        for (int i = 0; i < inputs.Count; i++)
        {
            inputsExp[i] /= inputsExpSum;
        }
        return inputsExp;
    }

    public static BitmapBounds GetCropBounds(int srcWidth, int srcHeight, int targetWidth, int targetHeight)
    {
        var modelHeight = targetHeight;
        var modelWidth = targetWidth;
        BitmapBounds bounds = new BitmapBounds();
        // we need to recalculate the crop bounds in order to correctly center-crop the input image
        float flRequiredAspectRatio = (float)modelWidth / modelHeight;

        if (flRequiredAspectRatio * srcHeight > (float)srcWidth)
        {
            // clip on the y axis
            bounds.Height = (uint)Math.Min((srcWidth / flRequiredAspectRatio + 0.5f), srcHeight);
            bounds.Width = (uint)srcWidth;
            bounds.X = 0;
            bounds.Y = (uint)(srcHeight - bounds.Height) / 2;
        }
        else // clip on the x axis
        {
            bounds.Width = (uint)Math.Min((flRequiredAspectRatio * srcHeight + 0.5f), srcWidth);
            bounds.Height = (uint)srcHeight;
            bounds.X = (uint)(srcWidth - bounds.Width) / 2; ;
            bounds.Y = 0;
        }
        return bounds;
    }
}
