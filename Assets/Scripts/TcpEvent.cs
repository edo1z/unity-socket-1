public enum TcpEventType
{
    Listening,
    Connected,
    Disconnected,
    Sent,
    Received,
    Destroyed,
}

public delegate void TcpEventHandler(TcpEvent tcp_event);

public class TcpEvent
{
    public TcpEventType type;
    public bool success;
    public string msg;

    public TcpEvent (TcpEventType t, bool s, string m)
    {
      type = t;
      success = s;
      msg = m;
    }
}

