/*


 CS2TS作为服务提供给flutter等项目使用,flutter等项目通过调用server的api或者是通过websocket获取视图数据
 或者Service.Server推送给flutter视图项目数据等,都通过这个类来进行管理.
 当前是使用Websocket来进行数据传输,后续可能还会提供http接口,或者是grpc,mqtt等接口,都可以通过这个类来进行管理.


 ServerMockExtensions使Server类的对象具备了一些mock的功能,可以直接调用类似server.MockShowCsCode()来进行调用测试代码.
 通过这种方式,可以在不需要flutter等项目的情况下,直接进行测试,验证代码的正确性.
 在正式发布的时候,可以将这些代码注释掉,或者是通过条件编译来进行控制,这样就可以保证发布的时候不会包含这些测试代码了.

*/


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
		Console.WriteLine("暂时屏蔽");
		// var nextSegment = Segment.PickFromCodeString(csCodeString[cursorPosition..]);
		// cursorPosition += nextSegment.Length;
		// // server.AddSegments(new List<Segment> { nextSegment });
		// Console.WriteLine($"已经发送了代码内容:{nextSegment.Content},当前游标位置:{cursorPosition}");
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
		Console.WriteLine("暂时屏蔽");
		// // var nextSegment = Segment.PickFromCodeString(csCodeString[cursorPosition..]);
		// var nextSegment = Segment.PickFromCodeString(csCodeString, ref cursorPosition);
		// // server.AddSegments(new List<Segment> { nextSegment2 });
		// segments.Add(nextSegment);
		// var mergedSegment = Segments.MergeBackwards(nextSegment,
		// 	segments.Count > 1 ? segments.GetRange(0, segments.Count - 1) : new List<Segment>(),
		// 	out var mergeSegmentCount,
		// 	out var mergedTotalSegmentCharCount);
		// cursorPosition += mergeSegmentCount > 0 ? (int)mergedTotalSegmentCharCount : nextSegment.Length;
		// //移除掉吃掉的segment
		// var currentIndex = segments.Count - 1;
		// segments.RemoveRange(currentIndex - (int)mergeSegmentCount, (int)mergeSegmentCount);
		// //替换成合并完的segment
		// segments[^1] = mergedSegment;
		// // Console.WriteLine($"已经发送了代码内容:{mergedSegment.Content},当前游标位置:{cursorPosition}");
		// Console.Write(mergedSegment.Content);
	}

	public static void MockParseOutAllSegmentsAndMergeBackward(this Server server, ref int cursorPosition,
		ref string csCodeString, ref List<Segment> segments)
	{
		segments.Clear();
		var cursorX = 0;
		var cursorY = 0;
		var segmentIndexOfWholeCodeString = 0;
		var allSegments = Segment.PickAllSegmentsFromWholeCodeString(csCodeString, ref cursorY, ref cursorX,
			ref segmentIndexOfWholeCodeString);
		segments.AddRange(allSegments);
		//粘连
		server.MockTryMergeAllBackward(segments);
		Console.WriteLine("");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("解析完毕");
		Console.ResetColor();
		//
		// server.MockPrintAllVisibleSegments(segments);
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
		//TODO
		// 都完事儿了以后向前这样的不中,比如 现在是 = , 之前是?? 那么虽然有 ??= 但是没有 ? = 所以不能合并
		// 应该解析完了一个segment以后就直接向前尝试合并
		//
		// 或者就算是统一合并的话 是不是需要向后合并方法, 根据当前字符找出以这个开头的然后 所有的都试一下往后
		// 比如 ? 找到 ?? 和 ??= 然后看后面的连起来的话能不能匹配,能匹配则往后走
		// var mergedSegments = new List<Segment>();
		/*


		 2024年01月23日19:50:14 通过预演向后合并的方式,发现有问题,比如 ?出现了,后面可以有 ?? 和 ??= 那怎么确定合并到哪个上面去
		 还是说我这个方法就只能匹配具备一种可能性的时候,用来确定可以和后面的合并与否?
		 一段示意代码,准备向后合并的方法

		 /// <summary>
		   ///     向后合并
		   /// </summary>
		   /// <returns></returns>
		   public static List<Segment> MergeForward(Segment segment,
		   List<Segment> nextSegments,
		   out uint mergeSegmentCount,
		   out uint mergedTotalSegmentCharCount)
		   {
		   /*
		   大概步骤:
		   先检查参数有效性,然后看能否直接最短路线返回.
		   再找以当前这个segment开头的Segments里面的static的集合(比当前的segment长的)
		   * /
		   }

		   那么我们是不是有多个可以返回的,如果是多个的话,这样确认的意义是什么尚不清楚,也许以后用的到.但是目前来看:
		   我应该专注于在处理完了任何一个语义以后尝试向前合并.


        */

		#region 之前的反向合并,从最后一个开始

		// var index = segments.Count - 1;
		// while (index >= 0)
		// {
		// 	var currentSegment = segments[index];
		//
		// 	//提取segments的index之前的所有segment
		// 	var previousSegments = segments.GetRange(0, index);
		//
		// 	index--;
		// 	var mergedSegment = Segments.MergeBackwards(currentSegment, previousSegments, out var mergeSegmentCount,
		// 		out var mergedTotalSegmentCharCount);
		// 	// mergedSegments.Add(mergedSegment);
		// 	//移除掉吃掉的segment
		// 	segments.RemoveRange(index + 1 - (int)mergeSegmentCount, (int)mergeSegmentCount);
		// 	//替换成合并完的segment
		// 	segments[^1] = mergedSegment;
		// }

		#endregion

		#region 新版本正向合并,从第一个开始,但是也是往前合并

		var segmentsCopy = new List<Segment>(segments);
		var index = 0;
		do
		{
			var currentSegment = segmentsCopy[index];


			//提取segments的index之前的所有segment
			var previousSegments = segmentsCopy.GetRange(0, index);

			#region MyRegion

			var thisSeg = currentSegment;
			if (index < 1)
			{
				index++;
				continue;
			}

			var previousSeg = previousSegments[^1];

			//??
			if (thisSeg.Content == "?" && previousSeg.Content == "?")
			{
			}

			#endregion

			var mergedSegment = Segments.MergeBackwards(currentSegment, previousSegments, out var mergeSegmentCount,
				out var mergedTotalSegmentCharCount);

			if (mergeSegmentCount == 0 || mergedTotalSegmentCharCount == 0)
			{
				//如果没有合并,那么就迭代器+1
				index++;
				continue;
			}
			//如果第 0,1,2,3 里面的 3把2给吃掉了,那就移除2,然后把3放到2的位置,3的长度增加吃掉的2的长度
			//如果第 0,1,2,3 里面的 3把2,1给吃掉了,那就移除1,2,然后把3放到1的位置,3的长度增加吃掉的2,1的长度
			//以此类推
			//1,移除
			//2,换位

			//1
			segmentsCopy.RemoveRange(index - (int)mergeSegmentCount, (int)mergeSegmentCount);
			index -= (int)mergeSegmentCount;
			//2
			segmentsCopy[index] = mergedSegment;
			//迭代器+
			index++;
		} while (index < segmentsCopy.Count);

		segments.Clear();
		segments.AddRange(segmentsCopy);

		#endregion

		Console.WriteLine("");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("合并完毕");
		Console.ResetColor();

		#region 输出,高亮显示2个字符和3个字符的segment

		//获取所有Segments静态类中的字段,里面是2个字符的segment
		var staticSegments = Segments.StaticSegments;
		//2个字符的segment
		var twoCharSegments = staticSegments.Where(s => s.Length == 2).ToList();
		//3个以上字符的segment
		var threeCharSegments = staticSegments.Where(s => s.Length >= 3).ToList();
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