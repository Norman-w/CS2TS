namespace CS2TS;

public partial class CSharpCodeParser
{
	private bool IsInNotesLine()
	{
		foreach (var s in _spaces)
			if (s is NotesLine)
				return true;

		return false;
	}

	private bool InInNotesArea()
	{
		foreach (var s in _spaces)
			if (s is NotesArea)
				return true;

		return false;
	}

	private bool IsInSharpLine()
	{
		foreach (var s in _spaces)
			if (s is SharpLine)
				return true;

		return false;
	}

	private bool IsInStatement()
	{
		return _spaces[^1] is StatementWithStructure;
	}

	private bool IsInNamespace()
	{
		return _spaces[^1] is Namespace;
	}

	private bool IsInClass()
	{
		return _spaces[^1] is Class;
	}

	private bool IsInEnum()
	{
		return _spaces[^1] is EnumDefine;
	}

	private bool isInVariable_Structure()
	{
		return _spaces[^1] is VariableWithStructure;
	}

	private bool IsInFunction()
	{
		return _spaces[^1] is Function;
	}
}