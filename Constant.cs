// ReSharper disable InconsistentNaming

namespace CS2TS;

public class Constant
{
	/// <summary>
	///     非操作符的空白字符,\n不算,因为\n可以终止注释行.虽然他也不可见
	/// </summary>
	public static readonly List<string> NonOperatorWhitespaceChars = new()
	{
		" ",
		"\t",
		"\r",
		"\f",
		"\v"
	};

	private static List<string>? _invisibleChars;

	/// <summary>
	///     不可见字符,包含换行符
	/// </summary>
	/// <returns></returns>
	public static List<string> InvisibleChars =>
		_invisibleChars ??= new List<string>(NonOperatorWhitespaceChars)
		{
			"\n"
		};

	/// <summary>
	///     String常量
	/// </summary>
	public static class String
	{
		/// <summary>
		///     WebSocket的路径
		/// </summary>
		public static class WebSocket
		{
			/// <summary>
			///     WebSocket的路径/子协议
			/// </summary>
			public static class SubProtocol
			{
				/// <summary>
				///     C# cs代码查看器
				/// </summary>
				public const string ControlPanel = "controlPannel";

				/// <summary>
				///     日志查看器,可以通过Logger这个路径来连接一个日志查看器,这样产生日志的时候,就可以通过这个日志查看器来实时查看日志了
				/// </summary>
				public const string CsCodeViewer = "csCodeViewer";

				/// <summary>
				///     控制面板,可以通过ControlPanel这个路径来连接一个控制面板,这样就可以通过控制面板来控制服务器了
				/// </summary>
				public const string Logger = "logger";
			}
		}
	}
}