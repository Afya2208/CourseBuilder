import type {SignInResponse } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

export function authorizationLogic() {

    const email = ref('')
    const router = useRouter()
    const route = useRoute()
    const password = ref('')
    const continueClick = () => {
        localStorage.setItem('no-account', 'true')
        router.push('/')
    }
    const authClick = async () => {
        const signInRequest = {
            email: email.value,
            password: password.value,
        }
        await api
        .post<SignInResponse>('sign-in', signInRequest)
        .then((res) => {
            const signInResponse = res.data
            api.defaults.headers.common = { Authorization: `Bearer ${signInResponse.token}` }
            localStorage.setItem('token', signInResponse.token)
            localStorage.setItem('userId', signInResponse.user.id.toString())
            useUserStore()
                .updateUserData()
                .then(() => {
                    let p = route.query.redirect as string || '/'
                    if (p.startsWith('/') && !p.startsWith('//')) {
                        router.push({ path: p })
                    }
                    else {
                        router.push({ path: '/' })
                    }
                })
        })
        .catch((err) => {
            if (err.status == 401) {
                alert('Неправильный пароль или почта')
            }
        })
    }
    return {continueClick, authClick, email, password};
}