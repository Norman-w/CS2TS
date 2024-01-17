/*

 目前使用Flutter作为代码显示器.
 这个类表示一个代码显示器的实例,可与其进行交互.

*/

using CS2TS.Model;
using CS2TS.SDK.API;
using CS2TS.SDK.WebSocket;
using CS2TS.Service.WebSocketServer;

namespace CS2TS.Service;

public class CsCodeViewerCommunicator
{
	private readonly ClientManager _clientManager;

	public CsCodeViewerCommunicator(ClientManager clientManager)
	{
		_clientManager = clientManager;
	}

	/// <summary>
	///     显示C#代码字符串
	/// </summary>
	/// <param name="csCodeString"></param>
	/// <returns></returns>
	public bool ShowCsCodeString(string csCodeString)
	{
		var req = new ShowCsCodeStringRequest
		{
			CsCodeString = csCodeString
		};
		//封装成 request package
		var package = new RequestPackage<ShowCsCodeStringResponse>
		{
			ApiName = req.GetApiName(), Data = req, RequestId = Guid.NewGuid().ToString(),
			SendTime = DateTime.Now, Initiator = EnumRequestInitiator.Server
		};
		if (_clientManager.CsCodeViewerClientList.Count == 0)
		{
			Console.WriteLine("没有连接的客户端");
			return false;
		}

		var lastClient = _clientManager.CsCodeViewerClientList.Last().Value;
		var stringContent = package.ToJson();
		Console.WriteLine($"发送给客户端:{lastClient.WebSocketConnection}的数据:{stringContent}");
		_clientManager.Server?.SendAsync(lastClient.WebSocketConnection, stringContent);
		Console.WriteLine("发送成功");
		return true;
	}

	/// <summary>
	///     显示C#代码字符串的最小语义单元集合,全量,将会清空原有的语义单元集合
	/// </summary>
	/// <param name="segments"></param>
	/// <returns></returns>
	public bool ShowSegments(List<Segment> segments)
	{
		var req = new ShowSegmentsRequest
		{
			Segments = segments
		};
		//封装成 request package
		var package = new RequestPackage<ShowSegmentsResponse>
		{
			ApiName = req.GetApiName(), Data = req, RequestId = Guid.NewGuid().ToString(),
			SendTime = DateTime.Now, Initiator = EnumRequestInitiator.Server
		};
		if (_clientManager.CsCodeViewerClientList.Count == 0)
		{
			Console.WriteLine("没有连接的客户端");
			return false;
		}

		var lastClient = _clientManager.CsCodeViewerClientList.Last().Value;
		var stringContent = package.ToJson();
		Console.WriteLine($"发送给客户端:{lastClient.WebSocketConnection}的数据:{stringContent}");
		_clientManager.Server?.SendAsync(lastClient.WebSocketConnection, stringContent);
		Console.WriteLine("发送成功");
		return true;
	}

	/// <summary>
	///     显示C#代码字符串的最小语义单元集合,增量,不会清空原有的语义单元集合
	/// </summary>
	/// <param name="segments"></param>
	/// <returns></returns>
	public bool AddSegments(List<Segment> segments)
	{
		var req = new AddSegmentsRequest
		{
			Segments = segments
		};
		//封装成 request package
		var package = new RequestPackage<AddSegmentsResponse>
		{
			ApiName = req.GetApiName(), Data = req, RequestId = Guid.NewGuid().ToString(),
			SendTime = DateTime.Now, Initiator = EnumRequestInitiator.Server
		};
		if (_clientManager.CsCodeViewerClientList.Count == 0)
		{
			Console.WriteLine("没有连接的客户端");
			return false;
		}

		var lastClient = _clientManager.CsCodeViewerClientList.Last().Value;
		var stringContent = package.ToJson();
		Console.WriteLine($"发送给客户端:{lastClient.WebSocketConnection}的数据:{stringContent}");
		_clientManager.Server?.SendAsync(lastClient.WebSocketConnection, stringContent);
		Console.WriteLine("发送成功");
		return true;
	}
}