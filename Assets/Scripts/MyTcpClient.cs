using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class MyTcpClient : MonoBehaviour
{
    private static TcpClient tcp_client;
    private static NetworkStream network_stream;
    private static bool is_connection;
    private static bool active = false;

    private static string message = string.Empty;

    public void OnGUI()
    {
        if (active)
        {
            if (!is_connection)
            {
                GUILayout.Label("is not connect.");
                return;
            }
            message = GUILayout.TextField(message);
            if (GUILayout.Button("Send!"))
            {
                try
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    network_stream.Write(buffer, 0, buffer.Length);
                    Debug.LogFormat(message);
                }
                catch
                {
                    Debug.LogError("transmission failed.");
                }
            }
        }
    }

    private void OnMouseDown()
    {
        MyTcpServer.OnInactive();
        Debug.Log("Client Start...");
        activate();
    }

    private void activate()
    {
        if (!active)
        {
            try
            {
                tcp_client = new TcpClient(MyTcpServer.ip_address, MyTcpServer.port);
                network_stream = tcp_client.GetStream();
                is_connection = true;
                active = true;
                Debug.LogFormat("connected.");
            }
            catch (SocketException)
            {
                Debug.LogError("connection failed.");
            }
        }
    }

    private static void OnDestroy()
    {
        Debug.Log("server destroy.");
        tcp_client?.Dispose();
        network_stream?.Dispose();
    }

    public static void OnInactive()
    {
        OnDestroy();
        active = false;
    }
}
