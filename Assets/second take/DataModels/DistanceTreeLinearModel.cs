using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
public struct DistanceTreeLinearModel
{
    public int nodeType;
    public Matrix4x4 translation;
    public Vector3 color;
    public Vector3 a;
    public Vector3 b;

    public override string ToString()
    {
        return $"{nodeType.ToString()}: {{{translation.ToString()}}}, a:{a.ToString()}, b: {b.ToString()}, color: {color.ToString()}";
    }
}
