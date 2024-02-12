/*


 修饰符类型的Segment,是用word来记录的
 每一种修饰符都是一个单独的word,每一种能修饰的目标都不同.
 TODO 本类中已经验证了一部分 比如abstract,其他的尚未验证



*/


namespace CS2TS.Model.Words;

public class ModifierWordSegments
{
	#region 类似于 static, readonly, virtual, override, sealed, abstract, async, new 这样的关键字,都叫做"修饰符"

	//每个修饰符都有自己的特点,所以要单独列出来

	public static readonly ModifierWordSegment Static = new()
	{
		Content = "static",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Class),
			typeof(Struct),
			typeof(Enum),
			typeof(Interface),
			typeof(Delegate),
			typeof(Record),
			typeof(Event)
		}
	};

	public static readonly ModifierWordSegment Readonly = new()
	{
		Content = "readonly",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Field)
		}
	};

	public static readonly ModifierWordSegment Virtual = new()
	{
		Content = "virtual",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Method)
		}
	};

	public static readonly ModifierWordSegment Override = new()
	{
		Content = "override",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Method)
		}
	};

	public static readonly ModifierWordSegment Sealed = new()
	{
		Content = "sealed",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Class),
			typeof(Struct),
			typeof(Enum),
			typeof(Interface),
			typeof(Delegate),
			typeof(Record),
			typeof(Event)
		}
	};

	public static readonly ModifierWordSegment Abstract = new()
	{
		Content = "abstract",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Class),
			typeof(Record),
			//比如在record中可以有public abstract event b c;
			typeof(Event),
			//也可以用于方法,比如public abstract void c();
			typeof(Method)
		}
	};

	public static readonly ModifierWordSegment Async = new()
	{
		Content = "async",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Method)
		}
	};

	public static readonly ModifierWordSegment New = new()
	{
		Content = "new",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Method),
			typeof(Property),
			typeof(Event),
			typeof(Field)
		}
	};

	#endregion
}
//
// public abstract record a
// {
// 	public delegate void b();
//
// 	public abstract void c();
// }