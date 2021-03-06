using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ORTreeModel : DistanceTreeNodeModel
{
    // Start is called before the first frame update
    void Start()
    {
        NodeType = DistanceTreeNodeType._OR;
        DesiredChildCount = 2;
    }

    private void OnEnable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).GetComponent<DistanceTreeNodeModel>();
            if (child != null)
            {
                children.Add(child);
            }
        }
    }


    public override DistanceTreeLinearModel GetLinearModel()
    {
        return new DistanceTreeLinearModel()
        {
            nodeType = (int)NodeType
        };
    }
}