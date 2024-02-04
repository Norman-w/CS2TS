using System.Runtime.Serialization;
using System.ServiceModel;

namespace CS2TS.Service.gRPCServer;

/// <summary>
///     服务端向客户端发送消息,告诉客户端自己的名称
/// </summary>
[DataContract]
public class ToClientSeverNameNotice
{
	[DataMember(Order = 1)] public string ServerName { get; set; }
}

/// <summary>
///     向客户端发送消息,要求获取客户端名称的请求
/// </summary>
[DataContract]
public class ToClientGetClientNameRequest
{
	[DataMember(Order = 1)] public string ServerName { get; set; }
}

/// <summary>
///     向客户端发送消息,要求获取客户端名称的响应,客户端响应自己的名称和时间
/// </summary>
[DataContract]
public class ToClientGetClientNameResponse
{
	[DataMember(Order = 1)] public string ClientName { get; set; }

	[DataMember(Order = 2)] public long ClientTime { get; set; }
}

/// <summary>
///     客户端发给服务器的请求,服务器不需要响应,告诉服务器自己的名称
/// </summary>
[DataContract]
public class ToServerClientNameNotice
{
	[DataMember(Order = 1)] public string ClientName { get; set; }
}

/// <summary>
///     向服务器发送消息,要求获取服务端名称的请求
/// </summary>
[DataContract]
public class ToServerGetServerNameRequest
{
	[DataMember(Order = 1)] public string ClientName { get; set; }
}

/// <summary>
///     向服务器发送消息,要求获取服务端名称的响应,服务端响应自己的名称和时间
/// </summary>
[DataContract]
public class ToServerGetServerNameResponse
{
	[DataMember(Order = 1)] public string ServerName { get; set; }

	[DataMember(Order = 2)] public long ServerTime { get; set; }
}

[ServiceContract(Name = "ToClientNeedResponse")]
public interface IToClientNeedResponse
{
	ToClientGetClientNameResponse GetClientName(ToClientGetClientNameRequest request);
}

[ServiceContract(Name = "ToClientNoResponse")]
public interface IToClientNoResponse
{
	void ToClientServerNameNotice(ToClientSeverNameNotice request);
}

[ServiceContract(Name = "ToServerNeedResponse")]
public interface IToServerNeedResponse
{
	ToServerGetServerNameResponse GetServerName(ToServerGetServerNameRequest request);
}

[ServiceContract(Name = "ToServerNoResponse")]
public interface IToServerNoResponse
{
	void ToServerClientNameNotice(ToServerClientNameNotice request);
}

public class ToClientNeedResponse : IToClientNeedResponse
{
	public ToClientGetClientNameResponse GetClientName(ToClientGetClientNameRequest request)
	{
		return new ToClientGetClientNameResponse
		{
			ClientName = "ClientName",
			ClientTime = DateTime.Now.Ticks
		};
	}
}

public class ToClientNoResponse : IToClientNoResponse
{
	public void ToClientServerNameNotice(ToClientSeverNameNotice request)
	{
		Console.WriteLine($"ServerName:{request.ServerName}");
	}
}

public class ToServerNeedResponse : IToServerNeedResponse
{
	public ToServerGetServerNameResponse GetServerName(ToServerGetServerNameRequest request)
	{
		return new ToServerGetServerNameResponse
		{
			ServerName = "ServerName",
			ServerTime = DateTime.Now.Ticks
		};
	}
}

public class ToServerNoResponse : IToServerNoResponse
{
	public void ToServerClientNameNotice(ToServerClientNameNotice request)
	{
		Console.WriteLine($"ClientName:{request.ClientName}");
	}
}