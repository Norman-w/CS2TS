namespace CS2TS;

public interface IResponse
{
	//ErrMsg
	string? ErrMsg { get; set; }

	//ErrCode
	string? ErrCode { get; set; }
}