namespace CS2TS;


public enum OperatorType
{
  UNKNOW,
  /// <summary>
  /// +加
  /// </summary>
  PLUS,
  PLUS_EQUAL,
  /// <summary>
  /// -减
  /// </summary>
  REDUCE,
  REDUCE_EQUAL,
  /// <summary>
  /// *乘
  /// </summary>
  MULTIPLY,
  MULTIPLY_EQUAL,
  /// <summary>
  /// ÷除
  /// </summary>
  DEVIDE,
  REVIDE_EQUAL,
  /// <summary>
  /// %取模运算
  /// </summary>
  MODULO,
  /// <summary>
  /// =等于
  /// </summary>
  EUQAL,
  LOGICAL_EQUAL,
  /// <summary>
  /// ||逻辑或
  /// </summary>
  LOGICAL_OR,
  /// <summary>
  /// &&逻辑与
  /// </summary>
  LOGICAL_AND,
  /// <summary>
  /// !逻辑非
  /// </summary>
  LOGICAL_NON,
  /// <summary>
  /// is 判断什么是什么
  /// </summary>
  IS,
  /// <summary>
  /// |或运算
  /// </summary>
  OR,
  /// <summary>
  /// &与运算
  /// </summary>
  AND,
  /// <summary>
  /// ^异或运算
  /// </summary>
  XOR,
  /// <summary>
  /// ~按位取反
  /// </summary>
  BITWISE_COMPLEMENT,
}
/// <summary>
/// 操作符
/// </summary>
public class Operator
{
  public Operator(Nullable<OperatorType> type)
  {
    Type = type == null ? OperatorType.UNKNOW : type.Value;
  }
  /// <summary>
  /// 符号的类型
  /// </summary>
  public OperatorType Type { get; set; }
}
