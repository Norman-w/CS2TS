using System.Reflection;

namespace CS2TS.Model;

public static class Segments
{
	//向前合并
	public static Segment MergeBackwards(
		Segment segment,
		List<Segment> previousSegments,
		out uint mergeSegmentCount,
		out uint mergedTotalSegmentCharCount)
	{
		if (previousSegments.Count == 0)
		{
			mergeSegmentCount = 0;
			mergedTotalSegmentCharCount = 0;
			return segment;
		}

		#region 初始变量定义

		//如果当前是一个 / 前面是 // 那么第一次可以粘连成一个注释,后面的 / 不能粘连
		//或者像 *= 这样的符号,也可以粘连, \r和\n也可以粘连, ++ 还有 ??=这种三个在一起的符号也可以粘连

		var alreadyMergeSegmentCount = 0u;
		var alreadyMergedTotalSegmentCharCount = 0u;

		var mergeSucceed = true;
		var index = previousSegments.Count - 1;
		var currentAfterMergeSegment = segment;

		#endregion

		//已经预定义了的静态segment集合
		var staticSegments = All;

		#region 测试代码

		//如果当前一个是?,前一个也是?
		if (segment.Content == "?" && index >= 0 && previousSegments[index].Content == "?")
		{
			Console.WriteLine("Segments:?还不会处理@!!!");
			//TODO

			// throw new Exception("Segments:?还不会处理@!!!");
			Console.WriteLine("Segments:?还不会处理@!!!");
		}

		//如果当前是=,前一个是?,再前一个也是?
		if (segment.Content == "=" && index >= 1 && previousSegments[index].Content == "?"
		    && previousSegments[index - 1].Content == "?")
		{
			Console.WriteLine("Segments:??=还不会处理@!!!");
			//TODO

			// throw new Exception("Segments:??=还不会处理@!!!");
			Console.WriteLine("Segments:??=还不会处理@!!!");
		}

		#endregion

		while (mergeSucceed && index >= 0)
		{
			var currentWaitMergeSegment = previousSegments[index];
			//如果是不可见的segment,那么算做可以吃掉的segment,要增加mergeSegmentCount和mergedTotalSegmentCharCount
			if (currentWaitMergeSegment.IsWhitespace || currentWaitMergeSegment.IsLineBreak)
			{
				alreadyMergeSegmentCount++;
				alreadyMergedTotalSegmentCharCount += (uint)currentWaitMergeSegment.Length;
				index--;
				continue;
			}

			index--;
			var mergedContent = currentWaitMergeSegment.Content + currentAfterMergeSegment.Content;
			//匹配mergedContent是否是staticSegments中的一个
			var matchedStaticSegment =
				staticSegments.FirstOrDefault(staticSegment => staticSegment.Content == mergedContent);
			// staticSegments.FirstOrDefault(staticSegment =>
			// staticSegment.Content.EndsWith(mergedContent));
			if (matchedStaticSegment != null)
			{
				mergeSucceed = true;
				alreadyMergeSegmentCount++;
				alreadyMergedTotalSegmentCharCount +=
					(uint)matchedStaticSegment.Length - (uint)currentAfterMergeSegment.Content.Length;
				currentAfterMergeSegment = matchedStaticSegment;
				continue;
			}

			break;
		}

		mergeSegmentCount = alreadyMergeSegmentCount;
		mergedTotalSegmentCharCount = alreadyMergedTotalSegmentCharCount;
		return currentAfterMergeSegment;
	}

	#region 一个字符的

	/// <summary>
	///     换行符,用于终止注释行.空格不算,因为空格只是断语义符.换行比较特殊,是本项目中才会用到的.
	///     正常的CS代码中的换行符是不具备什么用途的,都可以完全被替换掉不会影响逻辑.
	/// </summary>
	public static readonly Segment LineBreakSymbol = new() { Content = "\n" };

	/// <summary> 逗号,连接参数符号 </summary>
	public static Segment JoinParameterSymbol = new() { Content = "," };

	/// <summary> 分号,用于终止语句 </summary>
	public static Segment SemicolonSymbol = new() { Content = ";" };

	/// <summary> 大括号开始符号 </summary>
	public static Segment BracesStartSymbol = new() { Content = "{" };

