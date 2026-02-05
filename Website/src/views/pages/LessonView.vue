<script setup lang="ts">
import type { Lesson, Task, ContentBlock, TaskEditable, Correlation, TaskAnswer, TaskType } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'
import ContentBlockView from '../components/ContentBlockView.vue'
import TaskView from '../components/TaskView.vue'
import { useUserStore } from '@/stores/user'
import {
	BButton,
	BModal,
	useToggle,
	BForm,
	BFormFloatingLabel,
    BFormInput,
    BFormSelect,
	BFormCheckbox,
	BCard,
	BCardFooter,
	BCardText,
} from 'bootstrap-vue-next'
import type { ComponentExposed } from 'vue-component-type-helpers'


const lessonId = useRoute().params.lessonId
const lesson = ref<Lesson>()
const contentBlocks = ref<ContentBlock[]>([])
const tasks = ref<Task[]>([])
const taskTypes = ref<TaskType[]>([])
const refsToTaskViews = ref<ComponentExposed<typeof TaskView>[]>([])
const getData = async () => {
	await api.get<Lesson>(`lessons/${lessonId}`).then((res) => {
		lesson.value = res.data
	})
	await api.get<Task[]>(`lessons/${lessonId}/tasks`).then((res) => {
		tasks.value = res.data
	})
	await api.get<ContentBlock[]>(`lessons/${lessonId}/content-blocks`).then((res) => {
		contentBlocks.value = res.data
    })
    await api.get<TaskType[]>(`task-types`).then((res) => {
		taskTypes.value = res.data
    })
}
const createTask = () => showTaskModal()
const createContentBlock = () => {
    const newBlock: ContentBlock = {
		contentBlockTypeId: 1,
		order: contentBlocks.value.length + 1,
		id: 0,
		name: '',
		textValue: '',
	}
	contentBlocks.value.push(newBlock)
}

const saveBlocks = async () => {
    for (let index = 0; index < contentBlocks.value.length; index++) {
        let block = contentBlocks.value[index]!!
        const formData = new FormData()
        formData.append('Id', block.id.toString())
        formData.append('Name', block.name || '')
        formData.append('LessonId', lessonId!!.toString())
        formData.append('Order', block.order.toString())
        formData.append('ContentBlockTypeId', block.contentBlockTypeId.toString())
        switch (block.contentBlockTypeId) {
            case 1: {
                formData.append('TextValue', block.textValue || '')
                break
            }
            case 2: {
                if (block.file) {
                    formData.append('File', block.file)
                    formData.append('FileName', block.fileName || '')
                }
                break
            }
            case 3: 
            case 4: {
                if (block.file) {
                    formData.append('File', block.file)
                    formData.append('FileName', block.fileName || '')
                }
                formData.append('TextValue', block.textValue || '')
                break
            }  
        }
        console.log(JSON.stringify(formData))
        try {
            if (block.id == 0) {
                await api.post<number>(`content-blocks`, formData,
                    {
                        headers: {
                        "Content-Type": "multipart/form-data"
                    }}
                )
                    .then(res => {
                    block.id = res.data
                    alert("Блок сохранен - " + res.data)
                }) 
            }
            else {
                await api.put(`content-blocks`, formData, {
                    headers: {
                        "Content-Type": "multipart/form-data"
                    }
                })
                .then(res => {
                    alert("Блок сохранен - " + block.id)
                }) 
            }
        }
        catch (error) {
            console.error('Ошибка сохранения блока:', error)
        }
    }
}

const deleteTask = async (task: Task, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить задачу?`)) {
        if (task.id != 0) {
            await api.delete("tasks/" + task.id).then(res => {
                tasks.value.splice(index, 1)
            })
        }
        else {
            tasks.value.splice(index, 1)
        }
	}
}

const deleteBlock = async (block: ContentBlock, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить блок ${block.name}?`)) {
        if (block.id != 0) {
            await api.delete("content-blocks/" + block.id).then(res => {
                contentBlocks.value.splice(index, 1)
            })
        }
        else {
            contentBlocks.value.splice(index, 1)
        }
	}
}

const userStore = useUserStore()
const user = ref(userStore.user)
const checkTasks = async () => refsToTaskViews.value.forEach((x) => x.checkTask());

const isEditing = ref(false)
onMounted(async () => {
	await getData()
})
const cancelEditing = async () => {
	isEditing.value = false
	await getData()
}

