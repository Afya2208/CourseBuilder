<script lang="ts" setup>
import type { User } from '@/models/main'
import api from '@/services/api'
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const email = ref('')
const passwordHidden = ref(true)
const router = useRouter()
const password = ref('')
const authClick = async () => {
  await api
    .post<User>('sign-in', { email: email.value, password: password.value })
    .then((res) => {
      if (res.data) {
        api.defaults.headers.common = { Authorization: `Bearer ${res.data.token}` }
        sessionStorage.setItem('token', res.data.token)
        sessionStorage.setItem('userId', res.data.userId.toString())
        router.push({ path: '/' })
      }
    })
}
</script>

<template>
  <div>
    <h1>Авторизация</h1>
    <fieldset>
      <label>
        <span>Почта</span>
        <input v-model="email" placeholder="Почта" />
      </label>
      <label>
        <span>Пароль</span>
        <input
          v-model="password"
          :type="passwordHidden ? 'password' : 'text'"
          placeholder="Пароль..."
        />
      </label>
      <input
        type="button"
        @click="passwordHidden = !passwordHidden"
        :value="[passwordHidden ? 'показать' : 'скрыть']"
      />
      <br />
      <button @click="authClick">Войти</button>
    </fieldset>
  </div>
</template>

<style></style>
