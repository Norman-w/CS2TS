namespace CS2TS;

public class SemicolonParser
{
  public static bool Parse2Statement(List<string> words, CodeNode parent)
  {
    // List<Statement> ret = new List<Statement>();
    if (IsReturnStatement(words,parent))
    {
      parent.Chirldren.Add(Parse2ReturnStatement(words,parent));
      return true;
    }
    return false;
  }
  //判断当前这些词,是不是return句
  private static bool IsReturnStatement(List<string> words, CodeNode parent)
  {
    if (!(parent is Function) || words == null || words.Count == 0 || words.IndexOf("return") != 0)
    {
      return false;
    }
    return true;
  }
  private static ReturnStatement Parse2ReturnStatement(List<string> words, CodeNode parent)
  {
    ReturnStatement ret = new ReturnStatement();
    var nextList = words.GetRange(1, words.Count - 1);
    ret.Value = Parse2ReturnStatementVariable(nextList);
    return ret;
  }
  private static Variable Parse2ReturnStatementVariable(List<string> words)
  {
    return VariableBuilder.Parse2Variable(words);
  }
}