const saveChanges = async () => {
    try {
        await saveBlocks();
		alert('Изменения успешно сохранены')
	} catch (error) {
		console.error('Ошибка сохранения:', error)
		alert('Произошла ошибка при сохранении изменений, попробуйте позже')
	}
}
const close = () => {
	if (editableTask.value.id != 0) {
		selectedTaskIndex.value = undefined
		editableTask.value = getNullTask()
    }
    hideTaskModal()
}
const startEditingTask = async (task: Task, index:number) => {
    selectedTaskIndex.value = index
    editableTask.value = {
        id: task.id,
        question: task.question,
        lessonId: lesson.value!!.id,
		taskTypeId: task.taskTypeId,
        order: task.order,
    }
    if (editableTask.value.taskTypeId == 1) {
            await api.get<TaskAnswer[]>(`tasks/${editableTask.value.id}/answers`).then((res) => {
                editableTask.value.textAnswer = res.data.pop()?.textValue
            })
        }
        else if (editableTask.value.taskTypeId == 3) {
            await api.get<Correlation[]>(`tasks/${editableTask.value.id}/correlations`).then((res) => {
                editableTask.value.correlations = res.data;
            })
        }
        else if (editableTask.value.taskTypeId == 4 || editableTask.value.taskTypeId == 5) {
            await api.get<TaskAnswer[]>(`tasks/${editableTask.value.id}/answers`).then((res) => {
                editableTask.value.allAnswerOptions = res.data;
            })
    }
    showTaskModal();
}
const getNullTask = (): TaskEditable => {
	return {
		id: 0,
        question: '',
        lessonId: parseInt(lessonId as string),
		taskTypeId: 1,
        order: tasks.value.length + 1,
        textAnswer: ''
	}
}
const {hide:hideTaskModal, show:showTaskModal} = useToggle('task-modal')
const editableTask = ref<TaskEditable>(getNullTask())
const selectedTaskIndex = ref<number>()
const saveTask = async () => {
    if (!editableTask.value.question) {
        alert("Укажите вопрос")
        return
    }
    if (Number.isNaN(Number.parseInt(editableTask.value.order.toString())) || editableTask.value.order < 1) {
        alert("Укажите порядок > 0")
        return
    }
    editableTask.value.order = Math.trunc(editableTask.value.order);  
    var requestBody = {
        id: editableTask.value.id,
        lessonId: lesson.value?.id,
        order: editableTask.value.order,
        question: editableTask.value.question ?? "",
        taskTypeId: editableTask.value.taskTypeId,
        textAnswer: editableTask.value.textAnswer,
        allAnswerOptions: editableTask.value.allAnswerOptions,
        correlations: editableTask.value.correlations
    }
    console.log(requestBody)
    if (requestBody.id == 0) {
		await api
			.post<Task>('tasks', requestBody)
            .then((res) => {
				tasks.value.push(res.data)
				editableTask.value = getNullTask()
				hideTaskModal()
				alert('Задание успешно создано')
			})
			.catch((err) => {
				hideTaskModal()
				alert('Ошибка, повторите позже')
			})
	} else {
		await api
			.put<Task>('tasks', requestBody)
            .then((res) => {
                let taskView = refsToTaskViews.value[selectedTaskIndex.value!!];
                if (taskView) {
                    taskView.update(res.data);
                }
				alert('Задание успешно изменено')
			})
			.catch((err) => {
				alert('Ошибка, повторите позже')
			})
		hideTaskModal()
		editableTask.value = getNullTask()
		selectedTaskIndex.value = undefined
	}
}
const addCorr = () => {
    if (!editableTask.value.correlations) {
        editableTask.value.correlations = []
    }
    editableTask.value.correlations.push({
        left: '',
        right: '',
        id: 0,
        rightId: 0,
    }) 
}
const delCorr = (corr:Correlation, index:number) => {
    editableTask.value.correlations?.splice(index, 1)
}
const addAnswer= () => {
    if (!editableTask.value.allAnswerOptions) {
        editableTask.value.allAnswerOptions = []
    }
    editableTask.value.allAnswerOptions.push({
        taskId: editableTask.value.id,
        id: 0,
        isRight:false,
        textValue: ''
    }) 
}
const delAnswer = (answer:TaskAnswer, index:number) => {
    editableTask.value.allAnswerOptions?.splice(index, 1)
}
</script>

