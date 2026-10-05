<script setup lang="ts">
import type { Theme } from '@/models/main.ts'
import api from '@/services/api.ts'
import { useUserStore } from '@/stores/user.ts'
import { getColorForCard } from '@/util/methods.ts'
import { storeToRefs } from 'pinia'
import { inject, onMounted, ref, toRef, toRefs, type Ref } from 'vue'

const emits = defineEmits<{
	'card-clicked': [themeId: number]
}>()
const cardClicked = (themeId: number) => {
	emits('card-clicked', themeId)
}
const props = defineProps<{
	themes?: Theme[]
	loadError: boolean
}>()
const { themes, loadError } = toRefs(props)
const { user } = storeToRefs(useUserStore())

const deleteTheme = async (themeId: number, index: number) => {
	await api
		.delete<Theme>(`themes/${themeId}`)
		.then((res) => {
			alert(`Успешное удаление темы ${res.data.name}`)
			themes.value.splice(index, 1)
		})
		.catch((err) => {
			if (err.status == 400)
				alert('Ошибка, нельзя удалить тему, так как она используется другими данными')
		})
}
const addTheme = async () => {
	const newThemeName = prompt('Напишите, какую новую тему хотите добавить', 'Новая тема')
	if (newThemeName) {
		const newTheme: Theme = {
			id: 0,
			name: newThemeName,
		}
		await api.post<Theme>('themes', newTheme).then((res) => {
			alert('Новая тема успешно добавлена')
			themes.value.push(res.data)
		})
	}
}
const updateTheme = async (themeToUpdate: Theme) => {
	const newThemeName = prompt('Напишите новое название для темы', themeToUpdate.name)
	if (newThemeName) {
		const oldName = themeToUpdate.name
		themeToUpdate.name = newThemeName
		await api
			.put<Theme>(`themes`, themeToUpdate)
			.then((res) => {
				alert('Тема успешно обновлена')
			})
			.catch((err) => {
				alert('Ошибка: тема не обновлена')
				themeToUpdate.name = oldName
			})
	}
}
</script>

<template>
	<div>
		<h3 class="my-3">
			Темы
			<button
				v-if="user?.role.id == 1 || user?.role.id == 2"
				class="btn btn-primary"
				@click="addTheme"
			>
				Добавить тему
			</button>
		</h3>

		<div v-if="!themes && !loadError">
			<p>Загрузка данных, подождите, пожалуйста</p>
		</div>

		<div v-else-if="!themes && loadError">
			<p>Ошибка загрузки данных, попробуйте позже</p>
		</div>

		<div class="d-flex flex-wrap" v-else-if="themes && themes.length > 0">
			<div
				class="card m-2 p-1"
				:key="theme.id"
				:class="getColorForCard(index)"
				v-for="(theme, index) in themes"
			>
				<div class="card-body card-clickable p-2" @click="cardClicked(theme.id)">
					<h5 class="card-title m-auto">{{ theme.name }}</h5>
				</div>
				<div class="card-footer" v-if="user?.role.id == 2">
					<button
						class="btn btn-sm btn-outline-danger"
						@click="deleteTheme(theme.id, index)"
					>
						❌
					</button>
					<button class="btn btn-sm btn-outline-warning mx-1" @click="updateTheme(theme)">
						✏️
					</button>
				</div>
			</div>
		</div>

		<div v-else>
			<p>К сожалению, пока нет данных</p>
		</div>
	</div>
</template>

<style scoped></style>
