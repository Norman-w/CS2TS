using CS2TS.Model;

namespace CS2TS;

public class PermissionBuilder
{
  public static Nullable<PermissionEnum> Parse2Permission(string str)
  {
    switch (str)
    {
      case "protected":
        return PermissionEnum.Protected;
      case "private":
        return PermissionEnum.Private;
      case "public":
        return PermissionEnum.Public;
      case "internal":
        return PermissionEnum.Internal;
      default:
        return null;
    }
  }
}
