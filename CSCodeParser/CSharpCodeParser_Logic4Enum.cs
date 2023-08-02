using System.Text;

namespace CS2TS;

public partial class CSharpCodeParser
{
  
  #region 未处理的单词们转换成枚举中的值

  private void unProcessWords2Variable4EnumDefine(EnumDefine enumDefine)
  {
    //解析枚举的值
    object value = null;
    var name = _unProcessWords[0];
    var denghaoIndex = _unProcessWords.IndexOf("=");
    TypeDefine type = new TypeDefine() {Name = "unknown enum value type"};
    if (denghaoIndex>=0)
    {
      //如果有等号的话 比如 enum xxx { a = 1 },
      //当前枚举选项设置为等号后面的值,并且在处理下一个枚举值时(后面的一项)如果没有设定值 比如 enum xxx { a = 1, b }
      //为这一项的值+1,也就是自然的b为2
      var valStr = _unProcessWords[denghaoIndex + 1];
      if (enumDefine.Extends!= null)
      {
        value = long.Parse(valStr);
        type =
          enumDefine.Extends.Contains("long") ? new TypeDefine() {Name = "long"}
          : enumDefine.Extends.Contains("uint") ? new TypeDefine() {Name = "uint"}
          : new TypeDefine() {Name = "int"};
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

    var variable = new VariableNoStructure(name,type,null,null,null,null,null,value,null);
    enumDefine.Chirldren.Add(variable);
  }

  #endregion
}