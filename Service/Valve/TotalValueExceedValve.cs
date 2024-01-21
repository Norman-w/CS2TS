namespace CS2TS.Service.Valve;

/// <summary>
///     总值超过多少阀
/// </summary>
public class TotalValueExceedValve<T> : IValueValve<T>
{
	public TotalValueExceedValve()
	{
		Value = default(T) ?? throw new InvalidOperationException();
		Threshold = default(T) ?? throw new InvalidOperationException();
	}

	public TotalValueExceedValve(T threshold)
	{
		Value = default(T) ?? throw new InvalidOperationException();
		Threshold = threshold;
	}

	public Guid Id => Guid.NewGuid();
	public string Name => "总值超过多少阀";
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