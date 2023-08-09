using System.Text;
using CS2TS.Model;


namespace CS2TS
{
  public partial class CSharpCodeParser
  {
    /// <summary>
    /// C#代码解析器构造函数.构造时,初始化领空,在领空中添加一个 CodeFile的领空.也就是文件跟领空.
    /// </summary>
    public CSharpCodeParser()
    {
      _spaces.Add(new CodeFile());
    }

    /// <summary>
    /// 领空链,第0个元素为文件.最后一个元素为当前在什么领空中处理字符
    /// </summary>
    private readonly List<CodeNode> _spaces = new List<CodeNode>();

    /// <summary>
    /// 没有处理的所有的词素,比如 int a( 这里有三个词素 int a 和 (,  其中(为断句符号.遇到断句符号(或单词)时就需要处理之前没有处理的内容了.
    /// </summary>
    private readonly List<string> _unProcessWords = new List<string>();

    /// <summary>
    /// 当前没有处理的所有的char构成的一个临时的单词,如果单词被打断 比如 a; 我们定义了 分号是断词符号之一,所以当前的 待处理词素集中加入  a和; 分号
    /// </summary>
    private StringBuilder _tempWord = new StringBuilder();

    /// <summary>
    /// 分词符号,遇到这个的时候要把前面已经获取到的内容变为一个词条了.比如 using namespace xxx; 是通过分号把前面的未处理的单词和符号变成一个集合进行处理的.
    /// </summary>
    private readonly List<string> _wordSplitWords = new List<string>() {";", " ", "{", "(", ")", "}", ":", ">", "<", ",", "=", "?", "-", "[", "]"};

    /// <summary>
    /// 断句符号,遇到这个的时候证明前面堆积的单词或者符号可以进行一次处理了.
    /// </summary>
    private readonly List<string> _sentenceBreakWords = new List<string>() {";", "{", "}", "\r\n", "\n", "*/", ","};

    /// <summary>
    /// 将从文件中加载的CSharp(*.cs)文件解析为CodeFile结构.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public CodeFile? ParseCsFile(string path)
    {
      //读取输入的文件
      var file = File.ReadAllText(path, Encoding.GetEncoding("UTF-8"));
      //文件的字符数量
      var count = file.Length;
      for (var i = 0; i < count; i++)
      {
        //当前字符
        var currentChar = file[i].ToString();
        //尝试把当前字符放在半成品单词/备注/字符串 当中.如果已经处理完毕,就可以continue处理下一个字符了.
        if (TryAppend2TempWordOrUnProcessWordsOrSemanticInvalidArea(currentChar))
        {
          continue;
        }
        //走到这里的时候 待处理单词或者词汇表中已经有内容了 不是字符级别的处理了. 这时候就要看怎么处理这个或者这些词.
        if (TryStartSemanticInvalidArea(currentChar))
        {
          continue;
        }
        //如果当前待处理的内容中,最后一项或者 \r\n这样的连续项 触发了语句中单词的语义定义  那就处理是该添加什么还是结束什么.
        //目前没有处理小括号 方括号 尖括号. 只处理了大括号.
        //如果最后一个单词或者符号是断句触发,处理待处理的所有的单词
        if (IsSentenceBreakWord(out var currentSentenceBreakWord, out var brokeByTempWordOrUnProcessLastWord))
        {
          //如果处理待处理的所有单词失败的话
          if (!ProcessUnProcessWords(currentSentenceBreakWord, brokeByTempWordOrUnProcessLastWord))
          {
            //如果当前的临时单词不是空的(当前是用一个词来断开了另外一个词,但是处理失败了 有可能是 \r\n using 断了,但是只有一个using不能处理.
            //把这个临时单词放在待处理列表中.
            var unProcessWord = _tempWord.ToString().Trim(new char[] {'\r', '\n', ' '});
            if (unProcessWord.Length > 0)
            {
              _unProcessWords.Add(unProcessWord);
            }
            //然后重置临时单词
            _tempWord = new StringBuilder();
          }
        }
        //如果不是有效的断句符,继续下一个字符.
      }

      return _spaces[0] as CodeFile;
    }

