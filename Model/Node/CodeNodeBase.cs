using CS2TS.Model;

namespace CS2TS;

/// <summary>
/// 代码结构中的节点信息.每一个节点下面都会包含子节点的信息表.以及注释信息.
/// </summary>
public class CodeNode
{
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
}
