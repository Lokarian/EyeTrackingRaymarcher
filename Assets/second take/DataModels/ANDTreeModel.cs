using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class ANDTreeModel : DistanceTreeNodeModel
{
    
    // Start is called before the first frame update
    void Start()
    {
        NodeType = DistanceTreeNodeType._AND;
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
            nodeType = (int)this.NodeType
        };
    }
}
