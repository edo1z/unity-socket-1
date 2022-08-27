using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MyTcpServer : MonoBehaviour
{
    public static string ip_address = "127.0.0.1";
    public static int port = 5555;
    private static TcpListener tcp_listener;
    private static TcpClient tcp_client;
    private static NetworkStream network_stream;
    private static bool active = false;
    private static string messages = string.Empty;

    private void OnGUI()
    {
        if (active)
        {
            GUILayout.TextArea(messages);
        }
    }

    private void OnMouseDown()
    {
        MyTcpClient.OnInactive();
        Debug.Log("Server Start...");
        activate();
    }

    private void activate()
    {
        if (!active)
        {
            active = true;
            Task.Run(() => OnProcess());
        }
    }

    private void OnProcess()
    {
        var ipAddress = IPAddress.Parse(ip_address);
        tcp_listener = new TcpListener(ipAddress, port);
        tcp_listener.Start();
        Debug.Log("listening...");
        tcp_client = tcp_listener.AcceptTcpClient();
        Debug.Log("connected.");
        network_stream = tcp_client.GetStream();
        while (true)
        {
            var buffer = new byte[256];
            var count = network_stream.Read(buffer, 0, buffer.Length);
            if (count == 0)
            {
                OnDestroy();
                Task.Run(() => OnProcess());
                break;
            }
            var message = Encoding.UTF8.GetString(buffer, 0, count);
            messages += message + "\n";
        }
    }

    private static void OnDestroy()
    {
        Debug.Log("server destroy.");
        network_stream?.Dispose();
        tcp_client?.Dispose();
        tcp_listener?.Stop();
    }

    public static void OnInactive()
    {
        OnDestroy();
        active = false;
    }
}
