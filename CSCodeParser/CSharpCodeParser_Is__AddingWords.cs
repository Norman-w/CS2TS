using CS2TS.Model;

namespace CS2TS;

public partial class CSharpCodeParser
{
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

}
