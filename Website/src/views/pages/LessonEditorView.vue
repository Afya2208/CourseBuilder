<script setup lang="ts">
import type { ContentBlock, ContentBlockType, Lesson } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref } from 'vue'

const blocks = ref<ContentBlock[]>([])
const contentTypes = ref<ContentBlockType[]>([])
const createItem = () => {
	blocks.value.push({
		contentBlockTypeId: 1,
		order: blocks.value.length + 1,
		id: 0,
		name: '',
	})
}
const deleteItem = (index: number) => {
	blocks.value.splice(index, 1)
}
const createTask = () => {}
const fetch = async () => {
	await api.get<ContentBlockType[]>('content-block-types').then((res) => {
		contentTypes.value = res.data
	})
}
const loadFile = (event: Event, block: ContentBlock, extensions?: string[]) => {
	let input = event.target as HTMLInputElement
	if (FileReader && input.files && input.files[0]) {
		let file = input.files[0]
		let ext = file.name.split('.').pop() ?? ''
		if (!extensions || extensions.includes(ext)) {
			let reader = new FileReader()
			reader.onload = () => {
				block.fileUrl = reader.result
				block.fileName = file.name
			}
			reader.readAsDataURL(file)
		} else {
			event.preventDefault()
		}
	}
}

onMounted(async () => {
	await fetch()
})
</script>
<template>
	<div>
		<div>
			<button @click="createItem">Добавить блок содержания</button>
			<button @click="createTask">Добавить задание</button>
		</div>
		<div>
			<div class="item" v-for="(block, index) in blocks" :style="{ order: block.order }">
				<div>
					<button @click="deleteItem(index)">Удалить</button>
					<span>Тип</span>
					<select v-model="block.contentBlockTypeId">
						<option v-for="type in contentTypes" :value="type.id">
							{{ type.name }}
						</option>
					</select>
				</div>
				<div v-if="block.contentBlockTypeId == 1">
					<input v-model="block.name" placeholder="Заголовок блока текста" />
					<input v-model="block.textValue" placeholder="Содержание..." />
				</div>
				<div v-else-if="block.contentBlockTypeId == 2">
					<input
						type="file"
						@change="loadFile($event, block, ['.png', '.jpeg', '.jpg'])"
					/>
					<img :src="block.fileUrl" />
				</div>
				<div v-else-if="block.contentBlockTypeId == 3">
					<input type="file" @change="loadFile($event, block, ['.pdf'])" />
					<iframe :src="block.fileUrl"></iframe>
				</div>
				<div v-else-if="block.contentBlockTypeId == 4">
					<p>Прикрепите файл</p>
					<input type="file" @change="loadFile($event, block)" />
				</div>
			</div>
		</div>
	</div>
</template>
<style scoped>
.item {
	margin: 15px;
	background-color: green;
	border: solid slateblue 2px;
}
</style>
