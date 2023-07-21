// Copyright (c) homuler and The Vignette Authors
// This file is part of MediaPipe.NET.
// MediaPipe.NET is licensed under the MIT License. See LICENSE for details.

using System.Diagnostics;
using GestureClassification;
using Mediapipe.Net.Framework.Protobuf;

namespace ElectronBot.Braincase.Helpers;

public static class HandDataFormatHelper
{
    public static async Task SaveDataToTextAsync(List<NormalizedLandmark> landmarks)
    {
        if (landmarks.Count == 21)
        {
            var listDta = new List<float>();

            foreach (var landmark in landmarks)
            {
                listDta.Add(landmark.X);
                listDta.Add(landmark.Y);
                listDta.Add(landmark.Z);
            }

            var text = String.Join(",", listDta);

            var result = String.Join(",", text, Constants.Right);

            using StreamWriter file = new($"{Constants.Right}.txt", append: true);

            await file.WriteLineAsync(result);

            Console.WriteLine(result);
        }
    }

    public static string PredictResult(List<NormalizedLandmark> landmarks, string path = "")
    {
        if (landmarks.Count == 21)
        {
            var listDta = new List<float>();

            foreach (var landmark in landmarks)
            {
                listDta.Add(landmark.X);
                listDta.Add(landmark.Y);
                listDta.Add(landmark.Z);
            }


            // Create single instance of sample data from first line of dataset for model input
            MLModel1.ModelInput sampleData = new MLModel1.ModelInput()
            {
                Col0 = listDta[0],
                Col1 = listDta[1],
                Col2 = listDta[2],
                Col3 = listDta[3],
                Col4 = listDta[4],
                Col5 = listDta[5],
                Col6 = listDta[6],
                Col7 = listDta[7],
                Col8 = listDta[8],
                Col9 = listDta[9],
                Col10 = listDta[10],
                Col11 = listDta[11],
                Col12 = listDta[12],
                Col13 = listDta[13],
                Col14 = listDta[14],
                Col15 = listDta[15],
                Col16 = listDta[16],
                Col17 = listDta[17],
                Col18 = listDta[18],
                Col19 = listDta[19],
                Col20 = listDta[20],
                Col21 = listDta[21],
                Col22 = listDta[22],
                Col23 = listDta[23],
                Col24 = listDta[24],
                Col25 = listDta[25],
                Col26 = listDta[26],
                Col27 = listDta[27],
                Col28 = listDta[28],
                Col29 = listDta[29],
                Col30 = listDta[30],
                Col31 = listDta[31],
                Col32 = listDta[32],
                Col33 = listDta[33],
                Col34 = listDta[34],
                Col35 = listDta[35],
                Col36 = listDta[36],
                Col37 = listDta[37],
                Col38 = listDta[38],
                Col39 = listDta[39],
                Col40 = listDta[40],
                Col41 = listDta[41],
                Col42 = listDta[42],
                Col43 = listDta[43],
                Col44 = listDta[44],
                Col45 = listDta[45],
                Col46 = listDta[46],
                Col47 = listDta[47],
                Col48 = listDta[48],
                Col49 = listDta[49],
                Col50 = listDta[50],
                Col51 = listDta[51],
                Col52 = listDta[52],
                Col53 = listDta[53],
                Col54 = listDta[54],
                Col55 = listDta[55],
                Col56 = listDta[56],
                Col57 = listDta[57],
                Col58 = listDta[58],
                Col59 = listDta[59],
                Col60 = listDta[60],
                Col61 = listDta[61],
                Col62 = listDta[62],
            };


            MLModel1.SetModelPath(path);
            // Make a single prediction on the sample data and print results
            var predictionResult = MLModel1.Predict(sampleData);

            if (predictionResult.Score.Length > 0)
            {
                var ret = predictionResult.Score.ToList().Max();

                Debug.WriteLine($"label:{predictionResult.PredictedLabel}--score:{ret}");
                if (ret > 0.99)
                {
                    return predictionResult.PredictedLabel;
                }
            }
        }
        return "";
    }
}
