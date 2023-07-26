namespace CS2TS;

public partial class CSharpCodeParser
{
  
  /// <summary>
  /// 传入一个或者多个字符,确认他是不是断词符号.
  /// </summary>
  /// <param name="c"></param>
  /// <returns></returns>
  private bool IsWordBreakSymbol(string c)
  {
    return _splitWords.Contains(c);
  }

  /// <summary>
  /// 检查当前待处理的内容最后一项是否为断句符号
  /// </summary>
  /// <param name="current"></param>
  /// <param name="breakBy"></param>
  /// <returns></returns>
  private bool IsSentenceBreakWord(out string current, out string breakBy)
  {
    foreach (var v in _breakWords)
    {
      var checking = _tempWord.ToString();
      if (checking.Length == 0 && _unProcessWords.Count>0)
      {
        checking = _unProcessWords[^1];
      }
      // var checking = _unProcessWords.Count > 0 ? _unProcessWords[^1] : _tempWord.ToString();
      if (checking.EndsWith(v) == true)
      {
        current = v;
        breakBy = checking;
        return true;
      }
    }

    current = null;
    breakBy = null;
    return false;
  }

}
