using CS2TS.Model.Words;

namespace CS2TS.Model.Node;

public interface ICodeNode
{
}

/// <summary>
///     代码结构中的节点信息.每一个节点下面都会包含子节点的信息表.以及注释信息.
/// </summary>
public abstract class CodeNode : ICodeNode
{
	/// <summary>
	///     相对于父元素的位置
	/// </summary>
	public int Index { get; set; }


	/// <summary>
	///     代码内容
	/// </summary>
	public string CodeBody { get; set; } = "";

	public List<CodeNode> Children { get; } = new();

	/// <summary>
	///     代码节点类型的Segment表示
	/// </summary>
	public abstract Segment? CodeNodeTypeSegment { get; }

	// /// <summary>
	// /// 所有的标记信息
	// /// </summary>
	// public List<NoteBase> Notes { get; set; }
	// public void AddNotes(List<NoteBase> notes)
	// {
	//   if (notes.Count == 0)
	//   {
	//     return;
	//   }
	//
	//   for (int i = 0; i < notes.Count; i++)
	//   {
	//     this.Chirldren.Add(notes[i]);
	//   }
	// }

	public List<NoteBase> GetNotes()
	{
		return GetNodes<NoteBase>();
	}

	protected List<T> GetNodes<T>() where T : CodeNode
	{
		var notes = new List<T>();
		var type = typeof(T);
		foreach (var codeNode in Children)
		{
			var currentCodeNodeType = codeNode.GetType();
			if (currentCodeNodeType == type || currentCodeNodeType.GetInterface(type.Name) != null ||
			    currentCodeNodeType.IsSubclassOf(type)) notes.Add((T)codeNode);
		}

		return notes;
	}

	public static void GetNodesAllInside<T>(ref List<CodeNode> retList, CodeNode parent,
		bool removeGotFromParent = false) where T : CodeNode
	{
		if (parent.Children.Count == 0) return;

		var copy = new List<CodeNode>();
		copy.AddRange(parent.Children);
		var type = typeof(T);
		for (var i = 0; i < copy.Count; i++)
		{
			var current = copy[i];
			var currentCodeNodeType = current.GetType();
			if (currentCodeNodeType == type || currentCodeNodeType.GetInterface(type.Name) != null ||
			    currentCodeNodeType.IsSubclassOf(type))
			{
				if (removeGotFromParent) parent.Children.Remove(current);

				retList.Add(current);
			}

			GetNodesAllInside<T>(ref retList, current);
		}
	}

	/// <summary>
	///     获取类的祖先列表.最年迈的祖宗排在第0位置
	/// </summary>
	/// <param name="parent"></param>
	/// <param name="childType"></param>
	/// <param name="childName"></param>
	/// <returns></returns>
	public static List<T> FindAncestors<T>(CodeNode parent, Type childType, string childName) where T : IExtendAble
	{
		var found = false;
		var refPath = new List<T>();
		FindAncestorsOfClass(parent, ref found, ref refPath, childType, childName);

		return refPath;
	}

	private static void FindAncestorsOfClass<T>(CodeNode parent, ref bool found, ref List<T> refPath, Type childType,
		string childName) where T : IExtendAble
	{
		if (found) return;
		foreach (var codeNode in parent.Children)
		{
			if (codeNode is IExtendAble == false) continue;
			var type = codeNode.GetType();
			if (type == childType)
			{
				var clsName = (codeNode as IExtendAble)?.Name;
				if (clsName == childName)
				{
					found = true;
					return;
				}
			}

			if (codeNode is T)
			{
				refPath.Add((T)(IExtendAble)codeNode);
				FindAncestorsOfClass(codeNode, ref found, ref refPath, childType, childName);
				if (found) return;
				refPath.RemoveAt(refPath.Count - 1);
			}
		}
	}
}

/// <summary>
///     可以添加修饰符的CodeNode
/// </summary>
public interface IUseModifierCodeNode<T> where T : CodeNode
{
	/// <summary>
	///     可用的修饰符
	/// </summary>
	List<ModifierSegment> AvailableModifiers => ModifierSegments.GetAvailableModifiers<T>();
}

/// <summary>
///     命名的CodeNode,若非匿名则有名字,比如拉姆达表达式等就是匿名的,而函数等就是有名字的
/// </summary>
public interface INamedCodeNode
{
	/// <summary>
	///     名字
	/// </summary>
	string Name { get; set; }
}

/// <summary>
///     有名字的链的CodeNode,比如命名空间,类,接口等
///     NameChain的链的最后一个名字就是这个CodeNode的名字
///     INamedCodeNode的Name也是这个链的最后一个名字
///     NameChain到String的转换是NameChain.ToString(),会得到类似A.B.C的字符串
/// </summary>
public interface INameChainCodeNode : INamedCodeNode
{
	/// <summary>
	///     名字的链
	/// </summary>
	NameChain NameChain { get; set; }
}

/// <summary>
///     具备承载能力里面可以容下子CodeNode的CodeNode
/// </summary>
public abstract class ContainerCodeNode : CodeNode
{
	/// <summary>
	///     可以在这个CodeNode下面直接写的儿子CodeNode的类型,下一层的不算
	///     比如namespace下可以写class enum interface等但是不能写function等
	/// </summary>
	public abstract List<Type> SonCodeNodeValidTypes { get; }
}