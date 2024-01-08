using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;

namespace CS2TS.SDK;

public class WebSocketServer
{
	#region 字段

	//单例模式
	public static WebSocketServer Instance { get; } = new();


	//WebSocket cs代码查看器客户端列表
	private readonly Dictionary<string, WebSocket?> _csCodeViewerClientList = new();

	//cs代码查看器客户端锁
	private readonly object _csCodeViewerClientListLock = new();

	private string AddCsCodeViewerClient(WebSocket? client)
	{
		lock (_csCodeViewerClientListLock)
		{
			var clientId = Guid.NewGuid().ToString();
			_csCodeViewerClientList.Add(clientId, client);
			return clientId;
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
	private readonly Dictionary<string, WebSocket?> _loggerClientList = new();

	//日志查看器客户端锁
	private readonly object _loggerClientListLock = new();

	private string AddLoggerClient(WebSocket? client)
	{
		lock (_loggerClientListLock)
		{
			var clientId = Guid.NewGuid().ToString();
			_loggerClientList.Add(clientId, client);
			return clientId;
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
	///     WebSocket客户端连接回调函数
	/// </summary>
	/// <param name="client"></param>
	/// <returns></returns>
	public async Task OnClientConnected(WebSocket? client)
	{
		string? clientId;
		switch (client?.SubProtocol)
		{
			//判断是cs代码查看器客户端还是日志查看器客户端
			case CONSTANT.STRING.WEB_SOCKET.SUB_PROTOCOL.CS_CODE_VIEWER:
			{
				//cs代码查看器客户端
				lock (_csCodeViewerClientListLock)
				{
					clientId = AddCsCodeViewerClient(client);
				}

				break;
			}
			case CONSTANT.STRING.WEB_SOCKET.SUB_PROTOCOL.LOGGER:
			{
				//日志查看器客户端
				lock (_loggerClientListLock)
				{
					clientId = AddLoggerClient(client);
				}

				break;
			}
			default:
				//未知客户端
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("未知客户端,关闭连接,客户端协议:" + client?.SubProtocol);
				Console.ResetColor();
				await client?.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, "未知客户端", CancellationToken.None)!;
				return;
		}

		try
		{
			//接收客户端消息
			await Receive(client);
		}
		catch (Exception e)
		{
			Console.ForegroundColor = ConsoleColor.Red;

			Console.WriteLine("接收客户端消息异常:" + e.Message);
			Console.ResetColor();
			throw;
		}
		finally
		{
			// 在连接关闭时，从WebSocket管理器中移除WebSocket
			RemoveCsCodeViewerClient(clientId);
			RemoveLoggerClient(clientId);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("在接收完了数据以后,关闭连接");
			lock (_csCodeViewerClientListLock)
			{
				Console.WriteLine("当前客户端数量:" + _csCodeViewerClientList.Count);
			}

			Console.ResetColor();
			await client.CloseOutputAsync(WebSocketCloseStatus.InvalidPayloadData, "你被干掉了", CancellationToken.None);
		}
	}

	/// <summary>
	///     WebSocket客户端断开回调函数
	/// </summary>
	/// <param name="client"></param>
	/// <returns></returns>
	private async Task OnClientDisconnected(WebSocket? client)
	{
		if (client == null)
		{
			Console.WriteLine("客户端为空");
			return;
		}

		var clientId = client.SubProtocol;
		if (string.IsNullOrEmpty(clientId))
		{
			Console.WriteLine("客户端ID为空");
			return;
		}

		switch (client.SubProtocol)
		{
			//判断客户端类型
			case CONSTANT.STRING.WEB_SOCKET.SUB_PROTOCOL.CS_CODE_VIEWER:
			{
				//cs代码查看器客户端
				lock (_csCodeViewerClientListLock)
				{
					RemoveCsCodeViewerClient(clientId);
				}

				break;
			}
			case CONSTANT.STRING.WEB_SOCKET.SUB_PROTOCOL.LOGGER:
			{
				//日志查看器客户端
				lock (_loggerClientListLock)
				{
					RemoveCsCodeViewerClient(clientId);
				}

				break;
			}
			default:
				//未知客户端
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("未知的客户端~~ default");
				Console.ResetColor();
				await client.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, "未知客户端", CancellationToken.None);
				return;
		}
	}

	/// <summary>
	///     WebSocket客户端接收消息回调函数
	/// </summary>
	/// <param name="client"></param>
	/// <returns></returns>
	public async Task Receive(WebSocket? client)
	{
		try
		{
			if (client == null)
			{
				Console.WriteLine("客户端为空");
				return;
			}

			var buffer = new byte[1024 * 4];
			while (client.State == WebSocketState.Open)
			{
				var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				if (result.MessageType == WebSocketMessageType.Close)
				{
					await OnClientDisconnected(client);
				}
				else if (result.MessageType == WebSocketMessageType.Text)
				{
					var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
					await OnMessageReceived(client, message);
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("读取客户端消息失败,错误信息:" + e.Message);
		}
		//在finally中，判断连接是否关闭，如果没有则Abort掉，然后执行dispose，进而消除潜在的隐患。
		finally
		{
			if (client != null && client.State != WebSocketState.Closed)
			{
				client.Abort();
				client.Dispose();
			}
		}
	}

	#endregion

	#region 接收消息方法

	/// <summary>
	///     接收到客户端消息
	/// </summary>
	/// <param name="client"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	private static async Task OnMessageReceived(WebSocket? client, string message)
	{
		if (client == null)
		{
			Console.WriteLine("客户端为空");
			return;
		}

		switch (client.SubProtocol)
		{
			//判断是cs代码查看器客户端还是日志查看器客户端
			case CONSTANT.STRING.WEB_SOCKET.SUB_PROTOCOL.CS_CODE_VIEWER:
				//游戏登录器客户端
				await OnCsCodeViewerClientMessageReceived(client, message);
				break;
			case CONSTANT.STRING.WEB_SOCKET.SUB_PROTOCOL.LOGGER:
				//游戏服务端客户端
				await OnLoggerClientMessageReceived(client, message);
				break;
			default:
				//未知客户端
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("未知的客户端类型!!!!");
				Console.ResetColor();
				await client.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, "未知客户端", CancellationToken.None);
				return;
		}
	}

	#region cs代码查看器客户端消息

	/// <summary>
	///     cs代码查看器客户端消息
	/// </summary>
	/// <param name="client"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	private static async Task OnCsCodeViewerClientMessageReceived(WebSocket? client, string message)
	{
		//检查client
		if (client == null)
		{
			Console.WriteLine("客户端为空");
			return;
		}

		//message发过来应该是一个json,可以获取到里面的内容解析到对应的request,然后根据request的类型进行处理
		//正则提取 process.list 类似的接口名称 {"apiName":"process.list","xxx":"123456"}
		//需要考虑到 大括号,双引号然后后面跟apiName冒号再双引号  中间是请求名字 再双引号的条件
		//正则表达式
		// 使用正则表达式匹配 "apiName" 值
		var pattern = "\"apiName\":\"(.*?)\"";
		var match = Regex.Match(message, pattern);
		if (!match.Success)
		{
			//没有匹配到
			client.Abort();
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("根据输入的消息没有匹配到apiName,消息内容:" + message);
			Console.ResetColor();
			await client.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, "无效的请求,没有匹配到apiName",
				CancellationToken.None);
			return;
		}

		var apiName = match.Groups[1].Value;
		//获取apiName对应的请求类型
		RequestParser.RequestNameToTypeDic.TryGetValue(apiName, out var requestType);
		//判断是否获取到了请求类型
		if (requestType == null)
		{
			//没有获取到请求类型
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("没有获取到请求类型");
			Console.ResetColor();
			await client.CloseOutputAsync(WebSocketCloseStatus.ProtocolError, "无效的请求,没有匹配到请求类型",
				CancellationToken.None);
		}
	}

	private static async Task OnLoggerClientMessageReceived(WebSocket? client, string message)
	{
	}

	#endregion

	#endregion
}