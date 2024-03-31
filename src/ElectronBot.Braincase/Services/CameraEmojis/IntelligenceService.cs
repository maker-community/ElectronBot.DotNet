// Copyright (c) Microsoft Corporation. 
// Licensed under the MIT license. 

using System.Diagnostics;
using Microsoft.AI.MachineLearning;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.FaceAnalysis;
using Windows.Storage;

namespace ElectronBot.Braincase.Services;
public class IntelligenceService
{
    public event EventHandler IntelligenceServiceProcessingCompleted;
    public event EventHandler<ClassifiedEmojiEventArgs> IntelligenceServiceEmotionClassified;

    public event EventHandler<SoftwareBitmapEventArgs> FaceBoxFrameCaptured;

    private bool _isInitialized = false;
    readonly DateTime _lastPositiveIdentification = DateTime.Now;
    private FaceDetector _faceDetector;

    private IntelligenceService()
    {
    }

    private static IntelligenceService _current;

    public static IntelligenceService Current => _current ??= new IntelligenceService();

    private LearningModel _model = null;
    private LearningModelDevice _device;
    private LearningModelSession _session;
    private TensorFeatureDescriptor _inputImageDescriptor;
    private TensorFeatureDescriptor _outputTensorDescriptor;
    private LearningModelBinding _binding;

    public async Task<bool> InitializeAsync()
    {
        var modelLoaded = await LoadModelAsync();

        if (modelLoaded == true)
        {
            _faceDetector = await FaceDetector.CreateAsync();

            //CameraFrameService.Current.SoftwareBitmapFrameCaptured += Current_SoftwareBitmapFrameCaptured;

            _isInitialized = true;
        }
        return modelLoaded;
    }

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

