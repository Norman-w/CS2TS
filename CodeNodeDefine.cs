using System.Text;

public static class Define
{
    /// <summary>
    /// 权限的集合,确定有没有权限定义找这里
    /// </summary>
    public static List<string> Permissions = new List<string>() { "public", "private", "protected", "internal"};
}
public class CodeFile
{
    /// <summary>
    /// 所有的标记信息
    /// </summary>
    public List<object> Notes { get; set; }
    public void AddNotes(List<object> notes)
    {
        if (notes.Count == 0)
        {
            return;
        }
        if (this.Notes == null)
        {
            this.Notes = new List<object>();
        }
        this.Notes.AddRange(notes);
    }
    public List<string> Usings { get; set; }
    public List<NameSpace> Namespaces { get; set; }
    public List<Class> Classes { get; set; }
    public List<EnumDefine> Enums { get; set; }
    public List<Function> Functions { get; set; }
}
public enum CurrentWorkingInEnum
{
    Out,
    Namespace,
    Class,
    Function,
    Notes,
    Summary,
}
public class NameSpace
{
    /// <summary>
    /// 所有的标记信息
    /// </summary>
    public List<object> Notes { get; set; }
    public void AddNotes(List<object> notes)
    {
        if (notes.Count == 0)
        {
            return;
        }
        if (this.Notes == null)
        {
            this.Notes = new List<object>();
        }
        this.Notes.AddRange(notes);
    }

    public string Name { get; set; }
    public List<Class> Classes { get; set; }
    public List<EnumDefine> Enums { get; set; }
    public List<Interface> Interfaces { get; set; }
}
public enum PermissionEnum { Public, Protected, Private, Internal }
public class Class : Interface
{
    public bool IsStatic { get; set; }
    public List<Variable> Variables { get; set; }
    public List<Class> Classes { get; set; }
    //private List<object> notes = null;
}
public class Interface : VariableWithStructure
{
    public List<Function> Functions {get;set;}
}
public class Function
{
    /// <summary>
    /// 所有的标记信息
    /// </summary>
    public List<object> Notes { get; set; }
    public void AddNotes(List<object> notes)
    {
        if (notes.Count == 0)
        {
            return;
        }
        if (this.Notes == null)
        {
            this.Notes = new List<object>();
        }
        this.Notes.AddRange(notes);
    }
    public bool IsStatic { get; set; }
    public Nullable<PermissionEnum> Permission { get; set; }
    public string Name { get; set; }
    public List<Parameter> InParameters { get; set; }
    public Parameter ReturnParameter { get; set; }

    public bool IsOverride { get; set; }

    //public bool @int { get; set; }
    /// <summary>
    /// 语句段集合
    /// </summary>
    public List<Statement> Statements { get; set; }
}

public class TypeDefine
{
  /// <summary>
  /// 携带泛型等的完整类型名称
  /// </summary>
  public string FullName { get; set; }
  /// <summary>
  /// 该类型的名字
  /// </summary>
  public string Name { get; set; }
  /// <summary>
  /// 该类型是否为泛型类型
  /// </summary>
  public bool IsGeneric { get; set; }
  /// <summary>
  /// 作为泛型时的泛型内参数的个数,比如 List<T> 就是只有一个T Dictioary<string,object> 就是两个.
  /// </summary>
  public List<TypeDefine> GenericParamTypeList { get; set; }
}
public class Parameter
{
    // public int Index { get; set; }
    public string Name { get; set; }
    public TypeDefine Type { get; set; }
    public bool IsRef { get; set; }
    public bool IsOut { get; set; }
}
public class Statement
{
    /// <summary>
    /// 所有的标记信息
    /// </summary>
    public List<object> Notes { get; set; }
    public void AddNotes(List<object> notes)
    {
        if (notes.Count == 0)
        {
            return;
        }
        if (this.Notes == null)
        {
            this.Notes = new List<object>();
        }

        this.Notes.AddRange(notes);
    }

    public string CodeBody { get; set; }

    /// <summary>
    /// 语句段集合。if里面还可以嵌套if
    /// </summary>
    public List<Statement> Statements { get; set; }

    /// <summary>
    /// 什么类型的 比如 if  else if else while switch
    /// </summary>
    public string Type { get; set; }
}
/// <summary>
/// 语句段,带结构的语句段，像 if else else if 之类的都算
/// </summary>
public class StatementWithStructure : Statement
{

}
/// <summary>
/// if 语句段
/// </summary>
public class IfStatement :StatementWithStructure
{

}
/// <summary>
/// else if 语句段
/// </summary>
public class ElseIfStatement : IfStatement
{

}
/// <summary>
/// else 语句段
/// </summary>
public class ElseStatement: StatementWithStructure
{

}
public class NotesLine
{
    public NotesLine()
    {
        //this.Content = new StringBuilder();
    }
    public string Content
    {
        get
        {
            return sb.ToString();
        }
        set { }
    }
    StringBuilder sb = new StringBuilder();
    public void Append(string text)
    {
        sb.Append(text);
    }
    public void AppendFormat(string text, params object[] args)
    {
        sb.AppendFormat(text, args);
    }
}
public class NotesArea
{
    public List<string> Lines = new List<string>();
}
public class SharpLine : NotesLine
{
    //public SharpLine()
    //{
    //    this.Content = new StringBuilder();
    //}
    //public StringBuilder Content { get; set; }
}
/// <summary>
/// 变量,拥有名称,类型,作用域,值
/// </summary>
public class Variable
{
    /// <summary>
    /// 所有的标记信息
    /// </summary>
    public List<object> Notes { get; set; }
    public void AddNotes(List<object> notes)
    {
        if (notes.Count == 0)
        {
            return;
        }
        if (this.Notes == null)
        {
            this.Notes = new List<object>();
        }
        this.Notes.AddRange(notes);
    }
    public string Name { get; set; }
    public TypeDefine Type { get; set; }

    /// <summary>
    /// 代码内容
    /// </summary>
    public string CodeBody{ get; set; }

    /// <summary>
    /// 参数的作用域 public private protected internal
    /// </summary>
    public Nullable<PermissionEnum> Permission { get; set; }

    public Nullable<bool> IsStatic { get; set; }

    public Nullable<bool> IsReadonly { get; set; }

    public Nullable<bool> IsConst { get; set; }

    public Nullable<bool> IsOverride { get; set; }
}

/// <summary>
/// 不带大括号定义的变量
/// </summary>
public class VariableNoStructure : Variable
{
    public List<string> Extends { get; set; }
}
/// <summary>
/// 带大括号定义的变量
/// </summary>
public class VariableWithStructure : VariableNoStructure
{
    /// <summary>
    /// 作为字段的时候的时候可以使用的获取器和设置其
    /// </summary>
    public Function Getter { get; set; }
    public Function Setter { get; set; }
}
/// <summary>
/// 枚举类型的定义.
/// </summary>
public class EnumDefine : VariableWithStructure
{
    //public string Name { get; set; }
    //public bool IsStatic { get; set; }
    //public PermissionEnum Permission { get; set; }
    public List<Variable> Variables { get; set; }
    //public List<string> Extends { get; set; }
}
