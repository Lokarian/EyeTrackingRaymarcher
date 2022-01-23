using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
public struct DistanceTreeLinearModel
{
    public int nodeType;
    public Matrix4x4 translation;
    public Vector4 color;
    public Vector4 a;
    public Vector4 b;

    public override string ToString()
    {
        return $"{nodeType.ToString()}: {{{translation.ToString()}}}, a:{a.ToString()}, b: {b.ToString()}, color: {color.ToString()}";
    }
}
[StructLayout(LayoutKind.Sequential, Pack = 0)]
public struct DistanceTreeLinearModelStd430
{
    public int nodeType;
    public Vector3 padding1;
    public Matrix4x4 translation;
    public Vector4 color;
    public Vector4 a;
    public Vector4 b;

    public DistanceTreeLinearModelStd430(DistanceTreeLinearModel model)
    {
        nodeType = model.nodeType;
        translation = model.translation;
        color = model.color;
        a = model.a;
        b = model.b;
        padding1 = new Vector3(0, 0, 0);
    }
}

