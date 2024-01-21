using System.Text;

namespace CS2TS;

public partial class CSharpCodeParser
{
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
}