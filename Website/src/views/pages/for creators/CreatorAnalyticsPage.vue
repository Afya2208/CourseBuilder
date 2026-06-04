<script lang="ts" setup>
import { useUserStore } from '@/stores/user'
import { onMounted, ref } from 'vue'
import CreatorCoursesAnalytics from './analytics/CreatorCoursesAnalytics.vue'
import CuratorGroupAnalyticsTab from '@/views/pages/for creators/analytics/CuratorGroupAnalyticsTab.vue'
import KitsAnalyticsTab from '@/views/pages/for creators/analytics/KitsAnalyticsTab.vue'

const { user } = useUserStore()
onMounted(async () => {
	await getDataAsync()
})
const getDataAsync = async () => {}

const currentOption = ref<'Courses' | 'Groups' | 'Kits'>('Courses')
</script>

<template>
	<div class="container">
		<h3 class="my-2">Аналитика и отчетность</h3>
		<p>
			Информация по курсам
			<!--
			<select v-model="currentOption">
				<option value="Courses">Курсам</option>
				<option value="Groups">Группам</option>
				 <option value="Kits">Наборам</option>
			</select>
			-->
		</p>
		<CreatorCoursesAnalytics v-if="currentOption == 'Courses'" />
		<CuratorGroupAnalyticsTab v-if="currentOption == 'Groups'" />
		<KitsAnalyticsTab v-if="currentOption == 'Kits'" />
	</div>
</template>
