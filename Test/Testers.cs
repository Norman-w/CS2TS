namespace CS2TS;

public static class Testers
{
  //main
  // public static void Main(string[]? args)
  // {
    // TestCodePath();
  // }
  
  //test
  public static void TestSegmentLocation()
  {
    //计算执行一千万判断路径的计算的时间
    var start = DateTime.Now;
    //做一个大项目可能有一百万行代码,如果一百万行代码中有1000万个计算  测试一下
    for (int i = 0; i < 10000000; i++)
    {
      var method = SegmentLocation.File.Namespace.Class.Method;
      var property = SegmentLocation.File.Namespace.Class.Method.Property;
      var method2 = SegmentLocation.File.Namespace.Class.Method;

      if (method == property)
      {
        // Console.WriteLine("一样的哦");
      }
      else
      {
        // Console.WriteLine("不一样的哦");
      }
      if (method == method2)
      {
        // Console.WriteLine("两个method一样的哦");
      }
      else
      {
        // Console.WriteLine("两个method不一样的哦");
      }
    }
    var end = DateTime.Now;
    var ts = end - start;
    Console.WriteLine("执行一千万次判断路径的时间是:" + ts.TotalMilliseconds + "毫秒");
  }
}
