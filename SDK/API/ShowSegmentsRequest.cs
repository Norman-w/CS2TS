using CS2TS.Model;

namespace CS2TS.SDK.API;

/// <summary>
///     向客户端显示C#代码字符串的最小语义单元集合(全量,将会清空原有的语义单元集合)
/// </summary>
public class ShowSegmentsRequest : BaseRequest<ShowSegmentsResponse>
{
	public List<Segment> Segments { get; set; } = new();
}