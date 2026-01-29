export interface Course {
    id: number
    name: string
    description?: string
    price: number
    authorId?: number
    themes?: Theme[]
    lessonsCount?: number
    modulesCount?: number
}

export interface Theme {
    id: number
    name: string
}
export interface Lesson {
    id: number
    name: string
    description?: string
    moduleId: number
    lessonTypeId: number
    order: number
}
export interface Task {
    taskTypeId: number
    order: number
    id: number
    question: string
}
export interface TaskAnswer {
    textValue?: string
    file?: Uint8Array
    fileName?: string
    isRight?: boolean
    taskId: number
}
export interface Correlation {
    left: string
    right: string
    id: number
}
export interface ContentBlockType{
    id: number,
    name:string
}
export interface ContentBlock {
    contentBlockTypeId: number
    order: number
    id: number
    textValue?: string
    fileName?: string
    fileUrl?: string

    name: string
}
export interface Module {
    id: number
    name: string
    description?: string
    courseId: number
    lessonsCount?: number
    order: number
}
export interface ProblemDetails {
    status: number
    title: string
}
export interface Role {
    id: number,
    name:string
}
export interface UserInformation {
    userId:number
    lastName?: string
    middleName?: string
    firstName?: string
    position?: string
    phone?:string
}
export interface User {
    id: number
    email:string
    role:Role
    userInformation:UserInformation
}

export interface SignInResponse {
    token:string
    user:User
}
