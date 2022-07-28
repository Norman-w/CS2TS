using System.Diagnostics;
using System.Reflection;
using System.Text;
using CS2TS.Model;

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
  public TypeScriptCodeGenerator(CodeFile file, TypescriptGeneratorConfig? config = null)
  {
    _currentCode = new StringBuilder();
    _currentLayerDepth = 0;
    _config = config ?? new TypescriptGeneratorConfig();
    this._file = file;
    _currentSpaces = new List<CodeNode>();
  }

  #endregion

  #region 全局变量

  /// <summary>
  /// TS文件生成器的配置
  /// </summary>
  private readonly TypescriptGeneratorConfig? _config;
  private StringBuilder _currentCode;
  private int _currentLayerDepth;
  private readonly CodeFile _file;
  private readonly List<CodeNode> _currentSpaces;

  #endregion

  #region 公共函数

  /// <summary>
  /// 使用给定的代码结构中间量,生成TypeScripts代码
  /// </summary>
  /// <param name="codeFile">由cs解析出来的代码结构</param>
  /// <returns>生成的TypeScripts代码</returns>
  public string CreateTsFile()
  {
    _currentCode = new StringBuilder();
    processChildren(_file);
    return _currentCode.ToString();
  }

  #endregion

  #region 私有函数

  /// <summary>
  /// 获取类或接口的实际继承名称(非实际的可能是ns1.ns2.cls1.cls2.cls3,因为ts不支持类嵌套,所以提权查出来以后就是ns1.ns2.cls3)
  /// </summary>
  /// <param name="childName"></param>
  /// <returns></returns>
  private string FindExtentFullPath(string childName)
  {
    var ancestors = CodeNode.FindAncestors<IClassContainer>(_file, typeof(Class), childName);
    //ts中命名空间可以嵌套,类不能嵌套  类会被提权到他所在的命名你空间的儿子级.
    //所以继承的类如果还在某个类当中定义,就不需要那个类
    //比如ClassG定义在 namespaceA namespace B namespaceC classD classE classF当中.
    //返回的结果中只需要namespaceA.namespaceB.namespaceC.classF
    StringBuilder ancestorsPathBuilder = new StringBuilder();
    for (var index = 0; index < ancestors.Count; index++)
    {
      var ancestor = ancestors[index];
      //如果最后一项是class/interface的话 保留
      if (ancestor is Class && index != ancestors.Count-1)
      {
        continue;
      }

      if (ancestorsPathBuilder.Length > 0)
      {
        ancestorsPathBuilder.Append('.');
      }

      ancestorsPathBuilder.Append(ancestor.Name);
    }

    if (ancestorsPathBuilder.Length>0)
    {
      ancestorsPathBuilder.Append('.').Append(childName);
    }
    return ancestorsPathBuilder.ToString();
  }

  private string FindInterfaceFullPath(string interfaceName)
  {
    var ancestors = CodeNode.FindAncestors<IInterfaceContainer>(_file, typeof(Interface), interfaceName);
    //ts中命名空间可以嵌套,类不能嵌套  类会被提权到他所在的命名你空间的儿子级.
    //所以继承的类如果还在某个类当中定义,就不需要那个类
    //比如ClassG定义在 namespaceA namespace B namespaceC classD classE classF当中.
    //返回的结果中只需要namespaceA.namespaceB.namespaceC.classF
    StringBuilder ancestorsPathBuilder = new StringBuilder();
    for (var index = 0; index < ancestors.Count; index++)
    {
      var ancestor = ancestors[index];
      //如果最后一项是class/interface的话 保留
      if (ancestor is Interface && index != ancestors.Count-1)
      {
        continue;
      }

      if (ancestorsPathBuilder.Length > 0)
      {
        ancestorsPathBuilder.Append('.');
      }

      ancestorsPathBuilder.Append(ancestor.Name);
    }

    if (ancestorsPathBuilder.Length>0)
    {
      ancestorsPathBuilder.Append('.').Append(interfaceName);
    }
    return ancestorsPathBuilder.ToString();
  }

  // private void makeClassVisible()
  // {
  //
  // }

  void processChildren(CodeNode parent)
  {
    if (parent.Chirldren == null | parent.Chirldren.Count == 0)
    {
      return;
    }
    _currentSpaces.Add(parent);
    foreach (var chirld in parent.Chirldren)
    {
      var childType = chirld.GetType();
      if (childType == typeof(Using))
      {
        ProcessUsing(chirld as Using);
      }
      else if (childType.IsSubclassOf(typeof(NoteBase)))
      {
        ProcessNotes(chirld as NoteBase);
      }
      else if(childType == typeof(NameSpace))
      {
        ProcessNamespace(chirld as NameSpace);
      }
      else if(childType == typeof(Class))
      {
        ProcessClass(chirld as Class);
      }
      else if (childType == typeof(Interface))
      {
        ProcessInterface(chirld as Interface);
      }
      else if(childType == typeof(EnumDefine))
      {
        ProcessEnum(chirld as EnumDefine);
      }
      else if(childType == typeof(Function))
      {
        ProcessFunction(chirld as Function);
      }
      else if (chirld is Variable)
      {
        if (parent is Class)
        {
          ProcessVariable(chirld as Variable,
            true,
            true,
            parent,
            !_config.ConvertClass2Interface && _config.SetDefaultVariableValueForClass
          );
        }
        else if (parent is EnumDefine)
        {
          ProcessVariable(chirld as Variable,
            true,
            true,
            parent,
            !_config.ConvertClass2Interface && _config.SetDefaultVariableValueForClass
          );
        }
        else
        {
          ProcessVariable(chirld as Variable, false,false,
            parent,
            true
          );
        }
      }
      else if (chirld is Parameter)
      {

      }
    }
    _currentSpaces.RemoveAt(_currentSpaces.Count-1);
  }

  private static string GetTab(int layerDepth)
  {
    var tab = new StringBuilder();
    for (int i = 0; i < layerDepth; i++)
    {
      tab.Append('\t');
    }

    return tab.ToString();
  }

  private void ProcessUsing(Using u)
  {
    _currentCode.AppendFormat("import {0}", u.Name).AppendLine(";");
  }

  private void ProcessNamespace(NameSpace nameSpace)
  {
    // var notes = nameSpace.GetNotes();
    _currentCode.Append($"export namespace {nameSpace.Name}").AppendLine(" {");
    processChildren(nameSpace);
    //end namespace code
    _currentCode.AppendLine("}");
  }

  private void ProcessInterface(Interface @interface)
  {
    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);

    _currentCode.Append(tab).Append($"export interface {@interface.Name}").AppendLine(" {");

    //当接口有继承的时候,生成他的继承代码.如果继承的接口在本文件中,直接使用 namespaceName.innterfaceName的方式继承.
    //ts有一个特点是 接口和类的继承方式不一样 而cs是直接:后面跟类和接口名称,多个继承中间用逗号就行.
    //ts的接口相对简单一些 接口只能继承接口  所以在本文件中找到这个接口即可.但是如果是类,要判断继承的名字是类还是接口
    if (@interface.Extends!= null && @interface.Extends.Count>0)
    {
      // var interfaces = CodeNode.FilterOut<Interface>(_file, false);

    }
    processChildren(@interface);

    _currentCode.Append(tab).AppendLine("}");
    _currentLayerDepth--;
  }

  private void ProcessClass(Class cls)
  {
    #region ts不支持类中类,要把所有的类都提权出来到这个类上面

    //只要child中有children,把child中的子找出来,在自己所在的列表中,放在自己的前面(或者后面,要看注释或者其他情况,试一下就知道了),然后再往里面找
    //private void makeClassVisible(CodeNode owner, CodeNode classNode);
    //在class中遍历子,如果有子,调用makeClassVisible
    //如果没有 return
    // makeClassVisible(parent,parent.Chirldren.IndexOf(chirld),parent,chirld);

    // List<CodeNode> needUpgradeClasses = new List<CodeNode>();
    // CodeNode.GetNodesAllInside<Class>(ref needUpgradeClasses,cls, true);

    List<Class> needUpgradeClasses = cls.GetClasses();

    //从原来的地方删除掉.
    foreach (var needUpgradeClass in needUpgradeClasses)
    {
      cls.Chirldren.Remove(needUpgradeClass);
    }
    #endregion

    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);

      //转换成类
      _currentCode.Append(tab);
      if (cls.Permission == PermissionEnum.Public)
      {
        _currentCode.Append("export ");
      }

      //是按照interface来处理 还是按照class来处理
      var classOrInterface = _config.ConvertClass2Interface ? "interface" : "class";
      _currentCode.Append($"{classOrInterface} {cls.Name}");

      #region 处理类和接口的继承

      StringBuilder currentSpaceName = new StringBuilder();
      
      #region 获取当前这个类所在的位置
      for (int i = 0; i < _currentSpaces.Count; i++)
      {
        if (currentSpaceName.Length>0)
        {
          currentSpaceName.Append('.');
        }

        var current = _currentSpaces[i];
        if (current is IClassContainer)
        {
          currentSpaceName.Append((current as IClassContainer).Name);
        }
      }
      #endregion

      if (cls.Extends is {Count: > 0})
      {
        var extendsClassName = "";
        var implamentInterfaces = new List<string>();
        #region 获取继承的类
        extendsClassName = FindExtentFullPath(cls.Extends[0]);
        if (extendsClassName.StartsWith(currentSpaceName.ToString()) && extendsClassName.Length> currentSpaceName.Length)
        {
          extendsClassName = extendsClassName.Substring(currentSpaceName.Length+1);
        }
        #endregion

        #region 获取继承的接口
        //如果第一个不是类继承,那可能就是接口.
        var startIndex = string.IsNullOrEmpty(extendsClassName) ? 0 : 1;
        for (int i = startIndex; i < cls.Extends.Count; i++)
        {
          var implement = FindInterfaceFullPath(cls.Extends[i]);
          if (implement.StartsWith(currentSpaceName.ToString()) && implement.Length> currentSpaceName.Length)
          {
            implement = implement.Substring(currentSpaceName.Length+1);
          }
          //如果找到这个接口了,添加这个接口的绝对路径引用.如果没找到,保留使用引用的原始名称
          implamentInterfaces.Add(string.IsNullOrEmpty(implement)? cls.Extends[i] : implement);
        }
        #endregion

        #region 拼接继承字符串

        if (string.IsNullOrEmpty(extendsClassName) == false)
        {
          //如果有继承类
          _currentCode.AppendFormat(" extends {0}", extendsClassName);
        }

        if (implamentInterfaces.Count>0)
        {
          _currentCode.AppendFormat(" implements");
          for (var index = 0; index < implamentInterfaces.Count; index++)
          {
            if (index>0)
            {
              _currentCode.Append(',');
            }
            var implement = implamentInterfaces[index];
            _currentCode.AppendFormat(" {0}", implement);
          }
        }

        #endregion
      }

      #endregion
      _currentCode.AppendLine(" {");
      processChildren(cls);

    _currentCode.Append(tab).AppendLine("}");
    _currentLayerDepth--;

    //处理提权了的类
    if (needUpgradeClasses!= null && needUpgradeClasses.Count>0)
    {
      // var destIndex = parent.Chirldren.IndexOf(chirld);
      foreach (var node in needUpgradeClasses)
      {
        ProcessClass(node as Class);
      }
    }
  }

  private void ProcessEnum(EnumDefine enumDefine)
  {
    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);

    if (enumDefine.Permission == PermissionEnum.Public)
    {
      _currentCode.Append("export ");
    }

    _currentCode.Append(tab).Append($"enum {enumDefine.Name} ").AppendLine(" {");
    processChildren(enumDefine);
    _currentCode.Append(tab).AppendLine("}");
    _currentLayerDepth--;
  }

  private void ProcessVariable(Variable variable,
    bool ignorePermission,
    bool ignoreStatic,
    //如果在枚举中,结束符应该是, 其他地方结束符应该是;
    CodeNode parent,
    bool setDefaultValue = false
  )
  {
    if (variable is VariableWithStructure)
    {
      ProcessVariableWithStructure(variable as VariableWithStructure, ignorePermission, ignoreStatic,parent);
    }
    else// if (variable is VariableNoStructure)
    {
      ProcessVariableNoStructure(variable as VariableNoStructure, ignorePermission, ignoreStatic,parent, setDefaultValue);
    }
  }

  private void ProcessVariableNoStructure(VariableNoStructure? vns,
    bool ignorePermission,
    bool ignoreStatic,
    CodeNode parent,
    bool setDefaultValue
  )
  {
    //是否直接把简单定义的变量设置一个默认值.如果不设置的话.eslint可能会检查类中的变量没有给初值问题.
    _currentLayerDepth++;
    var tab = GetTab(_currentLayerDepth);

    _currentCode.Append(tab);
    if (parent is EnumDefine)
    {
      if (vns.Value == null)
      {
        _currentCode.Append(vns.Name);
      }
      else
      {
        _currentCode.AppendFormat("{0} = {1}", vns.Name, vns.Value);
      }
    }
    else
    {
      if (vns.Permission != null && !ignorePermission)
      {
        _currentCode.Append(vns.Permission.ToString()).Append(' ');
      }

      if (vns.IsStatic == true && !ignoreStatic)
      {
        _currentCode.Append("static ");
      }

      var typeName = TypeMapDefine.GetTypeScriptTypeName(vns.Type);
      var defaultValue = TypeMapDefine.GetTypeScriptTypeDefaultValue(vns.Type);

      _currentCode.Append(vns.Name);
      //类中如果变量没有值,那就设置为可为空
      if (parent.GetType() == typeof(Class) && vns.Value == null)
      {
        _currentCode.Append('?');
      }
      if (vns.Value != null)
      {
        _currentCode.AppendFormat(": {0} = {1}",  typeName, vns.Value);
      }
      else if (setDefaultValue && !string.IsNullOrEmpty(defaultValue))
      {
        _currentCode.AppendFormat(": {0} = {1}", typeName,
          defaultValue);
        // _currentCode.Append(vns.Name).Append(" = ").Append();
      }
      else
      {
        _currentCode.Append(": ").Append(typeName);
      }
    }

    _currentCode.AppendLine(parent is EnumDefine ? "," : ";");
    _currentLayerDepth--;
  }

  private void ProcessVariableWithStructure(VariableWithStructure? vws,
    bool ignorePermission,
    bool ignoreStatic,
    CodeNode parent
  )
  {
    if (parent is Class && vws.Getter == null && vws.Setter == null)
    {
      ProcessVariableNoStructure(vws,ignorePermission,ignoreStatic,parent,false);
      return;
    }
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


    //不用大括号的方式
    // _currentCode.Append(vws.Name).Append(": ").Append(typeName).AppendLine(";");
    _currentLayerDepth--;
  }

  private void ProcessFunction(Function function)
  {
    _currentLayerDepth++;
    // var classCode = new StringBuilder();
    var tab = GetTab(_currentLayerDepth);

    // processChildren(function);

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
