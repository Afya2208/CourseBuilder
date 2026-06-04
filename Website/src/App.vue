<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { useUserStore } from './stores/user'
import { onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import api from './services/api'
import { BApp, BLink } from 'bootstrap-vue-next'
import router from './router'
import MainUpperMenu from '@/views/pages/components/MainUpperMenu.vue'
import MainFooter from '@/views/pages/components/MainFooter.vue'

// при каждой загрузке App, то есть всего сайта
onMounted(async () => {
	// пробуем загрузить данные пользователя из хранилища браузера
	await useUserStore()
		.init()
		.then((res) => {})
})
</script>

<template>
	<header>
		<MainUpperMenu />
	</header>
	<main style="min-height: 81vh">
		<BApp>
			<RouterView />
		</BApp>
	</main>
	<footer class="bg-light py-4">
		<MainFooter />
	</footer>
</template>