	/// <summary> 大括号结束符号 </summary>
	public static Segment BracesEndSymbol = new() { Content = "}" };

	/// <summary> 方括号开始符号 </summary>
	public static Segment SquareBracketsStartSymbol = new() { Content = "[" };

	/// <summary> 方括号结束符号 </summary>
	public static Segment SquareBracketsEndSymbol = new() { Content = "]" };

	/*尖括号开始和结束是跟大于等于号一样的,省略*/

	/// <summary> 小括号开始符号 </summary>
	public static Segment ParenthesesStartSymbol = new() { Content = "(" };

	/// <summary> 小括号结束符号 </summary>
	public static Segment ParenthesesEndSymbol = new() { Content = ")" };

	/// <summary> 点符号 </summary>
	public static Segment DotSymbol = new() { Content = "." };

	/// <summary> #井号,用于region, define之类 </summary>
	public static Segment SharpSymbol = new() { Content = "#" };

	/// <summary> 冒号 </summary>
	public static Segment ColonSymbol = new() { Content = ":" };

	/// <summary> 问号,用于三目运算符,可组成??甚至??= </summary>
	public static Segment QuestionMarkSymbol = new() { Content = "?" };

	/// <summary> 除法符号 </summary>
	public static Segment DivisionSymbol = new() { Content = "/" };

	/// <summary> 乘法符号 </summary>
	public static Segment MultiplicationSymbol = new() { Content = "*" };

	/// <summary> 加法符号 </summary>
	public static Segment AdditionSymbol = new() { Content = "+" };

	/// <summary> 减法符号 </summary>
	public static Segment SubtractionSymbol = new() { Content = "-" };

	/// <summary> 取余符号 </summary>
	public static Segment ModuloSymbol = new() { Content = "%" };

	/// <summary> 等于符号 </summary>
	public static Segment EqualSymbol = new() { Content = "=" };

	/// <summary> 大于符号 </summary>
	public static Segment GreaterThanSymbol = new() { Content = ">" };

	/// <summary> 小于符号 </summary>
	public static Segment LessThanSymbol = new() { Content = "<" };

	/// <summary> 逻辑与符号 </summary>
	public static Segment AndSymbol = new() { Content = "&" };

	/// <summary> 逻辑或符号 </summary>
	public static Segment OrSymbol = new() { Content = "|" };

	/// <summary> 逻辑非符号 </summary>
	public static Segment NotSymbol = new() { Content = "!" };

	/// <summary> 逻辑异或符号 </summary>
	public static Segment XorSymbol = new() { Content = "^" };

	#endregion

	#region 两个字符的

	#region 中间不能有空格的

	/// <summary> 行注释符号 </summary>
	public static Segment AnnotationLineSymbol = new() { Content = "//" };

	/// <summary> 多行注释开始符号 </summary>
	public static Segment AnnotationAreaStartSymbol = new() { Content = "/*" };

	/// <summary> 多行注释结束符号 </summary>
	public static Segment AnnotationAreaEndSymbol = new() { Content = "*/" };

	/// <summary> 除等于符号 </summary>
	public static Segment DivisionEqualSymbol = new() { Content = "/=" };

	/// <summary> 乘等于符号 </summary>
	public static Segment MultiplicationEqualSymbol = new() { Content = "*=" };

	/// <summary> 加等于符号 </summary>
	public static Segment AdditionEqualSymbol = new() { Content = "+=" };

	/// <summary> 减等于符号 </summary>
	public static Segment SubtractionEqualSymbol = new() { Content = "-=" };

	/// <summary> 等于等于符号 </summary>
	public static Segment EqualEqualSymbol = new() { Content = "==" };

	/// <summary> 大于等于符号 </summary>
	public static Segment GreaterThanEqualSymbol = new() { Content = ">=" };

	/// <summary> 小于等于符号 </summary>
	public static Segment LessThanEqualSymbol = new() { Content = "<=" };

	/// <summary> 不等于符号 </summary>
	public static Segment NotEqualSymbol = new() { Content = "!=" };

	/// <summary> 逻辑与等于符号 </summary>
	public static Segment AndEqualSymbol = new() { Content = "&=" };

