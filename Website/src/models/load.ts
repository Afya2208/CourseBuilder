import type { LoadStatus } from "./loadStatus"

export type Load <T, E> = {
    data: T | null
    status: LoadStatus 
    error: E | null
}