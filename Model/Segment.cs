/*

 比如 public static class 这样的
   或者可能是分号,一组空格,一组\r\n,或一个\n,或者+ - * / 这样的符号都会是语义段的最小单元.
   对于备注类型的来说 // 是一个语义段, 剩下的注释内容是一段,然后\r\n或者\n就是一个语义段

   以下是dart中的定义:
   class Segment {
   //内容
   final String content;
   //是否解析过
   bool parsed = false;
   //所在行
   int line = 0;
   //segment的index(相对于当前行)
   int segmentIndexOnLine = 0;
   //segment的index(相对于整个文本)
   int segmentIndexOfWholeText = 0;
   //所在行的第几个字符开始
   int start = 0;
   //这个暂时不需要了,直接通过length来判断长度,结束位置就是start + length
   // //所在行的第几个字符结束
   // int end = 0;
   //是否为换行符
   bool get isLineBreak => content == '\n';

   //内容是否为不可见字符
   bool get isWhitespace => _isWhitespace(content);

   Segment(this.content);

   //定义一个方法,用来检测是否为不可见的字符(显示不出来的)
   bool _isWhitespace(String s) {
   //如果是空格,或者换行,或者制表符,或者回车,或者换页符,或者垂直制表符,就返回true
   //不可见字符集: s == ' ' || s == '\n' || s == '\t' || s == '\r' || s == '\f' || s == '\v';
   const whitespace = [' ', '\n', '\t', '\r', '\f', '\v'];
   //是否整个string中的所有字符都是不可见字符
   return s.split('').every((c) => whitespace.contains(c));
   }

   //是否选中的私有变量
   bool _selected = false;
   //获取选中的值
   bool get selected => _selected;
   //设置被选中
   void select() => _selected = true;
   //设置取消选中
   void unselect() => _selected = false;
   }
*/

using System.Reflection;
using System.Text;

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
	///     所在行
	/// </summary>
	public int Line { get; set; }

	/// <summary>
	///     segment的index(相对于当前行)
	/// </summary>
	public int SegmentIndexOnLine { get; set; }

	/// <summary>
	///     segment的index(相对于整个文本)
	/// </summary>
	public int SegmentIndexOfWholeText { get; set; }

	/// <summary>
	///     所在行的第几个字符开始
	/// </summary>
	public int Start { get; set; }

	/// <summary>
	///     长度
	/// </summary>
	public int Length => Content.Length;

	/// <summary>
	///     所在行的第几个字符结束
	/// </summary>
	public int End => Start + Length;

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
		_wordBreakWords.AddRange(Constant.InvisibleChars);
		//检查重复项
		var duplicateItems = _wordBreakWords.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key)
			.ToList();
		//输出重复项
		if (duplicateItems.Count > 0)
			Console.WriteLine($"重复的项:{string.Join(",", duplicateItems)}");
		//去重
		_wordBreakWords = _wordBreakWords.Distinct().ToList();
	}

	//拓展方法集合
	/// <summary>
	///     在给定的string中,找出头一个segment,
	///     比如 namespace CS2TS.Model; 这样的,
	///     会找出 namespace(之前的using已经被pick出去了),所以这里传入的csCodeString应该是subString之后的.具体游标在调用处处理
	/// </summary>
	/// <param name="csCodeString"></param>
	/// <returns></returns>
	public static Segment PickFromCodeString(string csCodeString)
	{
		var segmentContent = new StringBuilder();
		var index = 0;
		while (index < csCodeString.Length)
		{
			var currentChar = csCodeString[index];
			if (currentChar == '\n')
			{
			}

			segmentContent.Append(currentChar);
			if (WordBreakWords.Contains(currentChar.ToString()))
			{
				//但是如果segmentContent是空的,当前也是空的,那就继续往下走
				if (segmentContent.Length > 0 && string.IsNullOrWhiteSpace(segmentContent.ToString()) &&
				    Constant.NonOperatorWhitespaceChars.Contains(currentChar.ToString()))
				{
					index += 1;
					continue;
				}

				//如果segmentContent是空白的,但是当前的是可以break的非空字符,那就直接返回
				if (segmentContent.Length != 1) segmentContent.Remove(segmentContent.Length - 1, 1);

				break;
			}

			//如果前面是空的但是这个不是空的,那也要中断
			var previousCharsWithOutThisChar = csCodeString[..index];
			if (string.IsNullOrWhiteSpace(previousCharsWithOutThisChar) && previousCharsWithOutThisChar.Length > 0)
			{
				segmentContent.Remove(segmentContent.Length - 1, 1);
				break;
			}

			index += 1;
		}

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
			Content = segmentContent.ToString()
		};
	}
}