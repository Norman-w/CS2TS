# cs file to .ts file converter

Tiny, not based on Roslyn or any other library,just parse code file characters one by one.

(convert c#.net class, interface, namespace, enum, variable and function etc to typescript code file)

一个用于将Csharp/C#.Net编程语言中的cs文件,转换为TypeScripts编程语言中的ts文件的工具
可以转换命名空间,类,接口,枚举,变量,函数等

事实上我们可以通过很多种方式解析CS文件,但他们都没有办法读取和使用[Summary]注释以及[Region]区块标记,还有//, /**/
这些,然而这些备注或者注释非常有用,尤其是对于非英语母语的编程者们.
这是我写这个项目的最根本理由.

目前只为从C#开发的后端程序中导出SDK中的Domain和Request & Response
方便在TS中使用.

后期若时间充裕且确有重要使用场景时,会加入双向转换和代码全量转换(包括逻辑部分)

## **How to use**

如何使用

build this project you'll got CS2TS.exe

run CS2TS.exe with parameters at cmd like blow

> cs2ts.exe c:\test\domain.cs d:\tsFilesOut\

you may got the ts file named domain.ts

as you know the first parameter is the .cs file source full path
and the second is the .ts file out going path.

第一个参数是cs文件,第二个参数是生成的ts文件的保存位置.生成的文件名和源文件名相同

### 2022年07月24日11:33:02

Since "#region" may span regions, it's great for collapsing code most of the time, but worse when dealing with
non-standard CS code (or messing with "#region")
因为 "#region" 可能会跨越区域,他多数时候用来折叠代码很棒,但如果处理不规范的cs代码(或者乱用"#region"),会比较糟糕

### 2022年07月28日12:14:39

目前
使用 \TestCSFiles\Test.cs作为csharp待转换源码
使用 \GeneratedTSFiles\WIP.ts作为typescript的已生成代码.
namespace, enum, class, interface, using, variable 都已处理完毕.剩余Function生成部分未处理

## 2023年07月26日22:46:35 有了新的灵感

##### 是否可以逐个字符的处理,而不是只判断起始符号和结束符号,根据当前领空,当前正在处理的字符,来确定当前字符可能是什么的开始,可能是什么的结束

例如,当前领空是namespace,
...好吧 当我过去看了之前写的代码 好像本来就是这么处理的.我刚刚有这个突发奇想的时候只是因为觉得下次遇到 [
这个符号的时候就知道当前领空中可能 这个 [ 或者是某个单词可能是什么意图了.

不过是可以考虑一个问题的,就是 一个类型的元素 需要有哪几个关键词来确定他的类型,例如

[Attribute] 的时候,他是一个Attribute,确认方式是他的前面是不是有[, 结束是不是有], 中间是不是有Attribute等
哦 这个逻辑好像之前也有的 因为我们里面其实有一些 Is 什么什么的方法

## 2023年07月30日11:00:27 做了一个新的功能 WIP

##### 在SegmentLocation.cs中定义了用于表述节点路径的静态类.

详细使用和测试见Testers.cs中的TestSegmentLocation()方法

## 2024年01月05日23:22:18 csCodeViewer的Flutter项目准备和本项目连接,以接收本项目推送过去的显示输出

一个示例的基本WebSocat包内容

```json
{
  "Initiator":1,
  "ApiName": "test", 
  "RequestId": "11111", 
  "Data": "this is empty data"
}
```

这里的Data就相当于是body,然后这个Data可以解析成一个Request.
Initiator是一个int,用于标识是谁发起的请求,比如0是从WebSocat客户端发出,也就是从flutter发出,1就是从服务端发出,也就是本项目的进程

Request由本项目来定义.flutter项目中解析.

# WIKI

### 单词及符号

比如语句 using namespace xxx;
其中集合中包含 ```["using","namespace","xxx",";"]```
有的符号可能还是多个char的组合,比如  */ 是注释的结束符号 \r\n 是换行的符号

2024年01月08日18:02:53 添加了Utils里面的将静态类序列化为json的方法
同时添加了对WebSocket的支持

2024年01月18日22:20:52 基本完成了断语义最小单元,也就是分char为char组合出来单词或者符号等.
也完成了向前粘连,比如/向前粘连一个/就变成了// 也就是Segments中的静态AnnotationLineSymbol.
完成了较为明显的输出方法.可尝试使用l命令读取文件,然后sa,获取Segments all, 然后使用 rai, remove all invisible, 然后使用
tma , test/try merge all.
这样就能把该粘连到一块的都粘连上了而且不会有多余的空格(不可见字符和换行符等.)
再次重复一次测试顺序.
(ctrl+d)/(F5),l, sa, rai, tma试试看效果吧.

2024-01-22 17:11:28 已经可以通过CS2TS的命令 l, sa, pa,
把segments发送到CsCodeViewer且CsCodeViewer正确接收了.

2024年01月23日21:55:21一个武断的想法:
C#中的那种三个字符串的组成,一定不会是由1个直接跳到3个而中间没有跨度的.
比如,先用了 ?,然后用了?? 然后用了??=
相信后面不会有那种之前用了~然后没有~~ 直接就发明了一个 ~~~ 或者 ~~=的情况.
是因为字符越多越麻烦,越容易错,越不好理解肯定可字符少的来做发明.

所以,我们当前的MergeBackwards方法不用考虑那种特殊情况(目前还没发现的情况)
只要遇到?看前面有没有?,如果有组成??,然后遇到=看前面是不是?? 如果是就组成 ??=.

但是也说不准哈 没准以后就来了个 ??== 或者 ??++ 之类的.

所以,到底要不要做? 先不做,因为如果真出现这种情况,比如

!!!= 我们可以识别!可以识别=然后!!=不可以识别!!!=却可以识别
我们只需要添加一个!!=作为中间量就可以了,但是这个标识符的语义我们可以设置为中间量不具备实际意义,只是为了让我们的识别器能够识别出来.

### 2024年01月27日10:06:52现在已经可以正确的解析每一个语义的长度,起始位等信息了,对成对出现的括号也有了一些改进

服务端发送的csCodeString和SegmentsList都能正确的在前端显示,不会有多余的csCodeString未解析部分在前端显示出来了.

### 2024年02月05日13:49:54 准备开始分析最小语义单元如何组成代码结构树的了

首先一个idea是,通过当前语义 比如 "public" 以及当前所在的领空,判断这个public可能是什么意思,比如可能是一个类的修饰符,可能是一个函数的修饰符,可能是一个变量的修饰符等等.

另外一个idea是,像public这种语义单元,可以修饰哪些东西,比如public可以修饰类,函数,变量等等.

那么不管是哪个idea,都需要把这些关键词都实例化,有固定的类来保存他们.所以这个时候我们就需要明确Segment不再是之前的那些符号之类的了.也可以是一个关键词.
所以我决定把Segment定义为基类,在此基础上会有符号Segment,关键词Segment,修饰符Segment等等.
符号Segment命名为SymbolSegment,关键词Segment命名为KeywordSegment,修饰符Segment命名为ModifierSegment.
把原来的Segment修改成为基类,原来那些符号加减乘除之类的都改成继承自Segment的SymbolSegment.
然后新增KeywordSegment,ModifierSegment等等.
2024年02月10日13:56:32,对KeywordSegment,ModifierSegment的定义比较模糊
但是有了TypeSegments和PropertySegments的定义,TypeSegments是用来定义类型的,PropertySegments是用来定义属性的.
比如namespace, class, interface, enum, struct, delegate, event, var, record这都是TypeSegments
而public, private, protected, internal, static, readonly, const, virtual, override, sealed, abstract, extern,
partial这些都是PropertySegments

另外,还有一个明确的概念是:

### 任何Thing都有定义和使用.比如 public class A{} 这里的class A就是一个定义,而A a = new A(); 这里的A就是一个使用.

所以对于一些Segment,我们需要明确他们是定义还是使用,比如TypeSegments是定义,PropertySegments是定义的修饰符,而VariableSegments是使用的.
还可以定义UsingSegments为使用的,具体的还需要整理一下.

### 关于定义,使用

定义:
定义分为两部分,一部分是头部定义,一部分是体部定义.
比如一个class,他的头部定义是class A,体部定义是{}中的内容.
所以Segment增加属性CanFinishCodeNodeTypes,用于标识这个Segment是否可以结束一个CodeNode的定义,可以结束哪种类型的CodeNode的定义.
同时还可以标识是否可以开始一个新的CodeNode的定义,可以开始哪种类型的CodeNode的定义.
可以开始和结束body的segment都是符号,比如{,},;等等,整个的英文单词不具备这个能力.

#### 2024年02月12日23:07:28

对输出做了一些修改,现在可以通过Segment的类型来决定控制台输出的颜色,让其看起来更直观.
下一步准备定义系统值变量类型的Segment

比如系统值变量类型的可以定义为 SystemValueTypeWordSegment,
string,int,uint,double等的Segment的static属性都定义在SystemValueTypeWordSegments中.

### 2024年02月15日09:32:17 摸石头过河行为,找个文件当做示例,解析完了Segment以后,尝试在V2版本的Parse方法中测试

具体看一下怎么开辟领空,什么时机开辟,一个CodeNode的头和体怎么确立.在此之前先改造一下CodeNode,让它有一个属性表示有无体(
都有头),或者用类型来表示