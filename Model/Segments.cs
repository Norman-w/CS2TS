using System.Reflection;

namespace CS2TS.Model;

public static class Segments
{
	/// <summary>
	///     向前合并方法,通过传入当前的Segment以及他之前的Segments,尝试进行向前合并,比如 当前是=,前面是!,那么就可以合并成!=
	///     可向外传出合并的Segment数量,以及合并的总字符数量
	/// </summary>
	/// <param name="segment"></param>
	/// <param name="previousSegments"></param>
	/// <param name="mergeSegmentCount"></param>
	/// <param name="mergedTotalSegmentCharCount"></param>
	/// <returns></returns>
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

		List<Segment> alreadyMergeSegment = new();
		var alreadyMergedTotalSegmentCharCount = 0u;

		var mergeSucceed = true;
		var index = previousSegments.Count - 1;
		var currentAfterMergeSegment = segment;

		#endregion

		//已经预定义了的静态segment集合
		var staticSegments = All;

		while (mergeSucceed && index >= 0)
		{
			var currentWaitMergeSegment = previousSegments[index];
			//如果是不可见的segment,那么算做可以吃掉的segment,要增加mergeSegmentCount和mergedTotalSegmentCharCount
			//新的改动是换行符不会吃掉
			//2024年01月27日09:34:09新改动,为了保证文件的一致性,不能随便吃掉任何空白符.吃掉空白符的做法应该在以后定义
			//那样的方法应该是用来格式化代码用的,发现类似于括号的右半边的时候,然后往前检查是不是能遇到他的左半边,能遇到的话,把中间隔档的地方都吃掉
			// if (currentWaitMergeSegment.IsWhitespace
			//     //不能上来就吃掉前面的空白字符.如果第一个确实是可见字符,然后中间间隔了一些空白字符的话,那么就可以吃掉了
			//     && alreadyMergeSegment.Count > 0
			//     && alreadyMergeSegment[0].IsWhitespace == false)
			// {
			// 	alreadyMergeSegment.Add(currentWaitMergeSegment);
			// 	alreadyMergedTotalSegmentCharCount++;
			// 	index--;
			// 	continue;
			// }

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
				alreadyMergeSegment.Add(currentWaitMergeSegment);
				alreadyMergedTotalSegmentCharCount +=
					(uint)matchedStaticSegment.Length - (uint)currentAfterMergeSegment.Content.Length;
				currentAfterMergeSegment = matchedStaticSegment;
				continue;
			}

			break;
		}

		mergeSegmentCount = (uint)alreadyMergeSegment.Count;
		mergedTotalSegmentCharCount = alreadyMergedTotalSegmentCharCount;
		return currentAfterMergeSegment;
	}

	/// <summary>
	///     尝试将成对出现的符号,也就是像{},()等这样的符号中间的不可见字符去掉,比如\t呀,空格呀,换行符呀之类的.
	/// </summary>
	/// <returns></returns>
	public static Segment TryRemoveWhitespaceBetweenPairSymbol(
		Segment segment,
		List<Segment> previousSegments,
		out uint mergeSegmentCount,
		out uint mergedTotalSegmentCharCount)
	{
		//如果现在的符号是一个成对出现的符号的结尾  比如可能是一个括号的右半边 )
		//那么他的CanInsertContentInMiddle应该是true

		//如果不是这样的符号,那么就不用管了,直接返回
		//找到以当前这个符号作为右半边的所有可能
		var endWithThisSegmentStaticPairSegments = All.Where(staticSegment =>
			staticSegment.Content.EndsWith(segment.Content) && staticSegment.CanInsertContentInMiddle).ToList();

		//如果并没有,那么不是一个成对出现的符号,直接返回
		if (endWithThisSegmentStaticPairSegments.Count == 0)
		{
			mergeSegmentCount = 0;
			mergedTotalSegmentCharCount = 0;
			return segment;
		}

		//向前一直检查到结束
		var index = previousSegments.Count - 1;
		//中间隔档的空白符的列表
		var middleWhitespaceOrLineBreakSegments = new List<Segment>();
		//目标成对的segment是:
		var targetPairSegment = Segment.Empty;

		while (index >= 0)
		{
			var currentWaitMergeSegment = previousSegments[index];

			//如果不可见假装认为可以吃掉,记录到中间隔档空白符的列表中
			if (currentWaitMergeSegment.IsWhitespace || currentWaitMergeSegment == LineBreakSymbol)
			{
				middleWhitespaceOrLineBreakSegments.Add(currentWaitMergeSegment);
				index--;
				continue;
			}
			//其他的字符,那就要看是不是组合符号的左半边了,如果不是,那么就不用管了,直接返回
			//如果是,退出循环,进行下一步的处理

			//假设这个currentWaitMergeSegment就是一个左半边,和右半边也就是传入的segment组合起来,
			//看一下endWithThisSegmentStaticPairSegments中是否有这样的组合
			var mergedContent = currentWaitMergeSegment.Content + segment.Content;
			var matchedStaticSegments =
				endWithThisSegmentStaticPairSegments.Where(staticSegment => staticSegment.Content == mergedContent);
			//如果匹配到了多个,那就是错误了,因为一个左半边只能对应一个右半边,抛出异常
			var staticSegments = matchedStaticSegments as Segment[] ?? matchedStaticSegments.ToArray();
			switch (staticSegments.Length)
			{
				case > 1:
					throw new Exception(
						$"Segments:TryRemoveWhitespaceBetweenPairSymbol:一个左半边对应了多个右半边:{string.Join(",", staticSegments.Select(s => s.Content))}");
				//如果没有匹配到,那就不是一个左半边,直接返回
				case 0:
					mergeSegmentCount = 0;
					mergedTotalSegmentCharCount = 0;
					return segment;
			}

			//如果匹配到了且只有一个,那就是这个左半边对应的右半边,退出循环,进行下一步的处理(这个地方staticSegments.Length只能是1)
			targetPairSegment = staticSegments[0];
			break;
		}

		//如果没有找到(也就是index都找到头了),那么就不是一个成对出现的符号,直接返回
		if (targetPairSegment == Segment.Empty)
		{
			mergeSegmentCount = 0;
			mergedTotalSegmentCharCount = 0;
			return segment;
		}

		//否则就是找到了,然后把中间的空白符都吃掉,然后返回
		if (middleWhitespaceOrLineBreakSegments.Count > 0)
		{
			mergeSegmentCount = (uint)middleWhitespaceOrLineBreakSegments.Count;
			mergedTotalSegmentCharCount = (uint)middleWhitespaceOrLineBreakSegments.Sum(s => s.Length);
			return targetPairSegment;
		}

		//如果中间没有空白符,那么就不用管了,直接返回
		mergeSegmentCount = 0;
		mergedTotalSegmentCharCount = 0;
		return segment;
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
	public static Segment CloseParenthesesSymbol = new() { Content = "()", CanInsertContentInMiddle = true };

	/// <summary> 闭合空参数的尖括号 </summary>
	public static Segment CloseAngleBracketsSymbol = new() { Content = "<>", CanInsertContentInMiddle = true };

	/// <summary> 闭合空参数的方括号 </summary>
	public static Segment CloseSquareBracketsSymbol = new() { Content = "[]", CanInsertContentInMiddle = true };

	/// <summary> 闭合空参数的大括号 </summary>
	public static Segment CloseBracesSymbol = new() { Content = "{}", CanInsertContentInMiddle = true };

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