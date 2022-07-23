# cs file to .ts file converter

(convert c#.net class, interface, namespace, enum, variable and function etc to typescript code file)

一个用于将Csharp/C#.Net编程语言中的cs文件,转换为TypeScripts编程语言中的ts文件的工具
可以转换命名空间,类,接口,枚举,变量,函数等

目前只为从C#开发的后端程序中导出SDK中的Domain和Request & Response
方便在TS中使用.

后期若时间充裕且确有重要使用场景时,会加入双向转换和代码全量转换(包括逻辑部分)

## **How to use**

如何使用

build this project you'll got CS2TS.exe

run CS2TS.exe with parameters at cmd like blow

>cs2ts.exe c:\test\domain.cs d:\tsFilesOut\

you may got the ts file named domain.ts

as you know the first parameter is the .cs file source full path
and the second is the .ts file out going path.

第一个参数是cs文件,第二个参数是生成的ts文件的保存位置.生成的文件名和源文件名相同
