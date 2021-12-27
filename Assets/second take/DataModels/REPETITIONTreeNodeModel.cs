using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class REPETITIONTreeNodeModel : DistanceTreeNodeModel
{
    [Range(0.00f,100.0f)]
    public float Width=1;
    [Range(0.00f,100.0f)]
    public float Height=1;
    [Range(0.00f,100.0f)]
    public float Depth=1;
    
    // Start is called before the first frame update
    void Start()
    {
        NodeType = DistanceTreeNodeType._REPETITION;
        DesiredChildCount = 1;
    }

    
    
    public override bool LinearizeTree(List<DistanceTreeNodeModel> list)
    {
        if (!IsValid())
        {
            return false;
        }
        list.Add(this);
        foreach (var child in children)
        {
            if (!child.LinearizeTree(list))
            {
                return false;
            }
        }
        list.Add(new UNDOSPACETRANSFORMTreeModel());
        return true;
    }
    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            nodeType = (int)NodeType,
            translation = transform.worldToLocalMatrix,
            a=new Vector3(Width,Height,Depth),
        };
    }
    
}
