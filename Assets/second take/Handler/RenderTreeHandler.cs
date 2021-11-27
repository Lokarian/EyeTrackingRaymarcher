using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<DistanceTreeNodeModel> linearTree = new List<DistanceTreeNodeModel>();
    public UdpHandler udpHandler;
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
        var tempLinearTree = new List<DistanceTreeNodeModel>();
        if (Root != null)
        {
            if (Root.LinearizeTree(tempLinearTree))
            {
                linearTree = tempLinearTree;
            }
        }
        else
        {
            linearTree = new List<DistanceTreeNodeModel>();
        }

        var linearModels = linearTree.Select(a => a.GetLinearModel()).ToArray();
        buffer.SetData(linearModels);
        computeShader.SetInt(distanceTreeLengthId,linearModels.Length);
        udpHandler.distanceTree = linearModels;
        udpHandler.udpPackage.distanceTreeLength = linearModels.Length;
        Debug.Log($"Linear Tree Length: {linearModels.Length.ToString()}");
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
