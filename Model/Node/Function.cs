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

  // /// <summary>
  // /// 标记函数是否有函数体，如果没有函数体，那么他就直接是一个函数名称的定义。
  // /// 在ts中，类内的所有重名函数都是先定义他的函数头，然后再最下面定义一个可以匹配到所有这些头的一个广义的函数
  // /// 所以在生成ts时，就算本身从cs中解析来的同名函数有不同的体，也在处理的时候把他标记为 没有结构 也就是没有大括号里面的内容。
  // /// 然后通过判断这个字段来确定他的体处理完毕了没有。
  // /// </summary>
  // public bool HasStructure { get; set; }

  //public bool @int { get; set; }
  /// <summary>
  /// 语句段集合
  /// </summary>
  public List<Statement> Statements { get; set; }
}
