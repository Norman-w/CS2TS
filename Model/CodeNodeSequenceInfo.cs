namespace CS2TS.Model;

/// <summary>
/// 在当前空间内所有子元素的排列信息.比如 在一个class 中有很多的 variable 和很多的 function,但是他们可能是一个变量一个函数一个变量一个函数这样定义的
/// 我们保存的时候是把所有的变量保存在一个集合中,所有的函数保存在一个集合中.而实际使用时如果要更高度的还原源代码的结构,就需要用到排列信息.
/// 而如果要更高度的保持代码的可读性,有时候可能需要把变量放一堆,函数放在一堆.根据实际情况来确定.
/// </summary>
public class CodeNodeSequenceInfo
{
  /// <summary>
  /// 相对于父元素的索引
  /// </summary>
  public int IndexOfParent { get; set; }

  /// <summary>
  /// 当前元素的类型
  /// </summary>
  public Type Type { get; set; }

  /// <summary>
  /// 如类中的所有变量都会放到同一个list中保存,这个值记录当前描述的片段/元素 所在那个类型(变量/方法/枚举等)的数组中的索引.
  /// </summary>
  public int IndexOfThisTypeAllElemList { get; set; }

  /// <summary>
  /// 该片段的名字,注意他可能不唯一.比如 同一个名字的函数可以有多个. 同一个名字的类如果作为分段来写的话也可以有多个同名类的标记.
  /// </summary>
  public string Name { get; set; }
}
