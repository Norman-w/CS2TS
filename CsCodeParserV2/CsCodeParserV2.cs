/*

第二个全新的方式的Cs代码解析器.
主要思路是使用之前已经解析出来的各种Segment进行CodeNode的生成.
比如cs代码文件的起始部分可能像这个文件一样有一些注释,然后是命名空间,里面有一些类,类里面有一些字段和方法等等.
我们先把注释领空确定,结束了注释以后开始分析 using namespace xxx; 这时候就有了 CodeFile.Children并且里面有了一个UsingNamespaceNode.
然后再根据领空信息向内解析,必要时返回上层领空等等一些列操作直到解析完所有Segment,关闭所有领空.

*/

using System.Text;
using CS2TS.Model;
using CS2TS.Model.Node;
using CS2TS.Model.Words;

namespace CS2TS.CsCodeParserV2;

/// <summary>
///     第二个全新的方式的Cs代码解析器.用于解析Cs代码文件到一个CodeFile对象.
/// </summary>
public class CsCodeParserV2
{
	private CsCodeParserV2()
	{
	}

	/// <summary>
	///     代码节点的领空链.注意不是树状的,是表示路径的,用于当做游标.
	/// </summary>
	private List<CodeNode> CodeNodeSpaces { get; } = new() { new CodeFile() };

	/// <summary>
	///     解析Cs代码文件到一个CodeFile对象.
	/// </summary>
	/// <param name="segments"></param>
	/// <returns></returns>
	private CodeFile? Parse(List<Segment> segments)
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

		/*

		 尚未处理,待定,没有被认领的Segment集合.比如
		 public class A{}
		 在处理到public的时候,public就是待认领的AccessModifierSegment,然后在处理到class的时候,就认领了public.

        */
		List<Segment> unclaimedSegments = new();

		foreach (var currentSegment in segments)
		{
			if (currentSegment == AccessModifierSegments.Public)
				//TODO 测试代码
				Console.WriteLine("public");
			if (currentSegment == SymbolSegments.BracesEndSymbol)
				//TODO 测试代码
				Console.WriteLine("}");
			//获取当前领空
			var currentSpaceNode = CodeNodeSpaces[^1];
			var processSegmentResult =
				ProcessSegment(ref currentSpaceNode, currentSegment, ref unclaimedSegments);
			if (processSegmentResult) continue;

			//其他的没有被解析的WordSegment,如果不是空的,则可能是名字,判断当前领空是否可以接受名字,如果可以,则设置当前领空的名字
			if (currentSegment is WordSegment && !currentSegment.IsWhitespace)
				if (currentSpaceNode is INamedCodeNode namedCodeNode)
					if (currentSpaceNode is INameChainCodeNode nameChainCodeNode)
						nameChainCodeNode.NameChain.Names.Add(currentSegment.Content);
					else
						namedCodeNode.Name += currentSegment.Content;
		}

		#region 所有的Segment都处理完以后

		//看namespace是否只有一个并且是文件级的那种,就是以分号结束的那种.
		//如果是的话,就把CodeFile下面的所有其他类型的CodeNode都移动到这个namespace下面,然后关闭namespace领空,把这个namespace领空移动到CodeFile下面
		//如果不是的话,就把这个namespace领空移动到CodeFile下面
		var namespaceList = CodeNodeSpaces[0].Children.OfType<Namespace>().ToList();
		var namespaceCount = namespaceList.Count;
		if (namespaceCount == 1)
		{
			var onlyNamespace = namespaceList[0];
			if (onlyNamespace.IsFileLevelNamespace)
			{
				//把CodeFile下面的所有其他类型的CodeNode都移动到这个namespace下面
				var codeFile = CodeNodeSpaces[0] as CodeFile;
				var otherCodeNodes = codeFile!.Children.Where(x => x != onlyNamespace).ToList();
				foreach (var otherCodeNode in otherCodeNodes)
				{
					codeFile.Children.Remove(otherCodeNode);
					onlyNamespace.Children.Add(otherCodeNode);
				}
			}
			// //把这个namespace领空移动到CodeFile下面
			// if (CodeNodeSpaces[0] is CodeFile codeFile && !codeFile.Children.Contains(onlyNamespace))
			// 	codeFile!.Children.Add(onlyNamespace);
		}

