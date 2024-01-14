using CS2TS.Service;

namespace CS2TS;

public static class Tester
{
	public static void Test()
	{
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
			await Task.Delay(1000);
			var i = 0;
			while (i < 1000)
			{
				await Task.Delay(500);
				var content = "Hello Norman, this is a test message from C#, sent times: " + i;
				if (!server.ShowCsCodeString(content)) continue;
				i += 1;
				Console.WriteLine($"发送了{i}次");
			}
		});
		//不要让程序退出
		Console.ReadLine();
	}
}