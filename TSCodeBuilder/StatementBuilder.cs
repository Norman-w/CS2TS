using System.Text;

namespace CS2TS;

public class StatementBuilder
{
  public static string BuildStatement(Statement statement, Function parent, string tab)
  {
    var ret = new StringBuilder();
    if (statement is ReturnStatement)
    {
      var rrs = statement as ReturnStatement;
      return rrs.Value;
    }
    return ret.ToString();
  }
}
