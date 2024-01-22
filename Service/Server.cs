/*


 CS2TS作为服务提供给flutter等项目使用,flutter等项目通过调用server的api或者是通过websocket获取视图数据
 或者Service.Server推送给flutter视图项目数据等,都通过这个类来进行管理.
 当前是使用Websocket来进行数据传输,后续可能还会提供http接口,或者是grpc,mqtt等接口,都可以通过这个类来进行管理.


 ServerMockExtensions使Server类的对象具备了一些mock的功能,可以直接调用类似server.MockShowCsCode()来进行调用测试代码.
 通过这种方式,可以在不需要flutter等项目的情况下,直接进行测试,验证代码的正确性.
 在正式发布的时候,可以将这些代码注释掉,或者是通过条件编译来进行控制,这样就可以保证发布的时候不会包含这些测试代码了.

*/


using System.Reflection;
using CS2TS.Model;
using CS2TS.Service.WebSocketServer;

namespace CS2TS.Service;

public class Server
{
	private readonly ClientManager _clientManager = new();

	/// <summary>
	///     C#代码显示器交互器
	/// </summary>
	private readonly CsCodeViewerCommunicator? _csCodeViewerCommunicator;

	private readonly WebSocketServer.Server _server = new(8000);

	public Server()
	{
		_clientManager.Server = _server;
		_server.OnServerStarted += server =>
		{
			//启动成功
			Console.WriteLine("Server started");
		};
		_server.OnServerStopped += server =>
		{
			//启动失败
			Console.WriteLine("Server stopped");
		};
		_server.Listen();
		_csCodeViewerCommunicator = new CsCodeViewerCommunicator(_clientManager);
	}

	public bool ShowCsCodeString(string code)
	{
		return _csCodeViewerCommunicator?.ShowCsCodeString(code) ?? false;
	}

	public bool ShowSegments(List<Segment> segments)
	{
		return _csCodeViewerCommunicator?.ShowSegments(segments) ?? false;
	}

	public bool AddSegments(List<Segment> segments)
	{
		return _csCodeViewerCommunicator?.AddSegments(segments) ?? false;
	}
}

/// <summary>
///     为了方便测试,提供了一些扩展方法
/// </summary>
public static class ServerMockExtensions
{
	/// <summary>
	///     对server提供扩展,使Server的对象如server直接可以用  server.MockShowCsCode()来进行调用
	///     本方法是模拟用户输入l,load,然后显示代码
	/// </summary>
	/// <param name="server"></param>
	/// <param name="cursorPosition"></param>
	/// <param name="csCodeString"></param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static void MockShowCsCode(this Server server, ref int cursorPosition, ref string csCodeString)
	{
		if (csCodeString == null) throw new ArgumentNullException(nameof(csCodeString));
		if (cursorPosition < 0) throw new ArgumentOutOfRangeException(nameof(cursorPosition));

		var exePath = Environment.CurrentDirectory;
		var testCodeFilePath = "../../../Test/_1_InTestCSFiles/Test.cs";
		testCodeFilePath = Path.Combine(exePath, testCodeFilePath);
		var fileContent = File.ReadAllText(testCodeFilePath);
		server.ShowCsCodeString(fileContent);
		Console.WriteLine("已经发送了代码");
		cursorPosition = 0;
		csCodeString = fileContent;
	}

	/// <summary>
	///     模拟用户输入s,将csCodeString解析一个最小语义单元(segment),然后进行显示
	/// </summary>
	/// <param name="server"></param>
	/// <param name="cursorPosition"></param>
	/// <param name="csCodeString"></param>
	public static void MockParseOutASegment(this Server server, ref int cursorPosition, ref string csCodeString)
	{
		var nextSegment = Segment.PickFromCodeString(csCodeString[cursorPosition..]);
		cursorPosition += nextSegment.Length;
		// server.AddSegments(new List<Segment> { nextSegment });
		Console.WriteLine($"已经发送了代码内容:{nextSegment.Content},当前游标位置:{cursorPosition}");
	}

