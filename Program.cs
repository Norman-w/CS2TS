using Newtonsoft.Json;

namespace CS2TS
{
  static class Program
  {
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    static void Main(string[]? args)
    {
      Parser ps = new Parser();
      var testFile = @"/Users/coolking/Downloads/TestCsFiles23/Domain/Bag/BagItemsAlias.cs";
      if (args != null && args.Length>0)
      {
        testFile = args[0];
      }
      var codeFile = ps.ParseCsFile(testFile);
      var json = JsonConvert.SerializeObject(codeFile, Formatting.Indented);
      Console.WriteLine(json);
    }
  }
}
