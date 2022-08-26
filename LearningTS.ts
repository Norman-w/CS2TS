//这个文件里面测试一些学习到的ts知识
// function log(obj) {
//     console.log(obj)
// }
//测试方法入参的类型判断(接口)


//人 接口
interface IPerson {
    Name:string;
    Age:number,
    Say:Function
}
//人 类型
type TPerson =
    {
        Name:String,
        Age:number,
        Say:Function
    }
//人 类,继承自接口
class CPerson implements IPerson
{
    Name:'male1';
    Age:11;
    public Say:()=>{log(this)}
}

//人 类,继承自类型
class CPerson2 implements TPerson
{
    Name:String
    Age:number
    Say:Function
    
    constructor() {
        this.Say = ():void =>{
           console.log('this is CPerson2 implements TPerson') 
        }
    }
}

//动物 接口
interface IAnimal{
    Name:string;
    Age:number;
    Sound:()=>void;
}


const normanI:IPerson = {
    Name:'norman',
    Age:33,
    Say:()=>console.log('********hello world************')
}
const normanT:TPerson =
    {
        Name:'norman',
        Age:33,
        Say:()=>console.log('hello ******* this is person type!!!')
    }
const dogI:IAnimal = {
    Name:'tq',
    Age:3,
    Sound:()=>console.log('wang wang!')
}

function CallPersonT(person:TPerson)
{
    person.Say();
}
function CallPersonI(person:IPerson)
{
    // console.log(person.Say)
    person.Say();
}
function CallAnimalI(animal:IAnimal)
{
    animal.Sound()
}

function TestWhoAreYou(obj: IPerson|IAnimal|TPerson|CPerson|CPerson2) {
    //判断是否为某个类的对象 可以使用
    //obj instanceof Male
    //或者
    //obj.constructor == Male
    
    //判断是否属于某个类型,可以使用
    //typeof obj === "number"
    
    //todo 判断传入的内容是否继承了接口????what fuck!
    
    //todo 怎么判断传入的参数是个对象还是个接口? 怎么判断入参的类型是 类限定类型 还是接口限定类型????
}
TestWhoAreYou(normanI);
// TestWhoAreYou(norman);
// TestWhoAreYou(dog);