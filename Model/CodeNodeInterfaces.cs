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
public interface IExtendAble
{

}
public interface IClassContainer :IExtendAble
{
  public string Name { get; set; }
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

public interface IInterfaceContainer :IExtendAble
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

