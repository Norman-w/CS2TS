namespace CS2TS.Service.WebSocketServer;

/// <summary>
///     基础限流阀类
/// </summary>
public interface IValve
{
	/// <summary>
	///     阀门Id
	/// </summary>
	public Guid Id { get; }

	/// <summary>
	///     阀门名称
	/// </summary>
	public string Name { get; }

	/// <summary>
	///     阀门触发时间
	/// </summary>
	public DateTime? LastTriggerTime { get; set; }

	/// <summary>
	///     阀门触发次数,这个值是不会重置的,只会一直增加(自上次重置以来)
	/// </summary>
	public int TriggerCount { get; set; }

	/// <summary>
	///     阀门触发后的处理方式
	/// </summary>
	public OverloadHandleType OverloadHandleType { get; set; }

	/// <summary>
	///     阀门自重置/恢复时间,如果不会自动恢复的话,则为null
	/// </summary>
	public TimeSpan? SelfResetTimeSpan { get; set; }

	/// <summary>
	///     阀门自重置/恢复次数
	/// </summary>
	public int SelfResetCount { get; set; }

	/// <summary>
	///     尝试触发阀门,也就是报告消息给阀门,接收一个回调函数的参数,当需要记录值的时候调用这个回调函数
	/// </summary>
	/// <returns></returns>
	// public NoticeValveResult TryToTrigger<T>(Func<T, T>? recordValueCallback);

	/// <summary>
	///     重置阀门,是重置value不是TriggerCount
	/// </summary>
	public void Reset();
}