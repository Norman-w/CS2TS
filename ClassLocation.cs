namespace CS2TS;

public class ClassLocation : SegmentLocationBase
{
	private ClassLocation()
	{
	}
	public static ClassLocation Instance { get; } = new();
	public readonly ClassLocation Class = Instance;
	public readonly InterfaceLocation Interface = InterfaceLocation.Instance;
	public readonly StructLocation Struct = new();
	public readonly RecordLocation Record = new();
	public readonly EnumLocation Enum = new();
	public readonly DelegateLocation Delegate = new();
	public readonly MethodLocation Method = new();
	public readonly PropertyLocation Property = new();
	public readonly FieldLocation Field = new();
	public readonly EventLocation Event = new();
	public readonly ConstructorLocation Constructor = new();
	public readonly DestructorLocation Destructor = new();
	public readonly OperatorLocation Operator = new();
	public readonly IndexerLocation Indexer = new();
}