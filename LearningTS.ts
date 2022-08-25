//这个文件里面测试一些学习到的ts知识

//测试方法入参的类型判断(接口)
interface Person {
    Name:string;
    Age:number,
    Say:()=>void;
}
interface Animal{
    Name:string;
    Age:number;
    Sound:()=>void;
}
const norman:Person = {
    Name:'norman',
    Age:33,
    Say:()=>console.log('hello world')
}
const dog:Animal = {
    Name:'tq',
    Age:3,
    Sound:()=>console.log('wang wang!')
}

function TestWhoAreYou(obj: Person|Animal) {
    const person:Person = obj as Person;
    if(obj!= undefined && obj == person)
    {
        return PersonSay(obj as Person)
    }
    const animal:Animal = obj as Animal;
    if(typeof obj === typeof animal)
    {
        AnimalSound(animal);
    }
}
function PersonSay(person:Person)
{
    
}
function AnimalSound(animal:Animal)
{
    
}

TestWhoAreYou(norman);
TestWhoAreYou(dog);