    /// <summary>
    /// 处理待处理的单词表
    /// </summary>
    /// <param name="currentBreakWord"></param>
    /// <param name="currentBreakBy"></param>
    /// <returns></returns>
    private bool ProcessUnProcessWords(string? currentBreakWord, string? currentBreakBy)
    {
      if (TryEndSemanticInvalidArea())
      {
        return true;
      }

      #region 检测是否在字符串定义区域内,这个后续如果处理复杂逻辑的时候再处理

      #endregion

      //如果是一个空换行的话不需要处理了.因为换行能结束一行注释 但是运行到这里已经确认不是在注释中了.所以不算结束注释.
      if (currentBreakWord == "\n" && currentBreakBy == currentBreakWord)
      {
        _tempWord = new StringBuilder();
        return true;
      }

      #region 对逗号的处理,比如枚举中逗号是结束一个枚举值的定义

      if (currentBreakWord == ",")
      {
        var r = ParseComma();
        if (r)
        {
          return true;
        }
      }

      #endregion

      #region 对分号的处理,会影响引用和变量定义,也可以结束一个接口内的函数定义

      if (currentBreakWord == ";")
      {
        var r = ParseSemicolon();
        if (r)
        {
          return true;
        }
      }

      #endregion

      #region 对大括号开始的处理 会结束类或命名空间头

      else if (currentBreakWord == "{")
      {
        var r = ParseLeftCurlyBraces();
        if (r)
        {
          return true;
        }
      }

      #endregion

      #region 对大括号结束的处理 会结束类或者命名空间的定义

      else if (currentBreakWord == "}")
      {
        var r = ParseRightCurlyBraces();
        if (r)
        {
          return true;
        }
      }

      #endregion

      return false;
    }


    #region 处理字符,如果需要断词就把当前词添加到待处理单词列表中,如果不需要,就追加到当前正在处理的单词的末尾

    /// <summary>
    /// 把当前的字符放到目标位置.处理字符,如果需要断词就把当前词添加到待处理单词列表中,如果不需要,就追加到当前正在处理的单词的末尾.如果在注释中,直接追加到注释就不继续处理了.
    /// </summary>
    /// <param name="currentChar">当前要处理的字符串</param>
    private bool TryAppend2TempWordOrUnProcessWordsOrSemanticInvalidArea(string currentChar)
    {
      //返回值是当前自如是否已经处理过了.如果是的话,调用这个函数的地方所在的循环就可以继续处理下一个了.
      if (IsWordBreakSymbol(currentChar))
      {
        //这里的逻辑要好好理解并测试一下才可以明白
        if (TryAppend2SemanticInvalidArea(currentChar))
        {
          return true;
        }
        //如果不是在注释区域的分隔符,那么就直接加入到上一个词组当中.

        //如果当前的临时词长度大于0,当前的字符又是一个分词符号,那么要把之前的已经积攒的字符作为一个单词 添加到待处理单词集合中.
        if (_tempWord.ToString().Trim().Length > 0)
          _unProcessWords.Add(_tempWord.ToString());

        //如果当前的分次符还不是空的,把这个分词符号也加入到待处理词素当中.这样在处理的时候好知道这个分词符号是个啥.
        if (currentChar.Trim().Length > 0)
          _unProcessWords.Add(currentChar);

        //处理完了这个字符串以后.因为当前字符已经是分词符了.所以前面的词加到了待处理单词中以后,就可以清空了.
        _tempWord = new StringBuilder();
      }
      else
      {
        //如果不是分隔符,就是标准字符的话 直接加入到上一个单词中.
        _tempWord.Append(currentChar);
      }
      return false;
    }

    #endregion
    
    #region 使用没有解析的字符集合构建代码

    /// <summary>
    /// 使用没有解析的字符集合构建明文代码(非结构化)
    /// </summary>
    /// <returns></returns>
    private string BuildCodeBodyByUnProcessWords()
    {
      var codeString = new StringBuilder();
      foreach (var t in _unProcessWords)
      {
        if (codeString.Length > 0)
        {
          codeString.Append(' ');
        }

        codeString.Append(t);
      }

      return codeString.ToString();
    }

    #endregion

    #region 解析入参 使用给定的单词集. 是在大小括号中间的部分的所有单词 包括逗号

    private void JoinParameterInto(Parameter param, CodeNode parent)
    {
      switch (parent)
      {
        case Function function:
        {
          function.InParameters.Add(param);
          break;
        }
        case Parameter parameter:
        {
          var parentParameter = parameter;
          if (parentParameter.Type.IsGeneric)
          {
            parentParameter.Type.GenericParamTypeList ??= new List<Parameter> {param};
          }
          break;
        }
        default:
          throw new NotImplementedException("参数定义在什么地方?");
      }
    }

