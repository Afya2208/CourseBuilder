import type { Course } from "./main";

export interface CoursesAndTotalCount {
    courses: Course[],
    totalCount: number
}