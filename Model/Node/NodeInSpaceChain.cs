/*


 领空链中的一个节点.
 记录CodeNode对象
 潜在对象集合,可尝试吃掉一个Segment,能用到哪个(或多个)潜在对象上则应用上
 如果应用不上,潜在对象集合中则移除那个对象(CodeNode)
 直到最后剩一个潜在对象的时候,潜在对象则被确认为确切类型的领空链上节点类型.

 比如,在namespace中,namespace中可容纳的类型有 class, enum, interface等等
 当我们输入public这个修饰符Segment的时候,根据namespace中可容纳类型的集合以及public可以修饰的类型
 则可以缩小 "由namespace可容纳类型来创建的潜在对象类型集合" 的范围


*/


using CS2TS.Model.Words;

namespace CS2TS.Model.Node;

/// <summary>
///     领空链中的一个节点.
/// </summary>
public partial class NodeInSpaceChain
{
	public NodeInSpaceChain(CodeNode? confirmedCodeNode, NodeInSpaceChain? parent)
	{
		ConfirmedCodeNode = confirmedCodeNode;
		Parent = parent;
	}

	/// <summary>
	///     是否是一个已确认类型的节点
	/// </summary>
	public bool IsANodeTypeConfirmedNode => ConfirmedCodeNode != null;

	/// <summary>
	///     父节点
	/// </summary>
	public NodeInSpaceChain? Parent { get; set; }

	/// <summary>
	///     确认的CodeNode
	/// </summary>
	public CodeNode? ConfirmedCodeNode { get; set; }

	/// <summary>
	///     潜在的可能的CodeNode集合,当确认的CodeNode不为空时,此集合为空
	/// </summary>
	public List<CodeNode> PotentialCodeNodes { get; set; } = new();
}

/// <summary>
///     输入Segment到领空链中的节点的结果
/// </summary>
public record InputSegmentToNodeInSpaceChainResult
{
	/// <summary>
	///     是否成功
	/// </summary>
	public bool Success { get; init; }

	/// <summary>
	///     触发的新领地划界状态枚举,比如往当前namespace A后面输入了一个;以后,则触发了新的领地划界状态的Escape状态
	/// </summary>
	public TerritoryDemarcationStatusFlagEnum? NewTerritoryDemarcationStatusFlag { get; init; }
}

