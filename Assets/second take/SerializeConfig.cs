using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SerializeConfig
{
    public MonitorConfig[] monitors;
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

