<script lang="ts" setup>
import type { ProblemDetails, SignInResponse, User } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const email = ref('')
const router = useRouter()
const password = ref('')
const authClick = async () => {
    let signInRequest = {
        email: email.value,
        password: password.value,
    }
    await api.post<SignInResponse>('sign-in', signInRequest).then((res) => {
        let signInResponse = res.data
        api.defaults.headers.common = { Authorization: `Bearer ${signInResponse.token}` }
        sessionStorage.setItem('token', signInResponse.token)
        sessionStorage.setItem('userId', signInResponse.user.id.toString())
        useUserStore().updateUserData().then(() => {
            router.push({ path: '/' })
        })  
    })
        .catch(err => {
            if (err.status == 401) {
                alert("Неправильный пароль или почта")
            }
        })
}
</script>

<template>
    <div class="container text-center" style="max-width: 300px">
        <form>
            <h1 class="h3 mb-3 fw-normal">Авторизация</h1>
            <div class="form-floating my-1">
                <input
                    v-model="email"
                    type="email"
                    id="email"
                    class="form-control"
                    placeholder="example@domain.com"
                />
                <label for="email">Email</label>
            </div>
            <div class="form-floating my-1">
                <input
                    type="password"
                    id="password"
                    v-model="password"
                    class="form-control"
                    placeholder="Пароль"
                />
                <label for="password">Пароль</label>
            </div>
            <button @click="authClick" type="button" class="my-button btn btn-lg btn-primary my-1">
                Войти
            </button>
            <p class="mt-5 mb-3 text-muted">Если у Вас нет профиля, то обратитесь к администрации</p>
        </form>
    </div>
</template>
