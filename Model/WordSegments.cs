/*


 这个文档里面的单词都是待整理的
 所有的这个语言里面可以出现的关键词都可以写到这里,然后再整理到Model/Words里面.
 这里还有很多没有列出来的 比如async, yield, 等等.
 2024年02月11日11:58:50目前只整理到了了访问修修饰符(public/private/protected/internal)
 还有一个类型关键字(class/struct/enum/interface/delegate/record/event)

 其他的还不知道怎么归类和给那一类起名字呢



*/

using CS2TS.Model.Words;

namespace CS2TS;

public static class WordSegments
{
	#region 引用值

	public static SystemValueTypeSegment Null = new()
	{
		Content = "null"
	};

	public static SystemValueTypeSegment Typeof = new()
	{
		Content = "typeof"
	};

	public static SystemValueTypeSegment Sizeof = new()
	{
		Content = "sizeof"
	};

	public static SystemValueTypeSegment Default = new()
	{
		Content = "default"
	};

	public static SystemValueTypeSegment This = new()
	{
		Content = "this" //TODO 这个可能有不同的意义?
	};

	public static SystemValueTypeSegment NameOf = new()
	{
		Content = "nameof"
	};

	public static SystemValueTypeSegment Base = new()
	{
		Content = "base"
	};

	#endregion

	#region 参数类型限定

	public static SystemValueTypeSegment Params = new()
	{
		Content = "params"
	};

	public static SystemValueTypeSegment Ref = new()
	{
		Content = "ref"
	};

	public static SystemValueTypeSegment Out = new()
	{
		Content = "out"
	};

	#endregion

	#region 参数类型判断/确定/使用, 但是这个in 要考虑是不是要跟 for之类的单独作为什么逻辑

	//TODO: 这个in 要考虑是不是要跟 for之类的单独作为什么逻辑

	public static SystemValueTypeSegment Is = new()
	{
		Content = "is"
	};

	public static SystemValueTypeSegment As = new()
	{
		Content = "as"
	};

	public static SystemValueTypeSegment In = new()
	{
		Content = "in"
	};

	#endregion

	#region 循环关键词 foreach/for/while/do

	//TODO 实现

	#endregion

	#region 循环逻辑/条件 关键词 continue/break/goto

	//TODO 实现

	#endregion

	#region 领空关键字 using/return/try/catch/finally/this/throw/when/fixed/checked/unchecked/unsafe

	//TODO 实现

	#endregion

	#region 逻辑关系关键词 if/else/switch/case/default 等的

	//TODO 实现

	#endregion

	#region 异步操作关键词 await/yield/lock等,其中async是用于函数限定中,所以放在了函数相关的地方了

	//TODO,async是不是要找到合适的地方和合适的定义?

	#endregion

	#region 访问器关键词 get/set/add/remove

	//TODO, 实现,另外 get/set是不是已经在别的地方定义了?

	#endregion

	#region implicit/explicit

	//TODO, 实现,这是啥?

	#endregion
}