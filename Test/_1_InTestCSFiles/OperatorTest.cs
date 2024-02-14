namespace CS2TS.Test._1_InTestCSFiles;

public class OperatorTest
{
	/// <summary>
	///     测试
	/// </summary>
	public static void Test()
	{
		var dollar = new Dollar { Value = 100 };
		var rmb = new Rmb { Value = 650 };
		var result = dollar + rmb;
		Console.WriteLine("result: " + result.Value);
	}

	/// <summary>
	///     常量
	/// </summary>
	private static class Constant
	{
		public const float DollarToRmb = 6.5f;
	}

	/// <summary>
	///     人民币
	/// </summary>
	public class Rmb
	{
		public float Value { get; set; }
	}

	/// <summary>
	///     美元
	/// </summary>
	public class Dollar
	{
		/// <summary>
		///     数值
		/// </summary>
		public float Value { get; set; }

		/// <summary>
		///     加号,加上人民币则将人民币换算成美元
		/// </summary>
		/// <param name="dollar"></param>
		/// <param name="rmb"></param>
		/// <returns></returns>
		public static Dollar operator +(Dollar dollar, Rmb rmb)
		{
			return new Dollar { Value = dollar.Value + rmb.Value / Constant.DollarToRmb };
		}

		/// <summary>
		///     加号,加上人民币则将人民币(int类型)换算成美元
		///     实际使用一般都不会这样用,都是传入另一种类型的对象而不是系统类型int之类的.
		///     这样会有一个明确的类型转换,而不是模糊的int类型(虽然形参的名称是rmb,但容易把美元当成人民币的int类型)
		/// </summary>
		/// <param name="dollar"></param>
		/// <param name="rmb"></param>
		/// <returns></returns>
		[Obsolete("这个方法不建议使用,因为传入的是int类型,而不是Rmb类型")]
		public static Dollar operator +(Dollar dollar, int rmb)
		{
			return new Dollar { Value = dollar.Value + rmb / Constant.DollarToRmb };
		}
	}
}