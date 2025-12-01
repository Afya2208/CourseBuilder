import type { User } from '@/models/main'
import api from '@/services/api'

export async function getUserData(): Promise<User | null> {
  try {
    let user: User | null = null
    const userId = Number.parseInt(sessionStorage.getItem('userId') ?? 'NaN')
    const token = sessionStorage.getItem('token')
    if (Number.isNaN(userId) || !token) {
      return null
    }
    await api
      .get<User>('users/' + userId, { headers: { Authorization: 'Bearer ' + token } })
      .then((res) => {
        user = res.data
      })

    return user
  } catch (err) {
    console.error(err)
    return null
  }
}
