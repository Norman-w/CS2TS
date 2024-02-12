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
			typeof(Class)
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
			//应用于某个类时，sealed修饰符可阻止其他类继承自该类。
			typeof(Class),
			//属性可以为sealed,但是需要是override的
			// 应用于方法或属性时，sealed修饰符必须始终与 override 结合使用。
			typeof(Property),
			typeof(Method)
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