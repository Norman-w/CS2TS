namespace CS2TS.Model;

public class SymbolSegments
{
	#region 一个字符的

	/// <summary>
	///     换行符,用于终止注释行.空格不算,因为空格只是断语义符.换行比较特殊,是本项目中才会用到的.
	///     正常的CS代码中的换行符是不具备什么用途的,都可以完全被替换掉不会影响逻辑.
	/// </summary>
	public static readonly SymbolSegment LineBreakSymbol = new()
	{
		Content = "\n",
		CanFinishCodeNodeBodyTypes = new List<Type> { typeof(NotesLine) }
	};

	/// <summary> 逗号,连接参数符号 </summary>
	public static SymbolSegment JoinParameterSymbol = new() { Content = "," };

	/// <summary> 分号,用于终止语句 </summary>
	public static readonly SymbolSegment SemicolonSymbol = new()
	{
		Content = ";",
		CanFinishCodeNodeHeaderTypes = new List<Type>
		{
			//在结束命名空间时,只能结束命名空间的头,闭合整个带body的命名空间的话需要用大括回
			//但是如果就这样结束了的话,那文件里面 namespace A;下面又写了一个public class B{}; 这个class B的命名空间就不正确了.
			//TODO, 要确认一下这种写法的namespace是不是要特殊处理,因为这种命名空间属于文件级别,所有下面的class,struct,enum,interface,delegate等都是他的子级
			typeof(Namespace),
			//这个要确认一下,是完成了字段还是完成了字段赋值...
			typeof(Field)
		},
		CanStartCodeNodeBodyTypes = new List<Type>
		{
			//可以在命名空间的头,进入命名空间的体
			typeof(Namespace)
		}
	};

	/// <summary>
	///     文件尾,用于终止文件,能终止不由{}开启的命名空间
	/// </summary>
	public static readonly SymbolSegment EofSymbol = new()
	{
		Content = "\0",
		CanFinishCodeNodeBodyTypes = new List<Type>
		{
			//如果一个文件中只有一个分号结尾的命名空间,结束命名空间的方法就是文件尾
			typeof(Namespace)
		}
	};

	/// <summary> 大括号开始符号 </summary>
	public static readonly SymbolSegment BracesStartSymbol = new()
	{
		Content = "{",
		CanStartCodeNodeBodyTypes = new List<Type>
		{
			//大括回开始那种老版本的命名空间
			//TODO 尚未验证所有的类型,应该是 } 的作用是一一对应的
			typeof(Namespace),
			typeof(Class),
			typeof(Struct),
			typeof(EnumDefine),
			typeof(Interface),
			typeof(Delegate),
			typeof(Method),
			typeof(Property),
			typeof(Record)
		}
	};

	/// <summary> 大括号结束符号 </summary>
	public static readonly SymbolSegment BracesEndSymbol = new()
	{
		Content = "}",
		CanFinishCodeNodeBodyTypes = new List<Type>(BracesStartSymbol.CanStartCodeNodeBodyTypes)
	};

	/// <summary> 方括号开始符号 </summary>
	public static SymbolSegment SquareBracketsStartSymbol = new() { Content = "[" };

	/// <summary> 方括号结束符号 </summary>
	public static SymbolSegment SquareBracketsEndSymbol = new() { Content = "]" };

	/*尖括号开始和结束是跟大于等于号一样的,省略*/

	/// <summary> 小括号开始符号 </summary>
	public static SymbolSegment ParenthesesStartSymbol = new() { Content = "(" };

	/// <summary> 小括号结束符号 </summary>
	public static SymbolSegment ParenthesesEndSymbol = new() { Content = ")" };

	/// <summary> 点符号 </summary>
	public static SymbolSegment DotSymbol = new() { Content = "." };

	/// <summary> #井号,用于region, define之类 </summary>
	public static SymbolSegment SharpSymbol = new() { Content = "#" };

	/// <summary> 冒号 </summary>
	public static SymbolSegment ColonSymbol = new() { Content = ":" };

	/// <summary> 问号,用于三目运算符,可组成??甚至??= </summary>
	public static SymbolSegment QuestionMarkSymbol = new() { Content = "?" };

	/// <summary> 除法符号 </summary>
	public static SymbolSegment DivisionSymbol = new() { Content = "/" };

	/// <summary> 乘法符号 </summary>
	public static SymbolSegment MultiplicationSymbol = new() { Content = "*" };

	/// <summary> 加法符号 </summary>
	public static SymbolSegment AdditionSymbol = new() { Content = "+" };

	/// <summary> 减法符号 </summary>
	public static SymbolSegment SubtractionSymbol = new() { Content = "-" };

	/// <summary> 取余符号 </summary>
	public static SymbolSegment ModuloSymbol = new() { Content = "%" };

	/// <summary> 等于符号 </summary>
	public static SymbolSegment EqualSymbol = new()
	{
		Content = "=",
		CanFinishCodeNodeHeaderTypes = new List<Type>
		{
			//public int Count = 1;这里面的=就结束了头部,同时能进入到Body 也就是 =后面的部分,而这个body比较简单只有一个1
			typeof(Field)
		},
		CanStartCodeNodeBodyTypes = new List<Type>
		{
			typeof(Field)
		}
	};

	/// <summary> 大于符号 </summary>
	public static SymbolSegment GreaterThanSymbol = new() { Content = ">" };

	/// <summary> 小于符号 </summary>
	public static SymbolSegment LessThanSymbol = new() { Content = "<" };

	/// <summary> 逻辑与符号 </summary>
	public static SymbolSegment AndSymbol = new() { Content = "&" };

	/// <summary> 逻辑或符号 </summary>
	public static SymbolSegment OrSymbol = new() { Content = "|" };

