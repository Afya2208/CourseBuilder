<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { useUserStore } from './stores/user'
import { tryGetUser } from './util/userMethods'
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import api from './services/api'

// хранилище данных пользователя
const { user, fullName } = storeToRefs(useUserStore())
// при каждой загрузке App, то есть всего сайта
onMounted(async () => {
  // пробуем загрузить данные пользователя из хранилища браузера
  await tryGetUser().then((response) => {
    useUserStore().saveUser(response)
    api.defaults.headers.common.Authorization = `Bearer ${sessionStorage.getItem('token')}`
  })
})
</script>

<template>
  <header>
    <div class="px-3 py-2 bg-dark text-white">
      <div class="container">
        <div
          class="d-flex flex-wrap align-items-center justify-content-center justify-content-lg-start"
        >
          <a
            href="/"
            class="d-flex align-items-center my-2 my-lg-0 me-lg-auto text-white text-decoration-none"
          >
            CourseBuilder
          </a>
          <!--
          
          <ul class="nav col-12 col-lg-auto my-2 justify-content-center my-md-0 text-small">
            <li>
              <a href="#" class="nav-link text-secondary">
                IMAGE
                Home
              </a>
            </li>
            <li>
              <a href="#" class="nav-link text-white">
                IMAGE
                Dashboard
              </a>
            </li>
            <li>
              <a href="#" class="nav-link text-white">
                IMAGE
                Orders
              </a>
            </li>
            <li>
              <a href="#" class="nav-link text-white">
                <img class="d-block mx-auto mb-1" src="/src/assets/logo.svg" width="24" height="24"></img>
                Products
              </a>
            </li>
            <li>
              <a href="#" class="nav-link text-white">
                IMAGE
                Customers
              </a>
            </li>
          </ul>
          -->
        </div>
      </div>
    </div>

    <nav class="px-3 py-2 bg-light border-bottom">
      <div class="container d-flex flex-wrap">
        <ul class="nav me-auto">
          <li class="nav-item">
            <RouterLink class="nav-link link-dark pr-2" to="/">Курсы</RouterLink>
          </li>
          <li class="nav-item">
            <RouterLink class="nav-link link-dark px-2" to="/admin">Администрирование</RouterLink>
          </li>
          <li class="nav-item">
            <a href="#" class="nav-link link-dark px-2 active" aria-current="page">Главная</a>
          </li>
        </ul>
        <ul class="nav">
          <li class="nav-item">
            <RouterLink class="nav-link link-dark px-2" to="/auth">Авторизация</RouterLink>
          </li>
        </ul>
      </div>
    </nav>

    ---------------------------
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
