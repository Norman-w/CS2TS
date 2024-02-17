using CS2TS.Model;
using CS2TS.Model.Node;
using CS2TS.Model.Words;

namespace CS2TS;

public class Struct : ContainerCodeNode
{
	public Struct(string name)
	{
		Name = name;
	}

	public string Name { get; set; }
	public override Segment CodeNodeTypeSegment => CodeNodeTypeSegments.Struct;

	public override List<Type> SonCodeNodeValidTypes => new()
	{
		typeof(Variable),
		typeof(Function)
	};
}