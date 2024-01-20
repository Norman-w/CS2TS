namespace CS2TS.Service.WebSocketServer;

public interface IClientManager
{
	/// <summary>
	///     WebSocket客户端连接回调函数
	/// </summary>
	/// <param name="client"></param>
	/// <returns></returns>
	Task OnClientWebSocketConnected(Client? client);

	/// <summary>
	///     WebSocket客户端断开回调函数
	/// </summary>
	/// <param name="client"></param>
	/// <returns></returns>
	Task OnClientWebSocketDisconnected(Client? client);
}