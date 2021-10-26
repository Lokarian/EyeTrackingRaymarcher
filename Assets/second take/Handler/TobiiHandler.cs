using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.StreamEngine;

public class TobiiHandler : MonoBehaviour

{
    public Transform[] _eyes;
    private IntPtr _apiContext;
    private IntPtr _deviceContext;
    public static tobii_gaze_origin_t? gazeOrigin = null;
    private long lastUpdateTimestamp;

    static void GazeOriginCallback(ref tobii_gaze_origin_t origin)
    {
        gazeOrigin = origin;
        
    }

    void Start()
    {
        tobii_error_t result = Interop.tobii_api_create(out _apiContext, null);
        if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
            Debug.Log("Failed to create Api");

        List<string> urls;
        result = Interop.tobii_enumerate_local_device_urls(_apiContext, out urls);
        if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
            Debug.Log("Failed to get Devices List.");
        if (urls.Count == 0)
        {
            Debug.Log("Error: No device found");
            return;
        }

        Debug.Log("List: " + urls[0]);
        result = Interop.tobii_device_create(_apiContext, urls[0], out _deviceContext);
        if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
        {
            Debug.Log("Failed to get Device Handle:");
            Debug.Log(result);
            result = Interop.tobii_device_create(_apiContext, urls[0], out _deviceContext);
            if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
            {
                Debug.Log("Failed to get Device Handle:");
                Debug.Log(result);
            }
        }

        result = Interop.tobii_gaze_origin_subscribe(_deviceContext, GazeOriginCallback);
        if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
            Debug.Log("Failed to register Callback");
    }

    // Update is called once per frame
    void Update()
    {
        Interop.tobii_device_process_callbacks(_deviceContext);
        if (gazeOrigin.HasValue)
        {
            if (gazeOrigin.Value.timestamp_us != lastUpdateTimestamp)
            {
                lastUpdateTimestamp = gazeOrigin.Value.timestamp_us;
                //Debug.Log(gazeOrigin.Value.left.x+","+gazeOrigin.Value.left.y+","+gazeOrigin.Value.left.z+",");
                if (gazeOrigin.Value.left_validity == tobii_validity_t.TOBII_VALIDITY_VALID)
                {
                    _eyes[0].position = transform.TransformPoint(
                        gazeOrigin.Value.left.x / 100,
                        gazeOrigin.Value.left.y / 100,
                        -gazeOrigin.Value.left.z / 100);
                }

                if (gazeOrigin.Value.right_validity == tobii_validity_t.TOBII_VALIDITY_VALID)
                {
                    _eyes[1].position = transform.TransformPoint(
                        gazeOrigin.Value.right.x / 100,
                        gazeOrigin.Value.right.y / 100,
                        -gazeOrigin.Value.right.z / 100);
                }
            }
            
        }
    }

    void OnDestroy()
    {
        tobii_error_t result = Interop.tobii_gaze_origin_unsubscribe(_deviceContext);
        if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
            Debug.Log("Failed to unsubscribe from eye position stream.");
        result = Interop.tobii_device_destroy(_deviceContext);
        if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
            Debug.Log("Failed to destroy device.");
        result = Interop.tobii_api_destroy(_apiContext);
        if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
            Debug.Log("Failed to destroy API.");
    }
    
}