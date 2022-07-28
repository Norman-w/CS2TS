using System.Text;
using CS2TS.Model;

namespace CS2TS;

/// <summary>
/// 代码结构中的节点信息.每一个节点下面都会包含子节点的信息表.以及注释信息.
/// </summary>
public class CodeNode
{
  // public string Name { get; set; }
  public CodeNode()
  {
    Chirldren = new List<CodeNode>();
  }
  /// <summary>
  /// 相对于父元素的位置
  /// </summary>
  public int Index { get; set; }


  /// <summary>
  /// 代码内容
  /// </summary>
  public string CodeBody{ get; set; }

  public List<CodeNode> Chirldren { get; private set; }

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

  protected List<T> GetNodes<T>() where T:CodeNode
  {
    List<T> notes = new List<T>();
    var type = typeof(T);
    foreach (var codeNode in Chirldren)
    {
      var currentCodeNodeType = codeNode.GetType();
      if (currentCodeNodeType == type || currentCodeNodeType.GetInterface(type.Name)!= null || currentCodeNodeType.IsSubclassOf(type))
      {
        notes.Add((T)codeNode);
      }
    }
    return notes;
  }

  public static void GetNodesAllInside<T>(ref List<CodeNode> retList, CodeNode parent, bool removeGotFromParent = false) where T : CodeNode
  {
    if (parent.Chirldren == null || parent.Chirldren.Count == 0)
    {
      return;
    }

    var copy = new List<CodeNode>();
    copy.AddRange(parent.Chirldren);
    var type = typeof(T);
    for (int i = 0; i < copy.Count; i++)
    {
      var current = copy[i];
      var currentCodeNodeType = current.GetType();
      if (currentCodeNodeType == type || currentCodeNodeType.GetInterface(type.Name)!= null || currentCodeNodeType.IsSubclassOf(type))
      {
        if (removeGotFromParent)
        {
          parent.Chirldren.Remove(current);
        }

        retList.Add(current);
      }
      GetNodesAllInside<T>(ref retList, current);
    }
  }

  /// <summary>
  /// 获取类的祖先列表.最年迈的祖宗排在第0位置
  /// </summary>
  /// <param name="parent"></param>
  /// <param name="childType"></param>
  /// <param name="childName"></param>
  /// <returns></returns>
  public static List<IClassContainer> FindAncestors(CodeNode parent, Type childType, string childName)
  {
    bool found = false;
    List<IClassContainer> refPath = new List<IClassContainer>();
    findAncestorsOfClass(parent,ref found, ref refPath, childType, childName);

    return refPath;
  }

  private static void findAncestorsOfClass(CodeNode parent,ref bool found, ref List<IClassContainer> refPath, Type childType, string childName)
  {
    if (found)
    {
      return;
    }
    foreach (var codeNode in parent.Chirldren)
    {
      var type = codeNode.GetType();
      if (type == childType)
      {
        var clsName = (codeNode as IClassContainer).Name;
        if (clsName == childName)
        {
          found = true;
          return;
        }
      }
      if (codeNode is IClassContainer)
      {
        refPath.Add((codeNode as IClassContainer));
        findAncestorsOfClass(codeNode, ref found, ref refPath, childType,childName);
        if (found)
        {
          return;
        }
        refPath.RemoveAt(refPath.Count-1);
      }
    }
  }
}
