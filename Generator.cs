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
      foreach (var codeFileNamespace in codeFile.Namespaces)
      {
        processNamespace(codeFileNamespace);
      }

      foreach (var cls in codeFile.Classes)
      {
        processClassCode(cls);
      }

      foreach (var codeFileEnum in codeFile.Enums)
      {
        processEnum(codeFileEnum);
      }

      foreach (var codeFileFunction in codeFile.Functions)
      {
        processFunction(codeFileFunction);
      }

      return _currentCode.ToString();
    }

    #endregion

    #region 私有函数

    private string getTab(int layerDepth)
    {
        var tab = new StringBuilder();
        for (int i = 0; i < layerDepth; i++)
        {
            tab.Append('\t');
        }

        return tab.ToString();
    }

    private void processInterface(Interface @interface)
    {
      _currentLayerDepth++;
      // var classCode = new StringBuilder();
      var tab = getTab(_currentLayerDepth);

      _currentCode.Append(tab).AppendLine($"{tab}interface {@interface.Name}\r\n");
      _currentCode.Append(tab).AppendLine("{");

      if (@interface.Functions != null)
      {
        foreach (var function in @interface.Functions)
        {
          processFunction(function);
        }
      }

      //end namespace code
      _currentCode.AppendLine(getTab(_currentLayerDepth)).Append('}');
      _currentLayerDepth--;
    }
    private void processClassCode(Class cls)
    {
        _currentLayerDepth++;
        // var classCode = new StringBuilder();
        var tab = getTab(_currentLayerDepth);
        var toInterface = false || cls.Variables!= null && cls.Variables.Count>0 && cls.Classes == null && cls.Functions == null;

        if (toInterface)
        {
            _currentCode.Append(tab).AppendLine($"{tab}interface {cls.Name}\r\n");
            _currentCode.Append(tab).AppendLine("{");

            if (cls.Classes != null)
            {
              foreach (var subCls in cls.Classes)
              {
                processClassCode(subCls);
              }
            }

            if (cls.Variables != null)
            {
              foreach (var variable in cls.Variables)
              {
                processVariable(variable);
              }
            }

            if (cls.Functions != null)
            {
              foreach (var function in cls.Functions)
              {
                processFunction(function);
              }
            }
        }
        else
        {

        }
        //end class code
        _currentCode.AppendLine(getTab(_currentLayerDepth)).Append('}');
        _currentLayerDepth--;
    }

    private void processEnum(EnumDefine enumDefine)
    {
      _currentLayerDepth++;
      var tab = getTab(_currentLayerDepth);

      _currentCode.Append(tab);
      if (enumDefine.Permission!= null)
      {
        _currentCode.Append(enumDefine.Permission.ToString()).Append(' ');
      }

      if (enumDefine.IsStatic == true)
      {
        _currentCode.Append("static ");
      }

      var typeName = TypeMapDefine.GetTypeScriptTypeName(enumDefine.Type);
      _currentCode.Append(typeName).Append(' ').Append(enumDefine.Name).AppendLine(",");
      _currentLayerDepth--;
    }

    private void processNamespace(NameSpace nameSpace)
    {
      _currentLayerDepth++;
      // var classCode = new StringBuilder();
      var tab = getTab(_currentLayerDepth);

      _currentCode.Append(tab).AppendLine($"{tab}namespace {nameSpace.Name}\r\n");
      _currentCode.Append(tab).AppendLine("{");

      if (nameSpace.Classes != null)
      {
        foreach (var cClass in nameSpace.Classes)
        {
          processClassCode(cClass);
        }
      }

      if (nameSpace.Enums != null)
      {
        foreach (var enumDefine in nameSpace.Enums)
        {
          processEnum(enumDefine);
        }
      }

      if (nameSpace.Interfaces != null)
      {
        foreach (var iInterface in nameSpace.Interfaces)
        {
          processInterface(iInterface);
        }
      }

      //end namespace code
      _currentCode.AppendLine(getTab(_currentLayerDepth)).Append('}');
      _currentLayerDepth--;
    }

    private void processVariable(Variable variable)
    {
      if (variable is VariableNoStructure)
      {
        processVariableNoStructure(variable as VariableNoStructure);
      }
      else if (variable is VariableWithStructure)
      {
        processVariableWithStructure(variable as VariableWithStructure);
      }
    }

    private void processVariableNoStructure(VariableNoStructure? vns)
    {
      _currentLayerDepth++;
      var tab = getTab(_currentLayerDepth);

      _currentCode.Append(tab);
      if (vns.Permission!= null)
      {
        _currentCode.Append(vns.Permission.ToString()).Append(' ');
      }

      if (vns.IsStatic == true)
      {
        _currentCode.Append("static ");
      }

      var typeName = TypeMapDefine.GetTypeScriptTypeName(vns.Type);
      _currentCode.Append(typeName).Append(' ').Append(vns.Name).AppendLine(";");
      _currentLayerDepth--;
    }

    private void processVariableWithStructure(VariableWithStructure? vws)
    {
      _currentLayerDepth++;
      var tab = getTab(_currentLayerDepth);

      _currentCode.Append(tab);
      if (vws.Permission!= null)
      {
        _currentCode.Append(vws.Permission.ToString()).Append(' ');
      }

      if (vws.IsStatic == true)
      {
        _currentCode.Append("static ");
      }

      var typeName = TypeMapDefine.GetTypeScriptTypeName(vws.Type);

      _currentCode.Append(typeName).Append(' ').Append(vws.Name).AppendLine("{");

      #region 添加大括号内部的代码内容部分

      #endregion

      _currentCode.AppendLine(tab).AppendLine("}");
      _currentLayerDepth--;
    }

    private void processFunction(Function function)
    {
      _currentLayerDepth++;
      // var classCode = new StringBuilder();
      var tab = getTab(_currentLayerDepth);


      _currentLayerDepth--;
    }
    #endregion
}
