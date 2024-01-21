using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;

namespace CS2TS.Service.WebSocketServer;

/// <summary>
///     WebSocketSession管理器
/// </summary>
public class ClientManager : IClientManager
{
	#region 字段

	private Server? _server;

	/// <summary>
	///     被管理的WebSocketServer对象,在设置的时候,会自动添加事件
	/// </summary>
	public Server? Server
	{
		get => _server;
		set
		{
			_server = value;
			if (_server == null) return;
			//先清空事件
			_server.OnClose -= ServerOnClose;
			_server.OnOpen -= ServerOnOpen;
			_server.OnMessage -= ServerOnMessage;
			_server.OnError -= ServerOnError;

			//再添加事件,防止悬空事件
			_server.OnClose += ServerOnClose;
			_server.OnOpen += ServerOnOpen;
			_server.OnMessage += ServerOnMessage;
			_server.OnError += ServerOnError;
		}
	}

	private void ServerOnClose(WebSocketContext webSocketContext, byte[]? bytes)
	{
		Task.Run(async () => await OnClientWebSocketDisconnected(webSocketContext));
	}

	private void ServerOnOpen(WebSocketContext webSocketContext, byte[]? bytes)
	{
		Task.Run(async () => await OnClientWebSocketConnected(webSocketContext));
	}

	private void ServerOnMessage(WebSocketContext webSocketContext, byte[]? bytes)
	{
		Task.Run(async () => await OnClientMessage(webSocketContext));
	}

	private void ServerOnError(WebSocketContext webSocketContext, Exception e)
	{
		Task.Run(async () => await OnClientWebSocketError(webSocketContext, e));
	}

	//单例模式
	public static ClientManager Instance { get; } = new();

	//WebSocket cs代码查看器客户端列表
	private readonly Dictionary<string, WebSocketContext> _csCodeViewerClientList = new();

	/// <summary>
	///     WebSocket cs代码查看器客户端列表
	/// </summary>
	public Dictionary<string, WebSocketContext> CsCodeViewerClientList
	{
		get
		{
			lock (_csCodeViewerClientListLock)
			{
				return _csCodeViewerClientList;
			}
		}
	}

	//cs代码查看器客户端锁
	private readonly object _csCodeViewerClientListLock = new();


	private void AddCsCodeViewerClient(WebSocketContext webSocketContext)
	{
		lock (_csCodeViewerClientListLock)
		{
			_csCodeViewerClientList.Add(webSocketContext.SecWebSocketKey, webSocketContext);
		}
	}

	private void RemoveCsCodeViewerClient(string connectionId)
	{
		lock (_csCodeViewerClientListLock)
		{
			_csCodeViewerClientList.Remove(connectionId);
		}
	}


	//WebSocket 日志查看器客户端列表
	private readonly Dictionary<string, WebSocketContext?> _loggerClientList = new();

	//日志查看器客户端锁
	private readonly object _loggerClientListLock = new();

	private void AddLoggerClient(WebSocketContext webSocketContext)
	{
		lock (_loggerClientListLock)
		{
			var clientId = Guid.NewGuid().ToString();
			_loggerClientList.Add(clientId, webSocketContext);
		}
	}

	private void RemoveLoggerClient(string connectionId)
	{
		lock (_loggerClientListLock)
		{
			_loggerClientList.Remove(connectionId);
		}
	}

	#endregion

	#region 初始化

	#endregion

	#region 回调函数

	/// <summary>
	///     WebSocket客户端连接回调函数,在单例模式时,可以适用于Swagger等.
	/// </summary>
	/// <returns></returns>
	public async Task OnClientWebSocketConnected(WebSocketContext webSocketContext)
	{
		var webSocket = webSocketContext.WebSocket;
		switch (webSocket.SubProtocol)
		{
			//判断是cs代码查看器客户端还是日志查看器客户端
			case Constant.String.WebSocket.SubProtocol.CsCodeViewer:
			{
				//cs代码查看器客户端
				lock (_csCodeViewerClientListLock)
				{
					AddCsCodeViewerClient(webSocketContext);
				}

				break;
			}
			case Constant.String.WebSocket.SubProtocol.Logger:
			{
				//日志查看器客户端
				lock (_loggerClientListLock)
				{
					AddLoggerClient(webSocketContext);
				}

				break;
			}
			default:
				//未知客户端
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("ClientManager:未知客户端,关闭连接,客户端协议:" + webSocket.SubProtocol);
				Console.ResetColor();
				await webSocket.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, "未知客户端", CancellationToken.None);
				return;
		}

