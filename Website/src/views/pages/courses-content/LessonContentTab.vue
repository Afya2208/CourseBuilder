<script lang="ts" setup>
import type { ContentBlock } from '@/models/main'
import api from '@/services/api'
import ContentBlockView from '@/views/pages/courses-content/ContentBlockView.vue'
import { ref, toRaw, toRefs } from 'vue'
import { useRoute } from 'vue-router'
import type { ComponentExposed } from 'vue-component-type-helpers'

const lessonId = Number.parseInt(useRoute().params.lessonId as string)
const courseId = Number.parseInt(useRoute().params.courseId as string)
const moduleId = Number.parseInt(useRoute().params.moduleId as string)

const props = defineProps<{
	isEditing: boolean
	contentBlocks: ContentBlock[]
	userIsOwner: boolean
}>()
const blockViews = ref<ComponentExposed<typeof ContentBlockView>[]>([])
const { isEditing, contentBlocks, userIsOwner } = toRefs(props)

const saveAllBlocks = async () => {
	for (const blockView of blockViews.value) {
		await blockView.saveBlockAsync()
	}
}
const deleteBlock = async (block: ContentBlock, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить блок ${block.name}?`)) {
		if (block.id != 0) {
			await api.delete('content-blocks/' + block.id).then((res) => {
				contentBlocks.value.splice(index, 1)
			})
		} else {
			contentBlocks.value.splice(index, 1)
		}
	}
}

window.addEventListener('scroll', () => {
	if (window.innerHeight + window.scrollY >= document.documentElement.scrollHeight) {
		console.log('Конец страницы!')
		// Проверка типа требования - если нет заданий, то считаем занятие выполненым, если студент пролистал до конца
		// если задания есть, то занятие засчитается только, если выполнить все задания
		// запрос на выполнение занятия if (lesson.value?.isRequired ) { }
	}
})

const saveBlocks = async () => {
	try {
		syncOrders()
		await saveAllBlocks()
		alert('Изменения успешно сохранены')
	} catch (error) {
		console.error('Ошибка сохранения:', error)
		alert('Произошла ошибка при сохранении изменений, попробуйте позже')
	}
}
const addBlock = () => {
	const newBlock: ContentBlock = {
		contentBlockTypeId: 1,
		order: 1,
		id: 0,
		name: '',
		textValue: '',
		tempId: generateBlockKey(),
	}
	contentBlocks.value.push(newBlock)
}
const syncOrders = () => {
	contentBlocks.value.forEach((card, i) => {
		card.order = i + 1
	})
}

let counter = 0
const generateBlockKey = (): string => {
	return `temp-${Date.now()}-${counter++}`
}

function move(fromIndex: number, toIndex: number) {
	if (toIndex < 0 || toIndex >= contentBlocks.value.length || fromIndex === toIndex) return
	const arr = contentBlocks.value
	const [item] = arr.splice(fromIndex, 1)
	arr.splice(toIndex, 0, item)
}
</script>

<template>
	<div class="py-3">
		<div class="d-flex flex-wrap gap-2 justify-content-center" v-if="userIsOwner && isEditing">
			<button class="btn btn-primary" @click="addBlock">Добавить блок содержания</button>
			<button @click="saveBlocks" v-if="userIsOwner && isEditing" class="btn btn-success">
				Сохранить блоки содержания
			</button>
		</div>

		<div class="lesson-content-div" v-if="contentBlocks.length > 0">
			<ContentBlockView
				:ref="
					(ref) => {
						if (ref) blockViews[index] = ref
					}
				"
				:key="contentBlock.id === 0 ? contentBlock.tempId : contentBlock.id"
				:lessonId="lessonId"
				:index="index"
				:blocksCount="contentBlocks.length"
				@downClicked="move(index, index + 1)"
				@upClicked="move(index, index - 1)"
				@delete-block="(b) => deleteBlock(b, index)"
				:is-editing="isEditing"
				v-for="(contentBlock, index) in contentBlocks"
				:content="contentBlock"
			/>
		</div>
		<div v-else>
			<p>Для этого занятия нет теоретического материала</p>
		</div>
	</div>
</template>
<style scoped>
.lesson-content-div {
	display: grid;
	grid-template-columns: 1fr;
}
</style>
