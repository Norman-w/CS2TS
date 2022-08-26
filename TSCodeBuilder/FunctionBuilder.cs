using System.Text;
using CS2TS.Model;

namespace CS2TS;

public class FunctionBuilder
{
  #region 在同级别中获取相同名字的函数集合

  public static List<Function> GetSameNameFunctions(string functionName, CodeNode parent)
  {
    var fs = new List<Function>();
    for (int i = 0; i < parent.Chirldren.Count; i++)
    {
      var current = parent.Chirldren[i];
      if (current is Function && (current as Function).Name == functionName)
      {
        fs.Add(current as Function);
      }
    }
    return fs;
  }

  #endregion
  #region 把函数按照访问权限修饰符进行分组

  public static Dictionary<PermissionEnum, List<Function>> GroupFunctionsByPermission(List<Function> list)
  {
    Dictionary<PermissionEnum, List<Function>> ret = new Dictionary<PermissionEnum, List<Function>>();
    foreach (var f in list)
    {
      PermissionEnum realPermission = (f.Permission == null) ? PermissionEnum.Private : f.Permission.Value;
      if (!ret.ContainsKey(realPermission))
      {
        ret.Add(realPermission, new List<Function>());
      }
      ret[realPermission].Add(f);
    }
    return ret;
  }

  #endregion
  #region 把一堆同名函数按照ts的方式进行输出，传入的参数为克隆数组，不能是直接传入class中的，否则会影响class遍历children

  public static string BuildSameNameFunctionsCode(
    List<Function> functions, 
    CodeNode parent, 
    string tab,
    //追加到方法名字后面的权限名称。如果在cs中多个函数的权限不一样，在ts中没有办法那样做只能通过函数名字区分了。
    string appendPermissionAfterFuncName
    )
  {
    StringBuilder ret = new StringBuilder();
    #region 按照参数的多少进行排序，函数多的放在前面。类似于做一个表出来，检查每个参数的各个函数定义中的情况
    functions.Sort((a, b) =>
    {
      var aFuncParamsCount = a.InParameters == null? 0:a.InParameters.Count;
      var bFuncParamsCount = b.InParameters == null? 0:b.InParameters.Count;
      if (aFuncParamsCount > bFuncParamsCount)
        return -1;
      if (aFuncParamsCount < bFuncParamsCount)
        return 1;
      return 0;
    });
    #endregion
    //遍历参数多的那个，其他的函数如果没有这个参数，就标记为该参数可以为undefined
    var maxParamsFunction = functions[0];
    var permission = maxParamsFunction.Permission ?? PermissionEnum.Private;
    //每一行都有一个头定义，然后最后一行是对这些定义的总结 像下面这样
    // public fiiFunc(sss:boolean):void;
    // public fiiFunc(b: number) :void;
    // public fiiFunc(str: string) :void;
    // public fiiFunc() :string ;
    // public fiiFunc(param0? : number|string|boolean|undefined):string|void
    // {

    //默认的开头是 public fiiFunc(这样的 然后开始往里面加参数
    // var defaultHeaderStringBuilder = new StringBuilder(string.Format("{0} {1}(", permission.ToString().ToLower(), maxParamsFunction.Name));
    List<StringBuilder> functionHeaderDefineStringBuilders = new List<StringBuilder>();
    //实际被调用的方法体代码集,如下面的结构
    // private fiiFunc_0(b:number):void{
    //         
    // }
    List<StringBuilder> functionStructureDefineStringBuilders = new List<StringBuilder>();
    // Enumerable.Repeat(
    // defaultHeaderStringBuilder
    // ,functions.Count).ToList(); 
    //返回值的定义种类。是否有多个函数的返回值类型不一致
    Dictionary<string, Parameter> returnTypeDefines = new Dictionary<string, Parameter>();

    #region 制作函数的标头，同时检查一下函数的返回参数是否一样

    for (int j = 0; j < functions.Count; j++)
    {
      var currentFunc = functions[j];
      //构建函数声明头
      functionHeaderDefineStringBuilders.Add(new StringBuilder(
        BuildFunctionCode(currentFunc,appendPermissionAfterFuncName, parent, tab, true, null)
        ));
      //构建函数的真正结构.
      functionStructureDefineStringBuilders.Add(new StringBuilder(
            BuildFunctionCode(currentFunc,string.Format("_{0}",j),parent,tab,false, PermissionEnum.Private)
          )
        );
      //BuildFunctionCode会自动换行 所以这里不需要AppendLine
      ret.Append(functionHeaderDefineStringBuilders[j].ToString());
      //检查返回值类型是不是一样的
      var returnParamString = ParameterBuilder.ProcessParameter(currentFunc.ReturnParameter, true);
      if (returnTypeDefines.ContainsKey(returnParamString) == false)
      {
        returnTypeDefines.Add(returnParamString, currentFunc.ReturnParameter);
      }
    }

    #endregion

    //构建完同名函数的头以后，构建统领函数
    var leaderFunctionCode = BuildLeaderFunction(functions, parent, maxParamsFunction, returnTypeDefines, appendPermissionAfterFuncName, tab);
    
    ret.Append(leaderFunctionCode);


    //把实际被调用的函数结果放在下面
    foreach (var functionStructureDefineStringBuilder in functionStructureDefineStringBuilders)
    {
      ret.AppendLine(functionStructureDefineStringBuilder.ToString());
    }
    
    
    return ret.ToString();
  }

