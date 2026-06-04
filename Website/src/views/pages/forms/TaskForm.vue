<script setup lang="ts">
import type { Correlation, Task, TaskAnswer, TaskEditable, TaskType } from '@/models/main.ts'
import api from '@/services/api.ts'
import { Validator } from '@/util/validator.ts'
import {
	BModal,
	BForm,
	BFormInput,
	BFormFloatingLabel,
	BFormSelect,
	BButton,
	useToggle,
} from 'bootstrap-vue-next'
import { ref } from 'vue'

const props = defineProps<{
	taskTypes: TaskType[]
	lessonId: number
}>()
const emits = defineEmits<{
	saved: [savedTask: Task]
	closed: []
}>()
const { hide: hideTaskModal, show: showTaskModal } = useToggle('task-modal')
const getNewTask = (): TaskEditable => {
	return {
		lessonId: props.lessonId,
		taskTypeId: 1,
		order: 1,
		id: 0,
		question: '',
		score: 1,
	}
}
const task = ref<TaskEditable>(getNewTask())
const startCreatingTask = () => {
	task.value = getNewTask()
	showTaskModal()
}
const startEditingTask = async (taskToEdit: Task) => {
	task.value = {
		id: taskToEdit.id,
		question: taskToEdit.question,
		lessonId: taskToEdit.lessonId,
		taskTypeId: taskToEdit.taskTypeId,
		order: taskToEdit.order,
		score: taskToEdit.score,
	}
	switch (task.value.taskTypeId) {
		case 1: {
			await api.get<TaskAnswer[]>(`tasks/${task.value.id}/answers`).then((res) => {
				task.value.textAnswer = res.data.pop()?.textValue
			})
			break
		}
		case 3: {
			await api.get<Correlation[]>(`tasks/${task.value.id}/correlations`).then((res) => {
				task.value.correlations = res.data
			})
			break
		}
		case 4:
		case 5: {
			await api.get<TaskAnswer[]>(`tasks/${task.value.id}/answers`).then((res) => {
				task.value.allAnswerOptions = res.data
			})
			break
		}
	}
	showTaskModal()
}

const addCorrelation = () => {
	if (!task.value.correlations) {
		task.value.correlations = []
	}
	task.value.correlations.push({
		left: '',
		right: '',
		id: 0,
		rightId: 0,
	})
}
const deleteCorrelation = (index: number) => {
	task.value.correlations?.splice(index, 1)
}
const addAnswer = () => {
	if (!task.value.allAnswerOptions) {
		task.value.allAnswerOptions = []
	}
	task.value.allAnswerOptions.push({
		taskId: task.value.id,
		id: 0,
		isRight: false,
		textValue: '',
	})
}
const deleteAnswer = (index: number) => {
	task.value.allAnswerOptions?.splice(index, 1)
}
const checkFields = (): boolean => {
	const val: Validator = new Validator()
	val.validate(!task.value.question, 'Укажите вопрос')
	val.validate(
		Number.isNaN(Number.parseInt(task.value.order.toString())) || task.value.order < 1,
		'Укажите порядок > 0',
	)
	return val.returnResult()
}
const saveTask = async () => {
	if (checkFields()) {
		task.value.order = Math.trunc(task.value.order)
		const requestBody = {
			id: task.value.id,
			lessonId: props.lessonId,
			order: task.value.order,
			score:task.value.score,
			question: task.value.question,
			taskTypeId: task.value.taskTypeId,
			textAnswer: task.value.textAnswer,
			allAnswerOptions: task.value.allAnswerOptions,
			correlations: task.value.correlations,
		}
		if (requestBody.id == 0) {
			await api
				.post<Task>('tasks', requestBody)
				.then((res) => {
					hideTaskModal()
					emits('saved', res.data)
				})
				.catch((err) => {
					alert('Ошибка, повторите позже')
				})
		} else {
			await api
				.put<Task>('tasks', requestBody)
				.then((res) => {
					hideTaskModal()
					emits('saved', res.data)
				})
				.catch((err) => {
					alert('Ошибка, повторите позже')
				})
		}
	}
}
const close = () => {
	hideTaskModal()
	emits('closed')
}
defineExpose({ startCreatingTask, startEditingTask })
</script>

<template>
	<BModal
		id="task-modal"
		centered
		header-class="bg-dark text-white"
		header-close-class="bg-white"
		@close="close"
		title="Новое задание"
		no-close-on-backdrop
	>
		<BForm>
			<BFormFloatingLabel class="my-2" label="Вопрос задачи" label-for="task-question">
				<BFormInput
					id="task-question"
					v-model="task.question"
					type="text"
					placeholder="Сколько будет 2 + 2?"
				/>
			</BFormFloatingLabel>

			<BFormFloatingLabel
				class="my-2"
				label="Сколько баллов дается за задание"
				label-for="task-score"
			>
				<BFormInput id="task-score" v-model="task.score" type="number" />
			</BFormFloatingLabel>

			<div class="input-group my-2">
				<label class="input-group-text">Тип: </label>
				<select v-model="task.taskTypeId" class="form-select">
					<option v-for="op in taskTypes" :value="op.id" :key="op.id">
						{{ op.name }}
					</option>
				</select>
			</div>

			<div v-if="task.taskTypeId == 1">
				<BFormFloatingLabel class="my-2" label="Ответ" label-for="task-text-answer">
					<BFormInput
						id="task-text-answer"
						v-model="task.textAnswer"
						type="text"
						placeholder="4"
					/>
				</BFormFloatingLabel>
			</div>
			<div v-else-if="task.taskTypeId == 2">
				<p class="text-muted">
					В данном типе задания ответы должны проверяться кураторами/преподавателями
				</p>
			</div>
			<div v-else-if="task.taskTypeId == 3">
				<button type="button" class="btn btn-outline-success" @click="addCorrelation">
					Добавить соотношение
				</button>
				<table class="my-2">
					<tbody>
						<tr v-for="(correlation, index) in task.correlations" :key="correlation.id">
							<td>
								<input
									class="form-control"
									type="text"
									v-model="correlation.left"
								/>
							</td>
							<td>
								<input
									class="form-control"
									type="text"
									v-model="correlation.right"
								/>
							</td>
							<td>
								<button
									type="button"
									class="btn btn-outline-danger"
									@click="deleteCorrelation(index)"
								>
									X
								</button>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
			<div v-else-if="task.taskTypeId == 4 || task.taskTypeId == 5">
				<p class="small my-1 p-0">
					<button type="button" class="btn btn-outline-success btn-sm" @click="addAnswer">
						Добавить вариант
					</button>
				</p>
				<table>
					<tbody>
						<tr v-for="(option, index) in task.allAnswerOptions" :key="option.id">
							<td>
								<input
									v-model="option.isRight"
									type="checkbox"
									class="form-check-input"
								/>
							</td>
							<td>
								<input class="form-control" v-model="option.textValue" />
							</td>
							<td>
								<button
									type="button"
									class="btn btn-outline-danger"
									@click="deleteAnswer(index)"
								>
									X
								</button>
							</td>
						</tr>
					</tbody>
				</table>
				<p class="small my-1 p-0">Отметьте правильные варианты</p>
			</div>
			<div v-else-if="task.taskTypeId == 6">
				<p class="text-muted">
					В данном типе задания ответы должны проверяться кураторами/преподавателями
				</p>
			</div>
		</BForm>
		<template #footer>
			<BButton variant="primary" @click="saveTask()">Сохранить</BButton>
			<button class="btn btn-dark" @click="close">Отмена</button>
		</template>
	</BModal>
</template>
