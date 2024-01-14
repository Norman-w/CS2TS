/*

 Dart model:

 class RequestPackage {
   String apiName;
   String requestId;
   String data;
   RequestPackage({required this.apiName,required this.requestId, required this.data});
   Map<String, dynamic> toJson() =>
   {
   'apiName': apiName,
   'requestId': requestId,
   'data': data,
   };
   //fromJson
   RequestPackage.fromJson(Map<String, dynamic> json) :
   apiName = json['ApiName'],
   requestId = json['RequestId'],
   data = json['Data'];
   }


*/


using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CS2TS.SDK.WebSocket;

public enum EnumRequestInitiator
{
	Client = 1,
	Server = 2
}

//让RequestPackage接受泛型参数
//使其能够接受public class ShowCsCodeStringRequest : BaseRequest<ShowCsCodeStringResponse>
//注意T代表的不是BaseResponse,而是ShowCsCodeStringRequest<ShowCsCodeStringResponse>
public class RequestPackage<T> where T : BaseResponse
{
	public string? ApiName { get; set; }
	public string? RequestId { get; set; }
	public BaseRequest<T>? Data { get; set; }

	public EnumRequestInitiator Initiator { get; set; } = EnumRequestInitiator.Server;

	//发出时间
	public DateTime? SendTime { get; set; }

	public override string ToString()
	{
		return $"ApiName:{ApiName},RequestId:{RequestId},Data:{Data}";
	}

	public static RequestPackage<T>? FromJson(string json)
	{
		return JsonConvert.DeserializeObject<RequestPackage<T>>(json);
	}

	public string ToJson()
	{
		//首字母小写
		return JsonConvert.SerializeObject(this, new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		});
	}
}