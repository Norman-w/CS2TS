namespace CS2TS.Service.Valve;

/// <summary>
///     规定时间超过多少次阀
/// </summary>
public class InTimeExceedTimesValve : ITimeSpanValve, ITimesValve
{
	public Guid Id => Guid.NewGuid();
	public string Name => "规定时间超过多少次阀";
	public DateTime? LastTriggerTime { get; set; }
	public int TriggerCount { get; set; }
	public OverloadHandleType OverloadHandleType { get; set; }
	public TimeSpan? SelfResetTimeSpan { get; set; }
	public int SelfResetCount { get; set; }

	public void Reset()
	{
		TriggerCount = 0;
		LastTriggerTime = DateTime.MinValue;
	}

	public TimeSpan TimeSpan { get; set; }

	public int Threshold { get; set; }

	public TryToTriggerResult TryToTrigger()
	{
		var result = new TryToTriggerResult();

		//触发了
		TriggerCount++;
		Console.WriteLine("SafetyValve:InTimeExceedTimesValve触发了,当前次数:" + TriggerCount + ",阈值:" + Threshold);
		LastTriggerTime = DateTime.Now;
		if (TriggerCount <= Threshold) return result;
		Console.ForegroundColor = ConsoleColor.Magenta;
		Console.WriteLine("SafetyValve:InTimeExceedTimesValve达到阈值,当前次数:" + TriggerCount + ",阈值:" + Threshold);
		Console.ResetColor();
		//如果触发了,则赋值OverloadHandleType,表示为触发了
		result.OverloadHandleType = OverloadHandleType;
		return result;
	}
}