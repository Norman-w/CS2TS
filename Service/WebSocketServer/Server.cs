using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace CS2TS.Service.WebSocketServer;

/// <summary>
///     WebSocketServer
/// </summary>
public class Server
{
	/// <summary>
	///     服务器启动事件
	/// </summary>
	public delegate void ServerStartedEventHandler(Server server);

	public delegate void ServerStoppedEventHandler(Server server);

	public delegate void WebSocketErrorHandler(Client webSocket, Exception e);

	/// <summary>
	///     WebSocket事件
	/// </summary>
	public delegate void WebSocketHandler(Client client, byte[]? data);

	/// <summary>
	///     服务器监听的端口
	/// </summary>
	private readonly int _listenPort;

	/// <summary>
	///     发送队列
	/// </summary>
	private readonly ConcurrentDictionary<WebSocket, ConcurrentQueue<byte[]>> _sends = new();

	/// <summary>
	///     服务端监听对象
	/// </summary>
	private HttpListener? _listener;

	public Server(int port)
	{
		_listenPort = port;
	}

	/// <summary>
	///     新用户已连接到服务器事件
	/// </summary>
	public event WebSocketHandler? OnOpen;

	/// <summary>
	///     用户已退出事件
	/// </summary>
	public event WebSocketHandler? OnClose;

	/// <summary>
	///     收到用户消息事件
	/// </summary>
	public event WebSocketHandler? OnMessage;

	public event WebSocketErrorHandler? OnError;

	public event ServerStartedEventHandler? OnServerStarted;

	public event ServerStoppedEventHandler? OnServerStopped;


	/// <summary>
	///     监听
	/// </summary>
	/// <returns></returns>
	public Server Listen()
	{
		try
		{
			_listener = new HttpListener();
			_listener.Prefixes.Add($"http://*:{_listenPort}/");
			_listener.Start();
			OnServerStarted?.Invoke(this);
			Task.Run(async () =>
			{
				while (true)
				{
					var httpListenerContext = await _listener.GetContextAsync();
					if (!httpListenerContext.Request.IsWebSocketRequest) return;

					//新的链接已经建立,通过线程池处理,执行Accept方法
					ThreadPool.QueueUserWorkItem(
						r => { _ = Accept(httpListenerContext); }
					);
				}
			});
			//监听退出事件
			AppDomain.CurrentDomain.ProcessExit += (_, _) =>
			{
				Console.WriteLine("服务端退出,端口:{0}", _listenPort);
				OnServerStopped?.Invoke(this);
			};
		}
		catch (HttpListenerException e)
		{
			if (e.ErrorCode == 5)
			{
				var msg = "请以管理员身份运行程序或者输入: netsh http add urlacl url=http://*:" + _listenPort + "/ user=Everyone";
				Console.WriteLine(msg);
				throw new Exception(msg);
			}
		}

		return this;
	}

	/// <summary>
	///     接受客户端的请求
	/// </summary>
	private async Task Accept(HttpListenerContext httpListenerContext)
	{
		var webSocketContext = await httpListenerContext.AcceptWebSocketAsync("csCodeViewer");
		var client = new Client
		{
			ConnectTime = DateTime.Now,
			WebSocketConnection = webSocketContext.WebSocket,
			RemoteEndPoint = httpListenerContext.Request.RemoteEndPoint
		};
		try
		{
			NewAcceptHandler(client);
			var buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024 * 1);
			try
			{
				// var webSocket = client.WebSocketConnection;
				var webSocket = webSocketContext.WebSocket;
				var bufferData = new List<byte>();
				while (webSocket.State == WebSocketState.Open)
				{
					var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
					if (result.MessageType == WebSocketMessageType.Close)
						await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
							"Closing", CancellationToken.None);
					bufferData.AddRange(buffer.Take(result.Count).ToArray());
					if (!result.EndOfMessage) continue;
					var data = bufferData.ToArray();
					_ = Task.Run(() => { OnMessage?.Invoke(client, data); });
					bufferData.Clear();
				}
			}
			finally
			{
				Console.WriteLine("释放buffer,因为客户端已经退出");
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			OnOnError(client, null);
		}
		finally
		{
			NewQuitHandler(client);
		}
	}

	/// <summary>
	///     新用户连接
	/// </summary>
	private void NewAcceptHandler(Client client)
	{
		OnOpen?.Invoke(client, null);
		// //发送消息告诉他你上来了
		// SendAsync(webSocketSession, "你已作为客户端连接到服务器,来源:" + webSocketSession.RemoteEndPoint);
	}

	/// <summary>
	///     用户退出
	/// </summary>
	private void NewQuitHandler(Client? client)
	{
		if (client == null) return;
		OnClose?.Invoke(client, null);
		// Console.WriteLine("退出链接:" + webSocketSession.RemoteEndPoint);
	}

	/// <summary>
	///     向客户端发送字符串,默认UTF8编码
	/// </summary>
	/// <param name="token"></param>
	/// <param name="data"></param>
	/// <returns></returns>
	public void SendAsync(Client token, string data)
	{
		SendAsync(token, Encoding.UTF8.GetBytes(data));
	}

	/// <summary>
	///     向客户端发送消息,可发送二进制数据
	/// </summary>
	/// <param name="client"></param>
	/// <param name="data"></param>
	/// <returns></returns>
	public void SendAsync(Client client, byte[] data)
	{
		try
		{
			_sends.AddOrUpdate(client.WebSocketConnection, new ConcurrentQueue<byte[]>(new List<byte[]> { data }),
				(_, v) =>
				{
					v.Enqueue(data);
					return v;
				});
			//发送消息
			_ = Task.Run(async () =>
			{
				while (true)
				{
					if (!_sends.TryGetValue(client.WebSocketConnection, out var queue)) return;
					if (queue.TryDequeue(out var bytes))
						await client.WebSocketConnection.SendAsync(new ArraySegment<byte>(bytes),
							WebSocketMessageType.Text, true,
							CancellationToken.None);
					else
						break;
				}
			});
		}
		catch (Exception)
		{
			OnOnError(client, null);
			Console.WriteLine("发送消息失败");
		}
	}

	protected virtual void OnOnError(Client client, Exception? e)
	{
		if (e == null)
		{
			Console.WriteLine("客户端:{0}发生错误", client.RemoteEndPoint);
			return;
		}

		OnError?.Invoke(client, e);
	}

	public bool Stop()
	{
		try
		{
			_listener?.Stop();
			return true;
		}
		catch (Exception)
		{
			Console.WriteLine("停止失败");
			return false;
		}
	}
}