	/// <summary> 逻辑或等于符号 </summary>
	public static Segment OrEqualSymbol = new() { Content = "|=" };

	/// <summary> 逻辑异或等于符号 </summary>
	public static Segment XorEqualSymbol = new() { Content = "^=" };

	/// <summary> 左移符号 </summary>
	public static Segment LeftShiftSymbol = new() { Content = "<<" };

	/// <summary> 右移符号 </summary>
	public static Segment RightShiftSymbol = new() { Content = ">>" };

	/// <summary> 空合并符号 </summary>
	public static Segment NullCoalescingSymbol = new() { Content = "??" };

	/// <summary> 条件逻辑与符号 </summary>
	public static Segment ConditionalAndSymbol = new() { Content = "&&" };

	/// <summary> 条件逻辑或符号 </summary>
	public static Segment ConditionalOrSymbol = new() { Content = "||" };

	/// <summary> =>,lambda表达式 </summary>
	public static Segment LambdaSymbol = new() { Content = "=>" };

	#endregion

	#region 闭合的空参数各种括号.这种括号里面可以容纳多个不可见字符,不像 += 中间不能有空格

	/// <summary> 闭合空参数的括号 </summary>
	public static Segment CloseParenthesesSymbol = new() { Content = "()" };

	/// <summary> 闭合空参数的尖括号 </summary>
	public static Segment CloseAngleBracketsSymbol = new() { Content = "<>" };

	/// <summary> 闭合空参数的方括号 </summary>
	public static Segment CloseSquareBracketsSymbol = new() { Content = "[]" };

	/// <summary> 闭合空参数的大括号 </summary>
	public static Segment CloseBracesSymbol = new() { Content = "{}" };

	#endregion

	#endregion

	#region 三个字符的

	/// <summary> 左移等于符号 </summary>
	public static Segment LeftShiftEqualSymbol = new() { Content = "<<=" };

	/// <summary> 右移等于符号 </summary>
	public static Segment RightShiftEqualSymbol = new() { Content = ">>=" };

	/// <summary> 空合并等于符号 </summary>
	public static Segment NullCoalescingEqualSymbol = new() { Content = "??=" };

	/// <summary>
	///     一个测试符号,看看走到=号的时候,如果前面是3个!能不能被识别出来
	///     假定他的名字叫做 3确认等于
	/// </summary>
	public static Segment TrustTrustTrustEqual = new() { Content = "!!!=" };

	/// <summary>
	///     一个测试符号,看看走到=号的时候,如果前面是2个!能不能被识别出来
	///     我们之前已经能识别!和=也能识别!=,也将要识别有意义的!!!=了.
	///     假定这不具备意义,他的用途是用来给 !!!=搭桥以便能识别3个!的
	///     我们可以给Segment 加个字段或者是弄一个新类型叫"无意义的Segment",然后在解析的时候,如果遇到这个无意义的Segment,就跳过他,不会把他放到语法树里面去
	/// </summary>
	public static Segment TrustTrustEqual = new() { Content = "!!=" };

	#endregion

	#region 对外提供的对象 StaticSegments

	private static List<Segment>? _all;

	/// <summary>
	///     所有的静态字段,也就是所有的Segment
	/// </summary>
	/// <exception cref="Exception"></exception>
	public static List<Segment> All
	{
		get
		{
			//如果已经初始化过了,那么直接返回
			if (_all != null) return _all;
			//获取所有的静态字段
			var staticFields = typeof(Segments).GetFields(BindingFlags.Static | BindingFlags.Public);
			//获取所有的静态字段的值,并且过滤掉不是Segment的
			var staticSegments = staticFields.Select(field => field.GetValue(null)).Where(v => v is Segment)
				.Cast<Segment>().ToList();
			//安全检查,找重复的
			var duplicateItems = staticSegments.GroupBy(s => s.Content).Where(g => g.Count() > 1)
				.Select(g => g.Key)
				.ToList();
			//如果有重复的,那么抛出异常
			if (duplicateItems.Count > 0)
				throw new Exception($"Segments:重复的项:{string.Join(",", duplicateItems)}");
			//赋值给静态字段
			_all = staticSegments;
			return _all;
		}
	}

	#endregion
}