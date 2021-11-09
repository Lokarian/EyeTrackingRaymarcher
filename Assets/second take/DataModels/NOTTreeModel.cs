using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOTTreeModel : DistanceTreeNodeModel
{
    // Start is called before the first frame update
    void Start()
    {
        NodeType = DistanceTreeNodeType._NOT;
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

    // Update is called once per frame
    void Update()
    {
    }

    public override DistanceTreeLinearModel? GetLinearModel()
    {
        if (children.Count == 1)
        {
            return new DistanceTreeLinearModel()
            {
                nodeType = (int)NodeType
            };
        }
        else
        {
            return null;
        }
    }
}
