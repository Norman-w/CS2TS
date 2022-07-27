using CS2TS.Model;

namespace CS2TS;


/// <summary>
/// 枚举类型的定义.
/// </summary>
public class EnumDefine : VariableWithStructure,
  IVariableContainer
{
  public List<Variable> GetVariables()
  {
    return GetNodes<Variable>();
  }
  public EnumDefine(string name,
    PermissionEnum? permission,
    List<string> extends,
    Function getter,
    Function setter) : base(name, null, permission, null,null,null,null, null, extends, getter, setter)
  {
  }
}
