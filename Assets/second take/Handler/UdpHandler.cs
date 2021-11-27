using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Random = UnityEngine.Random;

public class UdpHandler : MonoBehaviour
{
    private static int localPort;

    // prefs
    public string IP = "192.168.2.107"; // define in init
    public int port = 8080; // define in init

    public bool send = false;

    public bool sendOnce = false;

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;
    public DistanceTreeLinearModel[] distanceTree;

    public UdpPackage udpPackage = new UdpPackage();

    public void Start()
    {
        init();
    }

    public void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            sendOnce = true;
        }
        if (sendOnce)
        {
            SendUpdate();
        }
        if (send&&Random.value<0.05)
        {
            SendUpdate();
        }
    }

    public void SendUpdate()
    {
        int sizeStruct = Marshal.SizeOf(udpPackage);
        int sizeArray = distanceTree == null || distanceTree.Length == 0
            ? 0
            : distanceTree.Length * Marshal.SizeOf(new DistanceTreeLinearModelStd430(distanceTree[0]));

        int size = sizeStruct + sizeArray;
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(sizeStruct);
        Marshal.StructureToPtr(udpPackage, ptr, true);
        Marshal.Copy(ptr, arr, 0, sizeStruct);
        Marshal.FreeHGlobal(ptr);
        if (sizeArray != 0)
        {
            int distanceTreeStructLength = Marshal.SizeOf(new DistanceTreeLinearModelStd430(distanceTree[0]));
            for (int i = 0; i < distanceTree.Length; i++)
            {
                byte[] objBytes = StructureToByteArray(new DistanceTreeLinearModelStd430(distanceTree[i]));
                Array.Copy(objBytes, 0, arr, sizeStruct + i * distanceTreeStructLength, distanceTreeStructLength);
            }
        }

        Debug.Log(ByteArrayToString(arr));
        SendBytes(arr);

        sendOnce = false;
    }

    // init
    public void init()
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        print("Sending to " + IP + " : " + port);
    }

    public static string ByteArrayToString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", "");
    }

    byte[] StructureToByteArray(object obj)
    {
        int len = Marshal.SizeOf(obj);

        byte[] arr = new byte[len];

        IntPtr ptr = Marshal.AllocHGlobal(len);

        Marshal.StructureToPtr(obj, ptr, true);

        Marshal.Copy(ptr, arr, 0, len);

        Marshal.FreeHGlobal(ptr);

        return arr;
    }

    // sendData
    private void SendBytes(byte[] data)
    {
        try
        {
            // Den message zum Remote-Client senden.
            client.Send(data, data.Length, remoteEndPoint);
            //}
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
}

public struct UdpPackage
{
    public Vector3 leftEyePos;
    public Vector3 rightEyePos;
    public Matrix4x4 cameraMatrix;
    public float marchEpsilon;
    public int maxIterations;
    public float ambientFactor;
    public float shininess;
    public Vector3 lightColor;
    public Vector3 lightPosition;
    public int distanceTreeLength;
}