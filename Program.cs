// using CS2TS.Helper;
//
// using System.Text;
// using CS2TS.Model;
// using Newtonsoft.Json;


using CS2TS.Model;
using CS2TS.Service;

namespace CS2TS;

public static class Program
{
	public static void Main(string[] args)
	{
		var needClose = false;

		#region 开启服务端

		var server = new Server();
		//已经解析的缓存的segments
		var segments = new List<Segment>();
		//文件
		var csCodeString = "";
		//当前游标位置
		var cursorPosition = 0;
		//循环监听用户的输入
		while (!needClose)
		{
			var input = Console.ReadLine();
			input = input?.ToLower() ?? "";
			input = input.Trim();
			//针对不同的输入进行不同的处理


			switch (input)
			{
				#region 输入l,load并显示csCodeString

				case "l":
					var exePath = Environment.CurrentDirectory;
					var testCodeFilePath = "../../../Test/_1_InTestCSFiles/Test.cs";
					testCodeFilePath = Path.Combine(exePath, testCodeFilePath);
					var fileContent = File.ReadAllText(testCodeFilePath);
					server.ShowCsCodeString(fileContent);
					Console.WriteLine("已经发送了代码");
					cursorPosition = 0;
					csCodeString = fileContent;
					break;

				#endregion

				#region 输入s,将csCodeString解析一个最小语义单元(segment),然后进行显示

				case "s":
					var nextSegment = NewParser.GetNextSegment(csCodeString, cursorPosition);
					cursorPosition += nextSegment.Length;
					// server.AddSegments(new List<Segment> { nextSegment });
					Console.WriteLine($"已经发送了代码内容:{nextSegment.Content},当前游标位置:{cursorPosition}");
					break;

				#endregion

				#region 输入ss,将csCodeString解析成一个最小语义单元(segment),并尝试和前一个segment粘连 然后进行显示

				case "ss":
					var nextSegment2 = NewParser.GetNextSegment(csCodeString, cursorPosition);
					// server.AddSegments(new List<Segment> { nextSegment2 });
					segments.Add(nextSegment2);
					var mergedSegment = Segments.MergeBackwards(nextSegment2,
						segments.Count > 1 ? segments.GetRange(0, segments.Count - 1) : new List<Segment>(),
						out var mergeSegmentCount,
						out var mergedTotalSegmentCharCount);
					cursorPosition += mergeSegmentCount > 0 ? (int)mergedTotalSegmentCharCount : nextSegment2.Length;
					//移除掉吃掉的segment
					var currentIndex = segments.Count - 1;
					segments.RemoveRange(currentIndex - (int)mergeSegmentCount, (int)mergeSegmentCount);
					//替换成合并完的segment
					segments[^1] = mergedSegment;
					Console.WriteLine($"已经发送了代码内容:{mergedSegment.Content},当前游标位置:{cursorPosition}");
					break;

				#endregion

				#region 退出

				case "exit":
					Console.WriteLine("退出");
					needClose = true;
					break;

				#endregion

				default:
					Console.WriteLine("输入l,load并显示csCodeString");
					break;
			}
		}

		#endregion

		/*
		var ps = new CSharpCodeParser();
		// var testFile = @"/Users/coolking/Downloads/TestCsFiles23/Domain/Bag/BagItemsAlias.cs";
		// var testFile = @"/Volumes/NormanData/Visual Studio 2008/Projects/速配项目/QP/Domain/Account/AccountGroup.cs";
		// var testFile = @"../../../TestCSFiles/Test.cs";
		var testFile = @"../../../TestCSFiles/TestCalc.cs";
		if (args != null && args.Length > 0) testFile = args[0];
		var codeFile = ps.ParseCsFile(testFile);
		// var json = JsonConvert.SerializeObject(codeFile, Formatting.Indented);
		// Console.WriteLine(json);

		// CodeNodeHelper hp = new CodeNodeHelper(codeFile);
		// hp.FindClass(codeFile, "aSubSub");

		return;
		var generator = new TypeScriptCodeGenerator(codeFile);
		var tsCode = generator.CreateTsFile();
		Console.WriteLine(tsCode);
		*/
	}
}