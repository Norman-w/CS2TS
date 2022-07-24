using System.Text;

namespace CS2TS;
/// <summary>
/// ts文件创建器
/// </summary>
public class Generator
{
    #region 构造函数

    public Generator()
    {
        _currentCode = new StringBuilder();
        _currentLayerDepth = 0;
    }

    #endregion

    #region 全局变量

    private StringBuilder _currentCode;
    private int _currentLayerDepth;

    #endregion

    #region 公共函数

    public string CreateTsFile(CodeFile codeFile)
    {
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
          ProcessClassCode(cls);
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
      if (nameSpace.Notes!= null)
      {
        foreach (var nameSpaceNote in nameSpace.Notes)
        {
          ProcessNotes(nameSpaceNote);
        }
      }
      _currentCode.Append($"namespace {nameSpace.Name}").AppendLine(" {");
      if (nameSpace.Classes != null)
      {
        foreach (var cClass in nameSpace.Classes)
        {
          ProcessClassCode(cClass);
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

      if (@interface.Notes!= null)
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

    private void ProcessClassCode(Class cls)
    {
        _currentLayerDepth++;
        var tab = GetTab(_currentLayerDepth);
        var toInterface = false || cls.Variables!= null && cls.Variables.Count>0 && cls.Classes == null && cls.Functions == null;

        if (cls.Notes!= null)
        {
          foreach (var clsNote in cls.Notes)
          {
            ProcessNotes(clsNote);
          }
        }

        if (toInterface)
        {
            _currentCode.Append(tab).Append($"interface {cls.Name}").AppendLine(" {");
            if (cls.Classes != null)
            {
              foreach (var subCls in cls.Classes)
              {
                ProcessClassCode(subCls);
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
                ProcessVariable(variable, true,true);
              }
            }

            if (cls.Functions != null)
            {
              foreach (var function in cls.Functions)
              {
                ProcessFunction(function);
              }
            }
        }
        else
        {

        }
        _currentCode.Append(tab).AppendLine("}");
        _currentLayerDepth--;
    }

    private void ProcessEnum(EnumDefine enumDefine)
    {
      _currentLayerDepth++;
      var tab = GetTab(_currentLayerDepth);

      if (enumDefine.Notes!= null)
      {
        foreach (var enumDefineNote in enumDefine.Notes)
        {
          ProcessNotes(enumDefineNote);
        }
      }
      _currentCode.Append(tab).Append($"enum {enumDefine.Name} ").AppendLine(" {");
      _currentCode.Append(tab).AppendLine("}");
      _currentLayerDepth--;
    }

    private void ProcessVariable(Variable variable,
      bool ignorePermission,
      bool ignoreStatic
    )
    {
      if (variable.Notes!= null)
      {
        foreach (var variableNote in variable.Notes)
        {
          ProcessNotes(variableNote);
        }
      }
      if (variable is VariableNoStructure)
      {
        ProcessVariableNoStructure(variable as VariableNoStructure, ignorePermission,ignoreStatic);
      }
      else if (variable is VariableWithStructure)
      {
        ProcessVariableWithStructure(variable as VariableWithStructure, ignorePermission,ignoreStatic);
      }
    }

    private void ProcessVariableNoStructure(VariableNoStructure? vns,
    bool ignorePermission,
    bool ignoreStatic)
    {
      _currentLayerDepth++;
      var tab = GetTab(_currentLayerDepth);

      _currentCode.Append(tab);
      if (vns.Permission!= null && !ignorePermission)
      {
        _currentCode.Append(vns.Permission.ToString()).Append(' ');
      }

      if (vns.IsStatic == true && !ignoreStatic)
      {
        _currentCode.Append("static ");
      }

      var typeName = TypeMapDefine.GetTypeScriptTypeName(vns.Type);
      _currentCode.Append(vns.Name).Append(": ").Append(typeName).AppendLine(";");
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
      if (vws.Permission!= null && !ignorePermission)
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

      if (function.Notes!= null)
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

      _currentCode.Append(tab).AppendLine("*/");
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
      _currentLayerDepth++;
      var tab = GetTab(_currentLayerDepth);
      _currentCode.Append(tab).Append("#").AppendLine(sharpLine.Content.TrimEnd('\r').TrimEnd('\n'));
      _currentLayerDepth--;
    }
    #endregion
}
