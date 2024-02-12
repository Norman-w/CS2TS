/*


 修饰符类型的Segment,是用word来记录的
 每一种修饰符都是一个单独的word,每一种能修饰的目标都不同.

*/


namespace CS2TS.Model.Words;

public class ModifierSegments
{
	#region 对外提供的对象 StaticSegments

	private static List<ModifierSegment>? _all;

	/// <summary>
	///     所有的静态字段,也就是所有的Segment
	/// </summary>
	/// <exception cref="Exception"></exception>
	public static List<ModifierSegment> All
	{
		get
		{
			//如果已经初始化过了,那么直接返回
			if (_all != null) return _all;
			_all = Segments.GetAllStaticSegments<ModifierSegment, ModifierSegments>();
			return _all;
		}
	}

	#endregion

	#region 类似于 static, readonly, virtual, override, sealed, abstract, async, new 这样的关键字,都叫做"修饰符"

	//每个修饰符都有自己的特点,所以要单独列出来

	public static readonly ModifierSegment Static = new()
	{
		Content = "static",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Class)
		}
	};

	public static readonly ModifierSegment Readonly = new()
	{
		Content = "readonly",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Field)
		}
	};

	public static readonly ModifierSegment Virtual = new()
	{
		Content = "virtual",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Method)
		}
	};

	public static readonly ModifierSegment Override = new()
	{
		Content = "override",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Method)
		}
	};

	public static readonly ModifierSegment Sealed = new()
	{
		Content = "sealed",
		UseForCodeNodeTypes = new List<Type>
		{
			//应用于某个类时，sealed修饰符可阻止其他类继承自该类。
			typeof(Class),
			//属性可以为sealed,但是需要是override的
			// 应用于方法或属性时，sealed修饰符必须始终与 override 结合使用。
			typeof(Property),
			typeof(Method)
		}
	};

	public static readonly ModifierSegment Abstract = new()
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

	public static readonly ModifierSegment Async = new()
	{
		Content = "async",
		UseForCodeNodeTypes = new List<Type>
		{
			typeof(Method)
		}
	};

	public static readonly ModifierSegment New = new()
	{
		/*

		 new修饰符用于方法,属性,事件,字段的示意:


		   public class A
		   {
		   public string C { get; set; }

		   public void B()
		   {
		   }

		   public string E;

		   public event Action F;
		   }

		   public class D : A
		   {
		   public new string C { get; set; }

		   public new void B()
		   {
		   }

		   public new string E;

		   public new event Action F;
		   }

        */
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