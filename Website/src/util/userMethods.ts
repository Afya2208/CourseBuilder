import type { User } from '@/models/main'
import api from '@/services/api'

export async function tryGetUser(): Promise<User | null> {
    try {
        let user: User | null = null
        const userId = Number.parseInt(sessionStorage.getItem('userId') ?? 'NaN')
        const token = sessionStorage.getItem('token')
        if (Number.isNaN(userId) || !token) {
            sessionStorage.removeItem('token')
            sessionStorage.removeItem('userId')
            return null
        }
        await api.get<User>('users/' + userId, { headers: { Authorization: 'Bearer ' + token } })
            .then((res) => {
                user = res.data
            })
            .catch((err) => {
                sessionStorage.removeItem('token')
                sessionStorage.removeItem('userId')
            })

        return user
    } 
    catch (err) {
        return null
    }
}
