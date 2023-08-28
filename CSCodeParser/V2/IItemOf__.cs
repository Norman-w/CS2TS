namespace CS2TS.CSCodeParser.V2;

public interface IItemOfParent
{
  public string? Name { get; set; }
}

public interface IItemOfCodeFile :IItemOfParent
{
}

public interface IItemOfNameSpace :IItemOfParent
{
  
}
public interface IItemOfClass :IItemOfParent
{
  
}

