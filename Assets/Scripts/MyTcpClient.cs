using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client
{
    private TcpClient client;
    private NetworkStream network_stream;
    private bool is_connected = false;

    public bool start(string ip, int port)
    {
        if (is_connected) return true;
        try
        {
            client = new TcpClient(ip, port);
            network_stream = client.GetStream();
            is_connected = true;
            Debug.Log("client connected!");
            return true;
        }
        catch
        {
            is_connected = true;
            return false;
        }
    }

    public bool send(string msg)
    {
        if (!is_connected) return false;
        try
        {
            var buffer = Encoding.UTF8.GetBytes(msg);
            network_stream.Write(buffer, 0, buffer.Length);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void OnDestroy()
    {
        client?.Dispose();
        network_stream?.Dispose();
        is_connected = true;
    }

}
