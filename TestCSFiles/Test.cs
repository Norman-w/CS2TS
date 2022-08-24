using bb;
using cc;
//这里是注释区域,里面我用了{还有/还有;还有其他的 之类的东西./*
///*注释区里面还可以有注释///////所以如果//或者/**/不再行的开头,都不是新注释行
using
System;
using System.Collections.Generic;
using System.Text;
using cc.dd.ee.ff;
using CS2TS;

/*
 * 这个一个注释段,命名空间没有summary
 */
namespace
    aa
{
    //这是普通的行注释
    /// <summary>
    /// 这是一个类
    /// </summary>
    public class a
    {
      public class aSub
      {
        public class aSubSub
        {
          public bool Finded { get; set; }
        }
      }
        protected int AA;}
    # region 这是一个区域///**/
#if debug///dddd
    //////111
#endif
    public enum EnumText
    {
        E1,E2 = 3,
        //这里应该等于4
        E4,
        E5
    }
    public enum EnumTest2:long
    {
        EL=111,
        EW = 111111111111111,
    }
    #endregion
    //命名空间内不能有属性
    public class aa: a, fii
    {
      public class bClass
      {

      }
        aa aobj;
        const int ct = 0;
        public static string str { get; set; }
        public int MyProperty { get; set; }
        protected new int AA { get; set; }
        /// <summary>
        /// 这是一个函数
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
            Console.WriteLine(a+b);
        }

        /// <summary>
        /// 遍历每一个to内的元素,如果from有同名的,替换to内的值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public Dictionary<string, string> replace(Dictionary<string, Dictionary<string,int>> from, Dictionary<string, Dictionary<string,int>> to)
        {
          var ret = new Dictionary<string, string>();
          foreach (var t in to)
          {
            if (from.ContainsKey(t.Key))
            {
              to[t.Key] = from[t.Key];
            }
          }
          return ret;
        }
        Nullable
            < long >
            format(int a, int b)
        {
            if (a== 0
                || b ==
                0
                )
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        public void fiiFunc(byte b)
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

        private string fiiFunc(bool bl)
        {
            return null;
        }
    }
}
namespace bb
{
    interface iii:ii
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
    interface ii
    {

    }

    namespace  dd
    {
      namespace ee
      {
        namespace ff
        {
          interface fii
          {
            void fiiFunc(byte b);
            void fiiFunc(string str);

            string fiiFunc();
          }
        }
      }
    }
}
