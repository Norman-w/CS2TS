using CS2TS.Model;

namespace CS2TS;

/// <summary>
/// 类,内部可以包含 变量/字段/属性 方法,类   不能包含接口,命名空间
/// </summary>
public class Class : Interface,
  IClassContainer, IVariableContainer
{
  public List<Class> GetClasses()
  {
    return GetNodes<Class>();
  }

  public List<Variable> GetVariables()
  {
    return GetNodes<Variable>();
  }

  public Class(string name, List<string> extends) : base(name, extends)
  {
  }
}
