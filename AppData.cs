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

*/

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
		public object Constant => CONSTANT.Instance;
	}

	#endregion

	// public SiteConfig SiteConfig { get; set; }

	// public StepsConfig StepsConfig { get; set; }

	// public StatesConfig StatesConfig { get; set; }

	#region OTHERS(STATIC, CONSTANT, ENUM, DEFINITION ETC)

	//TODO

	#endregion
}