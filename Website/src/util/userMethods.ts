import type { User } from '@/models/main'
import api from '@/services/api'

export async function tryGetUser(): Promise<User> {
    try {
        let user: User 
        const userId = Number.parseInt(sessionStorage.getItem('userId') ?? 'NaN')
        const token = sessionStorage.getItem('token')
        if (Number.isNaN(userId) || !token) {
            throw "Нет данных"
        }
        user = await (await api.get<User>('users/' + userId, { headers: { Authorization: 'Bearer ' + token } })).data
        return user
    } catch (err) {
        sessionStorage.removeItem('token')
        sessionStorage.removeItem('userId')
        return Promise.reject(err)
    }
}
