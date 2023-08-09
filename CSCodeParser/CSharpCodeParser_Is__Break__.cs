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
    return _wordSplitWords.Contains(c);
  }

  /// <summary>
  /// 检查当前待处理的内容最后一项是否为断句符号
  /// </summary>
  /// <param name="currentSentenceBreakWord">当前的断句符号或单词是什么</param>
  /// <param name="brokeByWhatTempWordOrUnProcessLastWord">已经被什么打断? 不完整单词 或 未处理的单词中的最后一个</param>
  /// <returns></returns>
  private bool IsSentenceBreakWord(out string? currentSentenceBreakWord, out string? brokeByWhatTempWordOrUnProcessLastWord)
  {
    foreach (var c in _sentenceBreakWords)
    {
      // 确定当前是检查临时未完成的词,还是检查未处理的词的最后一项. 如果临时未完成的词是空的,就用未处理的词的最后一项.
      var checkingWord = _tempWord.Length > 0 ? _tempWord.ToString() : _unProcessWords[^1];
      // 如果检查的词以断句符号结尾,就返回true,并且把当前的断句符号和检查的词返回.
      if (checkingWord.EndsWith(c))
      {
        // 返回
        currentSentenceBreakWord = c;
        brokeByWhatTempWordOrUnProcessLastWord = checkingWord;
        return true;
      }
    }

    currentSentenceBreakWord = null;
    brokeByWhatTempWordOrUnProcessLastWord = null;
    return false;
  }

}
