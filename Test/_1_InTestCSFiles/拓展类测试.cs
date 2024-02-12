namespace CS2TS.Test._1_InTestCSFiles;

public class A
{
	public static void Say()
	{
		Console.WriteLine("A");
	}
}
public class B
{
	public static void Say()
	{
		Console.WriteLine("B");
	}
}

//拓展类A,使其具有SayDouble方法
public static class AExtension
{
	public static void SayDouble(this A a)
	{
		A.Say();
		A.Say();
	}
}

public class C : A
{
	public static new void Say()
	{
		Console.WriteLine("C");
	}
}

public class Program
{
	public static void Test1()
	{
		A.Say();
		B.Say();
		C.Say();
		A a = new A();
		a.SayDouble();
	}
}

