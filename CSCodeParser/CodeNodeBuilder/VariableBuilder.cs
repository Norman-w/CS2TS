using System.Text;

namespace CS2TS;

public class VariableBuilder
{
  public static Variable Parse2Variable(List<string> words)
  {
    Variable ret = new VariableNoStructure(null, null, null, null, null, null, null, null,null);
    for (var i = 0; i < words.Count; i++)
      {
        words[i] = words[i].Replace("\t", "");
      }

      //作用域的相关字样
      var permisionIndex = Define.Permissions.IndexOf(words[0]);
      //const 字样的位置
      var constIndex = words.IndexOf("const");
      //staticc 字样的位置
      var staticIndex = words.IndexOf("static");
      //readonly 字样的位置
      var readonnlyIndex = words.IndexOf("readonly");
      var newIndex = words.IndexOf("new");
      //这里还可以添加 partial之类的关键字

      #region 名字后面的一个标志符的解析

      //public static readonly Nullable<int> vName;
      //static int vName = 0;
      //public bool vName { get; set; }
      //定义名称之前的那个分隔符. 如果有大括号的 前面一个是名称,如果有等号的,前面一个是名称,其他的,分号前面是名称
      int spliterIndex;
      if (words.Contains(":"))
      {
        spliterIndex = words.IndexOf(":");
      }
      else if (words.Contains("{"))
      {
        spliterIndex = words.IndexOf("{");
      }
      else if (words.Contains("="))
      {
        spliterIndex = words.IndexOf("=");
      }
      else
      {
        spliterIndex = words.IndexOf(";");
      }

      #endregion

      //名字前面的排除了之前作用域以及const等的固定字段以后的,都是类型的定义.
      //比如说  public  static readonly Nullable<bool> vaName 这样的
      //默认认为,变量类型为0位置,让后如果有 public static readonly const 之类的,每有一个 往后加一 加完以后的  就是 类型定义的部分
      int typeDefineStartPos = 0;
      if (newIndex >= 0)
      {
        ret.IsOverride = true;
        typeDefineStartPos++;
      }

      if (permisionIndex >= 0)
      {
        ret.Permission = PermissionBuilder.Parse2Permission(words[0]);
        typeDefineStartPos++;
      }

      if (staticIndex >= 0)
      {
        ret.IsStatic = true;
        typeDefineStartPos++;
      }

      if (readonnlyIndex >= 0)
      {
        ret.IsReadonly = true;
        typeDefineStartPos++;
      }

      if (constIndex >= 0)
      {
        ret.IsConst = true;
        typeDefineStartPos++;
      }

      int typeDefineEndPos = spliterIndex - 2;
      int nameDefinePos = typeDefineEndPos + 1;
      StringBuilder typeBuilder = new StringBuilder();
      //int typeDefineWordCount = typeDefineEndPos - typeDefineStartPos;
      for (int i = typeDefineStartPos; i <= typeDefineEndPos; i++)
      {
        typeBuilder.Append(words[i].Trim(new[] {'\t'}));
      }

      ret.Name = words[nameDefinePos].Replace("\n","").Replace("\r","");
      ret.Type = new TypeDefine() {Name = typeBuilder.ToString()};

      if (ret.CodeBody == null)
      {
        ret.CodeBody = "";
      }

      // ret.CodeBody += BuildCodeBodyByUnProcessWords();
      return ret;
  }
}