  #endregion

  private static string BuildLeaderFunction(
    List<Function> functions, 
    CodeNode parent, 
    Function maxParamsFunction, 
    Dictionary<string, Parameter> returnTypeDefines,
    string appendPermissionAfterFuncName, 
    string tab)
  {
    var maxParamCount = maxParamsFunction.InParameters == null ? 0 : maxParamsFunction.InParameters.Count;
     #region 处理统领函数，先生成统领函数头，包含public fiiFunc（这样的信息，然后后面一会加入参数，最后加入返回值)
    //统领函数的代码
    var leaderFunctionCode = new StringBuilder();
    leaderFunctionCode.Append(tab).Append(BuildPermission(maxParamsFunction, parent));
    //public fiiFunc(param0? : number|string|boolean|undefined):string|void
    leaderFunctionCode.Append(' ').Append(maxParamsFunction.Name).Append(appendPermissionAfterFuncName).Append('(');
    #endregion
    
    //所有函数的所有参数版本，key是参数名称，value是这个参数的版本
    var allParamVersions = new Dictionary<string, Dictionary<string,Parameter>>();
    //好了到这个地方已经有了所有的函数的头了。就是可以调用的头。也有了统领函数的头，然后下面开始定义他们的类型信息,用于统领函数
    #region 横着走，每一个参数遍历一次，每次中遍历多个函数，把他们的定义头完善并且把他们的可能类型加入到索引函数中。
    for (int i = 0; i < maxParamCount; i++)
    {
      //headers   column0 column1 column2

      //row0      row0c0  row0c1  row0c2
      //row1      row1c0  row1c1  row1c2
      //row2      row2c0  row2c1  --
      //row3      row3c0  row3c1  --
      //row4      row4c0  --      --
      //row5      --      --      --
      // var currentParamTypes = new Dictionary<string, TypeDefine>();
      //当前的这个参数是否可以不传（也就是说有的函数中没有这个参数)；
      var currentParamCanBeEmpty = false;
      //同样一个位置的函数可能有多个名字
      // var currentParamNames = new List<string>();
      //当前这个位置的函数的版本有哪些。如果名字或者是类型不完全一样的话 都记录到里面去
      var currentPosParamVersions = new Dictionary<string,Parameter>();
      for (int j = 0; j < functions.Count; j++)
      {
        var currentFunc = functions[j];
        //如果当前的函数 没有入参，或者是 当前的函数的入参表中没有这个入参（数量不够）那就用undefined来表示了
        if (currentFunc.InParameters == null || currentFunc.InParameters.Count - 1 < i)
        {
          currentParamCanBeEmpty = true;
        }
        //否则遍历出来这些入参的类型。
        else
        {
          var currentInParameterInCurrentFunc = currentFunc.InParameters[i];

          var currentInParameterCodeInCurrentFunc = ParameterBuilder.ProcessParameter(currentInParameterInCurrentFunc,false);
          if (currentPosParamVersions.ContainsKey(currentInParameterCodeInCurrentFunc) == false)
          {
            currentPosParamVersions.Add(currentInParameterCodeInCurrentFunc, currentInParameterInCurrentFunc);
          } 
        }
      }
      if (i > 0)
      {
        leaderFunctionCode.Append(',');
      }
      var currentParamRealName = string.Format("param{0}", i);
      //如果当前的入参类型不是一个 或者是名字不相同，那就生成为param0这样的格式，如果相同，那就用这个类型和这个名字
      if (currentPosParamVersions.Count > 1)
      {
        //param0:
        leaderFunctionCode.Append(currentParamRealName);
        //如果有的函数中包含undefined定义，也就是没有这个参数，这个参数还要支持 ? 标志符表示此参数可以没有
          if(currentParamCanBeEmpty)
          {
            leaderFunctionCode.Append('?');
          }
          leaderFunctionCode.Append(':');
        int typeKindIndex = 0;
        //number|string|undefined
        foreach (var current in currentPosParamVersions)
        {
          if (typeKindIndex > 0)
          {
            leaderFunctionCode.Append('|');
          }
          var currentParamVersion = current.Value;
          var currentParamTypeName = TypeMapDefine.GetTypeScriptTypeName(currentParamVersion.Type);
          leaderFunctionCode.Append(currentParamTypeName);
          
          typeKindIndex++;
        }
      }
      else
      {
        currentParamRealName = maxParamsFunction.InParameters[i].Name;
        
        leaderFunctionCode.Append(currentParamRealName);
        if(currentParamCanBeEmpty)
        {
          leaderFunctionCode.Append('?');
        }
        leaderFunctionCode.Append(':').
          Append(TypeMapDefine.GetTypeScriptTypeName(maxParamsFunction.InParameters[i].Type));
      }
      allParamVersions.Add(currentParamRealName, currentPosParamVersions);
    }

    leaderFunctionCode.Append(") :");
    #endregion

    #region 在统领函数中添加返回值的参数信息

    if (returnTypeDefines.Count > 1)
    {
      int returnTypeIndex = 0;
      foreach (var returnTypeDefine in returnTypeDefines)
      {
        if (returnTypeIndex > 0)
        {
          leaderFunctionCode.Append('|');
        }
        leaderFunctionCode.Append(ParameterBuilder.ProcessParameter(returnTypeDefine.Value, true));
        returnTypeIndex++;
      }
    }
    else
    {
      leaderFunctionCode.Append(ParameterBuilder.ProcessParameter(maxParamsFunction.ReturnParameter, true));
    }

    #endregion

    #region 添加统领函数结构体 
    
    leaderFunctionCode.AppendLine("")
      .Append(tab).AppendLine("{");
     
    #region 函数内部内容 也就是大括号里面的内容

    var allParamNames = new List<string>(allParamVersions.Keys);
    for (int i = 0; i < functions.Count; i++)
    {
      var current = functions[i];
      var currentFunctionParamCount = current.InParameters == null ? 0 : current.InParameters.Count;
      //构建if语句,增加if语句来判断统领函数所收到的值是属于哪个函数
      StringBuilder ifCode = new StringBuilder();

      bool hasParam = current.InParameters != null && current.InParameters.Count > 0;
      if (hasParam)
      {
        ifCode.Append("if(");
        for (int j = 0; j < currentFunctionParamCount; j++)
        {
          if (j > 0)
          {
            ifCode.Append(" && ");
          }
          var currentParamRealName = allParamNames[j];
          var currentParam = current.InParameters[j];
          //boolean string number etc.
          var currentTypeName = TypeMapDefine.GetTypeScriptTypeName(currentParam.Type);
          ifCode.AppendFormat("typeof {0} === \"{1}\"", currentParamRealName, currentTypeName);
        }
        ifCode.AppendLine(")")
          .Append(tab).AppendLine("{");
      }
      else
      {
        //没有参数的时候 直接就是 return 然后啥啥啥.因为没有参数的函数不可能同时有两个同一样名称的
      }
     
      //调用
      ifCode.Append(tab).Append("\t return this.").Append(maxParamsFunction.Name).Append(appendPermissionAfterFuncName).AppendFormat("_{0}", i)
        .Append("(");
      //依次写上对应的参数名称,函数需要多少个参数就写入多少个进去.
      
      for (int j = 0; j < currentFunctionParamCount; j++)
      {
        if (j > 0)
          ifCode.Append(", ");
        ifCode.Append(allParamNames[j]);
      }
      ifCode.AppendLine(");");
        if(hasParam)
      ifCode.Append(tab).AppendLine("}");

      leaderFunctionCode.Append(ifCode);
    }

    // var leaderFunctionContent = new StringBuilder("\treturn undefined;");
    //

    #endregion
    
      leaderFunctionCode.Append(tab).AppendLine("}");

    #endregion

    return leaderFunctionCode.ToString();
  }

