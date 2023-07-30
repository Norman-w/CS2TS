namespace CS2TS;

public abstract class SegmentLocationBase
{
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
  public override int GetHashCode()
  {
    return GetType().GetHashCode();
  }
  
  //判断method 是否和其他Location相等
  public static bool operator ==(SegmentLocationBase method, object other)
  {
    return method.GetType() == other.GetType();
  }
  public static bool operator !=(SegmentLocationBase method, object other)
  {
    return method.GetType() == other.GetType();
  }
}

public static class SegmentLocation
{
  public readonly static FileLocation File = new FileLocation();
}
public class FileLocation : SegmentLocationBase
{
  public readonly NamespaceLocation Namespace = new NamespaceLocation();
}
public class NamespaceLocation : SegmentLocationBase
{
  public readonly ClassLocation Class = new ClassLocation();
}
public class ClassLocation : SegmentLocationBase
{
  public readonly MethodLocation Method = new MethodLocation();
}
public class MethodLocation : SegmentLocationBase
{
  public readonly PropertyLocation Property = new PropertyLocation();
}
public class PropertyLocation : SegmentLocationBase
{
 
}