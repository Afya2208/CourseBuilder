export interface Course {
  id: number
  name: string
  description?: string
  price: number
  themes: Theme[]
}

export interface Theme {
  id: number
  name: string
}

export interface ProblemDetails {
  status:number,
  title:string
}

export interface SignInResponse {
  userId: number
  roleName: string
  roleId:number
  token:string

  lastName?: string
  middleName?:string
  firstName?: string
  position?:string
}
