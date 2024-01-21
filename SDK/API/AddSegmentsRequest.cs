using CS2TS.Model;

namespace CS2TS.SDK.API;

/// <summary>
///     向客户端显示C#代码字符串的最小语义单元集合(增量)
/// </summary>
public class AddSegmentsRequest : BaseRequest<AddSegmentsResponse>
{
	public List<Segment> Segments { get; set; } = new();
}