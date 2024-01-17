// ReSharper disable InconsistentNaming

namespace CS2TS;

public class Constant
{
	public static List<string> WhiteSpaceChars = new()
	{
		" ",
		"\n",
		"\t",
		"\r",
		"\f",
		"\v"
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
				public const string ControlPanel = "/csCodeViewer";

				/// <summary>
				///     日志查看器,可以通过Logger这个路径来连接一个日志查看器,这样产生日志的时候,就可以通过这个日志查看器来实时查看日志了
				/// </summary>
				public const string CsCodeViewer = "/logger";

				/// <summary>
				///     控制面板,可以通过ControlPanel这个路径来连接一个控制面板,这样就可以通过控制面板来控制服务器了
				/// </summary>
				public const string Logger = "/controlPanel";
			}
		}
	}
}