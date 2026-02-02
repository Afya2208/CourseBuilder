import type { ProblemDetails } from '@/models/main'
import router from '@/router'
import { useUserStore } from '@/stores/user'
import axios, { AxiosError, type AxiosResponse } from 'axios'
import { useRouter } from 'vue-router'

const api = axios.create({
    baseURL: 'http://localhost:5555',
})
// что делать при каждом запросе ЗАРАНЕЕ ДО ЗАПРОСА
api.interceptors.request.use(
    // дополнительные настройки для запросов
    (config) => {
        return config
    },
    (error) => {
        return Promise.reject(error)
    },
)
// что делать при ответе ЗАРАНЕЕ ДО ВЫДАЧИ ОТВЕТА
api.interceptors.response.use(
    (response: AxiosResponse) => {
        return response // дальнейшая передача ответа до места запроса
    },
    (error: AxiosError) => {
        if (error.response) {
            let userStore = useUserStore()
            let problemDetails = error.response.data as ProblemDetails
            console.error(JSON.stringify(problemDetails))
            if (userStore.user && problemDetails.status == 401) {
                alert("Закончилась сессия авторизации. Войдите в профиль снова")
                userStore.logOut()
                router.push("/auth")
            }
        }
        return Promise.reject(error) // дальнейшая передача error до места запроса
    },
)

export default api

