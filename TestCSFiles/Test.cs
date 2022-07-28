using bb;
using cc;
//这里是注释区域,里面我用了{还有/还有;还有其他的 之类的东西./*
///*注释区里面还可以有注释///////所以如果//或者/**/不再行的开头,都不是新注释行
using
System;
using System.Collections.Generic;
using System.Text;
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
    public class aa: a, iii
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
          }
        }
      }
    }
}
