/*


 implicit:隐式转换
 explicit:显式转换


*/

using System.Globalization;

namespace CS2TS.Test._1_InTestCSFiles;

/// <summary>
///     常量值定义
/// </summary>
public static class Constant
{
	/// <summary>
	///     1美元换多少人民币
	/// </summary>
	public const double DollarToRmb = 6.5;

	/// <summary>
	///     1美元换多少韩元
	/// </summary>
	public const double DollarToKrw = 1100;
}

/// <summary>
///     人
/// </summary>
public class Person
{
	/// <summary>
	///     名字
	/// </summary>
	public string Name { get; set; } = "无名氏";

	/// <summary>
	///     资产,默认以可隐式转换的美元表示,转换成其他货币需要显式转换标明意图
	/// </summary>
	public Dollar Money { get; set; } = new();
}

public class Rmb
{
	public double RmbAmount { get; set; }

	public static implicit operator Dollar(Rmb rmb)
	{
		return new Dollar
		{
			DollarAmount = rmb.RmbAmount / Constant.DollarToRmb
		};
	}

	public static implicit operator Rmb(Dollar dollar)
	{
		return new Rmb
		{
			RmbAmount = dollar.DollarAmount * Constant.DollarToRmb
		};
	}

	public static explicit operator Krw(Rmb rmb)
	{
		//先把rmb转换成dollar,然后再转换成krw
		return (Dollar)rmb;
	}

	public static explicit operator Rmb(Krw krw)
	{
		//先把krw转换成dollar,然后再转换成rmb
		return (Dollar)krw;
	}
}

/// <summary>
///     韩元
/// </summary>
public class Krw
{
	public double KrwAmount { get; set; }

	/*

	 可以直接用美元换韩元
	 如果换成人民币则需要显式转换

    */

	public static implicit operator Dollar(Krw krw)
	{
		return new Dollar
		{
			DollarAmount = krw.KrwAmount / Constant.DollarToKrw
		};
	}

	public static implicit operator Krw(Dollar dollar)
	{
		return new Krw
		{
			KrwAmount = dollar.DollarAmount * Constant.DollarToKrw
		};
	}

	public static explicit operator Rmb(Krw krw)
	{
		//先把krw转换成dollar,然后再转换成rmb
		return (Dollar)krw;
	}

	public static explicit operator Krw(Rmb rmb)
	{
		//先把rmb转换成dollar,然后再转换成krw
		return (Dollar)rmb;
	}
}

/// <summary>
///     美元,作为中间货币,人民币和韩元都可以直接换成美元,但是美元要换成什么,需要显式转换
/// </summary>
public class Dollar
{
	public double DollarAmount { get; set; }

	public override string ToString()
	{
		return DollarAmount.ToString(CultureInfo.InvariantCulture);
	}
}

public class ImplicitAndExplicit
{
	public static void Test()
	{
		#region 小明,美元换韩元和人民币

		var ming = new Person
		{
			Name = "小明",
			Money = new Dollar
			{
				DollarAmount = 1000
			}
		};
		//他想换成韩元的或者人民币的时候,需要显示的转换
		var rmbOfMing = (Rmb)ming.Money;
		Console.WriteLine($"小明有{ming.Money.DollarAmount}美元,换成人民币是{rmbOfMing.RmbAmount}元");
		var krwOfMing = (Krw)ming.Money;
		Console.WriteLine($"小明有{ming.Money.DollarAmount}美元,换成韩元是{krwOfMing.KrwAmount}元");

		#endregion

		#region 小红,人民币换韩元

		Console.WriteLine("小红只有人民币1000元,但是出国换货币的时候,都是央行的汇率,所以她的人民币要先转换成美元,然后再转换成其他货币");
		var rmbOfHong = new Rmb
		{
			RmbAmount = 1000
		};
		var hong = new Person
		{
			Name = "小红",
			// 不需要显示转换,因为Rmb有implicit转换成Dollar
			// Amount = (Dollar)rmbOfHong
			Money = rmbOfHong
		};
		//需要显示转换,因为Dollar没有implicit转换成Rmb
		var krwOfHong = (Krw)hong.Money;
		Console.WriteLine($"小红有{hong.Money.DollarAmount}美元,换成韩元是{krwOfHong.KrwAmount}元");

		#endregion

		#region 小黑,韩元换人民币

		Console.WriteLine("小黑只有韩元1000元,但是出国换货币的时候,都是央行的汇率,所以她的韩元要先转换成美元,然后再转换成其他货币");

		var krwOfHei = new Krw
		{
			KrwAmount = 1000
		};
		var hei = new Person
		{
			Name = "小黑",
			// 不需要显示转换,因为Krw有implicit转换成Dollar
			// Amount = (Dollar)krwOfHei
			Money = krwOfHei
		};

		//需要显示转换,因为Dollar没有implicit转换成Rmb
		var rmbOfHei = (Rmb)hei.Money;
		Console.WriteLine($"小黑有{hei.Money.DollarAmount}美元,换成人民币是{rmbOfHei.RmbAmount}元");

		#endregion

		#region 统一用Dollar来表示资产

		Console.WriteLine($"小明有{ming.Money.DollarAmount}美元");
		Console.WriteLine($"小红有{hong.Money.DollarAmount}美元");
		Console.WriteLine($"小黑有{hei.Money.DollarAmount}美元");

		#endregion

		#region 统一用Rmb来表示资产

		Console.WriteLine($"小明有{((Rmb)ming.Money).RmbAmount}人民币");
		Console.WriteLine($"小红有{((Rmb)hong.Money).RmbAmount}人民币");
		Console.WriteLine($"小黑有{((Rmb)hei.Money).RmbAmount}人民币");

		#endregion

		#region 统一用Krw来表示资产

		Console.WriteLine($"小明有{((Krw)ming.Money).KrwAmount}韩元");
		Console.WriteLine($"小红有{((Krw)hong.Money).KrwAmount}韩元");
		Console.WriteLine($"小黑有{((Krw)hei.Money).KrwAmount}韩元");

		#endregion
	}
}