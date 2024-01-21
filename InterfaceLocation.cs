namespace CS2TS;

public class InterfaceLocation : SegmentLocationBase
{
	private InterfaceLocation()
	{
	}
	public static InterfaceLocation Instance { get; } = new();
	public readonly InterfaceLocation Interface = Instance;
	public readonly StructLocation Struct = new();
	public readonly RecordLocation Record = new();
	public readonly EnumLocation Enum = new();
	public readonly DelegateLocation Delegate = new();
	public readonly MethodLocation Method = new();
	public readonly PropertyLocation Property = new();
	public readonly EventLocation Event = new();
	public readonly IndexerLocation Indexer = new();
	public readonly OperatorLocation Operator = new();
	public readonly ClassLocation Class = ClassLocation.Instance;
	public readonly FieldLocation Field = new();
}