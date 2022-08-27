using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTcpClient : MonoBehaviour
{
    public void OnMouseDown()
    {
        Debug.Log("Client");
        MyTcpServer.OnInactive();
    }
}
