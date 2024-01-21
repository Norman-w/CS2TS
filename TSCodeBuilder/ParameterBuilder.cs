using System.Text;

namespace CS2TS;

public class ParameterBuilder
{
  /// <summary>
  /// 生成parameter 参数的代码.如果参数有名字,自动带名字,没有名字的就不带. 如果参数是泛型 自动下钻.
  /// </summary>
  /// <param name="param"></param>
  /// <param name="forReturnParameter">是否为返回值生成参数.如果为返回值生成参数的话,nullable long的类型应该返回为 number|undefined 作为入参则是 number?</param>
  /// <returns></returns>
  public static string ProcessParameter(Parameter param, bool forReturnParameter)
  {
    var ret = new StringBuilder();
    var name = param.Name;
    //如果有名字的 要加上 名字和冒号的模式 name:
    if (!string.IsNullOrEmpty(name))
    {
      ret.Append(name);
      //如果可以不设置的 就是在后面有?的 并且没有默认值的 直接加问号
      //比如说 cs中的 int? a = null; 这种情况
      //应当转换为 ts的 a:number | null = null;
      //而不是 a?:number = null; 因为a?表示 可以不指定a,而不是a可以为null
      if (param.Nullable && param.DefaultValue == null)
      {
        ret.Append('?');
      }
      ret.Append(": ");
    }
    
    var type = param.Type;
    var typeName = type.Name;

    #region 处理字典类,特殊的 Dictionary<string,string> 转换成 {[Key:string],Value:string} 如果字典里面还有字典 还继续向内转换.

    if (type.IsGeneric && typeName == "Dictionary")
    {
      //Dictionary<string,string> 转换成 {[Key:string],Value:string}
      var keyParam = type.GenericParamTypeList[0];
      var valueParam = type.GenericParamTypeList[1];
      var keyType = keyParam.Type;
      var valueType = valueParam.Type;
      var keyTypeName = TypeMapDefine.GetTypeScriptTypeName(keyType);
      var valueTypeName = TypeMapDefine.GetTypeScriptTypeName(valueType);
      //dictionary结构开始
      ret.Append("{[Key: ");
      //如果key还是一个泛型的话 下钻
      if (keyType.IsGeneric)
      {
        ret.Append(ProcessParameter(keyParam, forReturnParameter));
      }
      //如果不是,直接就指定成转换出来的ts中的类型.
      else
      {
        ret.Append(keyTypeName);
      }

      ret.Append("]: ");
      //如果value还是一个泛型的话 下钻.
      if (valueType.IsGeneric)
      {
        ret.Append(ProcessParameter(valueParam,forReturnParameter));
      }
      //如果不是,直接就指定成转换出来的ts类型.
      else
      {
        ret.Append(valueTypeName);
      }
      //dictionary结构结束
      ret.Append('}');
    }
    
    #endregion

    #region 如果是nullable类型的

    else if (type.IsGeneric && typeName.ToLower() == "nullable")
    {
      //作为入参时,Nullable<long>转换成 number?
      //作为返回参数时, Nullable<long>转换成 number|undefined
      var innerParameter = type.GenericParamTypeList?[0];
      var innerParameterType = innerParameter?.Type;
      var innerParameterTypeName = innerParameterType == null? string.Empty : TypeMapDefine.GetTypeScriptTypeName(innerParameterType);

      //如果 还是一个泛型的话 下钻
      if (innerParameterType is {IsGeneric: true})
      {
        if (innerParameter != null)
          ret.Append(ProcessParameter(innerParameter, false));
      }
      //如果不是,直接就指定成转换出来的ts中的类型.
      else
      {
        if (forReturnParameter)
        {
          ret.Append(innerParameterTypeName).Append("|undefined");
        }
        else
        {
          ret.Append(innerParameterTypeName).Append('?');
        }
      }
    }

    #endregion

    #region 如果是可为空的带默认值的参数

    else if (param.Nullable && param.DefaultValue != null)
    {
      //比如 string? id 翻译为 id: string | null
      ret.Append($"{param.Type.Name} | null");
      //默认值
      ret.Append($" = {param.DefaultValue}");
    }

    #endregion

    //如果只是一般的,添加参数的类型即可,如果参数有名字的话前面已经添加了.
    else
    {
      ret.Append(TypeMapDefine.GetTypeScriptTypeName(type));
    }

    return ret.ToString();
  }

}
