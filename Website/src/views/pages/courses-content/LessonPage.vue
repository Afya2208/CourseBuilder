<script setup lang="ts">
import type { Lesson, ContentBlock, TaskType, Task } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { HttpStatusCode } from 'axios'
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import LessonContentTab from './LessonContentTab.vue'
import LessonTasksTab from './LessonTasksTab.vue'
import type { ComponentExposed } from 'vue-component-type-helpers'

const lessonId = Number.parseInt(useRoute().params.lessonId as string)
const courseId = Number.parseInt(useRoute().params.courseId as string)
const moduleId = Number.parseInt(useRoute().params.moduleId as string)
const lesson = ref<Lesson>()
const contentTab = ref<ComponentExposed<typeof LessonContentTab>>()
const taskTab = ref<ComponentExposed<typeof LessonTasksTab>>()

const { user } = useUserStore()
const userIsOwner = ref(false)
const isEditing = ref(false)

const refresh = async () => {
	window.location.reload()
}
const router = useRouter()

const nextLessonPath = ref('')
const previousLessonPath = ref('')

const taskTypes = ref<TaskType[]>()
const currentTab = ref<'Tasks' | 'Content'>('Content')
const contentBlocks = ref<ContentBlock[]>()
const tasks = ref<Task[]>()

const loadLessonAsync = async () => {
	await api.get<Lesson>(`lessons/${lessonId}`).then((res) => {
		lesson.value = res.data
	})
	await api.get<boolean>(`courses/${courseId}/is-created-by/${user?.id}`).then((res) => {
		userIsOwner.value = res.data
	})
	await api.get<boolean>(`lessons/${lessonId}/done-by/${user?.id}`).then((res) => {
		lessonIsDone.value = res.data
	})
}

const loadNearLessonsLinksAsync = async () => {
	await api
		.get<string>(`courses/${courseId}/modules/${moduleId}/lessons/${lessonId}/next-lesson-id`)
		.then((res) => {
			if (res.status != HttpStatusCode.NoContent) {
				nextLessonPath.value = res.data
			}
		})
		.catch((err) => console.log('Ошибка, get next lesson'))

	await api
		.get<string>(
			`courses/${courseId}/modules/${moduleId}/lessons/${lessonId}/previous-lesson-id`,
		)
		.then((res) => {
			if (res.status != HttpStatusCode.NoContent) {
				previousLessonPath.value = res.data
			}
		})
		.catch((err) => console.log('Ошибка, get prev lesson'))

	if (previousLessonPath.value) {
		let t = previousLessonPath.value.split('/')
		let lessId = t[t.length - 1]
		let done = false
		let haveOrder = false
		let prevLessTasksCount = 0
		await api.get<Task[]>(`lessons/${lessId}/tasks`).then((res) => {
			prevLessTasksCount = res.data.length
		})
		await api.get<boolean>(`lessons/${lessId}/done-by/${user?.id}`).then((res) => {
			done = res.data
		})
		await api.get<boolean>(`modules/${moduleId}/have-order`).then((res) => {
			haveOrder = res.data
		})
		if (!userIsOwner.value && !done && prevLessTasksCount > 0 && haveOrder) {
			alert('Предыдущее занятие не выполнено')
		}
		if (false) {
			router.push(
				`/courses/${courseId}/modules/${moduleId}/lessons/${lessonId}/previous-lesson-id`,
			)
			return
		}
	}
}

const loadTasksAsync = async () => {
	await api.get<Task[]>(`lessons/${lessonId}/tasks`).then((res) => {
		tasks.value = res.data
	})
}

const loadTasksTypesAsync = async () => {
	await api.get<TaskType[]>(`task-types`).then((res) => {
		taskTypes.value = res.data.filter((x) => x.id != 2 && x.id != 6)
	})
}

const loadContentBlocksAsync = async () => {
	await api.get<ContentBlock[]>(`lessons/${lessonId}/content-blocks`).then((res) => {
		contentBlocks.value = res.data
	})
}
const taskChanged = async (task: Task) => {
	await loadTasksAsync()
}
const lessonIsDone = ref(false)

onMounted(async () => {
	await loadLessonAsync()
	await loadNearLessonsLinksAsync()
	currentTab.value = lesson.value?.lessonTypeId == 1 ? 'Content' : 'Tasks'
	await loadContentBlocksAsync()
	await loadTasksTypesAsync()
	await loadTasksAsync()
})
</script>

<template>
	<div class="container" v-if="lesson">
		<div class="row">
			<div class="col">
				<h3 class="my-2">{{ lesson.name }}</h3>
				<p>
					{{ lesson.description }}
					<br />
					<span class="small text-secondary" v-if="lesson.isRequired">
						Это занятие обязательное для зачета и учитывается в прогрессе.
					</span>
					<br />
					<span class="text-primary fw-bold" v-if="lessonIsDone">Занятие выполнено</span>
				</p>
			</div>
			<div class="col text-right mt-2">
				<p v-if="nextLessonPath">
					<a class="btn btn-outline-primary" :href="`/${nextLessonPath}`"
						>Перейти к следующему занятию ➡️</a
					>
				</p>
				<p v-if="previousLessonPath">
					<a class="btn btn-outline-secondary" :href="`/${previousLessonPath}`"
						>Перейти к предыдущему занятию ⬅️</a
					>
				</p>
				<p>
					<RouterLink
						class="btn btn-outline-dark"
						:to="`/courses/${courseId}/modules/${lesson.moduleId}`"
					>
						Вернуться к списку занятий модуля
					</RouterLink>
				</p>
			</div>
		</div>

		<div class="d-flex flex-wrap justify-content-center m-2" v-if="userIsOwner">
			<button
				@click="isEditing = !isEditing"
				class="btn my-2"
				:class="{ 'btn-success': isEditing, 'btn-outline-success': !isEditing }"
			>
				<span v-if="isEditing">Режим редактирования</span>
				<span v-else>Режим просмотра</span>
			</button>
			<button @click="refresh" class="btn btn-outline-secondary m-2">
				Обновить страницу
			</button>
		</div>

		<ul class="nav nav-tabs justify-content-center">
			<li
				v-if="lesson?.lessonTypeId == 1"
				class="nav-item mx-2"
				@click="currentTab = 'Content'"
			>
				<a class="nav-link" :class="{ active: currentTab == 'Content' }">
					<h3>Теория</h3>
				</a>
			</li>
			<li class="nav-item" @click="currentTab = 'Tasks'">
				<a class="nav-link" :class="{ active: currentTab == 'Tasks' }">
					<h3>Задания</h3>
				</a>
			</li>
		</ul>

		<LessonContentTab
			:userIsOwner="userIsOwner"
			:content-blocks="contentBlocks ?? []"
			ref="contentTab"
			:is-editing="isEditing"
			v-if="currentTab == 'Content'"
		/>

		<LessonTasksTab
			:userIsOwner="userIsOwner"
			:task-types="taskTypes ?? []"
			:tasks="tasks ?? []"
			ref="taskTab"
			@taskChanged="taskChanged"
			:is-editing="isEditing"
			v-if="currentTab == 'Tasks'"
		/>
	</div>
</template>

<style scoped></style>
