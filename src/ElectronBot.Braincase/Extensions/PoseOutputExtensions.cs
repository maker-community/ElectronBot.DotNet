using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Mediapipe.Net.Solutions;

namespace ElectronBot.Braincase.Extensions;
public static class PoseOutputExtensions
{
    public static List<PoseLine> GetPoseLines(this PoseOutput poseOutput, double x, double y)
    {
        var result = new List<PoseLine>();
        if (poseOutput is { PoseLandmarks.Landmark: not null })
        {
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[14].X * (float)x, poseOutput.PoseLandmarks.Landmark[14].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[16].X * (float)x, poseOutput.PoseLandmarks.Landmark[16].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[13].X * (float)x, poseOutput.PoseLandmarks.Landmark[13].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[15].X * (float)x, poseOutput.PoseLandmarks.Landmark[15].Y * (float)y)
            });

            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[12].X * (float)x, poseOutput.PoseLandmarks.Landmark[12].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[11].X * (float)x, poseOutput.PoseLandmarks.Landmark[11].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[11].X * (float)x, poseOutput.PoseLandmarks.Landmark[11].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[13].X * (float)x, poseOutput.PoseLandmarks.Landmark[13].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[11].X * (float)x, poseOutput.PoseLandmarks.Landmark[11].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[23].X * (float)x, poseOutput.PoseLandmarks.Landmark[23].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[12].X * (float)x, poseOutput.PoseLandmarks.Landmark[12].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[24].X * (float)x, poseOutput.PoseLandmarks.Landmark[24].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[12].X * (float)x, poseOutput.PoseLandmarks.Landmark[12].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[14].X * (float)x, poseOutput.PoseLandmarks.Landmark[14].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[24].X * (float)x, poseOutput.PoseLandmarks.Landmark[24].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[23].X * (float)x, poseOutput.PoseLandmarks.Landmark[23].Y * (float)y)
            });
            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[24].X * (float)x, poseOutput.PoseLandmarks.Landmark[24].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[26].X * (float)x, poseOutput.PoseLandmarks.Landmark[26].Y * (float)y)
            });

            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[23].X * (float)x, poseOutput.PoseLandmarks.Landmark[23].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[25].X * (float)x, poseOutput.PoseLandmarks.Landmark[25].Y * (float)y)
            });

            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[26].X * (float)x, poseOutput.PoseLandmarks.Landmark[26].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[28].X * (float)x, poseOutput.PoseLandmarks.Landmark[28].Y * (float)y)
            });

            result.Add(new PoseLine()
            {
                StartVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[25].X * (float)x, poseOutput.PoseLandmarks.Landmark[25].Y * (float)y),
                EndVector2 = new Vector2(poseOutput.PoseLandmarks.Landmark[27].X * (float)x, poseOutput.PoseLandmarks.Landmark[27].Y * (float)y)
            });
        }
        return result;
    }

    public static List<PoseLine3D> GetPose3DLines(this PoseOutput poseOutput, double x, double y, double z)
    {
        var result = new List<PoseLine3D>();
        if (poseOutput is { PoseWorldLandmarks.Landmark: not null })
        {
            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[14].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[14].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[14].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[16].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[16].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[16].Z * (float)z)
            });
            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[13].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[13].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[13].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[15].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[15].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[15].Z * (float)z)
            });

            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[12].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[12].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[12].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[11].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[11].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[11].Z * (float)z)
            });
            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[11].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[11].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[11].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[13].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[13].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[13].Z * (float)z)
            });
            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[11].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[11].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[11].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[23].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[23].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[23].Z * (float)z)
            });
            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[12].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[12].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[12].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[24].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[24].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[24].Z * (float)z)
            });
            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[12].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[12].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[12].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[14].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[14].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[14].Z * (float)z)
            });
            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[24].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[24].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[24].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[23].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[23].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[23].Z * (float)z)
            });
            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[24].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[24].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[24].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[26].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[26].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[26].Z * (float)z)
            });

            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[23].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[23].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[23].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[25].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[25].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[25].Z * (float)z)
            });

            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[26].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[26].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[26].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[28].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[28].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[28].Z * (float)z)
            });

            result.Add(new PoseLine3D()
            {
                StartVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[25].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[25].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[25].Z * (float)z),
                EndVector3 = new Vector3(poseOutput.PoseWorldLandmarks.Landmark[27].X * (float)x, poseOutput.PoseWorldLandmarks.Landmark[27].Y * (float)y, poseOutput.PoseWorldLandmarks.Landmark[27].Z * (float)z)
            });
        }
        return result;
    }
}
