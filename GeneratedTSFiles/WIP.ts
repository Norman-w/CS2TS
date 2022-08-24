
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
        public fiiFunc(sss:boolean):void;
        public fiiFunc(b: number) :void;
        public fiiFunc(str: string) :void;
        public fiiFunc() :string ;
        public fiiFunc(param0? : number|string|boolean|undefined):string|void
        {
            //如果多个同名的函数的相同位置上的参数名字或者类型不同，就生成为param0 这样的名字。然后根据类型的判断具体的确定执行的是哪一个函数。
            if(typeof param0 === "number")
            {
                this.fiiFunc_v2();
                return;
            }
        }
        public fiiFunc_v2():void
        {
            
        }
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
