using CS2TS.Model;
using CS2TS.Model.Node;

namespace CS2TS;

/// <summary>
///     变量,拥有名称,类型,作用域,值
/// </summary>
public abstract class Variable : CodeNode
{
	public Variable(string name,
		TypeDefine type,
		PermissionEnum? permission,
		bool? isStatic,
		bool? isReadonly,
		bool? isConst, bool?
			isOverride,
		object value)
	{
		Name = name;
		Type = type;
		Permission = permission;
		IsStatic = isStatic;
		IsReadonly = isReadonly;
		IsConst = isConst;
		IsOverride = isOverride;
		Value = value;
	}

	public string Name { get; set; }
	public TypeDefine Type { get; set; }

  /// <summary>
  ///     参数的作用域 public private protected internal
  /// </summary>
  public PermissionEnum? Permission { get; set; }

	public bool? IsStatic { get; set; }

	public bool? IsReadonly { get; set; }

	public bool? IsConst { get; set; }

	public bool? IsOverride { get; set; }

  /// <summary>
  ///     参数的值是什么(字符串形式)
  /// </summary>
  public object Value { get; set; }

	// /// <summary>
	// /// 默认值是什么.
	// /// </summary>
	// public string DefualtValue { get; set; }
}

/// <summary>
///     不带大括号定义的变量
/// </summary>
public class VariableNoStructure : Variable
{
	public VariableNoStructure(string name,
		TypeDefine type,
		PermissionEnum? permission,
		bool? isStatic,
		bool? isReadonly,
		bool? isConst,
		bool? isOverride,
		object value, List<string> extends) : base(name, type, permission, isStatic, isReadonly, isConst, isOverride,
		value)
	{
		Extends = extends;
	}

	/// <summary>
	///     代码节点类型的Segment表示
	/// </summary>
	public override Segment? CodeNodeTypeSegment => null;

  /// <summary>
  ///     继承项的列表.比如 定义枚举时可继承自 uint,c#写做: enum xxx : uint
  /// </summary>
  public List<string> Extends { get; set; }
}

/// <summary>
///     带大括号定义的变量
/// </summary>
public class VariableWithStructure : VariableNoStructure
{
	public VariableWithStructure(string name,
		TypeDefine type,
		PermissionEnum? permission,
		bool? isStatic,
		bool? isReadonly,
		bool? isConst, bool?
			isOverride,
		object value,
		List<string> extends,
		Function getter,
		Function setter) : base(name, type, permission, isStatic, isReadonly, isConst, isOverride, value, extends)
	{
		Getter = getter;
		Setter = setter;
	}

  /// <summary>
  ///     作为字段的时候的时候可以使用的获取器和设置其
  /// </summary>
  public Function Getter { get; set; }

	public Function Setter { get; set; }
}