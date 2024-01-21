namespace CS2TS.Service.Valve;

/// <summary>
///     限制器类型
/// </summary>
public enum ValveType
{
	/*
	 包含总值超过多少类型,总值低于多少类型,规定时间内超过多少类型,规定时间内低于多少类型
	 规定时间内上升幅度类型,规定时间内下降幅度类型等
	 */
	/// <summary>
	///     总值超过多少类型
	/// </summary>
	TotalValueExceed,

	/// <summary>
	///     总值低于多少类型
	/// </summary>
	TotalValueLowerThan,

	/// <summary>
	///     规定时间内超过多少类型
	/// </summary>
	InTimeExceed,

	/// <summary>
	///     规定时间内低于多少类型
	/// </summary>
	InTimeLowerThan,

	/// <summary>
	///     规定时间内超过多少次类型
	/// </summary>
	InTimeExceedTimes,

	/// <summary>
	///     规定时间内低于多少次类型
	/// </summary>
	InTimeLowerThanTimes,

	/// <summary>
	///     规定时间内上升幅度类型
	/// </summary>
	InTimeRise,

	/// <summary>
	///     规定时间内下降幅度类型
	/// </summary>
	InTimeFall
}