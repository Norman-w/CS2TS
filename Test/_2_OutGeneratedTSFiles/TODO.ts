// using CS2TS.Helper;
//
// using System.Text;
// using CS2TS.Model;
// using Newtonsoft.Json;
class CSharpCodeParser
{
    ParseCsFile:Function
}
class TypeScriptCodeGenerator
{
    constructor(obj:any) {
    }
    CreateTsFile:Function
}
export namespace CS2TS {
    export class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        // [STAThread]
        private static Main(args?: string[]) :void {
            var ps:CSharpCodeParser = new CSharpCodeParser();
            var testFile = "../../../Program.cs";
            if (args != null && args.length>0)
            {
                testFile = args[0];
            }
            var codeFile = ps.ParseCsFile(testFile);
            var generator = new TypeScriptCodeGenerator(codeFile);
            var tsCode = generator.CreateTsFile();
            console.log(tsCode);
        }
        public Range(src:number[], start:number, end:number):number[]
        {
            var srcLength = src.length;
            if (start<srcLength-1 || end>srcLength-1)
            { 
                throw ("out of range");
            }
            var ret:Array<number> = new Array<number>();
            while (start <= end)
            {
                ret.push(src[start++]);
                // start++;
            }
            return ret;
    }
    }
}
