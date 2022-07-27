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
public interface IClassContainer
{
  List<Class> GetClasses();
}

public interface IFunctionContainer
{
  List<Function> GetFunctions();
}

public interface INamespaceContainer
{
  List<NameSpace> GetNamespaces();
}

public interface IInterfaceContainer
{
  List<Interface> GetInterfaces();
}

public interface IVariableContainer
{
  List<Variable> GetVariables();
}

public interface IEnumContainer
{
  List<EnumDefine> GetEnums();
}

