using System.Text;

namespace CS2TS;

/// <summary>
/// ts文件创建器
/// </summary>
public class TypeScriptCodeGenerator
{
  #region 构造函数

  /// <summary>
  /// 初始化TypeScripts生成器
  /// </summary>
  /// <param name="config">配置文件.如不指定,使用默认配置</param>
  public TypeScriptCodeGenerator(TypescriptGeneratorConfig? config = null)
  {
    _currentCode = new StringBuilder();
    _currentLayerDepth = 0;
    _config = config ?? new TypescriptGeneratorConfig();
  }

  #endregion

  #region 全局变量

  /// <summary>
  /// TS文件生成器的配置
  /// </summary>
  private readonly TypescriptGeneratorConfig? _config;
  private readonly StringBuilder _currentCode;
  private int _currentLayerDepth;

  #endregion

  #region 公共函数

  /// <summary>
  /// 使用给定的代码结构中间量,生成TypeScripts代码
  /// </summary>
  /// <param name="codeFile">由cs解析出来的代码结构</param>
  /// <returns>生成的TypeScripts代码</returns>
  public string CreateTsFile(CodeFile codeFile)
  {
    if (codeFile.Notes != null)
    {
      foreach (var codeFileNote in codeFile.Notes)
      {
        ProcessNotes(codeFileNote);
      }
    }
    if (codeFile.Namespaces != null)
    {
      foreach (var codeFileNamespace in codeFile.Namespaces)
      {
        ProcessNamespace(codeFileNamespace);
      }
    }
    if (codeFile.Classes != null)
    {
      foreach (var cls in codeFile.Classes)
      {
        ProcessClass(cls);
      }
    }
    if (codeFile.Enums != null)
    {
      foreach (var codeFileEnum in codeFile.Enums)
      {
        ProcessEnum(codeFileEnum);
      }
    }
    if (codeFile.Functions != null)
    {
      foreach (var codeFileFunction in codeFile.Functions)
      {
        ProcessFunction(codeFileFunction);
      }
    }
    return _currentCode.ToString();
  }

  #endregion

  #region 私有函数

  private static string GetTab(int layerDepth)
  {
    var tab = new StringBuilder();
    for (int i = 0; i < layerDepth; i++)
    {
      tab.Append('\t');
    }

    return tab.ToString();
  }

  private void ProcessNamespace(NameSpace nameSpace)
  {
    if (nameSpace.Notes != null)
    {
      foreach (var nameSpaceNote in nameSpace.Notes)
      {
        ProcessNotes(nameSpaceNote);
      }
    }

    _currentCode.Append($"export namespace {nameSpace.Name}").AppendLine(" {");
    if (nameSpace.Classes != null)
    {
      foreach (var cClass in nameSpace.Classes)
      {
        ProcessClass(cClass);
      }
    }

    if (nameSpace.Enums != null)
    {
      foreach (var enumDefine in nameSpace.Enums)
      {
        ProcessEnum(enumDefine);
      }
    }

    if (nameSpace.Interfaces != null)
    {
      foreach (var iInterface in nameSpace.Interfaces)
      {
        ProcessInterface(iInterface);
      }
    }

    //end namespace code
    _currentCode.AppendLine("}");
  }

  private void ProcessInterface(Interface @interface)
  {
    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);

    if (@interface.Notes != null)
    {
      foreach (var interfaceNote in @interface.Notes)
      {
        ProcessNotes(interfaceNote);
      }
    }

    _currentCode.Append(tab).Append($"interface {@interface.Name}").AppendLine(" {");

    if (@interface.Functions != null)
    {
      foreach (var function in @interface.Functions)
      {
        ProcessFunction(function);
      }
    }

