namespace CS2TS;

/// <summary>
/// TS文件生成器的配置
/// </summary>
public class TypescriptGeneratorConfig
{
  public TypescriptGeneratorConfig()
  {
    ConvertClass2Interface = false;
    SetDefaultVariableValueForClass = false;
  }
  /// <summary>
  /// 是否把类转换为TypeScripts的接口
  /// </summary>
  public bool ConvertClass2Interface { get; set; }

  /// <summary>
  /// 为类中的变量(字段/属性)自动设置默认值
  /// </summary>
  public bool SetDefaultVariableValueForClass { get; set; }
}
