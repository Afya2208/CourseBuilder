<script setup lang="ts">
import type { Lesson, Task, ContentBlock } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'
import ContentBlockView from '../components/ContentBlockView.vue'
import TaskView from '../components/TaskView.vue'
import { useUserStore } from '@/stores/user'

const lessonId = useRoute().params.lessonId
const lesson = ref<Lesson>()
const contentBlocks = ref<ContentBlock[]>([])
const tasks = ref<Task[]>([])
const refsToTaskViews = ref([])
const getData = async () => {
	await api.get<Lesson>(`lessons/${lessonId}`).then((res) => {
		lesson.value = res.data
	})
	await api.get<Task[]>(`lessons/${lessonId}/tasks`).then((res) => {
		tasks.value = res.data
		console.log(JSON.stringify(tasks.value))
	})
	await api.get<ContentBlock[]>(`lessons/${lessonId}/content-blocks`).then((res) => {
		contentBlocks.value = res.data
	})
}

const createContentBlock = () => {
	const newBlock: ContentBlock = {
		contentBlockTypeId: 1,
		order: contentBlocks.value.length + 1,
		id: 0,
		name: '',
		textValue: '',
		//!!!!!!
	}
	contentBlocks.value.push(newBlock)
}

const saveBlock = async (block: ContentBlock) => {
	try {
		if (block.file) {
			const formData = new FormData()

			// Добавляем данные блока
			formData.append('ContentBlockTypeId', block.contentBlockTypeId.toString())
			formData.append('Order', block.order.toString())
			formData.append('Name', block.name || '')
			formData.append('TextValue', block.textValue || '')

			// Добавляем файл
			formData.append('File', block.file)
			formData.append('FileName', block.fileName || '')
			formData.append('FileMimeType', block.fileMimeType || '')

			if (block.id === 0) {
				const response = await api.post(`content-blocks/with-file`, formData, {
					headers: {
						'Content-Type': 'multipart/form-data',
					},
				})
				return response.data
			} else {
				const response = await api.put(`content-blocks/${block.id}/with-file`, formData, {
					headers: {
						'Content-Type': 'multipart/form-data',
					},
				})
				return response.data
			}
		} else {
			const blockData = {
				id: block.id,
				contentBlockTypeId: block.contentBlockTypeId,
				order: block.order,
				name: block.name,
				textValue: block.textValue,
				lessonId: lesson.value!!.id,
			}
			if (block.id === 0) {
				const response = await api.post('content-blocks', blockData)
				return response.data
			} else {
				const response = await api.put(`content-blocks/${block.id}`, blockData)
				return response.data
			}
		}
	} catch (error) {
		console.error('Ошибка сохранения блока:', error)
		throw error
	}
}

const deleteBlock = async (block: ContentBlock, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить блок ${block.name}?`)) {
		if (block.id == 0) {
			contentBlocks.value.splice(index, 1)
		} else {
			block._toDelete = true
		}
	}
}

const userStore = useUserStore()
const user = ref(userStore.user)
const checkTasks = async () => {
	refsToTaskViews.value.forEach((x) => x.checkTask())
}
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
        /*
		const blocksToDelete = contentBlocks.value.filter(
			(block) => block._destroy && block.id !== 0,
		)
		for (const block of blocksToDelete) {
			await api.delete(`content-blocks/${block.id}`)
		}

		//const blocksToSave = contentBlocks.value.filter(block => !block._destroy)
		//const savedBlocks = []
        
		for (const block of blocksToSave) {
			const savedBlock = await saveContentBlock(block)
			savedBlocks.push(savedBlock)
		}
            */
		//contentBlocks.value = savedBlocks.filter(block => !block._destroy)
		isEditing.value = false
		alert('Изменения успешно сохранены')
	} catch (error) {
		console.error('Ошибка сохранения:', error)
		alert('Произошла ошибка при сохранении изменений, попробуйте позже')
	}
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
			<button @click="saveChanges" class="btn btn-outline-success mx-2">
				Сохранить изменения
			</button>
			<button @click="cancelEditing" class="btn btn-outline-secondary">Отмена</button>
		</div>

		<h2 v-if="lesson?.lessonTypeId == 1">
			Содержание занятия
			<button class="btn btn-primary" @click="createContentBlock" v-if="user?.role.id == 1 && isEditing">
				Добавить блок содержания
			</button>
		</h2>
		<div
			class="lesson-content-div"
			v-if="lesson?.lessonTypeId == 1 && contentBlocks.length > 0"
		>
			<ContentBlockView :style="{order: contentBlock.order}"
				:is-editing="isEditing"
				v-for="contentBlock in contentBlocks"
				:content="contentBlock"
			/>
		</div>
		<h2>
			Задания занятия
			<button class="btn btn-primary" v-if="user?.role.id == 1 && isEditing">
				Добавить задание
			</button>
		</h2>

		<div class="lesson-tasks-div" v-if="tasks.length > 0">
			<TaskView
				:is-editing="isEditing"
				v-for="(task, index) in tasks"
				:task="task"
				:ref="
					(ref) => {
						if (ref) refsToTaskViews[index] = ref
					}
				"
			/>
			<button class="btn btn-primary" @click="checkTasks">Проверить задания</button>
		</div>
		<p v-else>Заданий на этом занятии нет</p>
	</div>
</template>

<style scoped>
    .lesson-content-div {
        display: grid;
        grid-template-columns: 1fr;
    }
</style>
