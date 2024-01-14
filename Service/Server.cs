/*


 CS2TS作为服务提供给flutter等项目使用,flutter等项目通过调用server的api或者是通过websocket获取视图数据
 或者Service.Server推送给flutter视图项目数据等,都通过这个类来进行管理.
 当前是使用Websocket来进行数据传输,后续可能还会提供http接口,或者是grpc,mqtt等接口,都可以通过这个类来进行管理.


*/

namespace CS2TS.Service;

public class Server
{
	private WebSocketServer.Server _server = new(8000);
}