        return true;
    }

    public void CleanUp()
    {
        if (!_isInitialized)
        {
            return;
            //throw new InvalidOperationException("Service not initialized.");
        }

        CameraService.Current.SoftwareBitmapFrameCaptured -= Current_SoftwareBitmapFrameCaptured;

        _current = null;
        _model.Dispose();
        _session.Dispose();
        _model = null;
        _session = null;

        Debug.WriteLine("The evaluation event handler should have been removed");
        _isInitialized = false;
    }

    public event EventHandler<EmotionPageGaugeScoreEventArgs> ScoreUpdated;


    public async Task EmotionClassificationAsync(SoftwareBitmap e)
    {
        if (_session != null)
        {
            Debug.WriteLine("FrameCaptured");
            Debug.WriteLine($"Frame evaluation started {DateTime.Now}");

            VideoFrame inputFrame = VideoFrame.CreateWithSoftwareBitmap(e);

            ImageFeatureValue imageTensor = ImageFeatureValue.CreateFromVideoFrame(inputFrame);

            _binding = new LearningModelBinding(_session);

            TensorFloat outputTensor = TensorFloat.Create(_outputTensorDescriptor.Shape);
            List<float> _outputVariableList = new List<float>();

            // Bind inputs + outputs
            _binding.Bind(_inputImageDescriptor.Name, imageTensor);
            _binding.Bind(_outputTensorDescriptor.Name, outputTensor);

            // Evaluate results
            var results = await _session.EvaluateAsync(_binding, new Guid().ToString());

            Debug.WriteLine("ResultsEvaluated: " + results.ToString());

            var outputTensorList = outputTensor.GetAsVectorView();
            var resultsList = new List<float>(outputTensorList.Count);
            for (int i = 0; i < outputTensorList.Count; i++)
            {
                resultsList.Add(outputTensorList[i]);
            }

            var softMaxexOutputs = SoftMax(resultsList);

            double maxProb = 0;
            int maxIndex = 0;

            // Comb through the evaluation results
            for (int i = 0; i < Constants.POTENTIAL_EMOJI_NAME_LIST.Count(); i++)
            {
                // Record the dominant emotion probability & its location
                if (softMaxexOutputs[i] > maxProb)
                {
                    maxIndex = i;
                    maxProb = softMaxexOutputs[i];
                }
            }

            Debug.WriteLine($"Probability = {maxProb}, Threshold set to = {Constants.CLASSIFICATION_CERTAINTY_THRESHOLD}, Emotion = {Constants.POTENTIAL_EMOJI_NAME_LIST[maxIndex]}");

            // For evaluations run on the MainPage, update the emoji carousel
            if (maxProb >= Constants.CLASSIFICATION_CERTAINTY_THRESHOLD)
            {
                Debug.WriteLine("first page emoji should start to update");
                IntelligenceServiceEmotionClassified?.Invoke(this, new ClassifiedEmojiEventArgs(CurrentEmojis._emojis.Emojis[maxIndex]));
            }

            // Dispose of resources
            //if (e != null)
            //{
            //    e.Dispose();
            //    e = null;
            //}
            IntelligenceServiceProcessingCompleted?.Invoke(this, null);
            Debug.WriteLine($"Frame evaluation finished {DateTime.Now}");
        }
    }


    private async void Current_SoftwareBitmapFrameCaptured(object? sender, SoftwareBitmapEventArgs e)
    {
        Debug.WriteLine("FrameCaptured");
        Debug.WriteLine($"Frame evaluation started {DateTime.Now}");
        if (e.SoftwareBitmap != null)
        {
            BitmapPixelFormat bpf = e.SoftwareBitmap.BitmapPixelFormat;

            var uncroppedBitmap = SoftwareBitmap.Convert(e.SoftwareBitmap, BitmapPixelFormat.Nv12);
            var faces = await _faceDetector.DetectFacesAsync(uncroppedBitmap);

            if (faces.Count > 0)
            {
                //crop image to focus on face portion
                var faceBox = faces[0].FaceBox;

                VideoFrame inputFrame = VideoFrame.CreateWithSoftwareBitmap(e.SoftwareBitmap);

                VideoFrame tmp = null;

                tmp = new VideoFrame(e.SoftwareBitmap.BitmapPixelFormat, (int)(faceBox.Width + faceBox.Width % 2) - 2, (int)(faceBox.Height + faceBox.Height % 2) - 2);

                await inputFrame.CopyToAsync(tmp, faceBox, null);

                //crop image to fit model input requirements
                VideoFrame croppedInputImage = new VideoFrame(BitmapPixelFormat.Gray8, (int)_inputImageDescriptor.Shape[3], (int)_inputImageDescriptor.Shape[2]);
                var srcBounds = GetCropBounds(
                    tmp.SoftwareBitmap.PixelWidth,
                    tmp.SoftwareBitmap.PixelHeight,
                    croppedInputImage.SoftwareBitmap.PixelWidth,
                    croppedInputImage.SoftwareBitmap.PixelHeight);

                await tmp.CopyToAsync(croppedInputImage, srcBounds, null);


                if (tmp.SoftwareBitmap is not null)
                {
                    FaceBoxFrameCaptured?.Invoke(this, new SoftwareBitmapEventArgs(tmp.SoftwareBitmap));
                }

                ImageFeatureValue imageTensor = ImageFeatureValue.CreateFromVideoFrame(croppedInputImage);

                _binding = new LearningModelBinding(_session);

                TensorFloat outputTensor = TensorFloat.Create(_outputTensorDescriptor.Shape);
                List<float> _outputVariableList = new List<float>();

                // Bind inputs + outputs
                _binding.Bind(_inputImageDescriptor.Name, imageTensor);
                _binding.Bind(_outputTensorDescriptor.Name, outputTensor);

                // Evaluate results
                var results = await _session.EvaluateAsync(_binding, new Guid().ToString());

                Debug.WriteLine("ResultsEvaluated: " + results.ToString());

                var outputTensorList = outputTensor.GetAsVectorView();
                var resultsList = new List<float>(outputTensorList.Count);
                for (int i = 0; i < outputTensorList.Count; i++)
                {
                    resultsList.Add(outputTensorList[i]);
                }

                var softMaxexOutputs = SoftMax(resultsList);

                double maxProb = 0;
                int maxIndex = 0;

                // Comb through the evaluation results
                for (int i = 0; i < Constants.POTENTIAL_EMOJI_NAME_LIST.Count(); i++)
                {
                    // Record the dominant emotion probability & its location
                    if (softMaxexOutputs[i] > maxProb)
                    {
                        maxIndex = i;
                        maxProb = softMaxexOutputs[i];
                    }
                }

                Debug.WriteLine($"Probability = {maxProb}, Threshold set to = {Constants.CLASSIFICATION_CERTAINTY_THRESHOLD}, Emotion = {Constants.POTENTIAL_EMOJI_NAME_LIST[maxIndex]}");

                // For evaluations run on the MainPage, update the emoji carousel
                if (maxProb >= Constants.CLASSIFICATION_CERTAINTY_THRESHOLD)
                {
                    Debug.WriteLine("first page emoji should start to update");
                    IntelligenceServiceEmotionClassified?.Invoke(this, new ClassifiedEmojiEventArgs(CurrentEmojis._emojis.Emojis[maxIndex]));
                }

                // Dispose of resources
                if (e.SoftwareBitmap != null)
                {
                    e.SoftwareBitmap.Dispose();
                    e.SoftwareBitmap = null;
                }
            }
        }
        IntelligenceServiceProcessingCompleted?.Invoke(this, null);
        Debug.WriteLine($"Frame evaluation finished {DateTime.Now}");
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