    /// <summary>
    /// 解析入参 使用给定的单词集. 是在大小括号中间的部分的所有单词 包括逗号 比如 从(int a, int b, string c)中检索出a b c的相关信息
    /// </summary>
    /// <param name="words"></param>
    /// <returns></returns>
    private List<Parameter> Convert2Parameters(List<string> words)
    {
      var parentFunction = _spaces[^1] as Function;
      var ret = new List<Parameter>();
      var currentParam = new Parameter
      {
        Type = new TypeDefine()
      };
      //领空进入当前函数(因为如果words的数量小于2的话这个函数都进不来.所以这里放心大胆的进入领空.
      _spaces.Add(currentParam);
      foreach (var current in words)
      {
        // var next = i < words.Count - 2 ? words[words.Count ^ 2] : null;
        // var isGeneric = next == "<";
        //如果当前参数没有类型名字,用这个词赋值名字,然后继续下一个词
        if (currentParam?.Type.Name == null)
        {
          if (currentParam != null)
            currentParam.Type.Name = current;
          continue;
        }
        switch (current)
        {
          case "?":
            currentParam.Nullable = true;
            continue;
          //如果是泛型定义的开始
          case "<":
            //标记当前参数为泛型
            currentParam.Type.IsGeneric = true;
            //进入到泛型的参数
            currentParam = new Parameter
            {
              Type = new TypeDefine()
            };
            //泛型下面还有可能有泛型,所以要把新定义的参数作为领空.
            _spaces.Add(currentParam);
            //获取当前参数应该在领空指示器中的父级
            // var parent = _spaces[^2];
            // JoinParameterInto(currentParam, parent);
          
            //继续处理下一个单词
            continue;
          case ",":
          {
            //遇到逗号,把当前参数加入到父级.
            var parent = _spaces[^2];
            if( parent == parentFunction)
            {
              ret.Add(currentParam);
            }
            else
            {
              JoinParameterInto(currentParam, parent);
            }
            //退出当前参数的领空,因为当前的已经用逗号完成了.
            _spaces.RemoveAt(_spaces.Count-1);
            //继续进入下一个参数
            currentParam = new Parameter
            {
              Type = new TypeDefine()
            };
            //作为新的领空.因为有可能这个参数还是个泛型.
            _spaces.Add(currentParam);
            continue;
          }
          case ">":
          {
            //遇到大于号的时候,把当前的参数加入到父级,然后结束当前 Dictionary<string,int>的int的领空
            var parent = _spaces[^2];
            if( parent == parentFunction)
            {
              ret.Add(currentParam);
            }
            else
            {
              JoinParameterInto(currentParam, parent);
            }
            //退出当前参数的领空,因为当前的已经用  > 完成了.
            _spaces.RemoveAt(_spaces.Count-1);
            //这里不能像逗号一样进入到下一个参数的领空,因为在这后面没有 int同级别的参数了. 等待的下一个单词应该是这个参数的名称之类的.
            //所以要把currentParam从堆栈中取出来
            currentParam = parent as Parameter;
            continue;
          }
        }

        //如果类型已经解析完毕了,下面就到名字了.
          currentParam.Name = current;
      }

      //处理完最后一个字符以后,如果当前领空上还是当前参数的话,结束当前参数
      var currentSpace = _spaces[^1];
      if (currentSpace == currentParam)
      {
        // var parent = _spaces[^2];
        // if( parent == parentFunction)
        // {
        //   ret.Add(currentParam);
        // }
        // else
        // {
        //   JoinParameterInto(currentParam, parent);
        // }
        ret.Add(currentParam);
        _spaces.RemoveAt(_spaces.Count-1);
      }

      return ret;
    }

    #endregion
    
    
    #region 添加到父

    /// <summary>
    /// 添加for语句到父级，会自动清空待处理字符
    /// </summary>
    private StatementWithStructure AddForStatement2Parent()
    {
      var parent = _spaces[^1];
      StatementWithStructure forSt = new ForStatement();
      //for 语句段
      forSt.Type = "for";

      switch (parent)
      {
        case Function function:
        {
          function.Statements.Add(forSt);
          break;
        }
        //像 while,foreach 之类的语句段之内(嵌套循环啥的)
        case StatementWithStructure statementWithStructure:
        {
          statementWithStructure.Statements ??= new List<Statement>();
          statementWithStructure.Statements.Add(forSt);
          break;
        }
      }

      return forSt;
    }

