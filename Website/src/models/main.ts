export interface Course {
  id: number
  name: string
  description?: string
  price: number,
  authorId?: Number,
  themes?: Theme[]
}

export interface Theme {
  id: number
  name: string
}

export interface Module {
  id: number,
  name: string,
  description?: string
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
