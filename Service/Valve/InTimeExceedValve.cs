namespace CS2TS.Service.Valve;

/// <summary>
///     规定时间内超过多少值阀
/// </summary>
public class InTimeExceedValve<T> : ITimeSpanValve, IValueValve<T>
{
	public InTimeExceedValve()
	{
		Value = default(T) ?? throw new InvalidOperationException();
		Threshold = default(T) ?? throw new InvalidOperationException();
	}

	public InTimeExceedValve(T threshold)
	{
		Value = default(T) ?? throw new InvalidOperationException();
		Threshold = threshold;
	}

	public Guid Id => Guid.NewGuid();
	public string Name => "规定时间内超过多少值阀";
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
	public T Value { get; set; }
	public T Threshold { get; set; }

	public TryToTriggerResult TryToTrigger(Func<T, T>? changeValueCallback, Func<T, T, bool>? compareCallback)
	{
		var result = new TryToTriggerResult();
		if (changeValueCallback != null)
		{
			var newValue = changeValueCallback(Value);
			Value = newValue;
		}

		if (compareCallback != null)
		{
			var triggered = compareCallback(Value, Threshold);
			if (triggered)
			{
				TriggerCount++;
				LastTriggerTime = DateTime.Now;
				result.OverloadHandleType = OverloadHandleType;
			}
		}

		return result;
	}
}