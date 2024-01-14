using System.Net.WebSockets;

namespace CS2TS.SDK.WebSocketServer;

public interface IClientManager
{
	/// <summary>
	///     WebSocket客户端连接回调函数
	/// </summary>
	/// <param name="client"></param>
	/// <returns></returns>
	Task OnClientConnected(WebSocket? client);

	/// <summary>
	///     WebSocket客户端断开回调函数
	/// </summary>
	/// <param name="client"></param>
	/// <returns></returns>
	Task OnClientDisconnected(WebSocket? client);
}