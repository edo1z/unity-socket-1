using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Server
{
    private TcpListener listener;
    private TcpClient client;
    private NetworkStream network_stream;
    private TcpEventHandler handler;

    public Server(TcpEventHandler event_callback)
    {
        handler += event_callback;
    }

    public void start(string ip, int port)
    {
        listener = new TcpListener(IPAddress.Parse(ip), port);
        listener.Start();
        accept_loop();
    }

    private void accept_loop()
    {
        handler(new TcpEvent(TcpEventType.Listening, true, "server listening..."));
        client = listener.AcceptTcpClient();
        network_stream = client.GetStream();
        string message = "";
        while (true)
        {
            var buffer = new byte[256];
            var count = network_stream.Read(buffer, 0, buffer.Length);
            if (count == 0)
            {
                OnDestroy();
                Task.Run(() => accept_loop());
                break;
            }
            message = Encoding.UTF8.GetString(buffer, 0, count);
            handler(new TcpEvent(TcpEventType.Received, true, "received: " + message));
        }
    }

    private void OnDestroy()
    {
        network_stream?.Dispose();
        client?.Dispose();
        listener?.Stop();
        handler(new TcpEvent(TcpEventType.Destroyed, true, "server destroyed."));
    }

}
