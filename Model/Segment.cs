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

namespace CS2TS.Model;

/// <summary>
///     语义最小单元
/// </summary>
public class Segment
{
	/// <summary>
	///     代码内容
	/// </summary>
	public string Content { get; set; } = string.Empty;

	/// <summary>
	///     是否已经解析过
	/// </summary>
	public bool Parsed { get; set; }

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
	public int Length { get; set; }

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
	public bool IsWhitespace => Constant.WhiteSpaceChars.Contains(Content);
}