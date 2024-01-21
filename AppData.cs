/*
3 levels/types of definition/config/setting on server typically,
use for keep consistent between client and server and transfer config/setting data between them:


LEVEL 1, System level, only have to let client know once, never update:
    a, one to one: constant is absolute never change, it usually use to represent one dimension information
    b, one to many: enum is also absolute never change, it has more properties than constant
    c, one to more: static class is another case, it is readonly, only can be set when init, it is one to more relationship, it is part of constant, but it is absolute never change.

LEVEL 2, System level, can update, have to let client know when update:
    Usually update by control panel, not by user behavior.
    For example, use SettingManager fileWatcher to detect the change, then update the value, then update the version or timestamp etc, return to client when they need.
    Or use long connection, push to client actively.
    static config can be update, but it is only one instance, instance must exist, value can be update, instance can not be update, client need to update when it update.
    site config is one of them, because global is possible to change.
    additional/ dev/ different ver is also belong to this.

LEVEL 3, User level, of course can update, user can maintain these config/setting by themselves.
    Usually use to save some user current page, position, preference etc, usually not record business level content.
                    They use for special environment, such like test environment,
                    it may be a big collection, use to mark which item override the formal environment.
                    Or it may be some special config in some time period, such like 1~3 am, use to test and give to client.
                    In a word, this kind of config is additional, usually use for development and test.



Update way:

	For LEVEL 1, use version to mark if need update, because it exist when start the app.
	For LEVEL 2 and LEVEL 3, use timestamp to mark if need update, every api request, no matter what api, all carry timestamp in header, use to detect if need return to them.
	Every api has the ability to return the information, just return, no need to tell them it is too old, need update, then cancel this time api, then update, then call this api again, too much trouble.

For example, when get [step], [step] render need [stepsConfig], should just return [stepsConfig],
not tell them need update, then cancel this time step get, then update stepsConfig, then get step again, too much call, too much debounce etc.
When get user config, also carry (1 version, 2 timestamp) in header, then return all of them what they need.


3个级别/类型的定义/配置/设置在服务器上通常是:
用于在客户端和服务器之间保持一致并在它们之间传输配置/设置数据:


LEVEL 1, 系统级别, 只需让客户端知道一次, 永远不会更新:
	a, 一对一: 常量是绝对不会改变的, 它通常用于表示一维信息
	b, 一对多: 枚举也是绝对不会改变的, 它比常量有更多的属性
	c, 一对多: 静态类是另一种情况, 它是只读的, 只能在初始化时设置, 它是一对多的关系, 它是常量的一部分, 但它是绝对不会改变的.

LEVEL 2, 系统级别, 可以更新, 必须在更新时通知客户端:
	通常由控制面板更新, 而不是由用户行为更新.
	例如, 使用SettingManager fileWatcher检测更改, 然后更新值, 然后更新版本或时间戳等, 在客户端需要时返回.
	或者使用长连接, 主动推送给客户端.
	静态配置可以更新, 但它只是一个实例, 实例必须存在, 值可以更新, 实例不能更新, 客户端需要在它更新时更新.
	站点配置是其中之一, 因为全局可能会改变.
	附加/ dev/不同的ver也属于这个.

LEVEL 3, 用户级别, 当然可以更新, 用户可以自己维护这些配置/设置.
	通常用于保存一些用户当前页面, 位置, 偏好等, 通常不记录业务级内容.
					它们用于特殊环境, 比如测试环境,
					它可能是一个很大的集合, 用来标记哪个项目覆盖正式环境.
					或者它可能是某个时间段的一些特殊配置, 比如1~3 am, 用来测试和给客户端.
					总之, 这种配置是附加的, 通常用于开发和测试.



更新方式:

	对于LEVEL 1, 使用版本来标记是否需要更新, 因为它在启动应用程序时存在.
	对于LEVEL 2和LEVEL 3, 使用时间戳来标记是否需要更新, 每个api请求, 无论是什么api, 都在头部携带时间戳, 用于检测是否需要返回给它们.
	每个api都有返回信息的能力, 只需返回, 不需要告诉它们它太旧了, 需要更新, 然后取消这次api, 然后更新, 然后再次调用这个api, 太多的麻烦.

例如, 当获取[步骤], [步骤]渲染需要[步骤配置], 应该只返回[步骤配置],
不要告诉他们需要更新, 然后取消这次步骤获取, 然后更新步骤配置, 然后再次获取步骤, 太多的调用, 太多的防抖等.
当获取用户配置时, 也在头部携带(1版本, 2时间戳), 然后返回他们需要的所有内容.
*/

using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace CS2TS;

public class AppData
{
	public Level1Model Level1 { get; set; } = new();

	//TODO
	public bool NeedUpdateDefinition { get; set; } //is constant and definition enums etc version changed

	// public bool NeedUpdateSiteConfig => SiteConfig != null;//is modified site config

	// public bool NeedUpdateStepsConfig => StepsConfig != null;//is modified steps config
	//TODO
	public bool NeedUpdateUserConfig { get; set; } //is modified user config

	//TODO
	public bool NeedUpdateAdditionalConfig { get; set; } //is modified dev, different version, injection config etc

	public bool NeedUpdate => NeedUpdateDefinition
	                          // || NeedUpdateSiteConfig 
	                          // || NeedUpdateStepsConfig 
	                          || NeedUpdateUserConfig
	                          || NeedUpdateAdditionalConfig;

	#region 定义level1

	public class Level1Model
	{
		/// <summary>
		///     Constant内容,用于传递给客户端
		/// </summary>
		public JObject Constant => Utils.SerializeStaticClassToJObject<Container>();
	}

	#endregion

	// public SiteConfig SiteConfig { get; set; }

	// public StepsConfig StepsConfig { get; set; }

	// public StatesConfig StatesConfig { get; set; }

	#region OTHERS(STATIC, CONSTANT, ENUM, DEFINITION ETC)

	//TODO

	#endregion
}