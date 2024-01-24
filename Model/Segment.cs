/*

 比如 public static class 这样的
   或者可能是分号,一组空格,一组\r\n,或一个\n,或者+ - * / 这样的符号都会是语义段的最小单元.
   对于备注类型的来说 // 是一个语义段, 剩下的注释内容是一段,然后\r\n或者\n就是一个语义段
*/

using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CS2TS.Model;

/// <summary>
///     语义最小单元
/// </summary>
public partial class Segment
{
	/// <summary>
	///     代码内容
	/// </summary>
	public string Content { get; set; } = string.Empty;

	// /// <summary>
	// ///     是否已经解析过
	// /// </summary>
	// public bool Parsed { get; set; }

	/// <summary>
	///     所在行,在所有行中的第几行
	/// </summary>
	public int LineIndexOfAllLines { get; set; }


	/// <summary>
	///     第一个字符是这一行中的第几个字符.
	/// </summary>
	public int StartCharIndexOfLine { get; set; }

	/// <summary>
	///     第一个字符是整个文本中的第几个字符
	/// </summary>
	public int StartCharIndexOfAllLines { get; set; }

	/// <summary>
	///     segment的index(相对于当前行)
	/// </summary>
	public int SegmentIndexOfLine { get; set; }

	/// <summary>
	///     segment的index(相对于整个文本)
	/// </summary>
	public int SegmentIndexOfAllLines { get; set; }

	/// <summary>
	///     长度
	/// </summary>
	public int Length => Content.Length;

	/// <summary>
	///     是否为换行符
	/// </summary>
	public bool IsLineBreak => Content == "\n";

	/// <summary>
	///     是否为不可见字符
	/// </summary>
	public bool IsWhitespace =>
		// Constant.NonOperatorWhitespaceChars.Contains(Content);
		//要每一个都是不可见字符才行
		Content.ToCharArray().All(c => Constant.NonOperatorWhitespaceChars.Contains(c.ToString()));
}

public partial class Segment
{
	/// <summary>
	///     空的segment,不包含任何内容
	/// </summary>
	public static readonly Segment Empty = new()
	{
		Content = string.Empty
	};

	private static List<string>? _wordBreakWords;

	public static List<string> WordBreakWords
	{
		get
		{
			if (_wordBreakWords == null) InitWordBreakWords();

			return _wordBreakWords!;
		}
	}

