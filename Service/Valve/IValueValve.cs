using CS2TS.Service.WebSocketServer;

namespace CS2TS.Service.Valve;

/// <summary>
///     值阀
/// </summary>
public interface IValueValve<T> : IValve
{
	/// <summary>
	///     值,当前值
	/// </summary>
	public T Value { get; set; }

	//阈值
	public T Threshold { get; set; }

	/// <summary>
	///     回调函数
	/// </summary>
	/// <param name="changeValueCallback"></param>
	/// <param name="compareCallback"></param>
	/// <returns></returns>
	public TryToTriggerResult TryToTrigger(Func<T, T>? changeValueCallback, Func<T, T, bool>? compareCallback);
}