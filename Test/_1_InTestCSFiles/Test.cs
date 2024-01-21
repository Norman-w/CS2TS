//		上来就是注释啊,这前面好几个空格
/* 这是注释区间啊
 */

using cc;
using cc.dd.ee.ff;

//这里是注释区域,里面我用了{还有/还有;还有其他的 之类的东西./*
///*注释区里面还可以有注释///////所以如果//或者/**/不再行的开头,都不是新注释行

/*
 * 这个一个注释段,命名空间没有summary
 */
namespace
	aa
{
	//这是普通的行注释
	/// <summary>
	///     这是一个类
	/// </summary>
	public class a
	{
		protected int AA;

		public class aSub
		{
			public class aSubSub
			{
				public bool Finded { get; set; }
			}
		}
	}

	# region 这是一个区域

	///**/
#if debug ///dddd
    //////111
#endif
	public enum EnumText
	{
		E1,
		E2 = 3,

		//这里应该等于4
		E4,
		E5
	}

	public enum EnumTest2 : long
	{
		EL = 111,
		EW = 111111111111111
	}

	#endregion

	//命名空间内不能有属性
	public class aa : a, fii
	{
		private const int ct = 0;
		private aa aobj;
		public static string str { get; set; }
		public int MyProperty { get; set; }
		protected new int AA { get; set; }

		public void fiiFunc(byte b, byte b2, byte b3)
		{
			throw new NotImplementedException();
		}

		public void fiiFunc(int a, int b, int c)
		{
			throw new NotImplementedException();
		}

		public void fiiFunc(string str)
		{
		}

		public string fiiFunc()
		{
			return "this is fii func no param and return string";
		}

		/// <summary>
		///     这是一个函数
		/// </summary>
		/// <param name="a">函数的参数1</param>
		/// <param name="b">函数的参数2</param>
		/// <returns>返回结果</returns>
		public static int sum(int a, int b)
		{
			return a + b;
		}

		public void voidFunc(int a, int b)
		{
			int? c = 1;
			c ??= 0;
			c >>= 1;
			c <<= 2;
			Console.WriteLine(a + b % c);
		}

		/// <summary>
		///     遍历每一个to内的元素,如果from有同名的,替换to内的值
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public Dictionary<string, string> replace(Dictionary<string, Dictionary<string, int>> from,
			Dictionary<string, Dictionary<string, int>> to)
		{
			var ret = new Dictionary<string, string>();
			foreach (var t in to)
				if (from.ContainsKey(t.Key))
					to[t.Key] = from[t.Key];
			return ret;
		}

		private long?
			format(int a, int b)
		{
			if (a == 0
			    || b ==
			    0
			   )
				return null;
			return null;
		}

		public void fiiFunc(byte b)
		{
			throw new NotImplementedException();
		}

		//这个方法不是从接口中继承而来，所以他的修饰符可以是private的。如果该方法是从接口继承，那么他必须是public的
		private string fiiFunc(bool bl)
		{
			return null;
		}

		public class bClass
		{
		}
	}
}

namespace bb
{
	internal interface iii : ii
	{
		void voidFunc(int a, int b);

		interface iiii4
		{
			public string name { get; set; }
		}
	}
}

namespace cc
{
	internal interface ii
	{
	}

	namespace dd
	{
		namespace ee
		{
			namespace ff
			{
				internal interface fii
				{
					void fiiFunc(byte b, byte b2, byte b3);

					void fiiFunc(int a, int b, int c);
					void fiiFunc(string str);

					string fiiFunc();
				}
			}
		}
	}
}