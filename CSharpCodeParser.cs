using System.Text;
using CS2TS.Model;


namespace CS2TS
{
  public class CSharpCodeParser
  {
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
    /// 分词符号,遇到这个的时候要把前面已经获取到的内容变为一个词条了.
    /// </summary>
    private readonly List<string> _splitWords = new List<string>() {";", " ", "{", "(", ")", "}", ":", ">", "<", ",", "="};

    /// <summary>
    /// 断句符号,遇到这个的时候证明前面堆积的单词或者符号可以进行一次处理了.
    /// </summary>
    private readonly List<string> _breakWords = new List<string>() {";", "{", "}", "\r\n", "\n", "*/", ","};

    /// <summary>
    /// 将从文件中加载的CSharp(*.cs)文件解析为CodeFile结构.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public CodeFile ParseCsFile(string path)
    {
      string file = File.ReadAllText(path, Encoding.GetEncoding("UTF-8"));
      int count = file.Length;
      for (int i = 0; i < count; i++)
      {
        if (i == 1266)
        {
          
        }
        var i1 = _unProcessWords.IndexOf("i");
        var i2 = _unProcessWords.IndexOf("nt");
        if (i1+1==i2)
        {
          
        }
        string currentChar = file[i].ToString();
        //尝试把当前字符放在半成品单词/备注/字符串 当中.如果已经处理完毕,就可以continue处理下一个字符了.
        if (TryAppend2TempWordOrUnProcessWordsOrSemanticInvalidArea(currentChar))
        {
          continue;
        }
        //走到这里的时候 待处理单次或者词汇表中已经有内容了 不是字符级别的处理了. 这时候就要看怎么处理这个或者这些词.
        if (TryStartSemanticInvalidArea(currentChar))
        {
          continue;
        }
        //如果当前待处理的内容中,最后一项或者 \r\n这样的连续项 触发了语句中单词的语义定义  那就处理是该添加什么还是结束什么.
        //目前没有处理小括号 方括号 尖括号. 只处理了大括号.
        string currentBreakWord, currentBreakBy;
        //如果最后一个单词或者符号是断句触发,处理待处理的所有的单词
        if (IsSentenceBreakWord(out currentBreakWord, out currentBreakBy))
        {
          //如果处理待处理的所有单词失败的话
          if (!ProcessUnProcessWords(currentBreakWord, currentBreakBy))
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
    /// 尝试终止语义无效的区域(使用待处理单词和待处理单词表)
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
          _spaces[^2].Chirldren.Add(currentNote);
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
          _spaces[^2].Chirldren.Add(currentNode);
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
          _spaces[^2].Chirldren.Add(currentNote);
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

    /// <summary>
    /// 处理待处理的单词表
    /// </summary>
    /// <param name="currentBreakWord"></param>
    /// <param name="currentBreakBy"></param>
    /// <returns></returns>
    private bool ProcessUnProcessWords(string currentBreakWord, string currentBreakBy)
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

    #region 处理字符,如果需要断词就把当前词添加到待处理单次列表中,如果不需要,就追加到当前正在处理的单词的末尾

    /// <summary>
    /// 把当前的字符放到目标位置.处理字符,如果需要断词就把当前词添加到待处理单次列表中,如果不需要,就追加到当前正在处理的单词的末尾.如果在注释中,直接追加到注释就不继续处理了.
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

    #region 使用没有解析的字符集合构建代码

    /// <summary>
    /// 使用没有解析的字符集合构建明文代码(非结构化)
    /// </summary>
    /// <returns></returns>
    private string BuildCodeBodyByUnProcessWords()
    {
      StringBuilder codeString = new StringBuilder();
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
      if (parent is Function)
      {
        var fnParent = parent as Function;
        if (fnParent.InParameters == null)
        {
          fnParent.InParameters = new List<Parameter>();
        }
        fnParent.InParameters.Add(param);
      }
      else if (parent is Parameter)
      {
        var parentParameter = parent as Parameter;
        if (parentParameter.Type.IsGeneric)
        {
          if (parentParameter.Type.GenericParamTypeList == null)
          {
            parentParameter.Type.GenericParamTypeList = new List<Parameter>();
          }
          parentParameter.Type.GenericParamTypeList.Add(param);
        }
      }
      else
      {
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
      if (words.Count == 15)
      {
        
      }
      List<Parameter> ret = new List<Parameter>();
      Parameter currentParam = new Parameter();
      currentParam.Type = new TypeDefine();
      //领空进入当前函数(因为如果words的数量小于2的话这个函数都进不来.所以这里放心大胆的进入领空.
      _spaces.Add(currentParam);
      for (int i = 0; i < words.Count; i++)
      {
        var current = words[i];
        // var next = i < words.Count - 2 ? words[words.Count ^ 2] : null;
        // var isGeneric = next == "<";
        //如果当前参数没有类型名字,用这个词赋值名字,然后继续下一个词
        if (currentParam.Type.Name == null)
        {
          currentParam.Type.Name = current;
          continue;
        }

        //如果是泛型定义的开始
        if (current == "<")
        {
          //标记当前参数为泛型
          currentParam.Type.IsGeneric = true;
          //进入到泛型的参数
          currentParam = new Parameter();
          currentParam.Type = new TypeDefine();
          //泛型下面还有可能有泛型,所以要把新定义的参数作为领空.
          _spaces.Add(currentParam);
          //获取当前参数应该在领空指示器中的父级
          // var parent = _spaces[^2];
          // JoinParameterInto(currentParam, parent);
          
          //继续处理下一个单词
          continue;
        }

        if (current == ",")
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
          currentParam = new Parameter();
          currentParam.Type = new TypeDefine();
          //作为新的领空.因为有可能这个参数还是个泛型.
          _spaces.Add(currentParam);
          continue;
        }

        if (current == ">")
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
        //如果类型已经解析完毕了,下面就到名字了.
        if (currentParam.Type.Name!= null)
        {
          currentParam.Name = current;
        }
        //如果是泛型,进入到泛型参数定义的领空
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
    #region 解析变量的信息

    /// <summary>
    /// 解析变量的信息,填充到给定的变量.(从未处理的单词表中获取数据)
    /// </summary>
    /// <param name="va"></param>
    private void ParseVariableInfo(Variable va)
    {
      for (var i = 0; i < _unProcessWords.Count; i++)
      {
        _unProcessWords[i] = _unProcessWords[i].Replace("\t", "");
      }

      //作用域的相关字样
      var permisionIndex = Define.Permissions.IndexOf(_unProcessWords[0]);
      //const 字样的位置
      var constIndex = _unProcessWords.IndexOf("const");
      //staticc 字样的位置
      var staticIndex = _unProcessWords.IndexOf("static");
      //readonly 字样的位置
      var readonnlyIndex = _unProcessWords.IndexOf("readonly");
      var newIndex = _unProcessWords.IndexOf("new");
      //这里还可以添加 partial之类的关键字

      #region 名字后面的一个标志符的解析

      //public static readonly Nullable<int> vName;
      //static int vName = 0;
      //public bool vName { get; set; }
      //定义名称之前的那个分隔符. 如果有大括号的 前面一个是名称,如果有等号的,前面一个是名称,其他的,分号前面是名称
      int spliterIndex;
      if (_unProcessWords.Contains(":"))
      {
        spliterIndex = _unProcessWords.IndexOf(":");
      }
      else if (_unProcessWords.Contains("{"))
      {
        spliterIndex = _unProcessWords.IndexOf("{");
      }
      else if (_unProcessWords.Contains("="))
      {
        spliterIndex = _unProcessWords.IndexOf("=");
      }
      else
      {
        spliterIndex = _unProcessWords.IndexOf(";");
      }

      #endregion

      //名字前面的排除了之前作用域以及const等的固定字段以后的,都是类型的定义.
      //比如说  public  static readonly Nullable<bool> vaName 这样的
      //默认认为,变量类型为0位置,让后如果有 public static readonly const 之类的,每有一个 往后加一 加完以后的  就是 类型定义的部分
      int typeDefineStartPos = 0;
      if (newIndex >= 0)
      {
        va.IsOverride = true;
        typeDefineStartPos++;
      }

      if (permisionIndex >= 0)
      {
        va.Permission = ConvertString2Permission(_unProcessWords[0]);
        typeDefineStartPos++;
      }

      if (staticIndex >= 0)
      {
        va.IsStatic = true;
        typeDefineStartPos++;
      }

      if (readonnlyIndex >= 0)
      {
        va.IsReadonly = true;
        typeDefineStartPos++;
      }

      if (constIndex >= 0)
      {
        va.IsConst = true;
        typeDefineStartPos++;
      }

      int typeDefineEndPos = spliterIndex - 2;
      int nameDefinePos = typeDefineEndPos + 1;
      StringBuilder typeBuilder = new StringBuilder();
      //int typeDefineWordCount = typeDefineEndPos - typeDefineStartPos;
      for (int i = typeDefineStartPos; i <= typeDefineEndPos; i++)
      {
        typeBuilder.Append(_unProcessWords[i].Trim(new[] {'\t'}));
      }

      va.Name = _unProcessWords[nameDefinePos].Replace("\n","").Replace("\r","");
      va.Type = new TypeDefine() {Name = typeBuilder.ToString()};

      if (va.CodeBody == null)
      {
        va.CodeBody = "";
      }

      va.CodeBody += BuildCodeBodyByUnProcessWords();
    }

    #endregion

    #region 未处理的单词们转换成枚举中的值

    private void unProcessWords2Variable4EnumDefine(EnumDefine enumDefine)
    {
      //解析枚举的值
      object value = null;
      var name = _unProcessWords[0];
      var denghaoIndex = _unProcessWords.IndexOf("=");
      var isIntValue = true;
      if (denghaoIndex>=0)
      {
        //如果有等号的话,当前枚举选项设置为等号后面的值,并且后面的一项如果没有设定值,为这一项的值+1
        var valStr = _unProcessWords[denghaoIndex + 1];
        if (enumDefine.Extends!= null && enumDefine.Extends.Contains("long"))
        {
          isIntValue = true;
          value = long.Parse(valStr);
        }
        else
        {
          value = int.Parse(valStr);
        }
      }
      else
      {
        //如果枚举没有使用等号赋值,不需要计算该枚举的实际值,因为ts等都会自动计算.会将没有枚举值的枚举项自动设置为上一项+1
        // var lastEnumValue = parent.Variables.Count > 0 ? (int) parent.Variables[^1].Value : -1;
        // variable.Value = lastEnumValue +1;
      }

      var type = new TypeDefine();
      type.Name = isIntValue ? "int" : "long";
      var variable = new VariableNoStructure(name,type,null,null,null,null,null,value,null);
      enumDefine.Chirldren.Add(variable);
    }

    #endregion

    #region 解析逗号

    /// <summary>
    /// 解析逗号 , 可以出现在:函数参数,枚举,类继承
    /// </summary>
    /// <returns></returns>
    private bool ParseComma()
    {
      //上一个枚举的值,因为枚举默认是从0开始
      if (IsInEnum())
      {
        var parent = _spaces[^1] as EnumDefine;
        unProcessWords2Variable4EnumDefine(parent);
        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }
      return false;
    }

    #endregion

    #region 解析分号

    /// <summary>
    /// 解析分号 ;
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private bool ParseSemicolon()
    {
      //return true就是执行 continue;
      if (IsUsingAddingWords())
      {
        CodeFile code = _spaces[0] as CodeFile;
        var u = new Using(_unProcessWords[1]);
        code.Chirldren.Add(u);
        // code.GetUsings().Add(_unProcessWords[1]);
        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      //带括号的是函数定义或者是函数调用。
      //    不带括号的 带不带等号 都可以是变量定义。
      //    函数定义可以放在类或者接口中，还可以带权限。
      //    变量定义可以在类中，接口中，函数中，代码段中。
      //    函数调用可以在
      if (isVariable_AddingWords(out var variablePermission))
      {
        //有权限定义的变量一定是在类中的
        if (variablePermission != null)
        {
          var cls = (_spaces[^1] as Class)!;
          if (cls != null)
          {
            var name = _unProcessWords[^2].Replace("\r", "").Replace("\n","").Trim();
            var type = _unProcessWords[^3];
            VariableWithStructure v = new VariableWithStructure(name,new TypeDefine(){Name = type},
              null,
              null,
              null,
              null,
              null,
              null, null,null,null);
            ParseVariableInfo(v);
            // cls.Variables.Add(v);
            cls.Chirldren.Add(v);
            _tempWord = new StringBuilder();
            _unProcessWords.Clear();
            return true;
          }
        }
        //没有权限定义的变量可以放的地方就多了.比如可以放到函数中.
        else
        {
          object parent = _spaces[^1];
          //如果包含正反括号的属于函数头的定义

          if (parent is Interface)
          {
            if (IsFunctionAddingWords())
            {
              CreateFunction();
            }

            #region 不带括号的

            else
            {
              //没有括号的时候就是一个标准的变量定义
              VariableNoStructure vn = new VariableNoStructure(null,null,null,null,null,null,null,null,null);
              ParseVariableInfo(vn);
              if (parent is Class)
              {
                var clsParent = parent as Class;
                clsParent.Chirldren.Add(vn);
                _tempWord = new StringBuilder();
                _unProcessWords.Clear();
                return true;
              }
              else
              {
                throw new NotImplementedException("分号结束的变量定义在什么地方了？");
              }
            }

            #endregion
          }
          else
          {
            var st = new Statement();
            StringBuilder codeString = new StringBuilder();
            st.CodeBody = BuildCodeBodyByUnProcessWords();

            //看看添加到什么地方
            if (parent is Function)
            {
              var vrParent = parent as Function;
              if (vrParent.Statements == null)
              {
                vrParent.Statements = new List<Statement>();
              }

              vrParent.Statements.Add(st);
              _tempWord = new StringBuilder();
              _unProcessWords.Clear();
              return true;
            }
            else if (parent is StatementWithStructure)
            {
              var vrParent = parent as StatementWithStructure;
              if (vrParent.Statements == null)
              {
                vrParent.Statements = new List<Statement>();
              }

              vrParent.Statements.Add(st);
              _tempWord = new StringBuilder();
              _unProcessWords.Clear();
              return true;
            }
            else if (parent is VariableWithStructure)
            {
              var vsParent = parent as VariableWithStructure;
              //if (vsParent.StatementsWithStructure == null)
              //{
              //    vsParent.StatementsWithStructure = new List<VariableWithStructure>();
              //}
              //vsParent.StatementsWithStructure.Add(st);

              _tempWord = new StringBuilder();
              _unProcessWords.Clear();
              return true;
            }
          }
        }

        //如果上一个不是类的话 可能就是错误了
      }

      #region 不带public等修饰符的变量定义

      if (IsVariableAddingWords())
      {
        object parent = _spaces[^1];
        if (parent != null)
        {
          if (parent is Class)
          {
            Class cls = parent as Class;
            var v = new VariableNoStructure(null,null,null,null,null,null,null,null,null);
            ParseVariableInfo(v);
            cls.Chirldren.Add(v);
            _tempWord = new StringBuilder();
            _unProcessWords.Clear();
            return true;
          }
        }
      }

      #endregion

      return false;
    }

    #endregion

    #region 解析大括号开始

    /// <summary>
    /// 解析左侧花括号 大括号开始 { 可能的结构:
    /// if{}
    /// else if{}
    /// else{}
    /// 任何地方表示一段代码{}
    /// namespace (){}
    /// function(){}
    /// variable{}
    /// switch{}
    /// while{}
    /// foreach(){}
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private bool ParseLeftCurlyBraces()
    {
      #region if  else if  else 字段

      if (IsIfStatementAddingWords())
      {
        var parent = _spaces[^1];
        var st = new IfStatement();
        if (parent is Function)
        {
          var funcParent = parent as Function;

          if (funcParent.Statements == null)
          {
            funcParent.Statements = new List<Statement>();
          }

          funcParent.Statements.Add(st);
        }
        else if (parent is StatementWithStructure)
        {
          var swsParent = parent as StatementWithStructure;
          if (swsParent.Statements == null)
          {
            swsParent.Statements = new List<Statement>();
          }

          swsParent.Statements.Add(st);
        }
        else
        {

        }

        _spaces.Add(st);
        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      if (IsElseIfStatementAddingWords())
      {
        Function func = _spaces[^1] as Function;

        if (func.Statements == null)
        {
          throw new NotImplementedException("没有if 哪里来的else if");
        }

        var st = new ElseIfStatement();
        func.Statements.Add(st);
        _spaces.Add(st);
        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      if (IsElseStatementAddingWords())
      {
        Function func = _spaces[^1] as Function;

        if (func.Statements == null)
        {
          throw new NotImplementedException("没有if 哪里来的else");
        }

        var st = new ElseStatement();
        func.Statements.Add(st);
        _spaces.Add(st);
        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      #endregion

      #region 命名空间

      if (IsNamespaceAddingWords())
      {
        CodeFile code = _spaces[0] as CodeFile;
        var parent = _spaces[^1];
        var name = _unProcessWords[1].Replace("\r", "").Replace("\n","").Trim();
        var nm = new NameSpace(name);
        parent.Chirldren.Add(nm);
        _spaces.Add(nm);
        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      #endregion

      string typeName = null;
      Nullable<PermissionEnum> permission = null;

      #region if else if switch while 等

      if (IsStatementWithStructure(out typeName))
      {
        if (typeName == "for")
        {
          var st = AddForStatement2Parent();
          _spaces.Add(st);
        }
        else
        {
          throw new NotImplementedException("sha qingkuanng");
        }

        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      #endregion

      //如果是带权限(public等)带结构的
      if (isVariable_StructureAddingWords(out typeName, out permission))
      {
        CodeNode parent = _spaces[^1];
        VariableWithStructure vbOrInterface = new VariableWithStructure(null,null,null,null,null,null,null,null,null,null,null);
        int tagIndex = _unProcessWords.IndexOf(typeName);

        //class 标记后面的一个为类名称
        if (IsInterfaceAddingWords())
        {
          vbOrInterface = new Interface(null,null);
        }
        else if (typeName == "class")
        {
          vbOrInterface = new Class(null,null);
        }
        else
        {

        }

        #region 赋值基本参数

        ParseVariableInfo(vbOrInterface);

        #endregion

        //int typeTagIndex = unProcessWords.IndexOf(typeName);
        //class 标记后面的一个为类名称
        int maohaoIndex = _unProcessWords.IndexOf(":");

        #region 当有继承的时候

        int extentsWordsCount = _unProcessWords.Count - 1 - maohaoIndex - 1;
        if (maohaoIndex >= 0)
        {
          StringBuilder extBuilder = new StringBuilder();
          for (int ei = 0; ei < extentsWordsCount; ei++)
          {
            var currentExtWord = _unProcessWords[ei + maohaoIndex + 1];
            if (vbOrInterface.Extends == null)
            {
              vbOrInterface.Extends = new List<string>();
            }

            if (currentExtWord == ",")
            {
              if (extBuilder.Length > 0)
              {
                vbOrInterface.Extends.Add(extBuilder.ToString());
                extBuilder = new StringBuilder();
              }

              continue;
            }

            extBuilder.Append(currentExtWord.Trim(new []{'\r', '\n'}));
            if (ei == extentsWordsCount - 1)
            {
              vbOrInterface.Extends.Add(extBuilder.ToString());
              extBuilder = null;
            }
          }
        }

        #endregion

        #region 不同类型 加入到不同的地方去

        #region get set

        //如果type是空并且是get或者set的话 get就是个返回值为上一层的类型的返回值,set的返回值是void
        if (typeName == null && _unProcessWords[0] == "get")
        {
          Function fn = new Function();
          //fn.@int = false;
          var varParentObj = _spaces[^1];
          if (varParentObj is VariableWithStructure)
          {
            var varParent = varParentObj as VariableWithStructure;
            fn.Name = string.Format("get{0}", varParent.Name);
            fn.ReturnParameter = new Parameter();
            fn.ReturnParameter.Type = varParent.Type;
            varParent.Getter = fn;
            _spaces.Add(fn);
          }

        }
        else if (typeName == null && _unProcessWords[0] == "set")
        {
          Function fn = new Function();
          var varParentObj = _spaces[^1];
          if (varParentObj is VariableWithStructure)
          {
            var varParent = varParentObj as VariableWithStructure;
            fn.Name = string.Format("set{0}", varParent.Name);
            fn.ReturnParameter = new Parameter();
            fn.ReturnParameter.Type =new TypeDefine() {Name = "void"};
            varParent.Setter = fn;
            _spaces.Add(fn);
          }
        }

        #endregion

        #region 类

        else if (vbOrInterface is Class)
        {
            parent.Chirldren.Add(vbOrInterface as Class);
            _spaces.Add(vbOrInterface);
        }

        #endregion

        #region 接口

        else if (vbOrInterface is Interface)
        {
          // if (parent is NameSpace == false)
          // {
          //   throw new NotImplementedException("接口只能定义在命名空间中");
          // }


          parent.Chirldren.Add(vbOrInterface as Interface);
          _spaces.Add(vbOrInterface);
        }

        #endregion

        #region 如果类型名为枚举 enum

        else if (typeName.ToLower() == "enum")
        {
          EnumDefine ed = new EnumDefine(null,null,null,null,null);
          ParseVariableInfo(ed);
          //ed.Name = vbOrInterface.Name;
          ed.Extends = vbOrInterface.Extends;
          ed.IsStatic = _unProcessWords.Contains("static");
          ed.Permission = vbOrInterface.Permission == null ? PermissionEnum.Private : vbOrInterface.Permission.Value;

           parent.Chirldren.Add(ed);
          _spaces.Add(ed);
        }

        #endregion

        #region 如果是带()的函数定义

        //如果是一个函数的定义（有小括号对） 并且是在Interface里， class因为是继承自interface 所以也会是true
        else if (IsFunctionAddingWords() && parent is Interface)
        {
          if (parent is Interface)
          {
            CreateFunction();
          }
          else
          {
            
          }
        }

        #endregion

        #region 变量

        else if (vbOrInterface is VariableWithStructure)
        {
          if (parent is CodeFile || parent is NameSpace)
          {
            throw new NotImplementedException("在文件外部或者命名空间中不能定义变量");
          }

          if (parent is Class)
          {
            parent.Chirldren.Add(vbOrInterface);
          }
          else
          {
            //这是在往哪里加入带结构的变量呢?
          }

          _spaces.Add(vbOrInterface);
        }

        #endregion

        #region 其他的各种花括号可以包起来的结构或者是只有花括号对没有限定只是为了区分代码区间的  也算

        else
        {
          Console.WriteLine("当前大括号起始位置之前的字符没有被处理：", _unProcessWords.Count);
        }

          #endregion
        #endregion


        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      return false;
    }

    #endregion


    #region 根据现有的信息创建函数
    //根据已有的词汇表，创建函数，并且自动添加到领空和父亲，然后清空未处理的半截单词和未处理的所有单词
    private void CreateFunction()
    {
      var parent = _spaces[^1];
      Function fn = new Function();
      //括号前面的是函数名
      //函数名前面的是函数返回值类型
      int nameIndex = _unProcessWords.IndexOf("(") - 1;
      string name = _unProcessWords[nameIndex];
      //函数的返回值类型应该是在public等关键字后面一直到函数名处的所有内容.
      //////string returnType = unProcessWords[nameIndex - 1];


      fn.Permission = ConvertString2Permission(_unProcessWords[0].Replace("\t", "").Trim());
      fn.IsStatic = _unProcessWords.Contains("static");
      fn.IsOverride = _unProcessWords.Contains("override");
      int returnTypeDefStartPos = 0;
      if (fn.Permission != null)
      {
        returnTypeDefStartPos++;
      }

      if (fn.IsStatic)
      {
        returnTypeDefStartPos++;
      }

      if (fn.IsOverride)
      {
        returnTypeDefStartPos++;
      }

      #region 解析函数的返回值

          

      #endregion
      int returnTYpeDefWordsCount = nameIndex - returnTypeDefStartPos;
      // StringBuilder returnType = new StringBuilder();
      // for (int ri = 0; ri < returnTYpeDefWordsCount; ri++)
      // {
      //   returnType.Append(_unProcessWords[returnTypeDefStartPos + ri]);
      // }
      // fn.ReturnParameter = new Parameter();
      // fn.ReturnParameter.Type = new TypeDefine() {Name = returnType.ToString()};
      var returnParameterDefineWords = _unProcessWords.Skip(returnTypeDefStartPos).Take(returnTYpeDefWordsCount);
      var returnParameters = Convert2Parameters(new List<string>(returnParameterDefineWords));
      if (returnParameters.Count>1)
      {
        throw new NotImplementedException("错误,解析到的返回值不止1个");
      }

      fn.ReturnParameter = returnParameters[0];

      //class 标记后面的一个为类名称
      fn.Name = name.Replace("\r", "").Replace("\n","").Trim();

      
      // if (parent is Class)
      // {
      parent.Chirldren.Add(fn);
      // }
      //先进入函数定义领空再处理函数的参数集.不然会错乱,可能会把参数加到类中了.
      _spaces.Add(fn);

      #region 解析函数的参数

      var leftParenthesesPos = _unProcessWords.IndexOf("(");
      var rightParenthesesPos = _unProcessWords.IndexOf(")");
      if (rightParenthesesPos-1>leftParenthesesPos)
      {
        var parameterWordsList = _unProcessWords.Skip(leftParenthesesPos+1).Take(rightParenthesesPos-leftParenthesesPos-1);
        var parameters = Convert2Parameters(new List<string>(parameterWordsList));
        fn.InParameters = parameters;
      }
      #endregion
      
      _tempWord = new StringBuilder();
      _unProcessWords.Clear();
    }

    #endregion

    #region 解析大括号返回

    /// <summary>
    /// 解析右侧花括号 大括号结束 }
    /// </summary>
    /// <returns></returns>
    private bool ParseRightCurlyBraces()
    {
      if (IsInStatement())
      {
        if (IsValidEndWordForCurrent())
        {
          _spaces.RemoveAt(_spaces.Count - 1);
        }

        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      if (IsInNamespace())
      {
        if (IsValidEndWordForCurrent())
        {
          _spaces.RemoveAt(_spaces.Count - 1);
        }

        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      if (IsInClass())
      {
        if (IsValidEndWordForCurrent())
        {
          _spaces.RemoveAt(_spaces.Count - 1);
        }

        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      if (IsInEnum())
      {
        if (IsValidEndWordForCurrent())
        {
          //如果还有没有处理的枚举信息的话,再处理一下.因为最后一个枚举值可能不是以逗号结尾的.(通常都是这样)
          if (_unProcessWords.Count == 1 && _unProcessWords[0] == "}")
          {

          }
          else
          {
            var parent = _spaces[^1] as EnumDefine;
            unProcessWords2Variable4EnumDefine(parent);
          }
          _spaces.RemoveAt(_spaces.Count - 1);
        }
        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      if (isInVariable_Structure())
      {
        if (IsValidEndWordForCurrent())
        {
          _spaces.RemoveAt(_spaces.Count - 1);
        }

        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      if (IsInFunction())
      {
        if (IsValidEndWordForCurrent())
        {
          _spaces.RemoveAt(_spaces.Count - 1);
        }

        _tempWord = new StringBuilder();
        _unProcessWords.Clear();
        return true;
      }

      return false;
    }

    #endregion

    #region 添加到父

    /// <summary>
    /// 添加for语句到父级，会自动清空待处理字符
    /// </summary>
    private StatementWithStructure AddForStatement2Parent()
    {
      var parent = _spaces[^1];
      StatementWithStructure forSt = new StatementWithStructure();
      forSt.Type = "for";

      if (parent is Function)
      {
        var f = parent as Function;
        if (f.Statements == null)
        {
          f.Statements = new List<Statement>();
        }

        f.Statements.Add(forSt);
      }
      else if (parent is StatementWithStructure)
      {
        var s = parent as Function;
        if (s.Statements == null)
        {
          s.Statements = new List<Statement>();
        }

        s.Statements.Add(forSt);
      }

      return forSt;
    }

    #endregion

    private bool IsInStatement()
    {
      return _spaces[^1] is StatementWithStructure;
    }

    private bool IsInNamespace()
    {
      return _spaces[^1] is NameSpace;
    }

    private bool IsInClass()
    {
      return _spaces[^1] is Class;
    }

    private bool IsInEnum()
    {
      return _spaces[^1] is EnumDefine;
    }

    private bool isInVariable_Structure()
    {
      return _spaces[^1] is VariableWithStructure;
    }

    private bool IsInFunction()
    {
      return _spaces[^1] is Function;
    }

    private Nullable<PermissionEnum> ConvertString2Permission(string str)
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

    private bool isVariable_AddingWords(out Nullable<PermissionEnum> permission)
    {
      if (_unProcessWords == null || _unProcessWords.Count < 3)
      {
        permission = null;
        return false;
      }

      if (_unProcessWords[^1] == ";")
      {
        permission = Define.Permissions.Contains(_unProcessWords[0])
          ? ConvertString2Permission(_unProcessWords[0])
          : null;
        return true;
      }

      permission = null;
      return false;
    }

    private bool IsVariableAddingWords()
    {
      return _unProcessWords != null && _unProcessWords.Count == 3;
    }

    private bool IsFunctionAddingWords()
    {
      // 1 函数名  2( 3)  4{
      if (_unProcessWords == null || _unProcessWords.Count < 4)
      {
        return false;
      }

      int kuohaoIndex = _unProcessWords.IndexOf("(");
      int kuohuiIndex = _unProcessWords.IndexOf(")");
      //大括号和分号都可以是一个函数定义内容的标志符，在大括号或者分号前面的都可以是函数的定义，只不过分号前面的是没有函数内体，这通常就是在接口内定义函数了。
      int dakuohaoIndex = _unProcessWords.IndexOf("{");
      int fenhaoIndex = _unProcessWords.IndexOf(";");
      if (kuohaoIndex > 0
          && kuohuiIndex > 1
          && (dakuohaoIndex == _unProcessWords.Count - 1 || fenhaoIndex == _unProcessWords.Count -1) 
         )
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// 是否是给定的这种类型名字的带大括号对的定义 变量 带public之类的字样和结构
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool isVariable_Permission_StructureAddingWords_bak(out string typeName)
    {
      if (_unProcessWords == null || _unProcessWords.Count < 1)
      {
        typeName = null;
        return false;
      }

      if (_unProcessWords.Count > 3 && _unProcessWords[^1] == "{")
      {
        int maohaoIndex = _unProcessWords.IndexOf(":");
        int kuohaoIndex = _unProcessWords.IndexOf("(");
        if (kuohaoIndex >= 0 && kuohaoIndex < 2)
        {
          //需要一个类型一个名称,所以括号在第 2或者以后才算是函数 不然有可能是if之类的
          typeName = null;
          return false;
        }

        if (kuohaoIndex > -1)
        {
          typeName = _unProcessWords[kuohaoIndex - 2];
        }
        else if (maohaoIndex > -1)
        {
          typeName = _unProcessWords[maohaoIndex - 2];
        }
        else
        {
          typeName = _unProcessWords[^3];
        }

        if (Define.Permissions.Contains(_unProcessWords[0]))
        {
          //如果第一项是 public private等等的 算是有权限定义
          return true;
        }
      }

      typeName = null;
      return false;
    }

    /// <summary>
    /// 参数,带大括号包围的结构
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    private bool isVariable_StructureAddingWords(out string typeName, out Nullable<PermissionEnum> permission)
    {
      if (_unProcessWords == null ||
          //unProcessWords.Count < 3
          _unProcessWords.Count < 2
         )
      {
        typeName = null;
        permission = null;
        return false;
      }

      if (_unProcessWords[^1] == "{")
      {
        int maohaoIndex = _unProcessWords.IndexOf(":");
        if (maohaoIndex > -1)
        {
          typeName = _unProcessWords[maohaoIndex - 2];
        }
        else
        {
          typeName = _unProcessWords.Count == 2 ? null : _unProcessWords[^3];
        }

        if (Define.Permissions.Contains(_unProcessWords[0]))
        {
          permission = ConvertString2Permission(_unProcessWords[0]);
        }
        else
        {
          permission = null;
        }

        return true;
      }

      typeName = null;
      permission = null;
      return false;
    }

    private bool IsStatementWithStructure(out string statementType)
    {
      if (_unProcessWords == null ||
          _unProcessWords[^1] != "{"
         )
      {
        statementType = null;
        return false;
      }

      int tagCount = _unProcessWords.Count - 1;
      if (tagCount == 0)
      {
        statementType = null;
        return true;
      }
      else if (tagCount == 2)
      {
        if (_unProcessWords[0] == "else" && _unProcessWords[1] == "if")
        {
          statementType = "else if";
          return true;
        }
      }
      else if (tagCount == 1 && new List<string>() {"if", "else", "switch", "while"}.IndexOf(_unProcessWords[0]) == 0)
      {
        statementType = _unProcessWords[0];
        return true;
      }

      statementType = null;
      return false;
    }

    /// <summary>
    /// 是否为正您在添加If语句段 的词汇集
    /// </summary>
    /// <returns></returns>
    private bool IsIfStatementAddingWords()
    {
      if (_unProcessWords == null || _unProcessWords.Count < 1)
      {
        return false;
      }

      int ifIndex = _unProcessWords.IndexOf("if");
      int kuohaoIndex = _unProcessWords.IndexOf("(");
      int kuohuiIndex = _unProcessWords.IndexOf(")");
      int dakuohao = _unProcessWords.IndexOf("{");
      if (dakuohao > kuohuiIndex && kuohuiIndex > kuohaoIndex && kuohaoIndex > ifIndex && ifIndex >= 0)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// 是否为else if  语句
    /// </summary>
    /// <returns></returns>
    private bool IsElseIfStatementAddingWords()
    {
      if (_unProcessWords == null || _unProcessWords.Count < 1)
      {
        return false;
      }

      int elseIndex = _unProcessWords.IndexOf("else");
      int ifIndex = _unProcessWords.IndexOf("if");
      int kuohaoIndex = _unProcessWords.IndexOf("(");
      int kuohuiIndex = _unProcessWords.IndexOf(")");
      int dakuohao = _unProcessWords.IndexOf("{");
      if (dakuohao > kuohuiIndex && kuohuiIndex > kuohaoIndex && kuohaoIndex > elseIndex && elseIndex > ifIndex &&
          ifIndex >= 0)
      {
        return true;
      }

      return false;
    }

    private bool IsElseStatementAddingWords()
    {
      if (_unProcessWords == null || _unProcessWords.Count < 1)
      {
        return false;
      }

      int elseIndex = _unProcessWords.IndexOf("else");
      int dakuohao = _unProcessWords.IndexOf("{");
      if (dakuohao > elseIndex && elseIndex >= 0)
      {
        return true;
      }

      return false;
    }

    private bool IsInterfaceAddingWords()
    {
      if (_unProcessWords == null || _unProcessWords.Count < 1)
      {
        return false;
      }

      int interfaceWordIndex = _unProcessWords.IndexOf("interface");
      int dakuohaoIndex = _unProcessWords.IndexOf("{");
      //>+1是因为中间还有一个名字
      if (interfaceWordIndex >= 0 && dakuohaoIndex > interfaceWordIndex + 1)
      {
        return true;
      }

      return false;
    }

    private bool IsNamespaceAddingWords()
    {
      if (_unProcessWords == null || _unProcessWords.Count < 1)
      {
        return false;
      }

      if (_unProcessWords.Count == 3 && _unProcessWords[0] == "namespace")
      {
        return true;
      }

      return false;
    }

    private bool IsUsingAddingWords()
    {
      if (_unProcessWords == null || _unProcessWords.Count < 1)
      {
        return false;
      }

      if (_unProcessWords.Count == 3 && _unProcessWords[0] == "using")
      {
        return true;
      }

      return false;
    }

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

    private bool IsInNotesLine()
    {
      foreach (var s in _spaces)
      {
        if (s is NotesLine)
        {
          return true;
        }
      }

      return false;
    }

    private bool InInNotesArea()
    {
      foreach (var s in _spaces)
      {
        if (s is NotesArea)
        {
          return true;
        }
      }

      return false;
    }

    private bool IsInSharpLine()
    {
      foreach (var s in _spaces)
      {
        if (s is SharpLine)
        {
          return true;
        }
      }

      return false;
    }

    private bool IsValidEndWordForCurrent()
    {
      string usingContent = string.Format("{0}", _tempWord);
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

    private NotesLine GetCurrentNotesLine()
    {
      return (_spaces[^1] as NotesLine)!;
    }

    private NotesArea GetCurrentNotesArea()
    {
      return (_spaces[^1] as NotesArea)!;
    }

    private SharpLine GetCurrentSharpLine()
    {
      return (_spaces[^1] as SharpLine)!;
    }
  }
}
