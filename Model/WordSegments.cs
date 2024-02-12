/*


 这个文档里面的单词都是待整理的
 所有的这个语言里面可以出现的关键词都可以写到这里,然后再整理到Model/Words里面.
 这里还有很多没有列出来的 比如async, yield, 等等.
 2024年02月11日11:58:50目前只整理到了了访问修修饰符(public/private/protected/internal)
 还有一个类型关键字(class/struct/enum/interface/delegate/record/event)

 其他的还不知道怎么归类和给那一类起名字呢



*/

using CS2TS.Model;

namespace CS2TS;

public static class WordSegments
{
	#region 类似于 static, readonly, virtual, override, sealed, abstract, async, new 这样的关键字,都叫做"修饰符"

	public static readonly WordSegment Static = new() { Content = "static" };
	public static readonly WordSegment Readonly = new() { Content = "readonly" };
	public static readonly WordSegment Virtual = new() { Content = "virtual" };
	public static readonly WordSegment Override = new() { Content = "override" };
	public static readonly WordSegment Sealed = new() { Content = "sealed" };
	public static readonly WordSegment Abstract = new() { Content = "abstract" };
	public static readonly WordSegment Async = new() { Content = "async" };
	public static readonly WordSegment New = new() { Content = "new" };

	#endregion
}