<template>
	<div class="container">
		<div v-if="lesson">
			<h1>{{ lesson.name }}</h1>
			<p v-if="lesson.description">{{ lesson.description }}</p>
			<p>
				<RouterLink :to="`/courses/modules/${lesson.moduleId}`"
					>Вернуться к списку занятий модуля</RouterLink
				>
			</p>
		</div>
		<div class="d-flex flex-wrap" v-if="user?.role.id == 1">
			<button
				@click="isEditing = !isEditing"
				:class="{ 'btn-warning': isEditing, 'btn-outline-warning': !isEditing }"
				class="btn"
			>
				Режим редактирования
			</button>
			<button @click="cancelEditing" class="btn btn-outline-secondary mx-2">Обновить страницу</button>
		</div>

		<h2 v-if="lesson?.lessonTypeId == 1">
			Содержание занятия
			<button class="btn btn-primary" @click="createContentBlock" v-if="user?.role.id == 1 && isEditing">
				Добавить блок содержания
			</button>
            <button @click="saveChanges" v-if="user?.role.id == 1 && isEditing" class="btn btn-outline-success mx-2">
				Сохранить изменения
			</button>
		</h2>
		<div
			class="lesson-content-div"
			v-if="lesson?.lessonTypeId == 1 && contentBlocks.length > 0"
		>
			<ContentBlockView  :style="{order: contentBlock.order}"
                @delete-click="(b) => deleteBlock(b, index)"
                class="m-0"
				:is-editing="isEditing"
				v-for="contentBlock, index in contentBlocks"
				:content="contentBlock"
			/>
		</div>
		<h2>
			Задания занятия
			<button class="btn btn-primary" @click="createTask" v-if="user?.role.id == 1 && isEditing">
				Добавить задание
			</button>
		</h2>

		<div class="lesson-tasks-div" v-if="tasks.length > 0">
			<TaskView :index="index" @start-edit="(t)=>startEditingTask(t, index)"
             :style="{order: task.order}"
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
			<button class="btn btn-primary" v-if="!isEditing" @click="checkTasks">Проверить задания</button>
		</div>
		<p v-else>Заданий на этом занятии нет</p>
	</div>

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
					v-model.trim="editableTask.question"
					type="text"
					placeholder="Сколько будет 2 + 2?"
				/>
			</BFormFloatingLabel>

			<p>Тип:</p>
				<BFormSelect
					:options="taskTypes"
					v-model="editableTask.taskTypeId"
					value-field="id"
					text-field="name"
					id="task-type"
				/>

            <div v-if="editableTask.taskTypeId == 1">
                <BFormFloatingLabel class="my-2" label="Ответ" label-for="task-text-answer">
				<BFormInput
					id="task-text-answer"
					v-model.trim="editableTask.textAnswer"
					type="text"
					placeholder="4"
				/>
			    </BFormFloatingLabel>
            </div>
            <div v-else-if="editableTask.taskTypeId == 2">
                <p class="text-muted">В данном типе задания ответы должны проверяться кураторами/преподавателями</p>
            </div>
            <div v-else-if="editableTask.taskTypeId == 3">
                <button type="button" @click="addCorr">+</button>
                <table>
                    <tbody>
                        <tr v-for="correlation,index in editableTask.correlations">
                            <td><input class="form-control-sm" type="text" v-model="correlation.left"/></td>
                            <td><input class="form-control-sm" type="text" v-model="correlation.right"/></td>
                            <td><button type="button" @click="delCorr(correlation, index)">-</button></td>
                        </tr>
                    </tbody>
                </table>
            </div>
             <div v-else-if="editableTask.taskTypeId == 4 || editableTask.taskTypeId == 5">
                <button type="button" @click="addAnswer">+</button>
                <ul>
                    <li v-for="option, index in editableTask.allAnswerOptions">
                        <input class="form-control-sm" v-model="option.textValue"/> 
                        <span>является ответом</span>
                        <input v-model="option.isRight" type="checkbox"/>
                        <button type="button" @click="delAnswer(option, index)">-</button>
                    </li>
                </ul>
            </div>
            <div v-else-if="editableTask.taskTypeId == 6">
                <p class="text-muted">В данном типе задания ответы должны проверяться кураторами/преподавателями</p>
            </div>

			<BFormFloatingLabel class="my-2" label="Порядок" label-for="task-order">
				<BFormInput
					id="task-order"
					v-model.number="editableTask.order"
					type="text"
                    pattern="\d*"
					placeholder="1"
				/>
			</BFormFloatingLabel>
		</BForm>
		<template #footer>
			<BButton variant="primary" @click="saveTask()">Сохранить</BButton>
			<button class="btn btn-dark" @click="close">Отмена</button>
		</template>
	</BModal>
</template>

<style scoped>
    .lesson-content-div {
        display: grid;
        grid-template-columns: 1fr;
    }
</style>
