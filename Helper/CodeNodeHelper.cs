// using System.Reflection;
// using CS2TS.Model;
//
// namespace CS2TS.Helper;
//
// /// <summary>
// /// 帮助寻找代码节点文件中的节点.
// /// </summary>
// public class CodeNodeHelper
// {
//   private readonly CodeFile _file;
//   public CodeNodeHelper(CodeFile file)
//   {
//     this._file = file;
//   }
//
//   public T GetNode<T>(CodeNodeSequenceInfo sequenceInfo, CodeNode parent) where T : CodeNode
//   {
//     if (typeof(T) == typeof(Class))
//     {
//       var clsContainer = parent as IClassContainer;
//       return clsContainer.Classes[sequenceInfo.IndexOfThisTypeAllElemList] as T;
//     }
//     return null;
//   }
//
//   /// <summary>
//   /// 从父节点中获取class,用于在有类继承但是不知道继承的是类还是接口的时候.试图在文件或者命名空间中查找这个类
//   /// </summary>
//   /// <param name="parentNode"></param>
//   /// <param name="clsName"></param>
//   /// <returns></returns>
//   public Class? FindClass(CodeNode parentNode, string clsName)
//   {
//     if (parentNode.Sequences == null)
//     {
//       return null;
//     }
//
//     foreach (var sequence in parentNode.Sequences)
//     {
//       if (sequence.Type == typeof(Class) && sequence.Name == clsName)
//       {
//         var i = sequence.IndexOfThisTypeAllElemList;
//         var parentNodeAsClassContainer = parentNode as IClassContainer;
//         return parentNodeAsClassContainer.Classes[i];
//       }
//       else
//       {
//         //如果当前片段不是那个名字的类,那就继续向下向内找.
//         if (!sequence.Type.IsAssignableFrom(typeof(IClassContainer)))
//         {
//           continue;
//         }
//         else
//         {
//           var currentNode = GetNode<CodeNode>(sequence, parentNode);
//           var finded = FindClass(currentNode, clsName);
//           if (finded!= null)
//           {
//             return finded;
//           }
//           else
//           {
//             continue;
//           }
//         }
//       }
//     }
//
//     return null;
//   }
//
//   // public Interface FindInterface()
//   // {
//   //
//   // }
// }
