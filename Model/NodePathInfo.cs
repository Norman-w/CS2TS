namespace CS2TS.Model;

/// <summary>
/// 代码节点的路径信息
/// </summary>
/// <typeparam name="T"></typeparam>
public class NodePathInfo<T> where T:CodeNode
{
  public NodePathInfo()
  {
    Path = new List<string>();
  }
  public List<string> Path { get; set; }
}
