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

		//如果当前是一个 / 前面是 // 那么第一次可以粘连成一个注释,后面的 / 不能粘连
		//或者像 *= 这样的符号,也可以粘连, \r和\n也可以粘连, ++ 还有 ??=这种三个在一起的符号也可以粘连

		var _mergeSegmentCount = 0u;
		var _mergedTotalSegmentCharCount = 0u;

		var mergeSucceed = true;
		var index = previousSegments.Count - 1;
		var currentAfterMergeSegment = segment;
		//查找segments里面所有的static的字段
		var staticFields = typeof(Segments).GetFields(BindingFlags.Static | BindingFlags.Public);
		var staticSegments = staticFields.Select(field => field.GetValue(null)).Cast<Segment>().ToList();
		while (mergeSucceed && index >= 0)
		{
			var currentWaitMergeSegment = previousSegments[index];
			index--;
			var mergedContent = currentWaitMergeSegment.Content + currentAfterMergeSegment.Content;
			//匹配mergedContent是否是staticSegments中的一个
			var matchedStaticSegment =
				staticSegments.FirstOrDefault(staticSegment => staticSegment.Content == mergedContent);
			if (matchedStaticSegment != null)
			{
				mergeSucceed = true;
				_mergeSegmentCount++;
				_mergedTotalSegmentCharCount +=
					(uint)matchedStaticSegment.Length - (uint)currentAfterMergeSegment.Content.Length;
				currentAfterMergeSegment = matchedStaticSegment;
				continue;
			}

			break;
		}

		mergeSegmentCount = _mergeSegmentCount;
		mergedTotalSegmentCharCount = _mergedTotalSegmentCharCount;
		return currentAfterMergeSegment;
	}

	#region 一个字符的

	/// <summary> 除法符号 </summary>
	public static Segment DivisionSymbol = new() { Content = "/" };

	/// <summary> 乘法符号 </summary>
	public static Segment MultiplicationSymbol = new() { Content = "*" };

	/// <summary> 加法符号 </summary>
	public static Segment AdditionSymbol = new() { Content = "+" };

	/// <summary> 减法符号 </summary>
	public static Segment SubtractionSymbol = new() { Content = "-" };

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

	#endregion
}