import type { Correlation } from "@/models/main";
import type { AxiosError } from "axios";

export function randomInt(minInclude:number, maxInclude:number) : number {
    return Math.floor(Math.random() * (maxInclude-minInclude) + minInclude);
}

export function indexToChar(index:number) : string {
    let code = 'a'.charCodeAt(0)
    return String.fromCharCode(code+index);
}

export function mixCorrelations(originals:Correlation[]) : Correlation[] {
    //1 нужно нарандомить n индексов, чтобы они были от 0 до n и были разными
    let nums: number[] = []
    while(nums.length != originals.length) {
        let newRandom = randomInt(0, nums.length+1);
        console.log(newRandom)
        if (!nums.includes(newRandom)) {
            nums.push(newRandom)
        }
    }
    //2 теперь нужно создать новую корреляцию на основе оригинала и заменить 
    let mixed: Correlation[] = []
    for (let i = 0; i < originals.length; i++){
        let originalCorrelation = originals[i]
        let secondOriginal = originals[nums[i]??0]
        if (originalCorrelation && secondOriginal) {
            
            originalCorrelation.right = secondOriginal.right
            mixed.push(originalCorrelation)
        }
    } 
    return mixed
}


