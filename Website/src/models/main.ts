export interface Course {
    id:number,
    name:string,
    description?:string,
    price:number,
    themes:Theme[]
}

export interface Theme {
    id:number,
    name:string
}