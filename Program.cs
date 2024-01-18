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
					server.MockShowCsCode(ref cursorPosition, ref csCodeString);
					break;

				#endregion

				#region 输入s,将csCodeString解析一个最小语义单元(segment),然后进行显示

				case "s":
					server.MockParseOutASegment(ref cursorPosition, ref csCodeString);
					break;

				#endregion

				#region 输入ss,将csCodeString解析成一个最小语义单元(segment),并尝试和前一个segment粘连 然后进行显示

				case "ss":
					server.MockParseOutASegmentAndMergeBackward(ref cursorPosition, ref csCodeString, ref segments);
					break;

				#endregion

				#region 输入sa将csCodeString全部解析,也就是segment all,并且会尝试和前一个或多个segment粘连 然后进行显示

				case "sa":
					server.MockParseOutAllSegmentsAndMergeBackward(ref cursorPosition, ref csCodeString, ref segments);
					break;

				#endregion

				#region 输入pa, print all 将所有的segment打印出来

				case "pa":
					server.MockPrintAllVisibleSegments(segments);
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