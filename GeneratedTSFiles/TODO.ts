
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

        fiiFunc(age: number): void;
        fiiFunc(name: string): void;
        fiiFunc(): string;
        fiiFunc(p1__number_string_undefined_null_Array?: number | string): void | string {
            if(p1__number_string_undefined_null_Array === undefined)
            {
                return this['fiiFunc_v2']();
            }
            if(typeof p1__number_string_undefined_null_Array === typeof Number)
            {
                return this.fiiFunc_v0(p1__number_string_undefined_null_Array as number);
            }
            if(typeof p1__number_string_undefined_null_Array === typeof String)
            {
                return this.fiiFunc_v1(p1__number_string_undefined_null_Array as string);
            }
            return undefined;
        }
        private fiiFunc_v0(age:number):void
        {

        }
        private fiiFunc_v1(name:string):void
        {

        }
        private fiiFunc_v2():string
        {
            return "";
        }

        // public getInfo(name:string):string
        // public getInfo(age:number):string
        // public getInfo(str:any):any{
        //     if(typeof str === 'string') return '我叫 ' + str
        //     else return '我的年龄是 ' + str
        // }
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
                    fiiFunc(age: number):void;
                    fiiFunc(name:string) :void;
                    fiiFunc():string;
                }
            }
        }
    }
}
