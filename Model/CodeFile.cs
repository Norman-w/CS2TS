using CS2TS;
using CS2TS.Model;

public static class Define
{
    /// <summary>
    /// 权限的集合,确定有没有权限定义找这里
    /// </summary>
    public static List<string> Permissions = new List<string>() { "public", "private", "protected", "internal"};
}

public class CodeFile : CodeNode,
  INamespaceContainer, IClassContainer, IEnumContainer, IFunctionContainer
{
  private readonly List<string> _usings = new List<string>();
    // public List<string> Usings { get; set; }
    // public List<NameSpace> Namespaces { get; set; }
    // public List<Class> Classes { get; set; }
    // public List<EnumDefine> Enums { get; set; }
    // public List<Function> Functions { get; set; }
    public List<string> GetUsings()
    {
      return this._usings;
    }

    public List<NameSpace> GetNamespaces()
    {
      return GetNodes<NameSpace>();
    }

    public List<Class> GetClasses()
    {
      return GetNodes<Class>();
    }

    public List<EnumDefine> GetEnums()
    {
      return GetNodes<EnumDefine>();
    }

    public List<Function> GetFunctions()
    {
      return GetNodes<Function>();
    }
}

