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
      ret.Append(tab).Append('\t').Append("return");
      if (rrs.Value != null)
      {
        // ret.Append(' ').Append(rrs.Value.Name);
      }
      ret.AppendLine(";");
    }
    return ret.ToString();
  }
}
