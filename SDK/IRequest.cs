namespace CS2TS;

public interface IRequest<T> where T : IResponse
{
	//RequestId
	string RequestId { get; set; }

	//Get Api Name
	string? GetApiName();
}