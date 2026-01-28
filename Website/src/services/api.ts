import type { ProblemDetails } from '@/models/main'
import axios, { AxiosError, type AxiosResponse } from 'axios'
import { useRouter } from 'vue-router'

const api = axios.create({
  baseURL: 'http://localhost:5555',
})
// что делать при каждом запросе ЗАРАНЕЕ ДО ЗАПРОСА
api.interceptors.request.use(
  // допольнительные настройки для запросов
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
    console.error('ойойойой...')
    if (error.response) {
      let problemDetails = error.response.data as ProblemDetails
      console.error(JSON.stringify(problemDetails))
      alert(problemDetails.title)
    }
    return Promise.reject(error) // дальнейшая передача error до места запроса
  },
)

export default api

function handleAPIError(error: AxiosError) {
  if (error.status == 401) {
    console.log('Сессия закончена, требуется повторная авторизация')
    const router = useRouter()
    router.push({ path: '/' })
  }
  let problemDetails: ProblemDetails = {
    title: 'Ошибка',
    status: 400,
  }
  if (error.response) {
    console.log('Сессия закончена, sasaddsasadssadasdтребуется повторная авторизация')
    problemDetails = error.response.data as ProblemDetails
    alert(problemDetails.title)
  }
}
