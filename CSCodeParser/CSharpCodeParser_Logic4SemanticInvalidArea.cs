using System.Text;

namespace CS2TS;

public partial class CSharpCodeParser
{
  
  #region 尝试把字符放入到当前的无语意区

  /// <summary>
  /// 如果是一个分词符号的时候.尝试添加都的当前的语义无效的区域.如果成功了.字符就可以越过了.如果不成功,继续对这个字符进行别的处理.
  /// </summary>
  /// <param name="currentChar"></param>
  /// <returns></returns>
  private bool TryAppend2SemanticInvalidArea(string currentChar)
  {
    #region 检查是否在注释区域中,在注释中的分隔符被记录到临时文字中的,但是不一定是完成,因为\r\n这样的能结束注释行的 才算是完成符,那时候再收尾.

    if (IsInNotesLine())
    {
      //getCurrentNotesLine().Append(currentChar);
      _tempWord.Append(currentChar);
      return true;
    }

    if (InInNotesArea())
    {
      //getCurrentNotesArea().Lines.Add(currentChar);
      _tempWord.Append(currentChar);
      return true;
    }

    if (IsInSharpLine())
    {
      //getCurrentSharpLine().Append(currentChar);
      _tempWord.Append(currentChar);
      return true;
    }
    #endregion

    return false;
  }

  #endregion
  
    /// <summary>
    /// 尝试终止语义无效的区域(使用待处理单词和待处理单词表)
    /// 比如使用 */ 来结束一个区间注释 或使用 \r\n结束一行由 // 开始的注释
    /// </summary>
    /// <returns></returns>
    private bool TryEndSemanticInvalidArea()
    {
      #region 检测是否在注释区域内,任何换行符,语句开始或者终止符,只要是在注释区域内,就应该被添加为注释内容

      if (IsInNotesLine())
      {
        var currentNote = GetCurrentNotesLine();
        currentNote.Append(_tempWord.ToString());
        if (IsValidEndWordForCurrent())
        {
          _spaces[^2].Children.Add(currentNote);
          // _noOwnerNotes.Add(currentNote);
          _spaces.RemoveAt(_spaces.Count - 1);
        }

        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      if (InInNotesArea())
      {
        var currentNode = GetCurrentNotesArea();
        if (currentNode.Lines == null)
        {
          currentNode.Lines = new List<string>();
        }

        currentNode.Lines.Add(_tempWord.ToString());
        if (IsValidEndWordForCurrent())
        {
          _spaces[^2].Children.Add(currentNode);
          // _noOwnerNotes.Add(currentNode);
          _spaces.RemoveAt(_spaces.Count - 1);
        }

        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      if (IsInSharpLine())
      {
        var currentNote = GetCurrentSharpLine();
        currentNote.Append(_tempWord.ToString());
        if (IsValidEndWordForCurrent())
        {
          _spaces[^2].Children.Add(currentNote);
          // _noOwnerNotes.Add(currentNote);
          _spaces.RemoveAt(_spaces.Count - 1);
        }

        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }
      #endregion

      return false;
    }
    
    
    #region 尝试开辟一个新的语义无效区域(如果待处理词组前是以 // 之类的开始 就是要开始对应的种类的无效语义区域了)

    /// <summary>
    /// 尝试开辟一个新的语义无效区域
    /// 注释行 注释区域 等开头的检测.如果当前tempWord是为了开始一行或者一段注释的话 添加注释然后继续处理下一个字符
    /// </summary>
    /// <param name="currentChar"></param>
    /// <returns></returns>
    private bool TryStartSemanticInvalidArea(string currentChar)
    {
      #region 注释行 注释区域 等开头的检测.如果当前tempword是为了开始一行或者一段注释的话 添加注释然后继续处理下一个字符

      //如果要开始注释行并且当前没有在其它注释行或者区域中,也不在字符串中的话需要再后续添加一个判断逻辑
      if (IsNotesLineStartWord() && !IsInNotesLine() && !InInNotesArea())
      {
        _spaces.Add(new NotesLine());
        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      //如果要开始注释区域并且当前没有在其它注释行或者区域中,也不在字符串中的话需要再后续添加一个判断逻辑
      if (IsNotesAreaStartWord() && !InInNotesArea() && !IsInNotesLine())
      {
        _spaces.Add(new NotesArea());
        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      //如果要开始#开头的修饰行并且当前没有其他注释行或者区域中,也不在字符串中的话需要再后续添加一个判断逻辑
      if (IsSharpLineStartWord() && !InInNotesArea() && !IsInNotesLine())
      {
        _spaces.Add(new SharpLine());
        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }
      #endregion

      return false;
    }

    #endregion
}