  // private static string BuildLeaderFunctionStructure()
  // {
  //   
  // }
  /// <summary>
  /// 检查方法是否继承自接口（或类）
  /// </summary>
  /// <param name="function"></param>
  /// <param name="interface"></param>
  /// <returns></returns>
  public static bool IsExtendFromInterface(Function function, Interface @interface)
  {
    var functions = @interface.GetFunctions();
    if (functions.Count == 0)
      return false;
    var functionString = BuildFunctionCode(function, null, @interface, null, true, null);
    foreach (var current in functions)
    {
      if(current.Name!= function.Name)
        continue;
      var currentFunctionString = BuildFunctionCode(current, null, @interface, null, true, null);
      if (currentFunctionString == functionString)
        return true;
    }
    return false;
  }
  public static string BuildFunctionCode(
    Function function, 
    string appendPermissionAfterFuncName,
    CodeNode parent, 
    string tab, 
    bool asFunctionHeader,
    Nullable<PermissionEnum> overridePermission
    )
  {
    var functionCode = new StringBuilder(tab);

    #region 权限信息,Interface中的函数定义没有权限信息

    if (overridePermission!= null)
    {
      functionCode.Append(overridePermission.Value.ToString().ToLower());
    }
    else
    {
      functionCode.Append(BuildPermission(function, parent)).Append(' ');
    }

    #endregion

    #region static的信息

    if (function.IsStatic)
    {
      functionCode.Append("static ");
    }

    #endregion

    #region 函数名字和参数

    functionCode.Append(function.Name).Append(appendPermissionAfterFuncName).Append('(');
    //循环参数的类型进行依次的添加.
    StringBuilder allParamsSB = new StringBuilder();
    if (function.InParameters != null)
    {
      for (var i = 0; i < function.InParameters.Count; i++)
      {
        if (i > 0)
        {
          allParamsSB.Append(", ");
        }
        var parameter = function.InParameters[i];
        allParamsSB.Append(ParameterBuilder.ProcessParameter(parameter, false));
        // var tsTypeName = TypeMapDefine.GetTypeScriptTypeName(parameter.Type);
        // allParamsSB.AppendFormat("{0}: {1}", parameter.Name, tsTypeName);
      }
    }

    #endregion

    //添加所有入参以后添加函数的返回参数信息
    // var tsReturnType = TypeMapDefine.GetTypeScriptTypeName(function.ReturnParameter.Type);
    // _currentCode.Append(allParamsSB).Append(") :").Append(tsReturnType);
    functionCode.Append(allParamsSB).Append(") :").Append(ParameterBuilder.ProcessParameter(function.ReturnParameter, true));
    //要用type来判断 不能用 is Interface 判断.因为Class is Interface 是成立的.只要继承就会是true
    if (parent.GetType() == typeof(Interface) || asFunctionHeader)
    {
      functionCode.AppendLine(";");
    }
    else
    {
      functionCode.AppendLine(" {");

      #region 添加函数内部的内容

      //现阶段为了代码不报错,返回一个默认的结果
      var defaultReturnValue = TypeMapDefine.GetTypeScriptTypeDefaultValue(function.ReturnParameter.Type);
      if (function.ReturnParameter.Type.Name != "void")
      {
        functionCode.Append("return ").Append(defaultReturnValue).AppendLine(";");
      }

      #endregion

      //添加函数的收尾大括号
      functionCode.Append(tab).AppendLine("}");
    }
    return functionCode.ToString();
  }
  private static string BuildPermission(Function function, CodeNode parent)
  {
    StringBuilder functionCode = new StringBuilder();
    if (parent.GetType() != typeof(Interface))
    {
      //当cs中没有默认的权限信息的时候,是private.在ts中private需要默认指定.如果不指定就是public
      if (function.Permission == null)
      {
        functionCode.Append("private");
      }
      //其他的为了展示的更清楚,所有的也都加上修饰符
      else
      {
        functionCode.Append(function.Permission.ToString().ToLower());
      }
    }
    return functionCode.ToString();
  }
}
