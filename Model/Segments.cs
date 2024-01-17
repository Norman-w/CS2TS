namespace CS2TS.Model;

public static class Segments
{
	/// <summary> 除法符号 </summary>
	public static Segment DivisionSymbol = new() { Content = "/" };

	/// <summary> 行注释符号 </summary>
	public static Segment AnnotationLineSymbol = new() { Content = "//" };

	/// <summary> 多行注释开始符号 </summary>
	public static Segment AnnotationAreaStartSymbol = new() { Content = "/*" };

	/// <summary> 多行注释结束符号 </summary>
	public static Segment AnnotationAreaEndSymbol = new() { Content = "*/" };

	/// <summary> 乘法符号 </summary>
	public static Segment MultiplicationSymbol = new() { Content = "*" };

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
		while (mergeSucceed && index >= 0)
		{
			var currentWaitMergeSegment = previousSegments[index];
			index--;
			//如果当前是/ 看前面是啥
			if (segment == DivisionSymbol)
			{
				//如果前一个也是 / 那么就是注释
				if (currentWaitMergeSegment == DivisionSymbol)
				{
					mergeSucceed = true;
					_mergeSegmentCount++;
					_mergedTotalSegmentCharCount += (uint)DivisionSymbol.Length;
					currentAfterMergeSegment = AnnotationLineSymbol;
				}
				//如果前面是 */ 那么就是多行注释的开始
				else if (currentWaitMergeSegment == MultiplicationSymbol)
				{
					mergeSucceed = true;
					_mergeSegmentCount++;
					_mergedTotalSegmentCharCount += (uint)MultiplicationSymbol.Length;
					currentAfterMergeSegment = AnnotationAreaEndSymbol;
				}
				else
				{
					mergeSucceed = false;
				}
			}
			//如果当前是* 前面是/ 那就是多行注释的起点
			else if (segment == MultiplicationSymbol)
			{
				if (currentWaitMergeSegment == DivisionSymbol)
				{
					mergeSucceed = true;
					_mergeSegmentCount++;
					_mergedTotalSegmentCharCount += (uint)DivisionSymbol.Length;
					currentAfterMergeSegment = AnnotationAreaStartSymbol;
				}
				else
				{
					mergeSucceed = false;
				}
			}
		}

		mergeSegmentCount = _mergeSegmentCount;
		mergedTotalSegmentCharCount = _mergedTotalSegmentCharCount;
		return currentAfterMergeSegment;
	}
}