	/// <summary>
	///     初始化_wordBreakWords,其中包含所有的Segments中static字段的content是1个字符的
	///     另外还包括一些虽然不是操作符(没有逻辑语义不会打断内容的含义),但是也会影响拆词/拆语义的,比如空格,(换行不是,因为换行具备终止注释行的能力),制表符,回车,换页符,垂直制表符等等
	/// </summary>
	private static void InitWordBreakWords()
	{
		if (_wordBreakWords != null && _wordBreakWords.Count != 0) return;
		//获取所有的static字段
		var type = typeof(Segments);
		var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
		var staticSegments = fields.Select(field => field.GetValue(null)).Cast<Segment>().ToList();
		var staticSegmentsContent = staticSegments.Select(staticSegment => staticSegment.Content).ToList();
		_wordBreakWords = staticSegmentsContent;
		//再添加所有不可见字符,因为他们也能拆散语义
		_wordBreakWords.AddRange(Constant.NonOperatorWhitespaceChars);
		//检查重复项
		var duplicateItems = _wordBreakWords.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key)
			.ToList();
		//输出重复项
		if (duplicateItems.Count > 0)
			Console.WriteLine($"Segment:重复的项:{string.Join(",", duplicateItems)}");
		//去重
		_wordBreakWords = _wordBreakWords.Distinct().ToList();
	}

	// //拓展方法集合
	// /// <summary>
	// ///     在给定的string中,找出头一个segment,
	// ///     比如 namespace CS2TS.Model; 这样的,
	// ///     会找出 namespace(之前的using已经被pick出去了),所以这里传入的csCodeString应该是subString之后的.具体游标在调用处处理
	// /// </summary>
	// /// <param name="csCodeString"></param>
	// /// <returns></returns>
	// public static Segment PickFromCodeString(string csCodeString)
	// {
	// 	var segmentContent = new StringBuilder();
	// 	var index = 0;
	// 	while (index < csCodeString.Length)
	// 	{
	// 		var currentChar = csCodeString[index];
	// 		if (currentChar == '\n')
	// 		{
	// 		}
	//
	// 		segmentContent.Append(currentChar);
	// 		if (WordBreakWords.Contains(currentChar.ToString()))
	// 		{
	// 			//但是如果segmentContent是空的,当前也是空的,那就继续往下走
	// 			if (segmentContent.Length > 0 && string.IsNullOrWhiteSpace(segmentContent.ToString()) &&
	// 			    Constant.NonOperatorWhitespaceChars.Contains(currentChar.ToString()))
	// 			{
	// 				index += 1;
	// 				continue;
	// 			}
	//
	// 			//如果segmentContent是空白的,但是当前的是可以break的非空字符,那就直接返回
	// 			if (segmentContent.Length != 1) segmentContent.Remove(segmentContent.Length - 1, 1);
	//
	// 			break;
	// 		}
	//
	// 		//如果前面是空的但是这个不是空的,那也要中断
	// 		var previousCharsWithOutThisChar = csCodeString[..index];
	// 		if (string.IsNullOrWhiteSpace(previousCharsWithOutThisChar) && previousCharsWithOutThisChar.Length > 0)
	// 		{
	// 			segmentContent.Remove(segmentContent.Length - 1, 1);
	// 			break;
	// 		}
	//
	// 		index += 1;
	// 	}
	//
	// 	//如果segments中的static变量有这个segment,那么就直接返回static变量
	// 	//反射获取static变量
	// 	var type = typeof(Segments);
	// 	var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
	// 	var staticSegments = fields.Select(field => field.GetValue(null)).Cast<Segment>().ToList();
	// 	//对比Content
	// 	foreach (var staticSegment in staticSegments.Where(staticSegment =>
	// 		         staticSegment.Content == segmentContent.ToString()))
	// 		return staticSegment;
	// 	//如果没有,那么就新建一个
	//
	// 	return new Segment
	// 	{
	// 		Content = segmentContent.ToString()
	// 	};
	// }

	/// <summary>
	///     在给定的多行文本中,根据行号和所在行中的游标位置,找出一个segment
	///     执行本方法时会将csCodeString分割成多行,然后调用重载的方法
	/// </summary>
	/// <param name="csCodeString"></param>
	/// <param name="cursorLine"></param>
	/// <param name="cursorColumn"></param>
	/// <param name="segmentIndexOnWholeText"></param>
	/// <returns></returns>
	public static List<Segment> PickAllSegmentsFromWholeCodeString(
		string csCodeString, //整个文本
		ref int cursorLine, //在文本中的第几行
		ref int cursorColumn, //在当前行中的第多少个字符
		ref int segmentIndexOnWholeText //segment在整个文本中的index
	)
	{
		//分割行,保留空行,因为分割以后,会移除换行符,所以在分割之后,后面补全一个换行符保持原来的整行内容
		//也就是换行符属于这一行的结尾.所以分割后每行后面头添加一个分隔符(除了最后一行)
		var csCodeStringLines = csCodeString.Split(Segments.LineBreakSymbol.Content).ToList();
		//补全换行符
		for (var i = 0; i < csCodeStringLines.Count - 1; i++)
			csCodeStringLines[i] += Segments.LineBreakSymbol.Content;
		// var result = PickFromCodeString(csCodeStringLines, ref cursorLine, ref cursorColumn);
		// for (var i = 0; i < cursorLine; i++)
		// 	segmentIndexOnWholeText += csCodeStringLines[i].Length;
		// segmentIndexOnWholeText += cursorColumn;
		// result.SegmentIndexOfWholeText = segmentIndexOnWholeText;
		// return result;
		//逐行处理
		var segments = new List<Segment>();
		for (var i = 0; i < csCodeStringLines.Count; i++)
		{
			var line = csCodeStringLines[i];
			//如果是空的,那么就直接返回空的segment
			if (string.IsNullOrWhiteSpace(line))
			{
				segments.Add(Empty);
				continue;
			}

			//如果不是空的,那么就解析行
			var lineSegments = PickAllSegmentsFromLine(line, ref cursorColumn);
			//设置行号
			foreach (var lineSegment in lineSegments) lineSegment.LineIndexOfAllLines = i;
			//设置segment在行中的index和在整个文本中的index
			for (var j = 0; j < lineSegments.Count; j++)
			{
				lineSegments[j].SegmentIndexOfLine = j;
				lineSegments[j].SegmentIndexOfAllLines = segmentIndexOnWholeText++;
			}

			//添加到结果中
			segments.AddRange(lineSegments);
			//更新游标
			cursorLine += 1;
		}

		return segments;
	}

	public static List<Segment> PickAllSegmentsFromLine(
		string csCodeStringLine,
		ref int cursorColumn
	)
	{
		//参数有效性检查
		//如果是空的,那么就直接返回空的segment
		if (string.IsNullOrWhiteSpace(csCodeStringLine))
			return new List<Segment> { Empty };
		var result = new List<Segment>();
		while (cursorColumn < csCodeStringLine.Length)
		{
			var segment = PickFromCodeString(csCodeStringLine, ref cursorColumn);
			result.Add(segment);
		}


		//还原游标
		cursorColumn = 0;

		return result;
	}

	// /// <summary>
	// ///     在给定的多行文本中,根据行号和所在行中的游标位置,找出一个segment
	// ///     调用本方法前应该使用换行符分割整个文本,保证每一行都是单独的一行
	// /// </summary>
	// /// <param name="csCodeStringLines"></param>
	// /// <param name="cursorLine"></param>
	// /// <param name="cursorColumn"></param>
	// /// <returns></returns>
	// public static Segment PickFromCodeString(
	// 	List<string> csCodeStringLines, //整个文本,已经被分割成行了
	// 	ref int cursorLine, //在文本中的第几行
	// 	ref int cursorColumn //在当前行中的第多少个字符
	// )
	// {
	// 	//参数有效性检查
	// 	//如果是空的,那么就直接返回空的segment
	// 	if (csCodeStringLines.Count == 0)
	// 		return Empty;
	// 	if (cursorLine < 0 || cursorLine >= csCodeStringLines.Count)
	// 		throw new ArgumentOutOfRangeException(nameof(cursorLine));
	// 	var line = csCodeStringLines[cursorLine];
	// 	if (cursorColumn < 0 || cursorColumn >= line.Length)
	// 		throw new ArgumentOutOfRangeException(nameof(cursorColumn));
	// 	//如果是空的,那么就直接返回空的segment
	// 	if (string.IsNullOrWhiteSpace(line))
	// 		return Empty;
	// 	var result = PickFromCodeString(line, ref cursorColumn);
	// 	result.LineIndexOfAllLines = cursorLine;
	// 	result.SegmentIndexOfLine = cursorColumn;
	// 	return result;
	// }

	/// <summary>
	///     在给定的单行文本中,找出头一个segment
	///     调用本方法前应该使用换行符分割整个文本,保证传入的是单独的一行
	///     注意,此方法不会返回最后的换行符
	/// </summary>
	/// <param name="csCodeStringLine"></param>
	/// <param name="cursorColumn"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	private static Segment PickFromCodeString(
		string csCodeStringLine, //单行文本
		ref int cursorColumn //在当前行中的第多少个字符
	)
	{
		#region 值有效性检查

		//检查行是否多行(换行符的数量大于1个或者换行符的数量虽然是1但是不是最后一个),使用正则能忽略char和string的格式问题
		var breakSymbolCount = Regex.Matches(csCodeStringLine, Segments.LineBreakSymbol.Content).Count;
		//要是有\n,只能有一个,而且必须要在最后,要么就没有
		var isValidLine = breakSymbolCount == 0 ||
		                  (breakSymbolCount == 1 && csCodeStringLine.EndsWith(Segments.LineBreakSymbol.Content));
		if (!isValidLine)
			throw new Exception($"传入的行不是单行:{csCodeStringLine}.要么没有换行符,要么只有一个换行符,并且在最后");
		if (string.IsNullOrWhiteSpace(csCodeStringLine))
			return Empty;
		//如果这个行直接就是一个换行符就返回一个换行符
		if (Segments.LineBreakSymbol.Content == csCodeStringLine)
			return Segments.LineBreakSymbol;

		#endregion

		#region 正式提取

		var segmentContent = new StringBuilder();
		//记录下来刚开始进入循环之前是在什么位置
		var firstCharIndex = cursorColumn;
		while (cursorColumn < csCodeStringLine.Length)
		{
			if (csCodeStringLine == "\t\t\tc ??= 0;\n")
			{
			}

			var currentChar = csCodeStringLine[cursorColumn];

			if (currentChar == '\t')
			{
			}

			segmentContent.Append(currentChar);
			if (WordBreakWords.Contains(currentChar.ToString()))
			{
				//但是如果segmentContent是空的,当前也是空的,那就继续往下走,就是那种多个空格之类的连到一起的或者是空格+\t之类的
				if (segmentContent.Length > 0 && string.IsNullOrWhiteSpace(segmentContent.ToString()) &&
				    Constant.NonOperatorWhitespaceChars.Contains(currentChar.ToString()))
				{
					cursorColumn++;
					continue;
				}

				//如果内容不是只有一个字符,因为当前的字符是断语义符号,所以要把当前这个排除在外(上面segmentContent.Append(currentChar);的时候多加了)
				if (segmentContent.Length != 1)
				{
					segmentContent.Remove(segmentContent.Length - 1, 1);
					break;
				}

				cursorColumn++;
				break;
			}

			//如果前面是空的但是这个不是空的,那也要中断
			var previousCharsWithOutThisChar = csCodeStringLine[firstCharIndex..cursorColumn];
			if (string.IsNullOrWhiteSpace(previousCharsWithOutThisChar) && previousCharsWithOutThisChar.Length > 0)
			{
				segmentContent.Remove(segmentContent.Length - 1, 1);
				// cursorColumn++;
				break;
			}

			//正常添加到前面了,游标往后移动
			cursorColumn++;
		}

		#endregion

		//如果segments中的static变量有这个segment,那么就直接返回static变量
		//反射获取static变量
		var type = typeof(Segments);
		var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
		var staticSegments = fields.Select(field => field.GetValue(null)).Cast<Segment>().ToList();
		//对比Content
		foreach (var staticSegment in staticSegments.Where(staticSegment =>
			         staticSegment.Content == segmentContent.ToString()))
			return staticSegment;
		//如果没有,那么就新建一个

		return new Segment
		{
			Content = segmentContent.ToString(), StartCharIndexOfLine = cursorColumn - segmentContent.Length
		};
	}
}