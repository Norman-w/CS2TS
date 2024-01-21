using System.Net.WebSockets;

namespace CS2TS.Service.WebSocketServer;

public interface IClientManager
{
	/// <summary>
	///     WebSocket客户端连接回调函数
	/// </summary>
	/// <param name="webSocketContext"></param>
	/// <returns></returns>
	Task OnClientWebSocketConnected(WebSocketContext webSocketContext);

	/// <summary>
	///     WebSocket客户端断开回调函数
	/// </summary>
	/// <param name="webSocketContext"></param>
	/// <returns></returns>
	Task OnClientWebSocketDisconnected(WebSocketContext webSocketContext);
}