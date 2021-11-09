using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class RenderTreeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public DistanceTreeNodeModel Root;
    private int _bufferSize = 200;
    private ComputeBuffer buffer;
    static int bufferId = Shader.PropertyToID("distanceTree");
    static int distanceTreeLengthId = Shader.PropertyToID("distanceTreeLength");
    public List<int> KernelIndices = new List<int>() {0, 1};
    public ComputeShader computeShader;
    public int BufferSize
    {
        get => _bufferSize;
        set { _bufferSize = value;UpdateBufferSize(); }
    }

    void OnEnable()
    {
        UpdateBufferSize();
        UpdateLinearTree();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLinearTree();
    }

    public void UpdateLinearTree()
    {
        var linearTree = new List<DistanceTreeLinearModel>();
        Root.ConstructLinearTree(linearTree);
        buffer.SetData(linearTree.ToArray());
        computeShader.SetInt(distanceTreeLengthId,linearTree.Count);
        Debug.Log((object)(linearTree[2].ToString()));
        Debug.Log($"Linear Tree Length: {linearTree.Count.ToString()}");
    }
    private void UpdateBufferSize()
    {
        var sizeTest = new DistanceTreeLinearModel();
        buffer = new ComputeBuffer(_bufferSize,Marshal.SizeOf(sizeTest));
        foreach (var kernelIndex in KernelIndices)
        {
            computeShader.SetBuffer(kernelIndex,bufferId,buffer);
        }
    }
    
    void OnDisable () {
        buffer.Release();
        buffer = null;
    }
    
}
