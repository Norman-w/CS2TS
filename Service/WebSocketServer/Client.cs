using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace CS2TS.Service.WebSocketServer;

/// <summary>
///     WebSocket每个连接的客户端会话
/// </summary>
public class Client
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public WebSocket WebSocketConnection { get; set; } = null!;
	public Socket Socket { get; set; } = null!;
	public DateTime ConnectTime { get; set; } = DateTime.Now;
	public DateTime LastActiveTime { get; set; } = DateTime.Now;

	public bool IsOnline { get; set; }

	//RemoteEndPoint
	public EndPoint RemoteEndPoint { get; set; } = null!;
}