<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { useUserStore } from './stores/user'
import { getUserData } from './util/userMethods'
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import api from './services/api'

// хранилище данных пользователя
const { fullName } = storeToRefs(useUserStore())
// при каждой загрузке App, то есть всего сайта
onMounted(async () => {
  // пробуем загрузить данные пользователя из хранилища браузера
  const user = await getUserData()
  if (user) {
    // сохраняем пользователя в хранилище приложения
    useUserStore().signIn(user)
    api.defaults.headers.common.Authorization = `Bearer ${sessionStorage.getItem('token')}`
  }
})
</script>

<template>
  <header>
    <div>
      <nav>
        <RouterLink to="/">Курсы</RouterLink>
        <RouterLink to="/auth">Авторизация</RouterLink>
      </nav>
    </div>
    <div>Пользователь: {{ fullName }}</div>
  </header>
  <main>
    <RouterView />
  </main>
</template>

<style scoped></style>
