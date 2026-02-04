<script lang="ts" setup>
import type { Task, TaskEditable, TaskType } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref } from 'vue'

const taskTypes = ref<TaskType[]>([])
const tasks = ref<TaskEditable[]>([])
const addTask = () => {
	tasks.value.push({
		taskTypeId: 1,
		order: tasks.value.length + 1,
		id: 0,
		question: '',
		textAnswer: '',
	})
}
const deleteTask = (index: number) => {
	tasks.value.splice(index, 1)
}
const deleteCorrelation = (task: TaskEditable, index: number) => {
	if (task.correlations) {
		task.correlations.splice(index, 1)
	}
}
const fetch = async () => {
	await api.get<TaskType[]>('task-types').then((res) => {
		taskTypes.value = res.data
	})
}
const addCorrelation = (task: TaskEditable) => {
	if (!task.correlations) {
		task.correlations = []
	}
	task.correlations.push({
		left: '',
		right: '',
		id: 0,
	})
}
onMounted(async () => {
	await fetch()
})
</script>

<template>
	<div>
		<div>
			<button @click="addTask">Добавить задание</button>
		</div>
		<div>
			<div v-for="(task, index) in tasks" :style="{ order: task.order }">
				<div>
					<button @click="deleteTask(index)">Удалить</button>
					<span> Тип </span>
					<select v-model="task.taskTypeId">
						<option v-for="type in taskTypes" :value="type.id">{{ type.name }}</option>
					</select>
				</div>
				<div>
					<p>
						<input v-model="task.question" placeholder="Вопрос" />
					</p>
				</div>
				<div v-if="task.taskTypeId == 1">
					<p>
						<input v-model="task.textAnswer" placeholder="Ответ" />
					</p>
				</div>
				<div v-if="task.taskTypeId == 2">
					<p>Ответ на данный вопрос будет проверен назначенным преподавателем</p>
				</div>
				<div v-if="task.taskTypeId == 3">
					<div>
						<button @click="addCorrelation(task)">Добавить соотношение</button>
					</div>
					<table>
						<tbody>
							<tr v-for="(correlation, index) in task.correlations">
								<td>
									<input placeholder="Левая часть" v-model="correlation.left" />
								</td>
								<td>
									<input placeholder="Правая часть" v-model="correlation.right" />
								</td>
								<td>
									<button @click="deleteCorrelation(task, index)">Удалить</button>
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</div>
		</div>
	</div>
</template>

<style scoped></style>
