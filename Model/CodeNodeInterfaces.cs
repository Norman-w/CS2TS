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

public interface IContainer4DefineClass : IExtendAble
{
	// List<Container4Class> GetClasses();
	public List<Class> Classes { get; }
}

public interface IContainer4DefineFunction
{
	// List<Function> GetFunctions();
	public List<Function> Functions { get; }
}

public interface IContainer4DefineNamespace : IExtendAble
{
	// List<NameSpace> GetNamespaces();
	public List<Namespace> Namespaces { get; }
}

public interface IContainer4DefineInterface : IExtendAble
{
	// List<Container4Interface> GetInterfaces();
	public List<Interface> Interfaces { get; }
}

public interface IContainer4DefineVariable
{
	// List<Variable> GetVariables();
	public List<Variable> Variables { get; }
}

public interface IContainer4DefineEnum
{
	// List<EnumDefine> GetEnums();
	public List<EnumDefine> Enums { get; }
}

public interface IContainer4DefineRecord
{
	// List<Record> GetRecords();
	public List<Record> Records { get; }
}

public interface IContainer4DefineDelegate
{
	// List<Delegate> GetDelegates();
	public List<Delegate> Delegates { get; }
}

public interface IContainer4DefineUsing
{
	// List<Using> GetUsings();
	public List<Using> Usings { get; }
}