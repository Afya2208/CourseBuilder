<script lang="ts" setup>
import type { ProblemDetails, User } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const email = ref('')
const passwordHidden = ref(true)
const router = useRouter()
const password = ref('')
const authClick = async () => {
  await api.post<User>('sign-in', { email: email.value, password: password.value })
    .then((res) => {
    if (res.data) {
      api.defaults.headers.common = { Authorization: `Bearer ${res.data.token}` }
      sessionStorage.setItem('token', res.data.token)
      sessionStorage.setItem('userId', res.data.userId.toString())
      useUserStore().signIn(res.data)
      router.push({ path: '/' })
    }})
}
</script>

<template>
  <div class="container text-center" style="max-width: 300px;">
    <form>
        <h1 class="h3 mb-3 fw-normal">Авторизация</h1>
        <div class="form-floating my-1 ">
            <input v-model="email" type="email" id="email" class="form-control" placeholder="example@domain.com"/>
            <label for="email">Email</label>
        </div>
         <div class="form-floating my-1">
            <input type="password" id="password" class="form-control" placeholder="Пароль"/>
            <label for="password">Пароль</label>
        </div>
      <button @click="authClick" type="button" class="my-button btn btn-lg btn-primary my-1">Войти</button>
      <p class="mt-5 mb-3 text-muted">Если у Вас нет профиля, то обратитесь к администрации</p>
    </form>
</div>
</template>
