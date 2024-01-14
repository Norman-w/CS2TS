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

public class RequestPackage
{
	public string? ApiName { get; set; }
	public string? RequestId { get; set; }
	public string? Data { get; set; }

	public override string ToString()
	{
		return $"ApiName:{ApiName},RequestId:{RequestId},Data:{Data}";
	}

	public static RequestPackage? FromJson(string json)
	{
		return JsonConvert.DeserializeObject<RequestPackage>(json);
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