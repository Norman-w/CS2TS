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
        ct?: number;
        str?: string;
        MyProperty?: number;
        AA?: number;
        /// <summary>
        /// 这是一个函数
        /// </summary>
        /// <param name="a">函数的参数1</param>
        /// <param name="b">函数的参数2</param>
        /// <returns>返回结果</returns>
        public sum(a: number, b: number) :number {
            return Number();
        }

        public voidFunc(a: number, b: number) :void {
        }

        /// <summary>
        /// 遍历每一个to内的元素,如果from有同名的,替换to内的值
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public replace(from: {[Key: string]: {[Key: string]: number}}, to: {[Key: string]: {[Key: string]: number}}) :{[Key: string]: string} {
            return undefined;
        }

        private format(a: number, b: number) :number|undefined {
            return undefined;
        }

        public fiiFunc(b: number) :void;
        public fiiFunc(str: string) :void;
        public fiiFunc() :string;
        public fiiFunc(param0?:number|string) :void|string
        {
            return undefined;
        }

        private fiiFunc_PRIVATE(bl: boolean) :string {
            return String();
        }

        //这个方法不是从接口中继承而来，所以他的修饰符可以是private的。如果该方法是从接口继承，那么他必须是public的
    }
    export class bClass {
    }
}
export namespace bb {
    export interface iii {
        voidFunc(a: number, b: number) :void;
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
                    fiiFunc(b: number) :void;
                    fiiFunc(str: string) :void;
                    fiiFunc() :string;
                }
            }
        }
    }
}
