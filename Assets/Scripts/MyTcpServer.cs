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
    private static GUIStyle gui_style = new GUIStyle();
    private static Rect rect = new Rect();
    private static Color txt_color = new Color(0.2f, 0.9f, 0.2f, 1.0f);

    private void OnGUI()
    {
        if (active)
        {
            gui_style.alignment = TextAnchor.MiddleCenter;
            gui_style.fontSize = 30;
            gui_style.normal.textColor = txt_color;
            rect.x = Screen.width / 2;
            rect.y = Screen.height / 2;
            rect.width = 0;
            rect.height = 0;
            GUI.Label(rect, messages, gui_style);
        }
    }

    private void OnMouseDown()
    {
        MyTcpClient.OnInactive();
        activate();
        messages = "Server Start...";
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
        messages = "listening...";
        tcp_client = tcp_listener.AcceptTcpClient();
        messages = "connected.";
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
            messages = Encoding.UTF8.GetString(buffer, 0, count);
        }
    }

    private static void OnDestroy()
    {
        messages = "server destroy.";
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
