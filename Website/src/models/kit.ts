export type Kit = {
    id: number
    name: string
    description: string
    authorId?: number
    price: number,
    selectedCoursesId?: number[]
    coursesInfo: {
        id: number,
        name?: string
    }[]
}