public partial class NodeInSpaceChain
{
	/// <summary>
	///     获取当前节点的名称(领空链的完整名称)
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return GetFullName();
	}

	/// <summary>
	///     递归parent获取CodeNodeType的名称
	///     格式如: Namespace.Class
	/// </summary>
	/// <returns></returns>
	private string GetFullName()
	{
		var name = ConfirmedCodeNode?.GetType().Name;
		if (Parent != null) name = Parent.GetFullName() + "." + name;
		return name ?? string.Empty;
	}

	/// <summary>
	///     输入一个Segment,尝试应用到潜在对象上
	/// </summary>
	/// <param name="segment"></param>
	/// <returns></returns>
	public InputSegmentToNodeInSpaceChainResult Input(Segment segment)
	{
		switch (segment)
		{
			case SymbolSegment symbolSegment
				when
				ConfirmedCodeNode != null &&
				symbolSegment.CanFinishCodeNodeBodyTypes.Contains(ConfirmedCodeNode.GetType()):
			{
				//如果成功当前Segment处理了,那么就跳过后面的代码,直接处理下一个Segment
				return new InputSegmentToNodeInSpaceChainResult
				{
					Success = true, NewTerritoryDemarcationStatusFlag = TerritoryDemarcationStatusFlagEnum.Escape
				};
				//continue后不需要break了.直接返回到了foreach层继续处理下一个Segment
			}
			case SymbolSegment symbolSegment
				when
				ConfirmedCodeNode != null &&
				symbolSegment.CanStartCodeNodeBodyTypes.Contains(ConfirmedCodeNode.GetType()):
			{
				return new InputSegmentToNodeInSpaceChainResult
					{ Success = true, NewTerritoryDemarcationStatusFlag = TerritoryDemarcationStatusFlagEnum.IntoUse };
			}
			case ModifierSegment modifierSegment:
				HandleModifierSegment(modifierSegment);
				return new InputSegmentToNodeInSpaceChainResult { Success = true };
			case CodeNodeTypeSegment codeNodeTypeSegment:
				HandleCodeNodeTypeSegment(codeNodeTypeSegment);
				return new InputSegmentToNodeInSpaceChainResult { Success = true };
			default:
				throw new Exception("未知的Segment类型");
		}
	}

	private void HandleModifierSegment(ModifierSegment modifierSegment)
	{
		if (IsANodeTypeConfirmedNode)
		{
			// 已经确认CodeNode类型了可能就是要输入名称了 比如public static class A
			// 已经确认了CodeNode类型是Class,那么接下来就是输入名称了
		}
		else
		{
			//爹爹是不是一个容器类型,如果是的话,里面能容纳啥类型
			var parentContainerSonCodeNodeValidTypes =
				Parent?.ConfirmedCodeNode is ContainerCodeNode containerCodeNode
					? containerCodeNode.SonCodeNodeValidTypes
					: new List<Type>();
			//潜在类型中添加所有这些类型的实例
			foreach (
				var codeNode
				in parentContainerSonCodeNodeValidTypes.Select
				(
					parentContainerSonCodeNodeValidType =>
						(CodeNode)Activator.CreateInstance(parentContainerSonCodeNodeValidType)!
				)
			)
				PotentialCodeNodes.Add(codeNode);
		}

		//遍历每一个潜在对象,尝试应用segment
		foreach (var potentialCodeNode in PotentialCodeNodes)
			if (potentialCodeNode is IUseModifierCodeNode useModifierCodeNode)
				//如果能应用上,则确认这个CodeNode
				if (useModifierCodeNode.ModifierSegment == modifierSegment)
					if (useModifierCodeNode.ApplyModifier(modifierSegment) == false)
						throw new Exception("应用Modifier失败");
	}

	private void HandleCodeNodeTypeSegment(CodeNodeTypeSegment codeNodeTypeSegment)
	{
		//因为一定是一个有效的类型,所以直接确认,其他的潜在对象全部移除
		foreach (var paPotentialCodeNode in PotentialCodeNodes.Where
		         (
			         paPotentialCodeNode => paPotentialCodeNode.GetType() == codeNodeTypeSegment.CodeNodeType
		         )
		        )
		{
			ConfirmedCodeNode = paPotentialCodeNode;
			PotentialCodeNodes.Clear();
			return;
		}
	}
}

public partial class NodeInSpaceChain
{
	/// <summary>
	///     获取CodeFile根节点
	/// </summary>
	public static NodeInSpaceChain RootCodeFile => new(new CodeFile(), null);

	/// <summary>
	///     向上退一步
	/// </summary>
	/// <returns></returns>
	public NodeInSpaceChain? Back()
	{
		return Parent;
	}
}

/// <summary>
///     领空链游标指示器
/// </summary>
public class SpaceChainIndicator
{
	private readonly List<NodeInSpaceChain> _chain = new();

	public SpaceChainIndicator(NodeInSpaceChain root)
	{
		_chain.Add(root);
	}

	/// <summary>
	///     当前的领空链
	/// </summary>
	public NodeInSpaceChain Current => _chain[^1];

	/// <summary>
	///     root节点
	/// </summary>
	public NodeInSpaceChain Root => _chain[0];

	/// <summary>
	///     向上退一步,没有向下走一步,因为退回去以后,可能会有多个分支
	/// </summary>
	/// <returns></returns>
	public NodeInSpaceChain? GoBack()
	{
		if (_chain.Count == 1) return null;
		_chain.RemoveAt(_chain.Count - 1);
		return _chain[^1];
	}

	/// <summary>
	///     在当前节点下新建一个分叉的节点
	///     比如在namespace领空下调用这个函数,然后对接点进行一步一步的确认类型.
	/// </summary>
	/// <returns></returns>
	public NodeInSpaceChain NewBranch()
	{
		var newNode = new NodeInSpaceChain(null, Current);
		_chain.Add(newNode);
		return newNode;
	}
}