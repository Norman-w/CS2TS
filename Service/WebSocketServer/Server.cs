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
	///     客户端连接事件
	/// </summary>
	public delegate Task ClientWebSocketOnConnectedEventHandler(WebSocketContext webSocketContext);

	/// <summary>
	///     客户端断开连接事件
	/// </summary>
	public delegate Task ClientWebSocketOnDisconnectedEventHandler(WebSocketContext webSocketContext);

	/// <summary>
	///     客户端收到消息事件
	/// </summary>
	public delegate Task ClientWebSocketOnMessageEventHandler(WebSocketContext webSocketContext, byte[] data);

	public delegate Task ErrorEventHandler(WebSocketContext? webSocketContext, Exception e);

	/// <summary>
	///     服务器启动事件
	/// </summary>
	public delegate void ServerStartedEventHandler(Server server);

	public delegate void ServerStoppedEventHandler(Server server);

	/// <summary>
	///     服务器监听的端口
	/// </summary>
	private readonly int _listenPort;

	/// <summary>
	///     发送队列
	/// </summary>
	private readonly ConcurrentDictionary<WebSocketContext, ConcurrentQueue<byte[]>> _sends = new();

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
	public event ClientWebSocketOnConnectedEventHandler? OnClientConnected;

	/// <summary>
	///     用户已退出事件
	/// </summary>
	public event ClientWebSocketOnDisconnectedEventHandler? OnClientDisconnected;

	/// <summary>
	///     收到用户消息事件
	/// </summary>
	public event ClientWebSocketOnMessageEventHandler? OnClientMessage;

	public event ErrorEventHandler? OnError;

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
		WebSocketContext? webSocketContext = null;
		try
		{
			webSocketContext = await httpListenerContext.AcceptWebSocketAsync("csCodeViewer");
			Console.WriteLine("新的链接:{0}", webSocketContext.SecWebSocketKey);
			OnClientConnected?.Invoke(webSocketContext);
			var buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024 * 1);
			try
			{
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
					_ = Task.Run(() => { OnClientMessage?.Invoke(webSocketContext, data); });
					bufferData.Clear();
				}
			}
			catch (Exception e)
			{
				OnOnError(webSocketContext, e);
			}
			finally
			{
				Console.WriteLine("释放buffer,因为客户端已经退出");
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}
		catch (Exception e)
		{
			OnOnError(webSocketContext, e);
		}
		finally
		{
			try
			{
				if (webSocketContext != null) OnClientDisconnected?.Invoke(webSocketContext);
				else //并没有成功建立连接所以没有webSocketContext
					Console.WriteLine("webSocketContext为空,无法触发OnClientDisconnected事件");
			}
			catch (Exception e)
			{
				OnOnError(webSocketContext, e);
			}
		}
	}

	/// <summary>
	///     向客户端发送字符串,默认UTF8编码
	/// </summary>
	/// <param name="webSocketContext"></param>
	/// <param name="data"></param>
	/// <returns></returns>
	public void SendAsync(WebSocketContext webSocketContext, string data)
	{
		SendAsync(webSocketContext, Encoding.UTF8.GetBytes(data));
	}

	/// <summary>
	///     向客户端发送消息,可发送二进制数据
	/// </summary>
	/// <param name="webSocketContext"></param>
	/// <param name="data"></param>
	/// <returns></returns>
	public void SendAsync(WebSocketContext webSocketContext, byte[] data)
	{
		try
		{
			_sends.AddOrUpdate(webSocketContext, new ConcurrentQueue<byte[]>(new List<byte[]> { data }),
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
					if (!_sends.TryGetValue(webSocketContext, out var queue)) return;
					if (queue.TryDequeue(out var bytes))
						await webSocketContext.WebSocket.SendAsync(new ArraySegment<byte>(bytes),
							WebSocketMessageType.Text, true,
							CancellationToken.None);
					else
						break;
				}
			});
		}
		catch (Exception)
		{
			OnOnError(webSocketContext, null);
			Console.WriteLine("发送消息失败");
		}
	}

	protected virtual void OnOnError(WebSocketContext? webSocketContext, Exception? e)
	{
		if (e == null)
		{
			Console.WriteLine("客户端:{0}发生错误", webSocketContext?.SecWebSocketKey);
			return;
		}

		OnError?.Invoke(webSocketContext, e);
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