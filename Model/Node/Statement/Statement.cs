using System.Reflection.Metadata.Ecma335;

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
  // public List<Statement> Statements { get; set; }

  /// <summary>
  /// 什么类型的 比如 if  else if else while switch
  /// </summary>
  public string Type { get; set; }
}

public class ReturnStatement : Statement
{
  public string Value { get; set; }
}
public class VariableUseStatement : Statement
{
  public string Name { get; set; }
}
public class ArrayUseStatement : VariableUseStatement
{
  public Statement Value { get; set; }
}
public class FunctionCallStatement : VariableUseStatement
{
  public FunctionCallStatement()
  {
    this.Parameters = new List<Statement>();
  }
  public  List<Statement> Parameters { get; }
}