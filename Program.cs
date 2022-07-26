﻿// using CS2TS.Helper;
//
// using System.Text;
// using CS2TS.Model;
// using Newtonsoft.Json;

namespace CS2TS
{
  static class Program
  {
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    // [STAThread]
    static void Main(string[]? args)
    {
      CSharpCodeParser ps = new CSharpCodeParser();
      // var testFile = @"/Users/coolking/Downloads/TestCsFiles23/Domain/Bag/BagItemsAlias.cs";
      // var testFile = @"/Volumes/NormanData/Visual Studio 2008/Projects/速配项目/QP/Domain/Account/AccountGroup.cs";
      // var testFile = @"../../../TestCSFiles/Test.cs";
      var testFile = @"../../../TestCSFiles/TestCalc.cs";
      if (args != null && args.Length>0)
      {
        testFile = args[0];
      }
      var codeFile = ps.ParseCsFile(testFile);
      // var json = JsonConvert.SerializeObject(codeFile, Formatting.Indented);
      // Console.WriteLine(json);

      // CodeNodeHelper hp = new CodeNodeHelper(codeFile);
      // hp.FindClass(codeFile, "aSubSub");

      return;
      var generator = new TypeScriptCodeGenerator(codeFile);
      var tsCode = generator.CreateTsFile();
      Console.WriteLine(tsCode);
    }
  }
}
