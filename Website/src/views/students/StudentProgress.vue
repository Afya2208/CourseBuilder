<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { onMounted, ref } from 'vue'
import type { Course } from '@/models/main.ts'
import api from '@/services/api.ts'
import { BProgress, BProgressBar } from 'bootstrap-vue-next'
import { useUserStore } from '@/stores/user.ts'
import type { CourseProgress } from '@/models/courseProgress.ts'

const courses = ref<Course[]>()
const progresses = ref<CourseProgress[]>()
const { user } = useUserStore()
const loadCoursesAsync = async () => {
	await api.get<Course[]>(`courses/available-for/${user?.id}/short`).then((res) => {
		courses.value = res.data
		progresses.value = []
		for (let i = 0; i < courses.value?.length; i++) {
			let course = courses.value[i]
			api.get(`progress/${user?.id}/course/${course.id}`).then((pr) => {
				progresses.value[i] = pr.data
			})
		}
	})
}
onMounted(async () => {
	await loadCoursesAsync()
})
</script>

<template>
	<div class="container">
		<h3 class="mt-3" id="header">Прогресс по курсам</h3>

		<div class="d-flex flex-wrap my-3 gap-2">
			<div :key="course.id" class="card p-2" v-for="(course, index) in courses">
				<div class="card-title">
					{{ course.name }}
				</div>
				<p>
					<span v-if="progresses[index].status != 'NoRequiredTasks'"
						>Выполнено обязательных занятий:
						<strong
							>{{ progresses[index].solvedCount }} /
							{{ progresses[index].totalCount }}</strong
						></span
					>
					<span v-else> У курса нет обязательных занятий </span>
				</p>
				<div>
					<BProgress height="2rem">
						<BProgressBar
							v-if="progresses[index].status == 'InProgress'"
							variant="primary"
							:label="`${progresses[index].progressPercent}%`"
							:value="progresses[index].progressPercent"
						></BProgressBar>
						<BProgressBar
							v-else-if="progresses[index].status == 'Completed'"
							variant="success"

							:label="`${progresses[index].progressPercent}%`"
							:value="progresses[index].progressPercent"
						></BProgressBar>
						<BProgressBar
							v-else-if="progresses[index].status == 'NoRequiredTasks'"
							variant="secondary"

							:label="`${progresses[index].progressPercent}%`"
							:value="progresses[index].progressPercent"
						></BProgressBar>
					</BProgress>
				</div>
			</div>
		</div>
	</div>
</template>

<style scoped></style>