	/// <summary>
	///     模拟用户输入ss,将csCodeString解析成一个最小语义单元(segment),并尝试和前一个或多个segment粘连 然后进行显示
	/// </summary>
	/// <param name="server"></param>
	/// <param name="cursorPosition"></param>
	/// <param name="csCodeString"></param>
	/// <param name="segments"></param>
	public static void MockParseOutASegmentAndMergeBackward(this Server server, ref int cursorPosition,
		ref string csCodeString, ref List<Segment> segments)
	{
		var nextSegment = Segment.PickFromCodeString(csCodeString[cursorPosition..]);
		// server.AddSegments(new List<Segment> { nextSegment2 });
		segments.Add(nextSegment);
		var mergedSegment = Segments.MergeBackwards(nextSegment,
			segments.Count > 1 ? segments.GetRange(0, segments.Count - 1) : new List<Segment>(),
			out var mergeSegmentCount,
			out var mergedTotalSegmentCharCount);
		cursorPosition += mergeSegmentCount > 0 ? (int)mergedTotalSegmentCharCount : nextSegment.Length;
		//移除掉吃掉的segment
		var currentIndex = segments.Count - 1;
		segments.RemoveRange(currentIndex - (int)mergeSegmentCount, (int)mergeSegmentCount);
		//替换成合并完的segment
		segments[^1] = mergedSegment;
		// Console.WriteLine($"已经发送了代码内容:{mergedSegment.Content},当前游标位置:{cursorPosition}");
		Console.Write(mergedSegment.Content);
	}

	public static void MockParseOutAllSegmentsAndMergeBackward(this Server server, ref int cursorPosition,
		ref string csCodeString, ref List<Segment> segments)
	{
		//持续解析直到cursorPosition到达末尾
		while (cursorPosition < csCodeString.Length)
			//调用MockParseOutASegmentAndMergeBackward,不用单独在这里写一遍了
			server.MockParseOutASegmentAndMergeBackward(ref cursorPosition, ref csCodeString, ref segments);
		Console.WriteLine("");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("解析完毕");
		Console.ResetColor();
	}

	/// <summary>
	///     把所有的可见的字符打印出来,以空格分隔
	/// </summary>
	/// <param name="server"></param>
	/// <param name="segments"></param>
	public static void MockPrintAllVisibleSegments(this Server server, List<Segment> segments)
	{
		foreach (var s in segments.Where(s => s is { IsWhitespace: false, IsLineBreak: false }))
		{
			Console.Write(s.Content);
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.Write(" ");
			Console.ResetColor();
		}

		Console.WriteLine("");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("打印完毕");
		Console.ResetColor();

		Console.WriteLine("正在发送到客户端");
		server.ShowSegments(segments);
		Console.WriteLine("发送完毕");
	}

	public static void MockRemoveAllInvisibleSegments(this Server server, List<Segment> segments)
	{
		segments.RemoveAll(s => s.IsLineBreak || s.IsWhitespace);
		Console.WriteLine("");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("删除完毕");
		Console.ResetColor();
	}

	public static void MockTryMergeAllBackward(this Server server, List<Segment> segments)
	{
		// var mergedSegments = new List<Segment>();
		var index = segments.Count - 1;
		while (index >= 0)
		{
			var currentSegment = segments[index];

			//提取segments的index之前的所有segment
			var previousSegments = segments.GetRange(0, index);

			index--;
			var mergedSegment = Segments.MergeBackwards(currentSegment, previousSegments, out var mergeSegmentCount,
				out var mergedTotalSegmentCharCount);
			// mergedSegments.Add(mergedSegment);
			//移除掉吃掉的segment
			segments.RemoveRange(index + 1 - (int)mergeSegmentCount, (int)mergeSegmentCount);
			//替换成合并完的segment
			segments[^1] = mergedSegment;
		}

		Console.WriteLine("");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("合并完毕");
		Console.ResetColor();

		#region 输出,高亮显示2个字符和3个字符的segment

		//获取所有Segments静态类中的字段,里面是2个字符的segment
		var fields = typeof(Segments).GetFields(BindingFlags.Static | BindingFlags.Public);
		var staticSegments = fields.Select(field => field.GetValue(null)).Cast<Segment>().ToList();
		var duplicateItems = staticSegments.GroupBy(s => s.Content).Where(g => g.Count() > 1)
			.Select(g => g.Key)
			.ToList();
		//抛出异常
		if (duplicateItems.Count > 0)
			throw new Exception($"重复的项:{string.Join(",", duplicateItems)}");
		//2个字符的segment
		var twoCharSegments = staticSegments.Where(s => s.Length == 2).ToList();
		//3个字符的segment
		var threeCharSegments = staticSegments.Where(s => s.Length == 3).ToList();
		foreach (var s in segments.Where(s => s is { IsWhitespace: false, IsLineBreak: false }))
		{
			//2个字符的用粉色
			if (twoCharSegments.Contains(s))
			{
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.BackgroundColor = ConsoleColor.DarkCyan;
			}
			//3个字符的用黄色
			else if (threeCharSegments.Contains(s))
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.BackgroundColor = ConsoleColor.DarkBlue;
			}
			//其他的用白色
			else
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
			}

			Console.Write(s.Content);
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.Write(" ");
			Console.ResetColor();
		}

		#endregion
	}
}