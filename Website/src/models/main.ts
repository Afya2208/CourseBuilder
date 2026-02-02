export interface Course {
    id: number
    name: string
    description?: string
    price?: number
    authorId?: number
    themes?: Theme[]
    themesIds?: number[]
    lessonsCount?: number
    modulesCount?: number,
    modulesHaveOrder?: boolean
    minimalCompletionPercentage?: number
}
export interface CourseEditable {
    id: number
    name: string
    description?: string
    price?: number
    authorId: number
    themesIds: number[]
    modulesHaveOrder: boolean
    minimalCompletionPercentage: number
}

export interface Theme {
    id: number
    name: string
}

export interface TaskType {
    id: number
    name: string
}

export interface CorrelationPart {
    correlationId: number
    value: string
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
    userInformation: UserInformation,
    canRedact?: boolean
}
export interface UserEditable {
    id: number
    email: string
    roleId: number
    userInformation: UserInformation,
    canRedact?: boolean
}

export interface SignInResponse {
    token: string
    user: User
}