    #endregion

    /// <summary>
    /// 转换public private等字符串为一个PermissionEnum的枚举值
    /// 如果解析失败会返回null
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static PermissionEnum? ConvertString2Permission(string str)
    {
      switch (str)
      {
        case "protected":
          return PermissionEnum.Protected;
        case "private":
          return PermissionEnum.Private;
        case "public":
          return PermissionEnum.Public;
        case "internal":
          return PermissionEnum.Internal;
        default:
          return null;
      }
    }

    /// <summary>
    /// 当前的单词是否能有效的结束当前领空.
    /// 比如 大括号可以结束一个有效的if句段或带结构函数等
    /// </summary>
    /// <returns></returns>
    private bool IsValidEndWordForCurrent()
    {
      var usingContent = $"{_tempWord}";
      if (_unProcessWords.Count > 0)
      {
        usingContent = _unProcessWords[^1];
      }

      if (_spaces[^1] is NotesLine && usingContent.EndsWith("\n"))
      {
        return true;
      }

      if (_spaces[^1] is NotesArea && usingContent.EndsWith("*/"))
      {
        return true;
      }

      if (_spaces[^1] is NameSpace && usingContent.EndsWith("}"))
      {
        return true;
      }

      if (_spaces[^1] is Class && usingContent.EndsWith("}"))
      {
        return true;
      }

      if (_spaces[^1] is EnumDefine && usingContent.EndsWith("}"))
      {
        return true;
      }

      if (_spaces[^1] is SharpLine && usingContent.EndsWith("\n"))
      {
        return true;
      }

      if (_spaces[^1] is VariableWithStructure && usingContent.EndsWith("}"))
      {
        return true;
      }

      if (_spaces[^1] is Function && usingContent.EndsWith("}"))
      {
        return true;
      }

      if (_spaces[^1] is StatementWithStructure && usingContent.EndsWith("}"))
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// 获取当前的备注行(由//开头的),如果不是有效的备注行,则返回null
    /// </summary>
    /// <returns></returns>
    private NotesLine GetCurrentNotesLine()
    {
      return (_spaces[^1] as NotesLine)!;
    }

    /// <summary>
    /// 获取当前的备注块(由/*开头的),如果不是有效的备注块,则返回null
    /// </summary>
    /// <returns></returns>
    private NotesArea GetCurrentNotesArea()
    {
      return (_spaces[^1] as NotesArea)!;
    }

    /// <summary>
    /// 获取当前的 Sharp块(由#开头的),如果不是有效的Sharp块,则返回null
    /// </summary>
    /// <returns></returns>
    private SharpLine GetCurrentSharpLine()
    {
      return (_spaces[^1] as SharpLine)!;
    }
    
    
    /// <summary>
    /// 检查是否为大括号包围着的结构片段.比如 if {}  else if {} 这种的.后者是直接就 {} 包围的结构 但是 出参的片段类型就是空了
    /// </summary>
    /// <param name="statementType">如果确实是带大括号的片段,该参数为null或者片段的类型名称</param>
    /// <returns>返回是否为带大括号包围的结构的片段</returns>
    /// <exception cref="NotSupportedException">不支持的操作,需要增强</exception>
    private bool IsStatementWithStructure(out string? statementType)
    {
      if (_unProcessWords[^1] != "{"
         )
      {
        statementType = null;
        return false;
      }

      //去掉 { 之后的单词或符号的数量. 比如 else if { 去掉 { 以后就是2个(else 和 if)
      int tagCount = _unProcessWords.Count - 1;
      if (tagCount == 0)
      {
        statementType = null;
        //只有 { 就是普通大括号结构
        return true;
      }
      //去掉大括号之后有两个字符的好像只有  else if
      if (tagCount == 2)
      {
        //判断是否为 else if
        if (_unProcessWords[0] == "else" && _unProcessWords[1] == "if")
        {
          statementType = "else if";
          return true;
        }
        //else
        throw new NotSupportedException("去掉大括号以后,有两个关键字,但是这两个关键字不是 else if 是什么没有处理呢?");
      }
      //一个单词加上大括号的形式  通常就是  if  else switch 等
      if (tagCount == 1 && new List<string>() {"if", "else", "switch", "while"}.IndexOf(_unProcessWords[0]) == 0)
      {
        statementType = _unProcessWords[0];
        return true;
      }

      statementType = null;
      return false;
    }
  }
}
