// ReSharper disable InconsistentNaming

namespace CS2TS;

public class CONSTANT
{
	private static CONSTANT? _instance;

	/// <summary>
	///     String常量
	/// </summary>
	public STRING String = new();

	/// <summary>
	///     实例定义
	/// </summary>
	public static CONSTANT Instance => _instance ??= new CONSTANT();

	/// <summary>
	///     String常量
	/// </summary>
	public class STRING
	{
		/// <summary>
		///     WebSocket的路径
		/// </summary>
		public WEB_SOCKET WebSocket = new();

		/// <summary>
		///     WebSocket的路径
		/// </summary>
		public class WEB_SOCKET
		{
			/// <summary>
			///     WebSocket的路径/子协议
			/// </summary>
			public SUB_PROTOCOL SubProtocol = new();

			/// <summary>
			///     WebSocket的路径/子协议
			/// </summary>
			public class SUB_PROTOCOL
			{
				/// <summary>
				///     C# cs代码查看器
				/// </summary>
				public const string CS_CODE_VIEWER = "/csCodeViewer";

				/// <summary>
				///     日志查看器,可以通过Logger这个路径来连接一个日志查看器,这样产生日志的时候,就可以通过这个日志查看器来实时查看日志了
				/// </summary>
				public const string LOGGER = "/logger";

				/// <summary>
				///     控制面板,可以通过ControlPanel这个路径来连接一个控制面板,这样就可以通过控制面板来控制服务器了
				/// </summary>
				public const string CONTROL_PANEL = "/controlPanel";

				public string ControlPanel = CONTROL_PANEL;
				public string CsCodeViewer = CS_CODE_VIEWER;
				public string Logger = LOGGER;
			}
		}
	}
}