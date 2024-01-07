namespace CS2TS;

public abstract class BaseResponse : IResponse
{
	protected BaseResponse(string? requestId = null)
	{
		RequestId = requestId;
	}

	public string? RequestId { get; set; }
	public string? ErrMsg { get; set; }
	public string? ErrCode { get; set; }
}