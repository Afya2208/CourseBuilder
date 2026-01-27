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
}
export interface Task {
  taskTypeId: number
  order: number,
  id: number,
  question: string
}
export interface TaskAnswer {
  textValue?: string
  file?: Uint8Array,
  fileName?: string,
  isRight?: boolean,
  taskId: number
}
export interface Correlation {
  left: string,
  right: string,
  id: number
}
export interface ContentBlock {
  contentBlockTypeId: number,
  order: number,
  id: number,
  textValue?: string,
  fileName?: string,
  fileUrl?: string,
  
  name:string
}


export interface Module {
  id: number
  name: string
  description?: string
   lessonsCount?: number
}

export interface ProblemDetails {
  status: number
  title: string
}

export interface User {
  userId: number
  roleName: string
  roleId: number
  token: string

  lastName?: string
  middleName?: string
  firstName?: string
  position?: string
}
