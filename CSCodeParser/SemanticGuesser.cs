namespace CS2TS;
/*
 * 语义的猜测器.根据当前的位置和当前的词汇,猜测是正在输入什么内容
 * 
 */

/*
 * 直接返回一个变量
 * return a;
 * return   return start
 * a        variable use
 * ;        finish statement
 *
 *
 * 返回一个由变量0确认的变量a中的元素
 * return a[0];
 * return   return start
 * a        variable use
 * [        start select elem/enter variable
 * 0        variable
 * ]        end select elem/quit variable
 * ;        finish statement
 *
 *
 * 多重使用
 * return a[a.Length]
 * return   return start
 * a        variable use
 * [        start select elem/enter variable
 * a        variable use
 * .        enter variable
 * length   variable use
 * ]        end select elem/quit variable
 * ;        finish statement
 *
 *
 * return a[1-1]
 * return   return start
 * a        variable use
 * [        start select elem/enter variable
 * 1        const variable define
 * -        operator
 * 1        const variable define
 * ]        end select elem/quit variable
 * ;        finish statement
 *
 *
 * 
 */

public enum TypesOfStatementsWithinFunctions
{
    /// <summary>
    /// 返回语句
    /// </summary>
    Return,
    /// <summary>
    /// 变量使用
    /// </summary>
    VariableUse,
    /// <summary>
    /// 开始选择子元素的[括号
    /// </summary>
    StartSelectElem,
    /// <summary>
    /// 定义常量,定义并使用常量
    /// </summary>
    ConstVariableDefine,
    /// <summary>
    /// 操作符
    /// </summary>
    Operator,
    /// <summary>
    /// 进入变量中去,就是 . 符号 或者直接叫Enter,这样的话 [ 括号也可以叫进入
    /// </summary>
    EnterVariable,
    /// <summary>
    /// 结束选择子元素的]括号
    /// </summary>
    EndSelectElem,
    /// <summary>
    /// 这个可以没有
    /// </summary>
    FinishStatement
}
public class SemanticGuesser
{
    public static List<Statement> Guess(Function function, string word)
    {
        // var ret = new List<Statement>();
        // if (word == "return")
        // {
        //     ret.Add(new ReturnStatement(){Value = new VariableNoStructure()});
        // }
        //
        // return ret;
        return null;
    }
}