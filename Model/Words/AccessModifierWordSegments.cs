namespace CS2TS.Model.Words;

public class AccessModifierWordSegments
{
	#region 类似于public, private, protected这样的关键字,这几种都叫做"访问修饰符"

	/// <summary>
	///     用于代码节点类型的访问修饰符,可以用于的代码节点类型有:类,结构体,枚举,接口,委托,记录,事件,属性,字段,方法,构造函数,运算符等
	/// </summary>
	private static readonly List<Type> AccessModifierUseForCodeNodeTypes = new()
	{
		typeof(Class), typeof(Struct), typeof(Enum), typeof(Interface), typeof(Delegate), typeof(Record),
		typeof(Event), typeof(Property), typeof(Field), typeof(Method), typeof(Constructor), typeof(Operator)
	};

	public static readonly AccessModifierWordSegment Public = new()
	{
		Content = "public",
		UseForCodeNodeTypes = AccessModifierUseForCodeNodeTypes
	};

	public static readonly AccessModifierWordSegment Private = new()
		{ Content = "private", UseForCodeNodeTypes = AccessModifierUseForCodeNodeTypes };

	public static readonly AccessModifierWordSegment Protected = new()
		{ Content = "protected", UseForCodeNodeTypes = AccessModifierUseForCodeNodeTypes };

	public static readonly AccessModifierWordSegment Internal = new()
		{ Content = "internal", UseForCodeNodeTypes = AccessModifierUseForCodeNodeTypes };

	#endregion
}