using System.Text;
using CS2TS.Model;

namespace CS2TS;

public partial class CSharpCodeParser
{
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
      if (IsInFunction())
      {
        CodeNode parent = _spaces[^1];
        if (SemicolonParser.Parse2Statements(_unProcessWords, parent as Function))
        {
          _unProcessWords.Clear();
          _tempWord = new StringBuilder();
          return true;
        }
      }
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
              CreateFunction(false);
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
              // if (vrParent.Statements == null)
              // {
              //   vrParent.Statements = new List<Statement>();
              // }
              //
              // vrParent.Statements.Add(st);
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
          // if (swsParent.Statements == null)
          // {
          //   swsParent.Statements = new List<Statement>();
          // }
          //
          // swsParent.Statements.Add(st);
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
            CreateFunction(true);
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
}