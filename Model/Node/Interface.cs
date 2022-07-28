using CS2TS.Model;

namespace CS2TS;


public class Interface : VariableWithStructure,
  IInterfaceContainer, IFunctionContainer,IVariableContainer
{
  public List<Function> GetFunctions()
  {
    return GetNodes<Function>();
  }

  public List<Variable> GetVariables()
  {
    return GetNodes<Variable>();
  }

  public Interface(string name,
    List<string> extends) : base(name, null, PermissionEnum.Public,null,null,null,null,null, extends,null,null)
  {
  }

  public List<Interface> GetInterfaces()
  {
    return GetNodes<Interface>();
  }
}
