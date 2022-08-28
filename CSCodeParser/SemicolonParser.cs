using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CS2TS;

public class SemicolonParser
{
  public static bool Parse2Statements(List<string>? words, Function parent)
  {
    // List<Statement> ret = new List<Statement>();
    if (IsReturnStatement(words,parent))
    {
      parent.Chirldren.Add(Parse2ReturnStatement(words,parent));
      return true;
    }
    //如果前一个是return,当前这个应该是变量的调用(函数也是变量),然后继续往下走
    // else if (parent is ReturnStatement && words.Count > 0 && words[0] != ";")
    // {
    //   ReturnStatement returnStatement = parent as ReturnStatement;
    //   if (words.Count > 1)
    //   {
    //     if (words[1] == "(")
    //     {
    //       returnStatement.Value = new FunctionCallStatement();
    //     }
    //     else if (words[1] == "[")
    //     {
    //       returnStatement.Value = new ArrayUseStatement();
    //     }
    //     else if (words[1] == ".")
    //     {
    //       returnStatement.Value = new VariableUseStatement();
    //     }
    //     else
    //     {
    //       returnStatement.Value = new VariableUseStatement();
    //     }
    //   }
    // }
    return false;
  }
  //判断当前这些词,是不是return句
  private static bool IsReturnStatement(List<string>? words, CodeNode parent)
  {
    if (!(parent is Function) || words == null || words.Count == 0 || words.IndexOf("return") != 0)
    {
      return false;
    }
    return true;
  }
  private static ReturnStatement Parse2ReturnStatement(List<string>? words, Function parent)
  {
    ReturnStatement ret = new ReturnStatement();
    // words.RemoveAt(0);
    // if (words is not {Count: > 0})
    //   return ret;
    // var leader = words[0];
    // if (leader == ";")
    // {
    // }
    // else
    // {
    //   //如果都不是下钻,就是新定义当前为参数使用
    //   ret.Value = Parse2VariableUseStatement(words, ret);
    // }
    // return ret;
    
    
    //return 语句就原样照搬就可以了.
    StringBuilder str = new StringBuilder();
    foreach (var word in words)
    {
      if (str.Length > 0)
        str.Append(' ');
      str.Append(word);
    }
    ret.Value = str.ToString().Replace(" ;", ";");
    return ret;
  }
  private static VariableUseStatement Parse2VariableUseStatement(List<string> words, Statement parent)
  {
    var ret = new VariableUseStatement()
    {
      Name = words[0]
    };
    //用掉name,然后剩下的就是 [1-1之类的]
    words.RemoveAt(0);
    
    //看看领头的是什么,如果是[进入数组 如果是(进入函数 如果是.进入子级访问
    var leader = words[0];
    if (leader == "[")
    {
      ret = Parse2ArrayUseStatement(words, ret);
    }
    else if (leader == "(")
    {
      ret = Parse2FunctionCallStatement(words, ret);
    }
    else if (leader == ".")
    {
      ret.Chirldren.Add(Parse2VariableUseStatement(words,ret));
      //仍然是函数调用.
    }
    return ret;
  }
  private static ArrayUseStatement Parse2ArrayUseStatement(List<string> words, Statement parent)
  {
    //进来的时候应该是[开头的,如果开始是0 结束是2 那中间就一个元素 那就简单了.
    //如果开始0 结束是大于2的 那就复杂了.
    var start = words.IndexOf("[");
    var end = words.IndexOf("]");

    var ret = new ArrayUseStatement();
    if (end - start == 2)
    {
      words.RemoveAt(0);
      //简单一点的  array use 的下标直接是一个常量或者是一个变量引用
      // ret.Value = new VariableNoStructure(null, null, null, null, null, null, null, null, null);
      ret.Value = new VariableUseStatement()
      {
        Name = words[0]
      };
    }
    else
    {
      //这样就比较复杂一点了.
      words.RemoveAt(0);
      ret.Value = Parse2VariableUseStatement(words, ret);
    }
    return ret;
  }
  private static FunctionCallStatement Parse2FunctionCallStatement(List<string> words, Statement parent)
  {
    return null;
  }
  // private static Statement Parse2ReturnStatementVariable(List<string> words)
  // {
  //   return VariableBuilder.Parse2Variable(words);
  // }
}
