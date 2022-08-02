namespace CS2TS;


public class TypeDefine
{
  /// <summary>
  /// 携带泛型等的完整类型名称
  /// </summary>
  public string FullName { get; set; }
  /// <summary>
  /// 该类型的名字
  /// </summary>
  public string Name { get; set; }
  /// <summary>
  /// 该类型是否为泛型类型
  /// </summary>
  public bool IsGeneric { get; set; }
  /// <summary>
  /// 作为泛型时的泛型内参数的个数,比如 List<T> 就是只有一个T Dictioary<string,object> 就是两个.
  /// </summary>
  public List<Parameter> GenericParamTypeList { get; set; }
}
