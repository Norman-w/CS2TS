namespace CS2TS;

public abstract class SegmentLocationBase
{
	#region 确认是否相等的关系判断

	#region Equals

	private bool Equals(SegmentLocationBase other)
	{
		return GetType() == other.GetType();
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
			return false;
		if (ReferenceEquals(this, obj))
			return true;
		return obj.GetType() == GetType() && Equals((MethodLocation)obj);
	}

	#endregion

	public override int GetHashCode()
	{
		return GetType().GetHashCode();
	}

	#region 等号和不等号

	//判断method 是否和其他Location相等
	public static bool operator ==(SegmentLocationBase method, object other)
	{
		return method.GetType() == other.GetType();
	}

	public static bool operator !=(SegmentLocationBase method, object other)
	{
		return method.GetType() == other.GetType();
	}

	#endregion

	#endregion

	// public SegmentLocationBase? ParentLocation = null;
}

#region 语段所在位置的基础入口点.

public static class SegmentLocation
{
	public static readonly FileLocation File = new();
}

#endregion


#region 结构体

public class StructLocation : SegmentLocationBase
{
}

#endregion

#region 记录

public class RecordLocation : SegmentLocationBase
{
}

#region 委托

public class DelegateLocation : SegmentLocationBase
{
}

#endregion

#endregion

#region 语段所在位置的枚举段.里面可包含枚举值

public class EnumLocation : SegmentLocationBase
{
}

#endregion

#region 语段所在位置的方法段.里面可以包含变量定义,变量使用,变量赋值等

public class MethodLocation : SegmentLocationBase
{
	public readonly PropertyLocation Property = new();
}

#endregion

#region 语段所在位置的属性段

public class PropertyLocation : SegmentLocationBase
{
}

#endregion

#region 字段

public class FieldLocation : SegmentLocationBase
{
}

#endregion

#region 事件

public class EventLocation : SegmentLocationBase
{
}

#endregion

#region 构造函数

public class ConstructorLocation : SegmentLocationBase
{
}

#endregion

#region 析构函数

public class DestructorLocation : SegmentLocationBase
{
}

#endregion

#region 运算符重载

public class OperatorLocation : SegmentLocationBase
{
}

#endregion

#region 索引器

public class IndexerLocation : SegmentLocationBase
{
}

#endregion