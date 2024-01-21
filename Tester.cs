using CS2TS.Service;

namespace CS2TS;

public static class Tester
{
	public static void Test()
	{
		#region 准备变量

		//获取当前应用程序的运行路径,通常是在Debug/bin xxx下,AppDomain.CurrentDomain.BaseDirectory;和AppContext.BaseDirectory;都不行
		// /Users/norman/RiderProjects/CS2TS/bin/Debug/net6.0
		var currentAppPath = Environment.CurrentDirectory;
		//使用相对路径"../../../TestCSFiles/Test.cs";
		var csCodeFilePath = Path.Combine(currentAppPath, "../../../TestCSFiles/Test.cs");
		//延迟多少秒后开始发送
		var firstDelay = new TimeSpan(0, 0, 0, 1);
		//每隔多少秒发送
		var eachSendDelay = new TimeSpan(0, 0, 0, 0, 500);
		//发送多少次后退出
		const int sendTimes = 1000;

		var csCodeString = File.ReadAllText(csCodeFilePath);

		#endregion

		var server = new Server();
		// //延迟3秒后发送数据(应该连上了)
		// Task.Run(async () =>
		// {
		// 	await Task.Delay(3000);
		// 	server.ShowCsCodeString("Hello World");
		// });
		//延迟1秒后每500毫秒发送一次,大于1000次后退出
		Task.Run(async () =>
		{
			await Task.Delay(firstDelay);
			var i = 0;
			while (i < sendTimes)
			{
				await Task.Delay(eachSendDelay);
				if (!server.ShowCsCodeString(csCodeString)) continue;
				i += 1;
				Console.WriteLine($"发送了{i}次");
			}
		});
		//不要让程序退出
		Console.ReadLine();
	}
}