using CS2TS.Model;

namespace CS2TS;

public static class NewParser
{
	public static Segment GetNextSegment(string csCodeString, int cursorPosition)
	{
		return Segment.PickFromCodeString(csCodeString[cursorPosition..]);
	}
}