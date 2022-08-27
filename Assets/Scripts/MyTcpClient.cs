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
    private static string send_message = string.Empty;
    private static string message = string.Empty;
    private static GUIStyle gui_style = new GUIStyle();
    private static Rect rect = new Rect();
    private static Color txt_color = new Color(0.2f, 0.5f, 1.0f, 1.0f);

    public void OnGUI()
    {
        if (active)
        {
            rect.x = 10;
            rect.y = 10;
            rect.width = 200;
            rect.height = 25;
            send_message = GUI.TextField(rect, send_message, 100);
            if (!is_connection)
            {
                message = "is not connect.";
            }
            rect.x += 200;
            rect.width = 100;
            if (GUI.Button(rect, "Send!") && is_connection)
            {
                try
                {
                    var buffer = Encoding.UTF8.GetBytes(send_message);
                    network_stream.Write(buffer, 0, buffer.Length);
                    message = send_message;
                    send_message = "";
                }
                catch
                {
                    message = "transmission failed.";
                }
            }
            _show_message(message);
        }
    }

    private void _show_message(string msg)
    {
        gui_style.alignment = TextAnchor.MiddleCenter;
        gui_style.fontSize = 30;
        gui_style.normal.textColor = txt_color;
        rect.x = Screen.width / 2;
        rect.y = Screen.height / 2;
        rect.width = 0;
        rect.height = 0;
        GUI.Label(rect, msg, gui_style);
    }

    private void OnMouseDown()
    {
        MyTcpServer.OnInactive();
        activate();
    }

    private void activate()
    {
        if (!active || !is_connection)
        {
            active = true;
            try
            {
                tcp_client = new TcpClient(MyTcpServer.ip_address, MyTcpServer.port);
                network_stream = tcp_client.GetStream();
                is_connection = true;
                message = "connected.";
            }
            catch (SocketException)
            {
                message = "connection failed.";
            }
        }
    }

    private static void OnDestroy()
    {
        message = "server destroy.";
        tcp_client?.Dispose();
        network_stream?.Dispose();
    }

    public static void OnInactive()
    {
        OnDestroy();
        active = false;
    }
}
