namespace CS2TS;

/// <summary>
/// 参数,用于表示函数的入参 出参
/// </summary>
public class Parameter:CodeNode
{
  // public int Index { get; set; }
  public string Name { get; set; }
  public TypeDefine Type { get; set; }
  public bool IsRef { get; set; }
  public bool IsOut { get; set; }
}
