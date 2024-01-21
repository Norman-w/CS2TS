namespace CS2TS;

public partial class CSharpCodeParser
{
  
  /// <summary>
  /// 当前缓存的是否为注释行开头字符
  /// </summary>
  /// <returns></returns>
  private bool IsNotesLineStartWord()
  {
    return _tempWord.ToString().Trim().StartsWith("//");
  }

  /// <summary>
  /// 当前缓存的是否为注释区域开始字符
  /// </summary>
  /// <returns></returns>
  private bool IsNotesAreaStartWord()
  {
    return _tempWord.ToString().Trim().StartsWith("/*");
  }

  /// <summary>
  /// 当前缓存的字符,是否为#region这样的C#注释开头字符
  /// </summary>
  /// <returns></returns>
  private bool IsSharpLineStartWord()
  {
    return _tempWord.ToString().Trim().StartsWith("#");
  }

}
