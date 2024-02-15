using System.Text;
using CS2TS.Model.Node;

namespace CS2TS;

/// <summary>
/// 注释内容 有 注释行 注释区域  #注释等 里面不会再有其他子元素 都是字符 他本身就是注释 所以也不需要注释加在这个上面
/// </summary>
public class NoteBase : CodeNode
{
}
public class NotesLine :NoteBase
{
  public NotesLine()
  {
    //this.Content = new StringBuilder();
  }
  public string Content
  {
    get
    {
      return sb.ToString();
    }
    set { }
  }
  StringBuilder sb = new StringBuilder();
  public void Append(string text)
  {
    sb.Append(text);
  }
  public void AppendFormat(string text, params object[] args)
  {
    sb.AppendFormat(text, args);
  }
}
public class NotesArea: NoteBase
{
  public List<string> Lines = new List<string>();
}
public class SharpLine : NotesLine
{
  //public SharpLine()
  //{
  //    this.Content = new StringBuilder();
  //}
  //public StringBuilder Content { get; set; }
}
