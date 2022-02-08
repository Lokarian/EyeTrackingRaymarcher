using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PerformanceHandler : MonoBehaviour
{
    [Range(1, 200)] public int TargetFPS=60;
    public bool CorrectFPS=true;
    public int FrameWindowsSize = 100;
    public CameraHandler camera;
    private int[] timeStamps;
    private int timeStampPointer;
    private Stopwatch sw = new Stopwatch();
    // Start is called before the first frame update
    void Start()
    {
        timeStamps = new int[FrameWindowsSize];
        timeStampPointer = 0;
        sw.Start();
    }

    // Update is called once per frame
    void Update()
    {
        sw.Stop();
        var deltaT =sw.Elapsed;
        timeStamps[timeStampPointer] = deltaT.Milliseconds;
        sw.Restart();
        timeStampPointer++;
        if (timeStampPointer == FrameWindowsSize)
        {
            AnalyzeFrames();
        }
        timeStampPointer = timeStampPointer % FrameWindowsSize;
    }

    void AnalyzeFrames()
    {
        var fps= 1/(timeStamps.Average()/1000.0f);
        Debug.Log($"Fps: {fps}");
        var performanceFactor = fps / TargetFPS;
        if (!CorrectFPS)
        {
            Debug.Log($"Frametimes: {string.Join(",", timeStamps)}");
            return;
        }
        if (performanceFactor < 1)
        {
            camera.Resolution*= (float)Math.Sqrt(performanceFactor);
        }
        else
        {
            camera.Resolution= Math.Min(1,camera.Resolution* (float)Math.Sqrt(performanceFactor));
        }
    }
}