    _currentCode.Append(tab).AppendLine("}");
    _currentLayerDepth--;
  }

  private void ProcessClass(Class cls)
  {
    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);
    // var toInterface = false || cls.Variables != null && cls.Variables.Count > 0 && cls.Classes == null &&
    //   cls.Functions == null;

    if (cls.Notes != null)
    {
      foreach (var clsNote in cls.Notes)
      {
        ProcessNotes(clsNote);
      }
    }

      //转换成类
      _currentCode.Append(tab);
      if (cls.Permission == PermissionEnum.Public)
      {
        _currentCode.Append("export ");
      }

      //是按照interface来处理 还是按照class来处理
      var classOrInterface = _config.ConvertClass2Interface ? "interface" : "class";
      _currentCode.Append($"{classOrInterface} {cls.Name}").AppendLine(" {");
      if (cls.Classes != null)
      {
        foreach (var subCls in cls.Classes)
        {
          ProcessClass(subCls);
        }
      }

      if (cls.Variables != null)
      {
        foreach (var variable in cls.Variables)
        {
          if (variable.Permission == null)
          {
            continue;
          }
          //处理类中的变量时,如果类转换成TypeScripts的接口的话,就不需要把字段赋初值,另外如果是转换成类但是没指定要赋初值,也不需要设置.
          ProcessVariable(variable,
            true,
            true,
            !_config.ConvertClass2Interface && _config.SetDefaultVariableValueForClass
            );
        }
      }

      if (cls.Functions != null)
      {
        foreach (var function in cls.Functions)
        {
          ProcessFunction(function);
        }
      }


    _currentCode.Append(tab).AppendLine("}");
    _currentLayerDepth--;
  }

  private void ProcessEnum(EnumDefine enumDefine)
  {
    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);

    if (enumDefine.Notes != null)
    {
      foreach (var enumDefineNote in enumDefine.Notes)
      {
        ProcessNotes(enumDefineNote);
      }
    }

    _currentCode.Append(tab).Append($"enum {enumDefine.Name} ").AppendLine(" {");
    foreach (var enumDefineVariable in enumDefine.Variables)
    {
      if (enumDefineVariable.Notes!=null)
      {
        foreach (var note in enumDefineVariable.Notes)
        {
          ProcessNotes(note);
        }
      }
      if (enumDefineVariable.Value!= null)
      {
        _currentCode.AppendFormat("{0} = {1}", enumDefineVariable.Name,enumDefineVariable.Value).AppendLine(",");
      }
      else
      {
        _currentCode.Append(enumDefineVariable.Name).AppendLine(",");
      }
    }
    _currentCode.Append(tab).AppendLine("}");
    _currentLayerDepth--;
  }

  private void ProcessVariable(Variable variable,
    bool ignorePermission,
    bool ignoreStatic,
    bool setDefaultValue = false
  )
  {
    if (variable.Notes != null)
    {
      foreach (var variableNote in variable.Notes)
      {
        ProcessNotes(variableNote);
      }
    }

    if (variable is VariableNoStructure)
    {
      ProcessVariableNoStructure(variable as VariableNoStructure, ignorePermission, ignoreStatic, setDefaultValue);
    }
    else if (variable is VariableWithStructure)
    {
      ProcessVariableWithStructure(variable as VariableWithStructure, ignorePermission, ignoreStatic);
    }
  }

  private void ProcessVariableNoStructure(VariableNoStructure? vns,
    bool ignorePermission,
    bool ignoreStatic,
    bool setDefaultValue
    )
  {
    //是否直接把简单定义的变量设置一个默认值.如果不设置的话.eslint可能会检查类中的变量没有给初值问题.
    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);

    _currentCode.Append(tab);
    if (vns.Permission != null && !ignorePermission)
    {
      _currentCode.Append(vns.Permission.ToString()).Append(' ');
    }

    if (vns.IsStatic == true && !ignoreStatic)
    {
      _currentCode.Append("static ");
    }

    var typeName = TypeMapDefine.GetTypeScriptTypeName(vns.Type);
    _currentCode.Append(vns.Name).Append(": ").Append(typeName);
    if (setDefaultValue)
    {
      _currentCode.Append(" = ").Append(TypeMapDefine.GetTypeScriptTypeDefaultValue(vns.Type));
    }

    _currentCode.AppendLine(";");
    _currentLayerDepth--;
  }

  private void ProcessVariableWithStructure(VariableWithStructure? vws,
    bool ignorePermission,
    bool ignoreStatic
  )
  {
    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);

    _currentCode.Append(tab);
    if (vws.Permission != null && !ignorePermission)
    {
      _currentCode.Append(vws.Permission.ToString().ToLower()).Append(' ');
    }

    if (vws.IsStatic == true && !ignoreStatic)
    {
      _currentCode.Append("static ");
    }

    var typeName = TypeMapDefine.GetTypeScriptTypeName(vws.Type);

    _currentCode.Append(vws.Name).Append(": ").Append(typeName).AppendLine("{");

    #region 添加大括号内部的代码内容部分

    #endregion

    _currentCode.AppendLine(tab).AppendLine("}");
    _currentLayerDepth--;
  }

  private void ProcessFunction(Function function)
  {
    _currentLayerDepth++;
    // var classCode = new StringBuilder();
    var tab = GetTab(_currentLayerDepth);

    if (function.Notes != null)
    {
      foreach (var functionNote in function.Notes)
      {
        ProcessNotes(functionNote);
      }
    }

    _currentLayerDepth--;
  }

  private void ProcessNotes(NoteBase noteBase)
  {
    if (noteBase is NotesArea)
    {
      ProcessNotesArea(noteBase as NotesArea);
    }

    else if (noteBase is NotesLine)
    {
      ProcessNoteLine(noteBase as NotesLine);
    }
  }

  private void ProcessNotesArea(NotesArea notesArea)
  {
    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);
    _currentCode.Append(tab).AppendLine("/*");
    foreach (var notesAreaLine in notesArea.Lines)
    {
      _currentCode.Append(tab).AppendLine(notesAreaLine.TrimEnd('\r').TrimEnd('\n'));
    }

    // _currentCode.Append(tab).AppendLine("*/");
    _currentLayerDepth--;
  }

  private void ProcessNoteLine(NotesLine notesLine)
  {
    if (notesLine is SharpLine)
    {
      ProcessSharpLine(notesLine as SharpLine);
      return;
    }

    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);
    _currentCode.Append(tab).Append("//").AppendLine(notesLine.Content.TrimEnd('\r').TrimEnd('\n'));
    _currentLayerDepth--;
  }

  private void ProcessSharpLine(SharpLine sharpLine)
  {
    // _currentLayerDepth++;
    // var tab = GetTab(_currentLayerDepth);
    // _currentCode.Append(tab).Append("#").AppendLine(sharpLine.Content.TrimEnd('\r').TrimEnd('\n'));
    // _currentLayerDepth--;
  }

  #endregion
}
