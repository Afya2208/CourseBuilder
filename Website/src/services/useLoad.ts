import type { Load } from "@/models/load"
import type { LoadStatus } from "@/models/loadStatus"
import type { AxiosPromise } from "axios"
import { reactive, ref, type UnwrapRef } from "vue"

export function useLoad<T, E = any>() {
    const state = reactive({
        data: <T | undefined>(undefined),
        status: <LoadStatus>('idle'),
        error: <E | undefined>(undefined)
    })

    async function execute(request: AxiosPromise<T>) {
        state.status = 'loading'
        await request
        .then(res => {
            state.data = res.data as UnwrapRef<T>
            state.status = 'success'
        })
            .catch(err => {
            
        });
    }

    return {
        state,
        execute
    }
}