	/// <summary> 逻辑非符号 </summary>
	public static SymbolSegment NotSymbol = new() { Content = "!" };

	/// <summary> 逻辑异或符号 </summary>
	public static SymbolSegment XorSymbol = new() { Content = "^" };

	#endregion

	#region 两个字符的

	#region 中间不能有空格的

	/// <summary> 行注释符号 </summary>
	public static SymbolSegment AnnotationLineSymbol = new() { Content = "//" };

	/// <summary> 多行注释开始符号 </summary>
	public static SymbolSegment AnnotationAreaStartSymbol = new() { Content = "/*" };

	/// <summary> 多行注释结束符号 </summary>
	public static SymbolSegment AnnotationAreaEndSymbol = new() { Content = "*/" };

	/// <summary> 除等于符号 </summary>
	public static SymbolSegment DivisionEqualSymbol = new() { Content = "/=" };

	/// <summary> 乘等于符号 </summary>
	public static SymbolSegment MultiplicationEqualSymbol = new() { Content = "*=" };

	/// <summary> 加等于符号 </summary>
	public static SymbolSegment AdditionEqualSymbol = new() { Content = "+=" };

	/// <summary> 减等于符号 </summary>
	public static SymbolSegment SubtractionEqualSymbol = new() { Content = "-=" };

	/// <summary> 等于等于符号 </summary>
	public static SymbolSegment EqualEqualSymbol = new() { Content = "==" };

	/// <summary> 大于等于符号 </summary>
	public static SymbolSegment GreaterThanEqualSymbol = new() { Content = ">=" };

	/// <summary> 小于等于符号 </summary>
	public static SymbolSegment LessThanEqualSymbol = new() { Content = "<=" };

	/// <summary> 不等于符号 </summary>
	public static SymbolSegment NotEqualSymbol = new() { Content = "!=" };

	/// <summary> 逻辑与等于符号 </summary>
	public static SymbolSegment AndEqualSymbol = new() { Content = "&=" };

	/// <summary> 逻辑或等于符号 </summary>
	public static SymbolSegment OrEqualSymbol = new() { Content = "|=" };

	/// <summary> 逻辑异或等于符号 </summary>
	public static SymbolSegment XorEqualSymbol = new() { Content = "^=" };

	/// <summary> 左移符号 </summary>
	public static SymbolSegment LeftShiftSymbol = new() { Content = "<<" };

	/// <summary> 右移符号 </summary>
	public static SymbolSegment RightShiftSymbol = new() { Content = ">>" };

	/// <summary> 空合并符号 </summary>
	public static SymbolSegment NullCoalescingSymbol = new() { Content = "??" };

	/// <summary> 条件逻辑与符号 </summary>
	public static SymbolSegment ConditionalAndSymbol = new() { Content = "&&" };

	/// <summary> 条件逻辑或符号 </summary>
	public static SymbolSegment ConditionalOrSymbol = new() { Content = "||" };

	/// <summary> =>,lambda表达式 </summary>
	public static SymbolSegment LambdaSymbol = new() { Content = "=>" };

	#endregion

	#region 闭合的空参数各种括号.这种括号里面可以容纳多个不可见字符,不像 += 中间不能有空格

	/// <summary> 闭合空参数的括号 </summary>
	public static SymbolSegment CloseParenthesesSymbol = new() { Content = "()", CanInsertContentInMiddle = true };

	/// <summary> 闭合空参数的尖括号 </summary>
	public static SymbolSegment CloseAngleBracketsSymbol = new() { Content = "<>", CanInsertContentInMiddle = true };

	/// <summary> 闭合空参数的方括号 </summary>
	public static SymbolSegment CloseSquareBracketsSymbol = new() { Content = "[]", CanInsertContentInMiddle = true };

	/// <summary> 闭合空参数的大括号 </summary>
	public static SymbolSegment CloseBracesSymbol = new() { Content = "{}", CanInsertContentInMiddle = true };

	#endregion

	#endregion

	#region 三个字符的

	/// <summary> 左移等于符号 </summary>
	public static SymbolSegment LeftShiftEqualSymbol = new() { Content = "<<=" };

	/// <summary> 右移等于符号 </summary>
	public static SymbolSegment RightShiftEqualSymbol = new() { Content = ">>=" };

	/// <summary> 空合并等于符号 </summary>
	public static SymbolSegment NullCoalescingEqualSymbol = new() { Content = "??=" };

	/// <summary>
	///     一个测试符号,看看走到=号的时候,如果前面是3个!能不能被识别出来
	///     假定他的名字叫做 3确认等于
	/// </summary>
	public static SymbolSegment TrustTrustTrustEqual = new() { Content = "!!!=" };

	/// <summary>
	///     一个测试符号,看看走到=号的时候,如果前面是2个!能不能被识别出来
	///     我们之前已经能识别!和=也能识别!=,也将要识别有意义的!!!=了.
	///     假定这不具备意义,他的用途是用来给 !!!=搭桥以便能识别3个!的
	///     我们可以给Segment 加个字段或者是弄一个新类型叫"无意义的Segment",然后在解析的时候,如果遇到这个无意义的Segment,就跳过他,不会把他放到语法树里面去
	/// </summary>
	public static SymbolSegment TrustTrustEqual = new() { Content = "!!=" };

	#endregion

	#region 对外提供的对象 StaticSegments

	private static List<SymbolSegment>? _all;

	/// <summary>
	///     所有的静态字段,也就是所有的Segment
	/// </summary>
	/// <exception cref="Exception"></exception>
	public static List<SymbolSegment> All
	{
		get
		{
			//如果已经初始化过了,那么直接返回
			if (_all != null) return _all;
			_all = Segments.GetAllStaticSegments<SymbolSegment, SymbolSegments>();
			return _all;
		}
	}

	#endregion
}