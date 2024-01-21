using System.Text;

namespace CS2TS;

public partial class CSharpCodeParser
{
	#region 根据现有的信息创建函数

	//根据已有的词汇表，创建函数，并且自动添加到领空和父亲，然后清空未处理的半截单词和未处理的所有单词
	private void CreateFunction(bool hasStructure)
	{
		var parent = _spaces[^1];
		var fn = new Function();
		//括号前面的是函数名
		//函数名前面的是函数返回值类型
		var nameIndex = _unProcessWords.IndexOf("(") - 1;
		var name = _unProcessWords[nameIndex];
		//函数的返回值类型应该是在public等关键字后面一直到函数名处的所有内容.
		//////string returnType = unProcessWords[nameIndex - 1];


		fn.Permission = ConvertString2Permission(_unProcessWords[0].Replace("\t", "").Trim());
		fn.IsStatic = _unProcessWords.Contains("static");
		fn.IsOverride = _unProcessWords.Contains("override");
		var returnTypeDefStartPos = 0;
		if (fn.Permission != null) returnTypeDefStartPos++;

		if (fn.IsStatic) returnTypeDefStartPos++;

		if (fn.IsOverride) returnTypeDefStartPos++;

		#region 解析函数的返回值

		#endregion

		var returnTYpeDefWordsCount = nameIndex - returnTypeDefStartPos;

		//构造函数的返回值
		Parameter? constructorReturnParameter = null;
		//如果nameIndex在第0位的话,那一定是构造函数,忽略了返回值类型,也忽略了public等关键字
		//如果nameIndex在第1位的话,那一定是构造函数,忽略了返回值类型,但是没有忽略public等关键字
		if (nameIndex is 0 or 1)
		{
			returnTypeDefStartPos = 0;
			returnTYpeDefWordsCount = 0;
			var parentClassName = (parent as Class)?.Name;
			if (parentClassName != null)
				constructorReturnParameter = new Parameter
				{
					Name = "this",
					Type = new TypeDefine
					{
						Name = parentClassName
					}
				};
			fn.ReturnParameter = constructorReturnParameter;
			fn.IsConstructor = true;
		}

		// StringBuilder returnType = new StringBuilder();
		// for (int ri = 0; ri < returnTYpeDefWordsCount; ri++)
		// {
		//   returnType.Append(_unProcessWords[returnTypeDefStartPos + ri]);
		// }
		// fn.ReturnParameter = new Parameter();
		// fn.ReturnParameter.Type = new TypeDefine() {Name = returnType.ToString()};
		if (!fn.IsConstructor)
		{
			//不是构造函数,那就是一般的函数,返回值正常处理
			var returnParameterDefineWords = _unProcessWords.Skip(returnTypeDefStartPos).Take(returnTYpeDefWordsCount);
			var returnParameters = Convert2Parameters(new List<string>(returnParameterDefineWords));
			if (returnParameters.Count > 1) throw new NotImplementedException("错误,解析到的返回值不止1个");

			fn.ReturnParameter = returnParameters[0];
		}


		//class 标记后面的一个为类名称
		fn.Name = name.Replace("\r", "").Replace("\n", "").Trim();


		//先进入函数定义领空再处理函数的参数集.不然会错乱,可能会把参数加到类中了.
		_spaces.Add(fn);

		#region 解析函数的参数

		var leftParenthesesPos = _unProcessWords.IndexOf("(");
		var rightParenthesesPos = _unProcessWords.IndexOf(")");
		if (rightParenthesesPos - 1 > leftParenthesesPos)
		{
			var parameterWordsList = _unProcessWords.Skip(leftParenthesesPos + 1)
				.Take(rightParenthesesPos - leftParenthesesPos - 1);
			var parameters = Convert2Parameters(new List<string>(parameterWordsList));
			fn.InParameters = parameters;
		}

		#endregion


		// if (parent is Class)
		// {
		parent.Chirldren.Add(fn);
		// }

		//但是如果在分号结尾的时候，就是直接定义这个函数而没有结构，hasStructure如果是true，_unProcessWords就是用{结尾，如果是false就是_unProcessWords最后一个为;

		if (!hasStructure) _spaces.RemoveAt(_spaces.Count - 1);
		_tempWord = new StringBuilder();
		_unProcessWords.Clear();
	}

	#endregion
}