		try
		{
			//接收客户端消息
			await OnClientMessage(webSocketContext);
		}
		catch (Exception e)
		{
			Console.ForegroundColor = ConsoleColor.Red;

			Console.WriteLine("ClientManager:接收客户端消息异常:" + e.Message);
			Console.ResetColor();
			throw;
		}
		finally
		{
			// 在连接关闭时，从WebSocket管理器中移除WebSocket
			RemoveCsCodeViewerClient(webSocketContext.SecWebSocketKey);
			RemoveLoggerClient(webSocketContext.SecWebSocketKey);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("ClientManager:OnClientMessage退出等待");
			lock (_csCodeViewerClientListLock)
			{
				Console.WriteLine("ClientManager:当前客户端数量:" + _csCodeViewerClientList.Count);
			}

			Console.ResetColor();

			try
			{
				if (webSocket.State == WebSocketState.Open)
				{
					Console.WriteLine("ClientManager:正在干掉你");
					await webSocket.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData, "你被干掉了",
						CancellationToken.None);
					Console.WriteLine("ClientManager:你已经被干掉了");
				}
				else
				{
					Console.WriteLine("ClientManager:你不是Open状态,不干掉你");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("ClientManager:干掉你失败");
				Console.WriteLine(e);
				throw;
			}
		}
	}

	/// <summary>
	///     WebSocket客户端断开回调函数
	/// </summary>
	/// <param name="webSocketContext"></param>
	/// <returns></returns>
	public async Task OnClientWebSocketDisconnected(WebSocketContext webSocketContext)
	{
		var webSocket = webSocketContext.WebSocket;

		var clientId = webSocket.SubProtocol;
		if (string.IsNullOrEmpty(clientId))
		{
			Console.WriteLine("ClientManager:客户端ID为空");
			return;
		}

		switch (webSocket.SubProtocol)
		{
			//判断客户端类型
			case Constant.String.WebSocket.SubProtocol.CsCodeViewer:
			{
				//cs代码查看器客户端
				lock (_csCodeViewerClientListLock)
				{
					RemoveCsCodeViewerClient(clientId);
				}

				break;
			}
			case Constant.String.WebSocket.SubProtocol.Logger:
			{
				//日志查看器客户端
				lock (_loggerClientListLock)
				{
					RemoveLoggerClient(clientId);
				}

				break;
			}
			default:
				//未知客户端
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("ClientManager:未知的客户端~~ default");
				Console.ResetColor();
				await webSocket.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, "未知客户端", CancellationToken.None);
				return;
		}
	}

	private Task OnClientWebSocketError(WebSocketContext webSocketContext, Exception e)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine("ClientManager:" + webSocketContext.SecWebSocketKey + "客户端发生错误");
		Console.WriteLine("ClientManager:WebSocket错误:" + e.Message);
		Console.ResetColor();
		return Task.CompletedTask;
	}

	/// <summary>
	///     WebSocket客户端接收消息回调函数
	/// </summary>
	/// <returns></returns>
	public async Task OnClientMessage(WebSocketContext webSocketContext)
	{
		try
		{
			var webSocket = webSocketContext.WebSocket;

			var buffer = new byte[1024 * 4];
			while (webSocket.State == WebSocketState.Open)
			{
				var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				if (result.MessageType == WebSocketMessageType.Close)
				{
					await OnClientWebSocketDisconnected(webSocketContext);
				}
				else if (result.MessageType == WebSocketMessageType.Text)
				{
					var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
					await OnMessageReceived(webSocketContext, message);
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("ClientManager:读取客户端消息失败,错误信息:" + e.Message);
		}
		//在finally中，判断连接是否关闭，如果没有则Abort掉，然后执行dispose，进而消除潜在的隐患。
		finally
		{
			if (webSocketContext.WebSocket.State != WebSocketState.Closed)
			{
				webSocketContext.WebSocket.Abort();
				webSocketContext.WebSocket.Dispose();
			}
		}
	}

	#endregion

	#region 接收消息方法

	/// <summary>
	///     接收到客户端消息
	/// </summary>
	/// <param name="webSocketContext"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	private static async Task OnMessageReceived(WebSocketContext webSocketContext, string message)
	{
		var webSocket = webSocketContext.WebSocket;

		switch (webSocket.SubProtocol)
		{
			//判断是cs代码查看器客户端还是日志查看器客户端
			case Constant.String.WebSocket.SubProtocol.CsCodeViewer:
				//游戏登录器客户端
				await OnCsCodeViewerClientMessageReceived(webSocketContext, message);
				break;
			case Constant.String.WebSocket.SubProtocol.Logger:
				//游戏服务端客户端
				await OnLoggerClientMessageReceived(webSocketContext, message);
				break;
			default:
				//未知客户端
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("ClientManager:未知的客户端类型!!!!");
				Console.ResetColor();
				await webSocket.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, "未知客户端", CancellationToken.None);
				return;
		}
	}

	#region cs代码查看器客户端消息

	/// <summary>
	///     cs代码查看器客户端消息
	/// </summary>
	/// <param name="webSocketContext"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	private static Task OnCsCodeViewerClientMessageReceived(WebSocketContext webSocketContext, string message)
	{
		// var webSocket = webSocketContext.WebSocket;

		//message发过来应该是一个json,可以获取到里面的内容解析到对应的request,然后根据request的类型进行处理
		//正则提取 process.list 类似的接口名称 {"apiName":"process.list","xxx":"123456"}
		//需要考虑到 大括号,双引号然后后面跟apiName冒号再双引号  中间是请求名字 再双引号的条件
		//正则表达式
		// 使用正则表达式匹配 "apiName" 值
		var pattern = "\"apiName\":\"(.*?)\"";
		var match = Regex.Match(message, pattern);
		if (!match.Success)
		{
			Console.WriteLine($"{nameof(ClientManager)}:没匹配到ApiName,消息内容:{message}");
			//没有匹配到
			// webSocket.Abort();//这里暂时不做处理,如果需要处理液是需要在NoticeInvalidRequest里面处理
			Console.ForegroundColor = ConsoleColor.Red;
			if (!SafetyValveManager.NoticeInvalidRequest(webSocketContext, null, out _, out var noticeMessage))
				Console.WriteLine($"{nameof(ClientManager)}{noticeMessage}");

			Console.ResetColor();
			return Task.CompletedTask;
		}

		var apiName = match.Groups[1].Value;
		//获取apiName对应的请求类型
		RequestParser.RequestNameToTypeDic.TryGetValue(apiName, out var requestType);
		//判断是否获取到了请求类型
		if (requestType == null)
		{
			var noticeMessage = $"无效的请求,没有匹配到请求类型,消息内容:{message}";
			//没有获取到请求类型
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"{nameof(ClientManager)}{noticeMessage}");
			Console.ResetColor();
			SafetyValveManager.NoticeInvalidRequest(webSocketContext, null, out _, out _);
		}

		return Task.CompletedTask;
	}

	private static async Task OnLoggerClientMessageReceived(WebSocketContext webSocketContext, string message)
	{
		Console.WriteLine("ClientManager:日志输出器数量:" + Instance._loggerClientList.Count);
		Console.WriteLine("ClientManager:日志查看器客户端消息:" + message);
		Console.WriteLine("ClientManager:Context:" + webSocketContext.SecWebSocketKey);
		await Task.CompletedTask;
	}

	#endregion

	#endregion
}