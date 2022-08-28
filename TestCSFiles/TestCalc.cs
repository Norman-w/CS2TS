namespace CS2TS.TestCSFiles;

public class TestCals
{
  public string Return(string a)
  {
    return a;
  }
  public char Header(string a)
  {
    return a[1-1];
  }
  public char End(string a)
  {
    return a[a.Length-1];
  }
  public int Sum(int a, int b)
  {
    return a + b;
  }
  public int Max(int a, int b)
  {
    return a > b ? a : b;
  }
  public bool Equal(int a, int b)
  {
    return a == b;
  }
  // public string Append(string dest, int num)
  // {
  //   return string.Format("{0}{1}", dest, num);
  // }
  // public string Append(string dest, bool bl)
  // {
  //   var v = string.Format("{0}{1}", dest, bl);
  //   return v;
  // }
  // public byte[] Concat(byte[] a, byte[] b)
  // {
  //   var list = new List<byte>(a);
  //   for (int i = 0; i < b.Length; i++)
  //   {
  //     list.Add(b[i]);
  //   }
  //   return list.ToArray();
  // }
  // public byte[] Range(byte[] src, int start, int end)
  // {
  //   var srcLength = src.Length;
  //   if (start<srcLength-1 || end>srcLength-1)
  //   {
  //     throw new NotImplementedException("out of range");
  //   }
  //   List<byte> ret = new List<byte>();
  //   while (start <= end)
  //   {
  //     ret.Add(src[start++]);
  //     // start++;
  //   }
  //   return ret.ToArray();
  // }
  // public void SetValueTest()
  // {
  //   var a = 0;
  //   var b = 1;
  //   var c = 2;
  //   a = b = c++ - 3;
  // }
}
