//这里是注释区域,里面我用了{还有/还有;还有其他的 之类的东西./*
///*注释区里面还可以有注释///////所以如果//或者/**/不再行的开头,都不是新注释行
/*

 * 这个一个注释段,命名空间没有summary
 */
export namespace aa {
  //这是普通的行注释
  /// <summary>
  /// 这是一个类
  /// </summary>
  export class a {
    AA: number = Number();
  }
  //命名空间内不能有属性
  export class aa extends a implements bb.iii{
    str: string = String();
    MyProperty: number = Number();
    AA: number = Number();
  }
  enum EnumText  {
    E1,
    E2=333
  }
  enum EnumTest2 {
  }
}
export namespace bb  {
  import fii = cc.dd.ee.ff.fii;

  export interface iii extends fii{
  }
}
export namespace cc {
  export namespace dd {
    export namespace ee{
      export namespace ff{
        export interface fii
        {

        }
      }
    }
  }
  export interface ii {
  }
}
