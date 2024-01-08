namespace CS2TS;

public class BaseRequest : IRequest<BaseResponse>
{
	private static string? _apiName;
	public string RequestId { get; set; } = Guid.NewGuid().ToString();

	public string GetApiName()
	{
		if (!string.IsNullOrEmpty(_apiName)) return _apiName;
		//类名称,去掉Request,变成都是小写的,每一个单词之间用下点号隔开
		var apiName = GetType().Name.Replace("Request", "");
		var apiNameArray = apiName.ToCharArray();
		var apiNameList = new List<char>();
		foreach (var c in apiNameArray)
			if (char.IsUpper(c))
			{
				if (apiNameList.Count > 0) apiNameList.Add('.');
				apiNameList.Add(char.ToLower(c));
			}
			else
			{
				apiNameList.Add(c);
			}

		_apiName = new string(apiNameList.ToArray());
		return _apiName;
	}
}