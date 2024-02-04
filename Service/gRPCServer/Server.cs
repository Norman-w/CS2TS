using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Reflection;
using ProtoBuf.Grpc.Server;

namespace CS2TS.Service.gRPCServer;

public class Server
{
	public Server()
	{
		//启动grpc服务
		var thread = new Thread(InitGrpc);
		thread.Start();
	}

	/// <summary>
	///     在单独的线程中启动grpc服务
	/// </summary>
	private void InitGrpc()
	{
		//因为grpc的app.Run()方法是一个阻塞的方法,所以需要在单独的线程中启动

		#region 服务配置

		const int port = 5001;

		#endregion

		#region 构建builder

		var builder = WebApplication.CreateBuilder();
		//设置端口和监听协议
		builder.WebHost.UseKestrel(options =>
		{
			options.ListenAnyIP(port, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
		});

		//增加跨域支持
		builder.Services.AddCors(o => o.AddPolicy("AllowAll", corsPolicyBuilder =>
		{
			corsPolicyBuilder.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
				.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
		}));

		#endregion

		#region 注册builder服务

		//增加grpc服务
		builder.Services.AddCodeFirstGrpc();
		//增加grpc反射服务
		builder.Services.AddCodeFirstGrpcReflection();

		#endregion

		#region 构建app

		using var app = builder.Build();

		#endregion

		#region 服务注册

		//get
		app.MapGet("/",
			() =>
				"使用gRPC客户端访问gRPC服务,不能使用get请求. 看到此文本,说明gRPC服务已经启动成功. 请使用gRPC客户端访问gRPC服务.");

		//启用跨域
		app.UseCors("AllowAll");
		//注册grpc服务
		app.MapGrpcService<IToClientNoResponse>();
		app.MapGrpcService<IToClientNeedResponse>();
		app.MapGrpcService<IToServerNoResponse>();
		app.MapGrpcService<IToServerNeedResponse>();

		#endregion

		#region 开控制台输出proto文件的内容

		var schema = new SchemaGenerator();
		var toClientNeedResponse = schema.GetSchema<IToClientNeedResponse>();
		var toClientNoResponse = schema.GetSchema<IToClientNoResponse>();
		var toServerNeedResponse = schema.GetSchema<IToServerNeedResponse>();
		var toServerNoResponse = schema.GetSchema<IToServerNoResponse>();

		AutoGenerateProtoFile<IToClientNeedResponse>(schema);
		AutoGenerateProtoFile<IToClientNoResponse>(schema);
		AutoGenerateProtoFile<IToServerNeedResponse>(schema);
		AutoGenerateProtoFile<IToServerNoResponse>(schema);
		Console.WriteLine("已将proto文件写入到protos文件夹中");

		Console.WriteLine("以下是通过C#代码优先的cs文件生成的proto文件的内容:");
		Console.WriteLine(toClientNeedResponse);
		Console.WriteLine(toClientNoResponse);
		Console.WriteLine(toServerNeedResponse);
		Console.WriteLine(toServerNoResponse);

		#endregion

		#region 输出初始化完成准备启动的提示

		Console.BackgroundColor = ConsoleColor.DarkGreen;
		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine("gRPC服务初始化完成,准备启动");
		Console.ResetColor();

		#endregion

		#region 服务启动

		//启动服务
		app.Run();

		#endregion
	}

	private void AutoGenerateProtoFile<T>(SchemaGenerator schemaGenerator)
	{
		var type = typeof(T);
		//去掉I,然后首字母小写,就是proto文件的文件名
		var fileName = type.Name.Substring(1, 1).ToLower() + type.Name.Substring(2);
		var fileNameWithExtension = fileName + ".proto";
		var schema = schemaGenerator.GetSchema<T>();
		//获取当前运行文件夹,然后../../../三级可以获取到csproj文件所在的文件夹
		var currentDirectory = Directory.GetCurrentDirectory();
		if (currentDirectory == null) throw new Exception("获取当前运行文件夹失败");

		var projectDirectory = Directory.GetParent(currentDirectory)?.Parent?.Parent;
		if (projectDirectory == null) throw new Exception("获取项目文件夹失败");

		//判断protos文件夹是否存在
		var protosDirectory = Path.Combine(projectDirectory.FullName, "protos");
		if (!Directory.Exists(protosDirectory))
			//如果不存在,则创建
			Directory.CreateDirectory(protosDirectory);

		//然后判断protos文件夹中是否存在GrpcService.proto文件,也就是为IGrpcService服务生成的proto文件
		var protoFilePath = Path.Combine(protosDirectory, fileNameWithExtension);

		//写入proto文件的内容
		File.WriteAllText(protoFilePath, schema);
	}
}