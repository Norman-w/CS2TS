namespace CS2TS.Model;

//
// public interface ISequenceContainer
// {
//   List<CodeNodeSequenceInfo> Sequences { get; }
// }
//
// public interface INotesContainer
// {
//   List<NoteBase> Notes { get; }
// }
public interface IExtendAble : ICodeNode
{
	public string Name { get; set; }
}

public interface IContainer4Class : IExtendAble
{
	// List<Container4Class> GetClasses();
	public List<Class> Classes { get; }
}

public interface IContainer4Function
{
	// List<Function> GetFunctions();
	public List<Function> Functions { get; }
}

public interface IContainer4Namespace : IExtendAble
{
	// List<NameSpace> GetNamespaces();
	public List<Namespace> Namespaces { get; }
}

public interface IContainer4Interface : IExtendAble
{
	// List<Container4Interface> GetInterfaces();
	public List<Interface> Interfaces { get; }
}

public interface IContainer4Variable
{
	// List<Variable> GetVariables();
	public List<Variable> Variables { get; }
}

public interface IContainer4Enum
{
	// List<EnumDefine> GetEnums();
	public List<EnumDefine> Enums { get; }
}

public interface IContainer4Record
{
	// List<Record> GetRecords();
	public List<Record> Records { get; }
}

public interface IContainer4Delegate
{
	// List<Delegate> GetDelegates();
	public List<Delegate> Delegates { get; }
}

public interface IContainer4Using
{
	// List<Using> GetUsings();
	public List<Using> Usings { get; }
}