		#endregion

		return CodeNodeSpaces[0] as CodeFile;
	}

	/// <summary>
	///     处理当前Segment
	/// </summary>
	/// <param name="currentSpaceNode"></param>
	/// <param name="currentSegment"></param>
	/// <param name="unclaimedSegments"></param>
	/// <returns></returns>
	private bool ProcessSegment(ref CodeNode currentSpaceNode, Segment currentSegment,
		ref List<Segment> unclaimedSegments)
	{
		var currentSpaceNodeType = currentSpaceNode.GetType();
		//获取当前领空下可以书写什么,比如不能再PropertyNode下面写一个namespace,namespace下可以写ClassNode等等
		var sonCodeNodeValidTypes = currentSpaceNode is ContainerCodeNode node
			? node.SonCodeNodeValidTypes
			: null;
		switch (currentSegment)
		{
			//获取当前Segment是否可以结束当前的CodeNode
			case SymbolSegment symbolSegment
				when symbolSegment.CanFinishCodeNodeTypes.Contains(currentSpaceNodeType):
			{
				FinishCurrentSpaceNode(ref currentSpaceNode, currentSegment);
				//如果成功当前Segment处理了,那么就跳过后面的代码,直接处理下一个Segment
				return true; //continue;
				//continue后不需要break了.直接返回到了foreach层继续处理下一个Segment
			}
			case SymbolSegment symbolSegment:
			{
				if (symbolSegment == SymbolSegments.DotSymbol)
					//如果已经有名字了,并且之前的一部分是word而这一部分是 点号,那么就是名字的下一层
					//如 namespace A.B , A是名字, 当前是点号, B是名字的下一层
					//追加名字
					if (currentSpaceNode is INamedCodeNode namedCodeNode)
						if (currentSpaceNode is INameChainCodeNode) // nameChainCodeNode)
						{
							//不用操作,后面会调用:
							//nameChainCodeNode.NameChain.Names.Add(currentSegment.Content);
						}
						else
						{
							namedCodeNode.Name += SymbolSegments.DotSymbol.Content;
						}

				break;
			}
			//获取当前Segment可以用于修饰什么,根据可以修饰的类型缩小范围,如果有且只有一个类型的那就可以开辟领空了
			case ModifierSegment modifierSegment:
			{
				if (modifierSegment.UseForCodeNodeTypes.Count == 1)
				{
					var onlyOneType = modifierSegment.UseForCodeNodeTypes[0];
					if (sonCodeNodeValidTypes != null && sonCodeNodeValidTypes.Contains(onlyOneType))
						if (OpenNewSpaceNode(onlyOneType, out currentSpaceNode, ref unclaimedSegments, currentSegment))
							return true;
				}
				else
				{
					//暂时不能确定,先放到待定集合中
					unclaimedSegments.Add(modifierSegment);
				}

				break;
			}
			//获取当前Segment是什么类型名称,如果是类型名称的话就可以直接开辟领空了.
			case CodeNodeTypeSegment typeNameSegment:
			{
				var codeNodeType = typeNameSegment.CodeNodeType;
				if (sonCodeNodeValidTypes != null && sonCodeNodeValidTypes.Contains(codeNodeType))
					if (OpenNewSpaceNode(codeNodeType, out currentSpaceNode, ref unclaimedSegments, currentSegment))
						return true;

				break;
			}
			default:
				unclaimedSegments.Add(currentSegment);
				break;
		}

		return true;
	}

	/// <summary>
	///     开辟新的领空
	/// </summary>
	private bool OpenNewSpaceNode(Type codeNodeType, out CodeNode newSpaceNode,
		ref List<Segment> unclaimedSegments, Segment currentSegment)
	{
		//开辟领空
		//根据codeNodeType开辟领空
		//TODO
		newSpaceNode = (CodeNode)Activator.CreateInstance(codeNodeType)!;
		StringBuilder codeString = new();
		foreach (var seg in unclaimedSegments) codeString.Append(seg.Content);
		newSpaceNode.CodeBody += codeString.ToString();
		newSpaceNode.CodeBody += currentSegment.Content;
		ClaimUnclaimedSegments(newSpaceNode, ref unclaimedSegments);
		CodeNodeSpaces.Add(newSpaceNode);
		return true;
	}

	/// <summary>
	///     能开辟领空的时候,把未被认领的Segment集合中的Segment认领了,是什么修饰符就安排到这个新领空的对应的修饰符字段中去.
	/// </summary>
	private static void ClaimUnclaimedSegments(CodeNode newSpaceNode, ref List<Segment> unclaimedSegments)
	{
		//已经被认领的Segment集合
		List<Segment> appliedSegments = new();
		foreach (var unclaimedSegment in unclaimedSegments)
		{
			//如果不是modifier,抛出异常
			if (unclaimedSegment is not ModifierSegment)
				// throw new Exception($"未被认领的Segment集合中的Segment不是ModifierSegment类型,而是{unclaimedSegment.GetType()}类型");
				continue;
			//如果是modifier,那么就把他安排到新领空的对应的修饰符字段中去
			if (unclaimedSegment is ModifierSegment segment)
			{
				if (newSpaceNode is IUseStaticModifierCodeNode useStaticModifierCodeNode)
				{
					useStaticModifierCodeNode.ApplyModifier(segment);
					appliedSegments.Add(segment);
				}

				if (newSpaceNode is IUseAccessModifierCodeNode useAccessModifierCodeNode)
				{
					useAccessModifierCodeNode.ApplyModifier(segment);
					appliedSegments.Add(segment);
				}
			}
		}

		//去掉已经被认领的Segment
		foreach (var appliedSegment in appliedSegments) unclaimedSegments.Remove(appliedSegment);
	}

	/// <summary>
	///     结束当前领空
	///     结束领空时,会把codeBody中的内容赋值给当前领空的CodeBody,并且把当前领空添加到上一层领空的Children中
	/// </summary>
	/// <returns></returns>
	private void FinishCurrentSpaceNode(ref CodeNode currentSpaceNode, Segment currentSegment)
	{
		currentSpaceNode.CodeBody += currentSegment.Content;
		//结束当前领空
		CodeNodeSpaces.RemoveAt(CodeNodeSpaces.Count - 1);
		//上一层一定是一个ContainerCodeNode,所以可以直接转换,然后添加当前领空到上一层的Children中
		var parent = (ContainerCodeNode)CodeNodeSpaces[^1];
		parent.Children.Add(currentSpaceNode);
		//返回上一层领空
		currentSpaceNode = CodeNodeSpaces[^1];
	}

	public static void Test()
	{
		/*

		 namespace CS2TS.Test._1_InTestCSFiles;

		   public class 为v2准备的简单类
		   {
		   }

        */
		StringBuilder testCode = new();
		testCode.AppendLine("namespace CS2TS.Test._1_InTestCSFiles;");
		testCode.AppendLine();
		testCode.AppendLine("public class 为v2准备的简单类");
		testCode.AppendLine("{");
		testCode.AppendLine("}");

		var csCodeString = testCode.ToString();

		var segments = new List<Segment>();

		var cursorX = 0;
		var cursorY = 0;
		var segmentIndexOfWholeCodeString = 0;
		var allSegments = Segment.PickAllSegmentsFromWholeCodeString(csCodeString, ref cursorY, ref cursorX,
			ref segmentIndexOfWholeCodeString);
		segments.AddRange(allSegments);

		#region 粘连,合并

		var segmentsCopy = new List<Segment>(segments);
		var index = 0;
		do
		{
			if (index < 1)
			{
				index++;
				continue;
			}

			var currentSegment = segmentsCopy[index];


			//提取segments的index之前的所有segment
			var previousSegments = segmentsCopy.GetRange(0, index);

			var mergedSegment = Segments.MergeBackwards(currentSegment, previousSegments, out var mergeSegmentCount,
				out var mergedTotalSegmentCharCount);

			if (mergeSegmentCount == 0 || mergedTotalSegmentCharCount == 0)
			{
				//如果没有合并,那么就迭代器+1

				index++;
				continue;
			}
			//如果第 0,1,2,3 里面的 3把2给吃掉了,那就移除2,然后把3放到2的位置,3的长度增加吃掉的2的长度
			//如果第 0,1,2,3 里面的 3把2,1给吃掉了,那就移除1,2,然后把3放到1的位置,3的长度增加吃掉的2,1的长度
			//以此类推
			//1,移除
			//2,换位

			//1
			segmentsCopy.RemoveRange(index - (int)mergeSegmentCount, (int)mergeSegmentCount);
			index -= (int)mergeSegmentCount;
			//2
			segmentsCopy[index] = mergedSegment;
			//迭代器+
			index++;
		} while (index < segmentsCopy.Count);

		segments.Clear();
		segments.AddRange(segmentsCopy);

		#endregion

		#region 修正每一个segment的index数据

		//在当前行中,当前segment的序号,当遇到换行符,这个序号就会重置为0
		//在所有行中,当前segment的序号,一直递增
		var currentSegmentIndexOfAllLines = 0;
		//当前行的序号,一直递增,遇到一个换行符,这个序号就会递增1
		//第一个字符在当前行中的位置,在遇到换行符的时候,这个值会重置为0
		//第一个字符在所有行中的位置,一直递增
		//因为很多是从static中获取来的对象引用,所以这里要重新new一下,不然会影响到static中的数据
		var newSegmentsList = new List<Segment>();
		while (currentSegmentIndexOfAllLines < segments.Count)
		{
			var currentSegment = segments[currentSegmentIndexOfAllLines];

			var segmentType = currentSegment.GetType();
			var copySegment = (Segment)Activator.CreateInstance(segmentType)!;
			copySegment.CopyFrom(currentSegment);
			newSegmentsList.Add(copySegment);
			//
			// newSegmentsList.Add(new SymbolSegment
			// {
			// 	Content = currentSegment.Content,
			// 	SegmentIndexOfLine = currentSegmentIndexOfLine,
			// 	SegmentIndexOfAllLines = currentSegmentIndexOfAllLines,
			// 	LineIndexOfAllLines = currentLineIndexOfAllLines,
			// 	StartCharIndexOfLine = currentSegmentFirstCharIndexOfLine,
			// 	StartCharIndexOfAllLines = currentSegmentFirstCharIndexOfAllLines
			// });

			// Console.WriteLine(
			// 	$"第{currentSegmentIndexOfAllLines}个segment,内容:{currentSegment.Content},在行中位置:{currentSegment.SegmentIndexOfLine},在所有行中位置:{currentSegment.SegmentIndexOfAllLines},在所有行中的行号:{currentSegment.LineIndexOfAllLines},在行中的第一个字符位置:{currentSegment.StartCharIndexOfLine},在所有行中的第一个字符位置:{currentSegment.StartCharIndexOfAllLines}");

			currentSegmentIndexOfAllLines++;

			if (currentSegment.IsLineBreak)
			{
			}
		}

		#endregion

		Console.WriteLine("");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("解析完毕");
		Console.WriteLine("最后一个Segment的最后一个字符的位置:" +
		                  (newSegmentsList[^1].StartCharIndexOfAllLines + newSegmentsList[^1].Length));
		Console.ResetColor();


		//正式解析segment的list到CodeFile对象
		CsCodeParserV2 parser = new();
		parser.Parse(newSegmentsList);
	}
}