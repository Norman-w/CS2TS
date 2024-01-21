namespace CS2TS;

public class NamespaceLocation : SegmentLocationBase
{
	public static NamespaceLocation Instance { get; } = new();
	public readonly NamespaceLocation Namespace = Instance;
	public readonly ClassLocation Class = ClassLocation.Instance;
	public readonly InterfaceLocation Interface = InterfaceLocation.Instance;
	public readonly StructLocation Struct = new();
	public readonly RecordLocation Record = new();
	public readonly EnumLocation Enum = new();
	public readonly DelegateLocation Delegate = new();
}