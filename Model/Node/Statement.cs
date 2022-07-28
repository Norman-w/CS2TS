namespace CS2TS;


/// <summary>
/// 语句段 if  else 之类的 或者可能就是普通的一句  a=b; 后续在搞清各项关系的时候会逐步的拓展.
/// </summary>
public class Statement : CodeNode
{
  public string CodeBody { get; set; }

  /// <summary>
  /// 语句段集合。if里面还可以嵌套if
  /// </summary>
  public List<Statement> Statements { get; set; }

  /// <summary>
  /// 什么类型的 比如 if  else if else while switch
  /// </summary>
  public string Type { get; set; }
}
/// <summary>
/// 语句段,带结构的语句段，像 if else else if 之类的都算
/// </summary>
public class StatementWithStructure : Statement
{

}
/// <summary>
/// if 语句段
/// </summary>
public class IfStatement :StatementWithStructure
{

}
/// <summary>
/// else if 语句段
/// </summary>
public class ElseIfStatement : IfStatement
{

}
/// <summary>
/// else 语句段
/// </summary>
public class ElseStatement: StatementWithStructure
{

}