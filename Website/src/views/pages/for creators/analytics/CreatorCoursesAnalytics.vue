<script lang="ts" setup>
import type { Course } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { onMounted, ref } from 'vue'
import type { CourseAnalytics } from '@/models/courseAnalytics.ts'
import CourseStatChart from '@/views/pages/components/CourseStatChart.vue'

const currentCourseIndex = ref<number>()
const courses = ref<Course[]>()
const coursesAnalytics = ref<CourseAnalytics[]>()

const { user } = useUserStore()

const loadDataAsync = async () => {
	await api.get<Course[]>('courses/by-user/' + user?.id + '/short').then((res) => {
		courses.value = res.data
		coursesAnalytics.value = []
		for (let i = 0; i < courses.value?.length; i++) {
			let course = courses.value[i]
			api.get<CourseAnalytics[]>('analytics/courses/' + course?.id).then((aa) => {
				coursesAnalytics.value[i] = aa.data
			})
		}
	})
}

onMounted(async () => {
	await loadDataAsync()
})
</script>

<template>
	<div>
		<h5>Общая статистика:</h5>
		<p>Всего курсов: {{ courses?.length }}</p>
		<h5>
			Статистика по курсу
			<select v-model="currentCourseIndex">
				<option v-for="(course, index) in courses" :value="index + 1">
					{{ course.name }}
				</option>
			</select>
		</h5>
		<div v-if="currentCourseIndex && coursesAnalytics">
			<p>
				Сколько раз приобрели отдельно:
				{{ coursesAnalytics[currentCourseIndex-1].individualCount }}
				<br />
				Сколько раз приобрели в наборах:
				{{ coursesAnalytics[currentCourseIndex-1].fromKitsCount }}
				<br />
				Сколько раз завершили:
				{{ coursesAnalytics[currentCourseIndex-1].doneCount }}
			</p>
			<CourseStatChart
				:doneCount="coursesAnalytics[currentCourseIndex-1].doneCount"
				:individual-count="coursesAnalytics[currentCourseIndex-1].individualCount"
				:from-kits-count="coursesAnalytics[currentCourseIndex-1].fromKitsCount"
			/>
		</div>
	</div>
</template>
