using CS2TS.Model;
using CS2TS.Model.Node;

namespace CS2TS;

public class Function : CodeNode
{
  /// <summary>
  /// 方法的名称
  /// </summary>
  public string Name { get; set; } = "un set function name";
  /// <summary>
  /// 是否为静态方法
  /// </summary>
  public bool IsStatic { get; set; }
  /// <summary>
  /// 方法的权限,比如 public private protected
  /// </summary>
  public PermissionEnum? Permission { get; set; }
  /// <summary>
  /// 方法的入参,默认是空数组,但不是null
  /// </summary>
  public List<Parameter> InParameters { get; set; } = new List<Parameter>();
  /// <summary>
  /// 方法的返回值,当方法是构造函数的时候,是没有返回值的 该属性为null
  /// </summary>
  public Parameter? ReturnParameter { get; set; }

  /// <summary>
  /// 该方法是否为重写方法
  /// </summary>
  public bool IsOverride { get; set; }

  // /// <summary>
  // /// 标记函数是否有函数体，如果没有函数体，那么他就直接是一个函数名称的定义。
  // /// 在ts中，类内的所有重名函数都是先定义他的函数头，然后再最下面定义一个可以匹配到所有这些头的一个广义的函数
  // /// 所以在生成ts时，就算本身从cs中解析来的同名函数有不同的体，也在处理的时候把他标记为 没有结构 也就是没有大括号里面的内容。
  // /// 然后通过判断这个字段来确定他的体处理完毕了没有。
  // /// </summary>
  // public bool HasStructure { get; set; }

  //public bool @int { get; set; }
  /// <summary>
  /// 语句段集合,比如 if, else , else if , switch, a=b , calc() 之类的语句段.
  /// 默认是空数组,不是null,因为一般的函数里面都是有内容的,就算void里面可以不写东西,一般也都会写一个return;的.
  /// </summary>

  /// <summary>
  /// 是否为构造函数
  /// </summary>
  public bool IsConstructor { get; set; }
  public List<Statement> Statements { get; set; } = new List<Statement>();
}
