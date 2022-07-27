using CS2TS.Model;

namespace CS2TS;

/// <summary>
/// 命名空间,里面可以包含命名空间,类,接口
/// </summary>
public class NameSpace : CodeNode,
  INamespaceContainer,IClassContainer,IInterfaceContainer,IEnumContainer
{
  public NameSpace(string name)
  {
    Name = name;
  }

  public string Name { get; set; }
  public List<NameSpace> GetNamespaces()
  {
    throw new NotImplementedException();
  }

  public List<Class> GetClasses()
  {
    throw new NotImplementedException();
  }

  public List<Interface> GetInterfaces()
  {
    throw new NotImplementedException();
  }

  public List<EnumDefine> GetEnums()
  {
    throw new NotImplementedException();
  }
}

