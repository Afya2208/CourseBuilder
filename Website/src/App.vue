<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { useUserStore } from './stores/user';
import { getUserData } from './util/userMethods';
import { onMounted, ref } from 'vue';
import { storeToRefs } from 'pinia';

// хранилище данных пользователя
const {user, fullName} = storeToRefs(useUserStore())
// при каждой загрузке App, то есть всего сайта
onMounted(async () => {
  // пробуем загрузить данные пользователя
  let user = await getUserData()
  // сохраняем пользователя
  useUserStore().signIn(user)
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
    <div>
      Пользователь: {{ fullName }}
    </div>
  </header>
  <main>
    <RouterView />
  </main>
</template>

<style scoped></style>
