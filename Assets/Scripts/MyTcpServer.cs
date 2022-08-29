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
    private Socket listener = null;
    private List<Socket> sockets = new List<Socket>();
    private TcpEventHandler handler;

    public Server(TcpEventHandler event_callback)
    {
        handler += event_callback;
    }

    public void start(int port)
    {
        try
        {
          listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
          listener.Bind(new IPEndPoint(IPAddress.Any, port));
          listener.Listen(1);
          accept_receive_loop();
        }
        catch
        {
          handler(new TcpEvent(TcpEventType.Listening, false, "server start failed."));
        }
    }

    private void accept_receive_loop()
    {
        handler(new TcpEvent(TcpEventType.Listening, true, "server listening..."));
        while (true)
        {
            accept();
            receive();
        }
    }

    private void accept()
    {
        if (listener != null && listener.Poll(0, SelectMode.SelectRead))
        {
            sockets.Add(listener.Accept());
            handler(new TcpEvent(TcpEventType.Connected, true, sockets.Count + " connected!"));
        }
    }

    private void receive()
    {
        string msg = "";
        for (int i = 0; i < sockets.Count; i++)
        {
            if (sockets[i] != null && sockets[i].Poll(0, SelectMode.SelectRead))
            {
                try
                {
                    byte[] buffer = new byte[256];
                    int recv_size = sockets[i].Receive(buffer, buffer.Length, SocketFlags.None);
                    if (recv_size > 0)
                    {
                        msg = Encoding.UTF8.GetString(buffer, 0, recv_size);
                        handler(new TcpEvent(TcpEventType.Received, true, "received: " + msg));
                    }
                    else
                    {
                        disconnect(i);
                    }
                }
                catch
                {
                    handler(new TcpEvent(TcpEventType.Received, false, "receive failed."));
                    disconnect(i);
                }
            }
        }
    }

    private void disconnect(int idx)
    {
        if (sockets[idx] == null) return;
        try
        {
            sockets[idx].Shutdown(SocketShutdown.Both);
            sockets[idx].Close();
            sockets[idx] = null;
            handler(new TcpEvent(TcpEventType.Disconnected, true, "disconnected!"));
        }
        catch
        {
            Debug.Log("socket close failed.");
            sockets[idx] = null;
            handler(new TcpEvent(TcpEventType.Disconnected, false, "disconnect failed."));
        }
    }

}

