using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Chat : MonoBehaviour
{
    private string ip = "127.0.0.1";
    private int port = 5555;

    private Rect server_btn_rect = new Rect(10, 10, 100, 25);
    private Rect client_btn_rect = new Rect(120, 10, 100, 25);
    private Rect log_rect = new Rect(280, 10, 200, 25);
    private Rect message_field_rect = new Rect(10, 40, 300, 25);
    private Rect message_btn_rect = new Rect(350, 40, 100, 25);
    private Rect chat_message_rect = new Rect(10, 70, 500, 50);
    private Color server_btn_color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    private Color client_btn_color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
    private Color client_connected_btn_color = new Color(0.0f, 1.0f, 1.0f, 1.0f);

    public bool is_server = false;
    public bool is_client_connected = false;
    private string log_message = "";
    private string send_message = "";
    private List<string> chat_messages = new List<string>();
    private Server server;
    private Client client;

    void Start()
    {
        Screen.SetResolution(480, 270, false, 60);
        server = new Server(event_callback);
        client = new Client(event_callback);
    }

    void OnGUI()
    {
        set_server_btn_color();
        if (GUI.Button(server_btn_rect, "SERVER"))
        {
            is_server = true;
            Task.Run(() => server.start(port));
        }
        set_client_btn_color();
        if (GUI.Button(client_btn_rect, "CLIENT"))
        {
            is_server = false;
            Task.Run(() => client.start(ip, port));
        }
        display_log();
        display_send_message_form();
        display_chat_messages();
    }

    void display_log()
    {
        GUI.Label(log_rect, log_message);
    }

    void display_send_message_form()
    {
        send_message = GUI.TextField(message_field_rect, send_message, 50);
        if (GUI.Button(message_btn_rect, "SEND"))
        {
            if (is_server)
            {
                server.send(send_message);
            }
            else
            {
                client.send(send_message);
            }
        }
    }

    void display_chat_messages()
    {
        Rect rect = chat_message_rect;
        float y = rect.y;
        for (int i = 0; i < chat_messages.Count; i++)
        {
            rect.y = y + 25 * i;
            GUI.Label(rect, chat_messages[i]);
        }
    }

    private void init_btn_color()
    {
        GUI.backgroundColor = Color.white;
        GUI.color = Color.white;
    }

    private void set_server_btn_color()
    {
        init_btn_color();
        if (is_server)
        {
            GUI.color = server_btn_color;
        }
    }

    private void set_client_btn_color()
    {
        init_btn_color();
        if (!is_server)
        {
            GUI.color = client_btn_color;
            if (is_client_connected)
            {
                GUI.color = client_connected_btn_color;
            }
        }
    }

    private void event_callback(TcpEvent tcp_event)
    {
        if (tcp_event.type == TcpEventType.Received)
        {
            if (is_server)
            {
                server.send(tcp_event.msg);
            }
            else
            {
                add_chat_message(tcp_event.msg);
            }
        }
        else if (tcp_event.type == TcpEventType.Sent)
        {
            if (is_server)
            {
                add_chat_message(tcp_event.msg);
            }
        }
        log_message = tcp_event.msg;
    }

    private void add_chat_message(string msg)
    {
        chat_messages.Insert(0, msg);
    }

}

