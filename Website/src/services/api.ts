import router from '@/router'
import { useUserStore } from '@/stores/user'
import axios, { AxiosError, type AxiosResponse } from 'axios'

const api = axios.create({
	baseURL: 'http://localhost:5555',
})

api.interceptors.request.use(

	(config) => {
		const userStore = useUserStore()
		if (userStore.token && userStore.user) {
			config.headers.Authorization = `Bearer ${userStore.token}`
		}
		return config
	},
    (error) => {
        if (error.code == "ERR_NETWORK") {
            
        }
		return Promise.reject(error)
	},
)

api.interceptors.response.use(
	(response: AxiosResponse) => {
		return response 
	},
    (error: AxiosError) => {
        if (error.code == "ERR_NETWORK") {
            
        }
		if (error.status == 401) {
			const userStore = useUserStore()
			if (userStore.token) {
				userStore.logOut()
				alert('Сессия окончена. Войдите в профиль снова')
				router.push('/auth')
			}
		}
		return Promise.reject(error) 
	},
)

export default api
