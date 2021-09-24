using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.StreamEngine;


[SelectionBase]
public class Positions : MonoBehaviour
{
    public Vector3 _rightEye;
    private Vector3 _leftEye;
    private IntPtr _apiContext;
    private IntPtr _deviceContext;
    public static tobii_gaze_origin_t GazeOrigin;
    public float screenwidthmm = 720;
    public float screenheightmm = 345;
    private Transform _camera;
    private Transform _plane;
    private Transform _tl;
    private Transform _tr;
    private Transform _br;
    private Transform _bl;

    public Positions()
    {
    }

    static void GazeOriginCallback(ref tobii_gaze_origin_t origin)
    {
        GazeOrigin = origin;
    }
    
    void OnDrawGizmos () {
    		Gizmos.color = Color.cyan;
            if(_tl==null)
                return;
    		Gizmos.DrawLine(_camera.position, _tl.position);
    		Gizmos.DrawLine(_camera.position, _tr.position);
    		Gizmos.DrawLine(_camera.position, _br.position);
    		Gizmos.DrawLine(_camera.position, _bl.position);
    	}

    // Start is called before the first frame update
    void Start()
    {
        _plane = transform.GetChild(0);
        _plane.localScale = new Vector3(screenwidthmm / 1000 / 5 / 2, 1, screenheightmm / 1000 / 5 / 2);
        _camera = transform.GetChild(1);
        _tl = _plane.GetChild(0);
        _tr = _plane.GetChild(1);
        _br = _plane.GetChild(2);
        _bl = _plane.GetChild(3);
        _rightEye = Vector3.zero;
        _leftEye = Vector3.zero;

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
        Interop.tobii_device_process_callbacks(_deviceContext);
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
                GazeOrigin.left.z,
                GazeOrigin.left.y
            );
        }

        _camera.localPosition = _rightEye / 1000;
    }

    public Transform GetTl()
    {
        return _tl;
    }
    public Transform GetTr()
    {
        return _tr;
    }
    public Transform GetBl()
    {
        return _bl;
    }
    public Transform GetBr()
    {
        return _br;
    }
    
}