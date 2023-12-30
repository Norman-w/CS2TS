namespace CS2TS;

public class FileLocation : SegmentLocationBase
{
	public readonly ClassLocation Class = ClassLocation.Instance;
	public readonly EnumLocation Enum = new();
	public readonly NamespaceLocation Namespace = NamespaceLocation.Instance;
	public readonly StructLocation Struct = new();
	public readonly RecordLocation Record = new();
	public readonly InterfaceLocation Interface = InterfaceLocation.Instance;
	public readonly DelegateLocation Delegate = new();
	public readonly MethodLocation Method = new();
}