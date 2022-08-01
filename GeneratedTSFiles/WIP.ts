import bb;
import cc;
//这里是注释区域,里面我用了{还有/还有;还有其他的 之类的东西./*
///*注释区里面还可以有注释///////所以如果//或者/**/不再行的开头,都不是新注释行
import System;
import System.Collections.Generic;
import System.Text;
import cc.dd.ee.ff;
import CS2TS;
/*

 * 这个一个注释段,命名空间没有summary
 */
export namespace aa {
  //这是普通的行注释
  /// <summary>
  /// 这是一个类
  /// </summary>
  export class a {
    AA?: number;
  }
  export class aSub {
  }
  export class aSubSub {
    Finded?: boolean;
  }
  //////111
  export 	enum EnumText  {
    E1,
    E2 = 3,
    //这里应该等于4
    E4,
    E5,
  }
  export 	enum EnumTest2  {
    EL = 111,
    EW = 111111111111111,
  }
  //命名空间内不能有属性
  export class aa extends a implements cc.dd.ee.ff.fii {
    aobj?: aa;
    ct?: number = Number();
    str?: string;
    MyProperty?: number;
    AA?: number;
    /// <summary>
    /// 这是一个函数
    /// </summary>
    /// <param name="a">函数的参数1</param>
    /// <param name="b">函数的参数2</param>
    /// <returns>返回结果</returns>
    public sum() :number {
      return Number();
    }
    public voidFunc() :void {
    }
    private format() :Nullable<long> {
      return ;
    }
    public fiiFunc() :void {
    }
  }
  export class bClass {
  }
}
export namespace bb {
  export interface iii {
    voidFunc() :void;
  }
  export interface iiii4 {
  }
}
export namespace cc {
  export interface ii {
  }
  export namespace dd {
    export namespace ee {
      export namespace ff {
        export interface fii {
          fiiFunc() :void;
        }
      }
    }
  }
}
