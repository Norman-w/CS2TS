namespace CS2TS;


/// <summary>
/// 语句段,带结构的语句段，像 if else else if 之类的都算
/// </summary>
public abstract class StatementWithStructure : Statement
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

public class WhileStatement : StatementWithStructure
{
  
}

public class ForStatement : StatementWithStructure
{
  
}

public class ForeachStatement : StatementWithStructure
{
  
}

public class NothingStatement : StatementWithStructure
{
  
}