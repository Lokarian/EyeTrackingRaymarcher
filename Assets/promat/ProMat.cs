using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.StreamEngine;


[SelectionBase]
public class ProMat : MonoBehaviour
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

    public Transform[] Corners;
    public Transform lookTarget;
    public bool drawNearCone, drawFrustum;

    Camera theCam;

    static void GazeOriginCallback(ref tobii_gaze_origin_t origin)
    {
        GazeOrigin = origin;
    }

    // Start is called before the first frame update
    void Start()
    {
        theCam= Camera.main;
        _plane = transform.GetChild(0);
        _plane.localScale = new Vector3(screenwidthmm / 1000 / 5 / 2, 1, screenheightmm / 1000 / 5 / 2);
        _camera = transform.GetChild(1);
        _tl = _plane.GetChild(0);
        _tr = _plane.GetChild(1);
        _br = _plane.GetChild(2);
        _rightEye = Vector3.one;
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
        camProMatrix();
    }

    public Transform GetTl()
    {
        return _tl;
    }
    public Transform GetTr()
    {
        return _tr;
    }
    public Transform GetBr()
    {
        return _br;
    }

    private void camProMatrix()
    {
        Vector3 pa, pb, pc, pd;
        pa = Corners[0].position; //Bottom-Left
        pb = Corners[1].position; //Bottom-Right
        pc = Corners[2].position; //Top-Left
        pd = Corners[3].position; //Top-Right

        Vector3 pe = theCam.transform.position;// eye position

        Vector3 vr = (pb - pa).normalized; // right axis of screen
        Vector3 vu = (pc - pa).normalized; // up axis of screen
        Vector3 vn = Vector3.Cross(vr, vu).normalized; // normal vector of screen

        Vector3 va = pa - pe; // from pe to pa
        Vector3 vb = pb - pe; // from pe to pb
        Vector3 vc = pc - pe; // from pe to pc
        Vector3 vd = pd - pe; // from pe to pd

        float n = -lookTarget.InverseTransformPoint(theCam.transform.position).z; // distance to the near clip plane (screen)
        float f = theCam.farClipPlane; // distance of far clipping plane
        float d = Vector3.Dot(va, vn); // distance from eye to screen
        float l = Vector3.Dot(vr, va) * n / d; // distance to left screen edge from the 'center'
        float r = Vector3.Dot(vr, vb) * n / d; // distance to right screen edge from 'center'
        float b = Vector3.Dot(vu, va) * n / d; // distance to bottom screen edge from 'center'
        float t = Vector3.Dot(vu, vc) * n / d; // distance to top screen edge from 'center'

        Matrix4x4 p = new Matrix4x4(); // Projection matrix
        p[0, 0] = 2.0f * n / (r - l);
        p[0, 2] = (r + l) / (r - l);
        p[1, 1] = 2.0f * n / (t - b);
        p[1, 2] = (t + b) / (t - b);
        p[2, 2] = (f + n) / (n - f);
        p[2, 3] = 2.0f * f * n / (n - f);
        p[3, 2] = -1.0f;

        theCam.projectionMatrix = p; // Assign matrix to camera

        if (drawNearCone)
        { //Draw lines from the camera to the corners f the screen
            Debug.DrawRay(theCam.transform.position, va, Color.blue);
            Debug.DrawRay(theCam.transform.position, vb, Color.blue);
            Debug.DrawRay(theCam.transform.position, vc, Color.blue);
            Debug.DrawRay(theCam.transform.position, vd, Color.blue);
        }

        if (drawFrustum) DrawFrustum(theCam); //Draw actual camera frustum
    }
    Vector3 ThreePlaneIntersection(Plane p1, Plane p2, Plane p3)
    { //get the intersection point of 3 planes
        return ((-p1.distance * Vector3.Cross(p2.normal, p3.normal)) +
                (-p2.distance * Vector3.Cross(p3.normal, p1.normal)) +
                (-p3.distance * Vector3.Cross(p1.normal, p2.normal))) /
               (Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal)));
    }

    void DrawFrustum(Camera cam)
    {
        Vector3[] nearCorners = new Vector3[4]; //Approx'd nearplane corners
        Vector3[] farCorners = new Vector3[4]; //Approx'd farplane corners
        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(cam); //get planes from matrix
        Plane temp = camPlanes[1]; camPlanes[1] = camPlanes[2]; camPlanes[2] = temp; //swap [1] and [2] so the order is better for the loop

        for (int i = 0; i < 4; i++)
        {
            nearCorners[i] = ThreePlaneIntersection(camPlanes[4], camPlanes[i], camPlanes[(i + 1) % 4]); //near corners on the created projection matrix
            farCorners[i] = ThreePlaneIntersection(camPlanes[5], camPlanes[i], camPlanes[(i + 1) % 4]); //far corners on the created projection matrix
        }

        for (int i = 0; i < 4; i++)
        {
            Debug.DrawLine(nearCorners[i], nearCorners[(i + 1) % 4], Color.red, Time.deltaTime, false); //near corners on the created projection matrix
            Debug.DrawLine(farCorners[i], farCorners[(i + 1) % 4], Color.red, Time.deltaTime, false); //far corners on the created projection matrix
            Debug.DrawLine(nearCorners[i], farCorners[i], Color.red, Time.deltaTime, false); //sides of the created projection matrix
        }
    }
}