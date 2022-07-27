namespace CS2TS;

public static class TypeMapDefine
{
  /// <summary>
  /// 使用给定的C#格式确认其在TypeScript中的对应格式
  /// </summary>
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
      case "int64":
      case "int32":
      case "uint":
      case "uint16":
      case "uint32":
      case "uint64":
      case "long":
      case "double":
      case "decimal":
      case "float":
      case "single":
      case "byte":
        ret = "number";
        break;
      case "bool":
        ret = "boolean";
        break;
      default:
        ret = type.Name;
        break;
    }

    return ret;
  }

  /// <summary>
  /// 获取指定的类型定义对应的TypeScripts中的默认值
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static string GetTypeScriptTypeDefaultValue(TypeDefine type)
  {
    var ret = "";
    switch (type.Name.ToLower())
    {
      case "string":
        ret = "String()";
        break;
      case "int":
      case "int64":
      case "int32":
      case "uint":
      case "uint16":
      case "uint32":
      case "uint64":
      case "long":
      case "double":
      case "decimal":
      case "float":
      case "single":
      case "byte":
        ret = "Number()";
        break;
      case "bool":
        ret = "Boolean";
        break;
    }

    return ret;
  }
}
