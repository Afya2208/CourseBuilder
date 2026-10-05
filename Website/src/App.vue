<script setup lang="ts">
import { RouterView } from 'vue-router'
import { useUserStore } from './stores/user'
import { onMounted } from 'vue'
import { BApp } from 'bootstrap-vue-next'
import MainUpperMenu from '@/views/components/MainUpperMenu.vue'
import MainFooter from '@/views/components/MainFooter.vue'

onMounted(async () => {
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
