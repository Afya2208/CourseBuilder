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
export interface Task extends LessonContent {

}
export interface ContentBlock extends LessonContent {

}
export interface LessonContent {

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
