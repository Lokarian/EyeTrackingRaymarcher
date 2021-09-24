using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.StreamEngine;

public class TrailTest : MonoBehaviour
{
    private Vector3 _rightEye;
    private Vector3 _leftEye;
    private IntPtr _apiContext;
    private IntPtr _deviceContext;
    public Transform[] _eyes;
    
    public static tobii_gaze_origin_t GazeOrigin;
  
    static void GazeOriginCallback(ref tobii_gaze_origin_t origin)
    {
        GazeOrigin = origin;
       
    }

    // Start is called before the first frame update
    void Start()
    {
        _rightEye= Vector3.zero;
        _leftEye= Vector3.zero;
        
        tobii_error_t result = Interop.tobii_api_create(out _apiContext, null);
        if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
            Debug.Log("Failed to create Api");

        List<string> urls;
        result = Interop.tobii_enumerate_local_device_urls(_apiContext, out urls);
        Debug.Log(urls.Count);
        if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
            Debug.Log("Failed to get Devices List.");
        if (urls.Count == 0)
        {
            Debug.Log("Error: No device found");
            return;
        }
        Debug.Log("List: "+urls[0]);
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
        tobii_gaze_origin_callback_t callback = new tobii_gaze_origin_callback_t(GazeOriginCallback);
        result = Interop.tobii_gaze_origin_subscribe(_deviceContext, GazeOriginCallback);
        if (result != tobii_error_t.TOBII_ERROR_NO_ERROR)
            Debug.Log("Failed to register Callback");
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

    // Update is called once per frame
    void Update()
    {
        if (GazeOrigin.right_validity == tobii_validity_t.TOBII_VALIDITY_VALID)
        {
            this._rightEye = new Vector3(
                GazeOrigin.right.x,
                GazeOrigin.right.y,
                -GazeOrigin.right.z
            );
        }

        if (GazeOrigin.left_validity == tobii_validity_t.TOBII_VALIDITY_VALID)
        {
            this._leftEye = new Vector3(
                GazeOrigin.left.x,
                GazeOrigin.left.y,
                -GazeOrigin.left.z
            );
        }
        
        Interop.tobii_device_process_callbacks(_deviceContext);
        _eyes[0].transform.position = _leftEye/100;
        _eyes[1].transform.position = _rightEye/100;
    }
}