<script lang="ts" setup>
import type { ProblemDetails, SignInResponse } from '@/models/main'
import api from '@/services/api'
import { useCounterStore } from '@/stores/counter'
import { useUserStore } from '@/stores/user'
import axios from 'axios'
import { ref } from 'vue'
import { useRouter } from 'vue-router'

let email = ref('')
let passwordHidden = ref(true)
let router = useRouter()
let password = ref('')
let authClick = async () => {
  await api.post<SignInResponse>("sign-in", {email: email.value, password:password.value}).then(res=>{
    if (res.data != undefined) {
      sessionStorage.setItem("token", res.data.token)
      sessionStorage.setItem("userId", res.data.userId.toString())
      api.interceptors.request
      router.push({path:"/"})
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
        <input v-model="password" :type="[passwordHidden? 'password':'text']" placeholder="Пароль..." />
      </label>
      <input type="button" @click="passwordHidden = !passwordHidden" :value="[passwordHidden? 'показать':'скрыть']" />
      <br />
      <button @click="authClick">Войти</button>
    </fieldset>
  </div>
</template>

<style></style>
