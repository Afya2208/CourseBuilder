<script lang="ts" setup>
import type { Task, TaskType } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import TaskView from '@/views/pages/courses-content/TaskView.vue'
import TaskForm from '@/views/pages/forms/TaskForm.vue'
import { onMounted, ref, toRefs } from 'vue'
import type { ComponentExposed } from 'vue-component-type-helpers'
import { useRoute } from 'vue-router'
import ContentBlockView from '@/views/pages/courses-content/ContentBlockView.vue'

const props = defineProps<{
	isEditing: boolean
	taskTypes: TaskType[]
	tasks: Task[]
	userIsOwner: boolean
}>()

const emits = defineEmits<{
	taskChanged: [t: Task]
}>()
const { user } = useUserStore()
const { tasks, taskTypes, userIsOwner, isEditing } = toRefs(props)

const lessonId = Number.parseInt(useRoute().params.lessonId as string)
const courseId = Number.parseInt(useRoute().params.courseId as string)
const moduleId = Number.parseInt(useRoute().params.moduleId as string)

const taskForm = ref<ComponentExposed<typeof TaskForm>>()
const createTask = () => taskForm.value?.startCreatingTask()
const editTask = (task: Task) => taskForm.value?.startEditingTask(task)

const orderChanged = ref(false)
const refsToTaskViews = ref<ComponentExposed<typeof TaskView>[]>([])

const onTaskFormSaved = async (t: Task) => {
	emits('taskChanged', t)
}

const deleteTask = async (task: Task, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить задачу?`)) {
		if (task.id != 0) {
			await api.delete('tasks/' + task.id).then((res) => {
				tasks.value.splice(index, 1)
			})
		} else {
			tasks.value.splice(index, 1)
		}
	}
}

const checkTasks = async () => {
	for (const x of refsToTaskViews.value) {
		await x.checkTask()
	}
	const details = refsToTaskViews.value.map((x) => {
		return {
			userTryId: 0,
			taskId: x.theTask.id,
			isSolved: x.userAnswer.isRight,
			score: x.theTask.score,
		}
	})
	let userGotScore = 0
	let maxScore = 0
	for (let d of details) {
		maxScore += d.score
		if (d.isSolved) {
			userGotScore += d.score
		}
	}
	if (!userIsOwner.value) {
		await api.post(`progress/${user?.id}/tried-lesson/${lessonId}`, details).then((res) => {
			alert('Прогресс сохранен')
		})
	}
	scoreText.value = `Вы получили ${userGotScore} из ${maxScore} баллов`
}
const scoreText = ref<string>(null)
const syncOrders = () => {
	tasks.value.forEach((card, i) => {
		card.order = i + 1
	})
}

function move(fromIndex: number, toIndex: number) {
	if (toIndex < 0 || toIndex >= tasks.value.length || fromIndex === toIndex) return
	orderChanged.value = true
	const arr = tasks.value
	const [item] = arr.splice(fromIndex, 1)
	arr.splice(toIndex, 0, item)
}

const saveOrder = async () => {
	syncOrders()
	await api
		.post(
			'/tasks/order',
			tasks.value.map((x) => {
				return { id: x.id, order: x.order }
			}),
		)
		.then((res) => {
			alert('Порядок заданий сохранен')
			orderChanged.value = false
		})
}
</script>

<template>
	<div class="py-3">
		<div v-if="userIsOwner && isEditing" class="d-flex flex-wrap justify-content-center gap-2">
			<button class="btn btn-primary" @click="createTask">Добавить задание</button>
			<button v-if="orderChanged" class="btn btn-outline-info" @click="saveOrder">
				Сохранить порядок заданий
			</button>
		</div>

		<TaskForm
			ref="taskForm"
			:task-types="taskTypes"
			@saved="onTaskFormSaved"
			:lesson-id="lessonId"
		/>

		<div class="lesson-tasks-div" v-if="tasks.length > 0">
			<TaskView
				:tasksCount="tasks.length"
				:index="index"
				:key="task.id"
				@downClicked="move(index, index + 1)"
				@upClicked="move(index, index - 1)"
				@start-edit="(t) => editTask(t)"
				@delete-click="(t) => deleteTask(t, index)"
				:is-editing="isEditing"
				v-for="(task, index) in tasks"
				:task="task"
				:ref="
					(ref) => {
						if (ref) refsToTaskViews[index] = ref
					}
				"
			/>
			<button class="btn btn-primary my-2" v-if="!isEditing" @click="checkTasks">
				Проверить ответы
			</button>
		</div>

		<p class="text-center m-2" v-else>Заданий в данном занятии нет.</p>
		<p class="text-center m-2" v-if="scoreText">{{ scoreText }}</p>
	</div>
</template>
