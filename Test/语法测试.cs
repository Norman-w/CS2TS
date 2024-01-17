// //直接在cs文件下面,可以定义命名空间,类,接口,结构体,枚举,委托,记录
// //也可以定义变量,但是不能包含public等修饰符
// //也可以定义方法,但是不能包含public等修饰符
// //不能定义属性,因为属性必须在类中
// //可以使用变量,赋值,调用方法,调用委托,调用记录,调用结构体,调用枚举,调用结构体
//
// var a = 10;
//
// a=10;
//
// void Test()
// {
// }
//
// internal enum MyEnum
// {
// }
//
// namespace MyNameSpace
// {
// 	//在命名空间内还可以定义命名空间,类,接口,结构体,枚举,委托,记录
// 	//对于方法,变量,属性,只能定义在类中
// 	delegate void MyDelegate2();
// }
//
// internal class MyClass
// {
// }
//
// internal delegate void MyDelegate();
//
// internal interface MyInterface
// {
// 	event MyDelegate MyEvent;
// 	//indexer
// 	int this[int index] { get; set; }
// 	interface IInterface
// 	{
// 		
// 	}
// 	class IClass
// 	{
// 		
// 	}
// 	enum IEnum
// 	{
// 		
// 	}
// 	delegate void IDelegate();
// 	record IRecord
// 	{
// 		
// 	}
// 	ref struct IStruct
// 	{
// 		
// 	}
// 	
// 	async Task Test()
// 	{
// 		await Task.CompletedTask;
// 	}
// 	//除此之外还可以在这里定义方法,变量,属性
// }
//
// internal record MyRecord
// {
// 	MyInterface.IInterface iInterface;
// }
//
// internal struct MyStruct
// {
// }

