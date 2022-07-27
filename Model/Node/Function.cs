using CS2TS.Model;

namespace CS2TS;

public class Function : CodeNode
{
  public string Name { get; set; }
  public bool IsStatic { get; set; }
  public PermissionEnum? Permission { get; set; }
  public List<Parameter> InParameters { get; set; }
  public Parameter ReturnParameter { get; set; }

  public bool IsOverride { get; set; }

  //public bool @int { get; set; }
  /// <summary>
  /// 语句段集合
  /// </summary>
  public List<Statement> Statements { get; set; }
}
