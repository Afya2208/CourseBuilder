export type Kit = {
    id: number
    name: string
    description: string
    authorId?: number
    price: number,
    selectedCoursesId?: number[]
    courses: {
        id: number,
        name?: string
    }[]
}