namespace CS2TS;

public enum StatementType
{
  Structure,
  VariableDefine,
  VariableSet,
  Return,
}
// /// <summary>
// /// 语句段的类型,带大括号的
// /// </summary>
// public enum StatementStructureType
// {
//   IF,
//   ELSE_IF,
//   ELSE,
//   DO,
//   WHILE,
//   FOR,
//   FOREACH,
//   NONE,
// }

/// <summary>
/// 语句段 if  else 之类的 或者可能就是普通的一句  a=b; 后续在搞清各项关系的时候会逐步的拓展.
/// </summary>
public class Statement : CodeNode
{
  public string CodeBody { get; set; }

  /// <summary>
  /// 语句段集合。if里面还可以嵌套if
  /// </summary>
  // public List<Statement> Statements { get; set; }

  /// <summary>
  /// 什么类型的 比如 if  else if else while switch
  /// </summary>
  public string Type { get; set; }
}

public class ReturnStatement : Statement
{
  public Variable Value { get; set; }
}