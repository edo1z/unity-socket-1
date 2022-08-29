using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client
{
    private Socket socket = null;
    private bool is_connected = false;
    private TcpEventHandler handler;

    public Client(TcpEventHandler event_callback)
    {
        handler += event_callback;
    }

    public void start(string ip, int port)
    {
        if (is_connected) return;
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.Connect(ip, port);
            is_connected = true;
            handler(new TcpEvent(TcpEventType.Connected, false, "connected!"));
            receive_loop();
        }
        catch
        {
            is_connected = false;
            handler(new TcpEvent(TcpEventType.Connected, false, "connect failed."));
        }
    }

    public void receive_loop()
    {
      byte[] buffer = new byte[256];
      int recv_size = 0;
      string msg = "";
      while(is_connected)
      {
        if (socket != null && socket.Poll(0, SelectMode.SelectRead))
        {
            try
            {
                buffer = new byte[256];
                recv_size = socket.Receive(buffer, buffer.Length, SocketFlags.None);
                if (recv_size > 0)
                {
                    msg = Encoding.UTF8.GetString(buffer, 0, recv_size);
                    handler(new TcpEvent(TcpEventType.Received, true, msg));
                }
                else
                {
                    disconnect();
                }
            }
            catch
            {
                handler(new TcpEvent(TcpEventType.Received, false, "receive failed."));
                disconnect();
            }
        }
      }
    }

    public void send(string msg)
    {
        if (!is_connected) return;
        try
        {
            var buffer = Encoding.UTF8.GetBytes(msg);
            socket.Send(buffer, buffer.Length, SocketFlags.None);
            handler(new TcpEvent(TcpEventType.Sent, true, msg));
        }
        catch
        {
            handler(new TcpEvent(TcpEventType.Sent, false, "send failed: " + msg));
        }
    }

    private void disconnect()
    {
        try
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;
            handler(new TcpEvent(TcpEventType.Disconnected, true, "disconnected!"));
        }
        catch
        {
            socket = null;
            handler(new TcpEvent(TcpEventType.Disconnected, false, "disconnect failed."));
        }
    }

}
