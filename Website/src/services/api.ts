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
		const userStore = useUserStore()
		if (userStore.token && userStore.user) {
			config.headers.Authorization = `Bearer ${userStore.token}`
		}
		return config
	},
    (error) => {
        if (error.code == "ERR_NETWORK") {
            alert("Ошибка: не получается подключиться к серверу")
        }
		return Promise.reject(error)
	},
)
// что делать при ответе ЗАРАНЕЕ ДО ВЫДАЧИ ОТВЕТА
api.interceptors.response.use(
	(response: AxiosResponse) => {
		return response // дальнейшая передача ответа до места запроса
	},
    (error: AxiosError) => {
        if (error.code == "ERR_NETWORK") {
            alert("Ошибка: не получается подключиться к серверу")
        }
		if (error.status == 401) {
			const userStore = useUserStore()
			if (userStore.token) {
				userStore.logOut()
				alert('Сессия окончена. Войдите в профиль снова')
				router.push('/auth')
			}
		}
		return Promise.reject(error) // дальнейшая передача error до места запроса
	},
)

export default api
