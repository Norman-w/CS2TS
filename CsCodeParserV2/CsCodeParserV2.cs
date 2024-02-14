/*

第二个全新的方式的Cs代码解析器.
主要思路是使用之前已经解析出来的各种Segment进行CodeNode的生成.
比如cs代码文件的起始部分可能像这个文件一样有一些注释,然后是命名空间,里面有一些类,类里面有一些字段和方法等等.
我们先把注释领空确定,结束了注释以后开始分析 using namespace xxx; 这时候就有了 CodeFile.Children并且里面有了一个UsingNamespaceNode.
然后再根据领空信息向内解析,必要时返回上层领空等等一些列操作直到解析完所有Segment,关闭所有领空.

*/

using CS2TS.Model;

namespace CS2TS.CsCodeParserV2;

/// <summary>
///     第二个全新的方式的Cs代码解析器.用于解析Cs代码文件到一个CodeFile对象.
/// </summary>
public class CsCodeParserV2
{
	/// <summary>
	///     解析Cs代码文件到一个CodeFile对象.
	/// </summary>
	/// <param name="segments"></param>
	/// <returns></returns>
	public CodeFile? Parse(List<Segment> segments)
	{
		/*

		 大体思路是 遇到一个Segment,看这个Segment可能是什么意图,然后后面的一个一个Segment逐次确认之前猜测出的可能性.
		 比如,第一个就是using,领空位于CodeFile,
		 那么这个using就是用来在CodeFile中引用命名空间的,
		 所以可能要开辟的新领空所使用的CodeNode的类型可能是UsingNamespaceNode,
		 因为这个地方不能使用 using(var stream = new FileStream()) 这种语法,
		 所以这个地方的可能性就是using namespace xxx;


		 或者如,第一个就是public,那么通过这个public可以修饰的CodeNode类型和当前的领空CodeFile能获得到一个交集,
		 CodeFile的当前定义中包含这些:
		 ...
		 public class CodeFile : CodeNode,
		   IContainer4DefineNamespace,
		   IContainer4DefineClass,
		   IContainer4DefineEnum,
		   IContainer4DefineFunction,
		   IContainer4DefineUsing
		 ...
		 然后public属于AccessModifierSegments中的一个Segment,可以修饰的包含
		 ...
		 private static readonly List<Type> AccessModifierUseForCodeNodeTypes = new()
		   {
		   typeof(Class), typeof(Struct), typeof(Enum), typeof(Interface), typeof(Delegate), typeof(Record),
		   typeof(Event), typeof(Property), typeof(Field), typeof(Method), typeof(Constructor), typeof(Operator)
		   };
		...
		所以求出交集就可能是Class, Enum, Function, 并且由于 using不使用访问修饰符,所以这个地方可能是Class, Enum, Function中的一个.
		所以这三种就被添加到备选的可能性中,然后再根据后面的Segment逐次确认之前猜测出的可能性.
		如果后面一个是 static 还是不太能确定,但是如果后面跟了一个 class,那么就确定了,这个是一个类的定义.

		再往后,遇到了{以后, Class类中定义了 BodyStartSegment为SymbolSegments.BracesStartSegment,那么就确定了这个是一个类的定义.
		领空被正式确立并且开始解析内部的定义,这个时候就要开辟新的领空,并且这个领空的类型就是ClassNode.

		中间的过程同理,最后得到一个匹配的}以后,领空被关闭,并且返回上一层领空,直到所有的Segment都被解析完毕.


        */
		//TODO: 实现这个方法
		return null;
	}
}