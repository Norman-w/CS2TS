namespace CS2TS.Model;

public static class Segments
{
	private static List<Segment>? _all;

	public static List<Segment> All
	{
		get
		{
			if (_all != null) return _all;
			_all = new List<Segment>();
			_all.AddRange(SymbolSegments.All);
			return _all;
		}
	}

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

	//TODO:这个方法还没有测试过
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
			if (currentWaitMergeSegment.IsWhitespace || currentWaitMergeSegment == SymbolSegments.LineBreakSymbol)
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
}