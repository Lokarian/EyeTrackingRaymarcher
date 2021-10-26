using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SerializeConfig
{
    public MonitorConfig[] monitors;
    public EyeTracking eyeTracking;
}
[Serializable]
public class MonitorConfig
{
    public Vector3 topLeft;
    public Vector3 topRight;
    public Vector3 bottomLeft;
    public StereoType stereoType;
    public int monitorIndex;
}
[Serializable]
public class EyeTracking
{
    public bool usesTobii;
    public int tobiiMonitorIndex;
    public bool usesVRPN;
    public Matrix4x4 vrpnTransformationMatrix;
    public Vector3 leftEyeOffSet;
    public Vector3 rightEyeOffSet;
}

