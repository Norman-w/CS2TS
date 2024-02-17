using CS2TS.Model;
using CS2TS.Model.Node;
using CS2TS.Model.Words;

namespace CS2TS;

public class Event : CodeNode
{
	public Event(string name)
	{
		Name = name;
	}

	public string Name { get; set; }
	public override Segment? CodeNodeTypeSegment => CodeNodeTypeSegments.Event;
}