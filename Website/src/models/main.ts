export interface Course {
	id: number
	name: string
	description?: string
	price: number
	authorId?: number
	themes?: Theme[]
	linkedGroupId?: number
	isPublic: boolean
	themesIds?: number[]
	lessonsCount?: number
	modulesCount?: number
	modulesHaveOrder?: boolean
}
export interface CourseEditable {
	id: number
	name: string
	description?: string,
	linkedGroupId?:number
    price: number,
    authorId: number
    isPublic:boolean,
	themesIds: number[]
	modulesHaveOrder: boolean
}

export interface Theme {
	id: number
	name: string
}

export interface TaskType {
	id: number
	name: string
}


export interface UserAnswer {
	textValue?: string
	file?: Uint8Array
	fileName?: string
	isRight?: boolean
	selectedTaskAnswersId: number[]
	selectedTaskAnswerId: number
	taskId: number
}
export interface Lesson {
	id: number
    name: string,
    closedUntil?: Date,
    maxTriesCount?: number
    isRequired: boolean,
	description?: string
	moduleId: number
	lessonTypeId: number
	order: number
}
export interface Task {

    lessonId:number
	taskTypeId: number
	order: number
	id: number
    question: string
    score: number
}
export interface TaskEditable extends Task {
	textAnswer?: string
	answer?: TaskAnswer
	allAnswerOptions?: TaskAnswer[]
	allRightAnswer?: TaskAnswer[]
	correlations?: Correlation[]
}
export interface TaskAnswer {
	textValue?: string
	file?: Uint8Array
	fileName?: string
	isRight?: boolean
	taskId: number
	id: number
}
export interface Correlation {
	left: string
	right: string
    id: number
    rightId: number
}
export interface ContentBlockType {
	id: number
	name: string
}
export interface ContentBlock {
	contentBlockTypeId: number
	order: number
	id: number
	textValue?: string
	fileData?: string
	tempId: string
	fileNameView?: string
	name: string
}
export interface Module {
	id: number
	name: string
	description?: string
	courseId: number
	lessonsCount?: number
	lessonsHaveOrder?: boolean
	order: number
}
export interface ProblemDetails {
	status: number
	title: string
}
export interface Role {
	id: number
	name: string
}
export interface LessonType {
	id: number
	name: string
}
export interface UserInformation {
	userId: number
	lastName?: string
	middleName?: string
	firstName?: string
	position?: string
	phone?: string
}
export interface User {
	id: number
	email: string
	role: Role
	userInformation: UserInformation
}
export interface UserEditable {
	id: number
	email: string
    roleId: number
    password?:string
	userInformation: UserInformation
}

export interface SignInResponse {
	token: string
	user: User
}
