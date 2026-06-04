<script lang="ts" setup>
import { useUserStore } from '@/stores/user'
import { ref } from 'vue'
import CreatorCoursesPage from './for creators/CreatorCoursesPage.vue'
import AvailableCoursesPage from './for students/AvailableCoursesPage.vue'
import CreatorKitsPage from './for creators/CreatorKitsPage.vue'
import AvailableKitsPage from './for students/AvailableKitsPage.vue'

const currentTab = ref<'CreatedCourses' | 'AvailableCourses' | 'CreatedKits' | 'AvailableKits'>(
	'AvailableCourses',
)
const { user } = useUserStore()
if (user?.role.id == 1) {
	currentTab.value = 'CreatedCourses'
}
</script>

<template>
	<div class="container">
		<h3 class="m-3">Мои курсы</h3>

		<ul class="nav nav-tabs gap-2 my-2">
			<li v-if="user?.role.id == 3" class="nav-item" @click="currentTab = 'AvailableCourses'">
				<a class="nav-link" :class="{ active: currentTab == 'AvailableCourses' }">
					<span>Доступные курсы для изучения</span>
				</a>
			</li>
			<li v-if="user?.role.id == 1" class="nav-item" @click="currentTab = 'CreatedCourses'">
				<a class="nav-link" :class="{ active: currentTab == 'CreatedCourses' }">
					<span>Курсы, созданные мною</span>
				</a>
			</li>
			<li v-if="user?.role.id == 3" class="nav-item" @click="currentTab = 'AvailableKits'">
				<a class="nav-link" :class="{ active: currentTab == 'AvailableKits' }">
					<span>Приобретённые наборы</span>
				</a>
			</li>
			<li v-if="user?.role.id == 1" class="nav-item" @click="currentTab = 'CreatedKits'">
				<a class="nav-link" :class="{ active: currentTab == 'CreatedKits' }">
					<span>Созданные наборы</span>
				</a>
			</li>
		</ul>

		<CreatorCoursesPage v-if="currentTab == 'CreatedCourses'" />
		<AvailableCoursesPage v-else-if="currentTab == 'AvailableCourses'" />
		<AvailableKitsPage v-else-if="currentTab == 'AvailableKits'" />
		<CreatorKitsPage v-else-if="currentTab == 'CreatedKits'" />
	</div>
</template>
