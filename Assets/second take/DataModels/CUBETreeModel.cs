using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUBETreeModel : DistanceTreeNodeModel
{
    [Range(0.01f,100.0f)]
    public float Width=1;
    [Range(0.01f,100.0f)]
    public float Height=1;
    [Range(0.01f,100.0f)]
    public float Depth=1;
    void Start()
    {
        NodeType = DistanceTreeNodeType._CUBE;
        DesiredChildCount = 0;
    }
    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            translation = transform.worldToLocalMatrix,
            color = _color,
            nodeType = (int)NodeType,
            a=new Vector3(Width,Height,Depth),
            b = new Vector3((float)Math.Pow(transform.worldToLocalMatrix.determinant, (double) 1 / 3),0.0f,0.0f)
        };
    }

}
