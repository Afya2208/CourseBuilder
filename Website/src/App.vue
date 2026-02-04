<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { useUserStore } from './stores/user'
import { tryGetUser } from './util/userMethods'
import { onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import api from './services/api'
import { BApp } from 'bootstrap-vue-next'
import router from './router'

// хранилище данных пользователя
const { user, fullName } = storeToRefs(useUserStore())
// при каждой загрузке App, то есть всего сайта
onMounted(async () => {
	// пробуем загрузить данные пользователя из хранилища браузера
	await useUserStore()
		.init()
		.then((res) => {})
})
const logout = () => {
	if (confirm('Вы точно хотите выйти из профиля?')) {
		delete api.defaults.headers.common.Authorization
		useUserStore().logOut()
		router.push('/')
	}
}
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
				</div>
			</div>
		</div>
		<nav class="px-3 py-2 bg-light border-bottom">
			<div class="container d-flex flex-wrap align-items-center">
				<div class="col">
					<ul class="nav nav-underline">
						<li class="nav-item">
							<RouterLink class="nav-link link-dark pr-2" active-class="active" to="/"
								>Главная</RouterLink
							>
						</li>
						<li class="nav-item">
							<RouterLink
								class="nav-link link-dark pr-2"
								active-class="active"
								to="/courses"
								>Курсы</RouterLink
							>
						</li>
						<li class="nav-item" v-if="user && user.role.id == 2">
							<RouterLink class="nav-link link-dark px-2" to="/admin"
								>Администрирование</RouterLink
							>
						</li>
					</ul>
				</div>
				<div class="col">
					<ul class="nav justify-content-end">
						<li v-if="!user" class="nav-item">
							<RouterLink class="nav-link link-dark px-2" to="/auth"
								>Авторизация</RouterLink
							>
						</li>
						<button v-else class="btn btn-secondary px-2" @click="logout">
							Выйти из профиля
						</button>
						<p v-if="user" class="link-dark px-2 m-0">
							{{ fullName }}
							<br />
							<span v-if="user.userInformation.position"
								>{{ user.userInformation.position }} -
							</span>
							<span>{{ user.role.name }}</span>
						</p>
					</ul>
				</div>
			</div>
		</nav>
	</header>
	<main>
		<BApp>
			<RouterView />
		</BApp>
	</main>
</template>

<style scoped></style>
