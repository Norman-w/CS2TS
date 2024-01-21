namespace CS2TS.SDK.API;

/// <summary>
///     显示C#代码字符串请求
/// </summary>
public class ShowCsCodeStringRequest : BaseRequest<ShowCsCodeStringResponse>
{
	/// <summary>
	///     要显示的C#代码字符串
	/// </summary>
	public string? CsCodeString { get; set; }
}