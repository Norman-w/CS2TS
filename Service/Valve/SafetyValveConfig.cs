using CS2TS.Service.WebSocketServer;

namespace CS2TS.Service.Valve;

public class SafetyValveConfig
{
	/// <summary>
	///     api对应的阀门
	/// </summary>
	public Dictionary<string, List<IValve>> ApiValves { get; set; } = new();

	/// <summary>
	///     在SafetyValveManager中的方法对应的阀门
	/// </summary>
	public Dictionary<string, List<IValve>> FunctionValves { get; set; } = new();
}