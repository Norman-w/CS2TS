namespace CS2TS.Service.Valve;

public class TryToTriggerResult
{
	public string? ErrorMessage { get; set; }
	public bool IsError => !string.IsNullOrWhiteSpace(ErrorMessage);

	public OverloadHandleType? OverloadHandleType { get; set; }
}