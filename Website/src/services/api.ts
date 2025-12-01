import type { ProblemDetails } from '@/models/main'
import axios from 'axios'
import { useRouter } from 'vue-router'

const api = axios.create({
  baseURL: 'http://localhost:5555',
})
api.interceptors.request.use(
  (config) => {
    const token = sessionStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    if (error.status == 401) {
      console.log('Сессия закончена, требуется повторная авторизация')
      const router = useRouter()
      router.push({ path: '/' })
    }
    if (error.response) {
      const problemDetails: ProblemDetails = error.response.data
      alert(problemDetails.title)
    }
    return Promise.reject(error)
  },
)

export default api
