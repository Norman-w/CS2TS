namespace CS2TS.Service.Valve;

/// <summary>
///     过载后的处理方式(用flag来表示,可以组合) 包含限流(等待),拒绝连接,提出连接等
/// </summary>
[Flags]
public enum OverloadHandleType
{
	/// <summary>
	///     限流(等待)
	/// </summary>
	Overload = 0x1,

	/// <summary>
	///     拒绝连接
	/// </summary>
	Refuse = 0x2,

	/// <summary>
	///     提出连接
	/// </summary>
	KickOut = 0x4
}