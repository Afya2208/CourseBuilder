export interface SignUpRequest {
    email: string,
    password: string
    roleId: number
	lastName?: string
	middleName?: string
	firstName?: string
	position?: string
	phone?: string
}