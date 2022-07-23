using System.Text;

namespace CS2TS;
/// <summary>
/// ts文件创建器
/// </summary>
public class Generator
{
    #region 构造函数

    public Generator()
    {
        _currentCode = new StringBuilder();
        _currentLayerDepth = 0;
    }

    #endregion
    
    #region 全局变量

    private StringBuilder _currentCode;
    private int _currentLayerDepth;

    #endregion

    #region 公共函数

    public string CreateTsFile(CodeFile codeFile)
    {
        string tsCode = "";
        if (codeFile == null)
        {
            return null;
        }
        return tsCode;
    }

    #endregion

    #region 私有函数

    private string getTab(int layerDepth)
    {
        var tab = new StringBuilder();
        for (int i = 0; i < layerDepth; i++)
        {
            tab.Append('\t');
        }

        return tab.ToString();
    }

    private StringBuilder startClassCode(Class cls)
    {
        _currentLayerDepth++;
        var classCode = new StringBuilder();
        var tab = getTab(_currentLayerDepth);
        var toInterface = false || cls.Variables!= null && cls.Variables.Count>0 && cls.Classes == null && cls.Functions == null;

        if (toInterface)
        {
            classCode.Append(tab).AppendLine($"{tab}interface {cls.Name}\r\n");
            classCode.Append(tab).AppendLine("{");
        }
        else
        {
            
        }

        return classCode;
    }

    private void endClassCode(ref StringBuilder classCode)
    {
        classCode.AppendLine(getTab(_currentLayerDepth));
        _currentLayerDepth--;
    }

    #endregion
}