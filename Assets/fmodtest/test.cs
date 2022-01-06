using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [Range(-100.0f, 100.0f)]public float a = 1;
    [Range(-100.0f, 100.0f)]public float b = 1;

    public ComputeShader CS;
    private ComputeBuffer buf ;
    private int aID;
    private int bID;
    private int kernelID;
    private int bufferID;

    public Transform[] objs = new Transform[3];
    // Start is called before the first frame update
    void Start()
    {
        buf = new ComputeBuffer(1, 4*3);
        kernelID = CS.FindKernel("CSMain");
        aID = Shader.PropertyToID("a");
        bID = Shader.PropertyToID("b");
        bufferID = Shader.PropertyToID("buffer");
        CS.SetBuffer(kernelID, bufferID, buf);
    }

    // Update is called once per frame
    void Update()
    {
        CS.SetFloats(aID,VectorToArray(objs[0].position));
        CS.SetFloats(bID,VectorToArray(objs[1].position));
        CS.Dispatch(kernelID,1,1,1);
        float[] temp = new float[3];
        buf.GetData(temp);
        if (float.IsNaN(temp[0]) || float.IsNaN(temp[1]) || float.IsNaN(temp[2]))
        {
            Debug.Log("NaN");
            return;
        }
        objs[2].position = new Vector3(temp[0], temp[1], temp[2]);
    }

    private void OnDestroy()
    {
        buf.Dispose();
    }
    private float[] VectorToArray(Vector3 vecIn)
    {
        return new float[3] {vecIn.x, vecIn.y, vecIn.z};
    }
}
