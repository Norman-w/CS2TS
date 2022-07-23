namespace CS2TS;

public static class TypeMapDefine
{
  /// <summary>
  /// 使用给定的C#格式确认其在TypeScript中的对应格式
  /// </summary>
  /// <param name="nameInCodeFile"></param>
  /// <returns></returns>
  public static string GetTypeScriptTypeName(TypeDefine type)
  {
    var ret = "any";
    if (type.IsGeneric)
    {
      return type.FullName;
    }
    switch (type.Name.ToLower())
    {
      case "string":
        ret = "string";
        break;
      case "int":
      case "long":
      case "double":
      case "decimal":
      case "float":
      case "single":
      case "byte":
        ret = "number";
        break;
    }

    return ret;
  }
}
