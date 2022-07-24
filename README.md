# cs file to .ts file converter
Tiny, not based on Roslyn or any other library,just parse code file characters one by one.

(convert c#.net class, interface, namespace, enum, variable and function etc to typescript code file)


一个用于将Csharp/C#.Net编程语言中的cs文件,转换为TypeScripts编程语言中的ts文件的工具
可以转换命名空间,类,接口,枚举,变量,函数等

事实上我们可以通过很多种方式解析CS文件,但他们都没有办法读取和使用[Summary]注释以及[Region]区块标记,还有//, /**/ 这些,然而这些备注或者注释非常有用,尤其是对于非英语母语的编程者们.
这是我写这个项目的最根本理由.

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

### 2022年07月24日11:33:02
Since "#region" may span regions, it's great for collapsing code most of the time, but worse when dealing with non-standard CS code (or messing with "#region")
因为 "#region" 可能会跨越区域,他多数时候用来折叠代码很棒,但如果处理不规范的cs代码(或者乱用"#region"),会比较糟糕

