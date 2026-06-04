<script setup lang="ts">
import type { CoursesAndTotalCount } from '@/models/coursesAndTotalCount.ts'
import type { Course, Theme } from '@/models/main.ts'
import api from '@/services/api.ts'
import { useUserStore } from '@/stores/user.ts'
import { BButton, BFormSelect, BInputGroup, BInputGroupText } from 'bootstrap-vue-next'
import { storeToRefs } from 'pinia'
import { onMounted, ref, toRefs, watch } from 'vue'
import { RouterLink } from 'vue-router'
import PaginationView from '@/views/pages/components/PaginationView.vue'
import { pluralizeRu } from '@/util/methods.ts'

const { user } = storeToRefs(useUserStore())
const props = defineProps<{
	themes?: Theme[]
	selectedThemeCardId?: number
}>()

watch(
	() => props.selectedThemeCardId,
	async (newVal, oldVal) => {
		if (newVal) {
			searchOptions.value.themeId = newVal
			searchChanged.value = true
			await searchCourses()
		}
	},
)

const { themes } = toRefs(props)
const courses = ref<Course[]>()
const loadError = ref(false)
const clearSearch = () => {
	searchOptions.value = getDefaultSearchOptions()
}
const getDefaultSearchOptions = () => {
	return {
		pageNumber: 1,
		pageSize: 15,
		text: '',
	}
}
const searchOptions = ref<{
	pageNumber: number
	themeId?: number
	pageSize: number
	text?: string
}>(getDefaultSearchOptions())

watch(
	() => searchOptions.value.themeId,
	() => {
		searchChanged.value = true
	},
)
watch(
	() => searchOptions.value.text,
	() => {
		searchChanged.value = true
	},
)

const totalCount = ref<number>(0)
const searchChanged = ref(false)

const searchCourses = async () => {
	loadError.value = false
	let requestPath = 'courses/search?'
	requestPath += `pageSize=${searchOptions.value.pageSize}`
	if (searchOptions.value.text) {
		requestPath += `&text=${encodeURIComponent(searchOptions.value.text)}`
	}
	if (searchOptions.value.themeId) {
		requestPath += `&themeId=${searchOptions.value.themeId}`
	}
	if (searchChanged.value) {
		requestPath += `&pageNumber=1`
		searchOptions.value.pageNumber = 1
		searchChanged.value = false
	} else {
		requestPath += `&pageNumber=${searchOptions.value.pageNumber}`
	}
	await api
		.get<CoursesAndTotalCount>(requestPath)
		.then((res) => {
			courses.value = res.data.courses
			totalCount.value = res.data.totalCount
		})
		.catch(() => (loadError.value = true))
}

onMounted(async () => {
	await searchCourses()
})

const onSelectedPage = async () => {
	await searchCourses()
}
const formsModules = ['модуль', 'модуля', 'модулей']
const formsLessons = ['занятие', 'занятия', 'занятий']

const counts = (c: Course): string => {
	return `${c.modulesCount ?? 0} ${pluralizeRu(c.modulesCount ?? 0, formsModules)}, ${c.lessonsCount ?? 0} ${pluralizeRu(c.lessonsCount ?? 0, formsLessons)}`
}
</script>

<template>
	<div>
		<h3 class="mt-3" id="header">Образовательные курсы</h3>
		<p class="text-secondary small">
			Посмотреть наборы курсов можно
			<RouterLink to="/kits">на странице наборов</RouterLink>
		</p>

		<div class="d-flex flex-wrap">
			<input
				type="search"
				style="width: 350px"
				class="m-1 form-control"
				v-model.trim="searchOptions.text"
				placeholder="Поиск..."
			/>
			<BInputGroup class="m-1 w-50">
				<BInputGroupText class="d-inline">Тема:</BInputGroupText>
				<BFormSelect
					class="d-inline w-25"
					:options="themes"
					style="width: 250px"
					value-field="id"
					text-field="name"
					v-model="searchOptions.themeId"
				/>
			</BInputGroup>

			<button type="button" class="btn m-1 btn-outline-primary" @click="searchCourses">
				Поиск
			</button>
			<BButton variant="outline-secondary" class="m-1" @click="clearSearch"
				>Очистить поля</BButton
			>
		</div>

		<div v-if="!courses && !loadError">
			<p>Загрузка данных, подождите, пожалуйста</p>
		</div>

		<div v-else-if="!courses && loadError">
			<p>Ошибка загрузки данных, попробуйте позже</p>
		</div>

		<div v-else-if="courses && courses.length > 0" class="row row-cols-1 row-cols-md-3 g-4 p-2">
			<div class="col" v-for="(course, index) in courses" :key="course.id">
				<div class="card h-100 card-clickable" style="min-height: 330px">
					<div class="card-body d-flex flex-column">
						<h5 class="card-title">{{ course.name }}</h5>
						<p class="card-text my-1">
							{{ course.description }}
						</p>
						<RouterLink class="stretched-link" :to="`courses/${course.id}`" />
						<p class="card-text my-1 text-primary mt-auto">
							<span v-if="course.price > 0">Стоимость: {{ course.price }} руб.</span>
							<span v-else>Бесплатно</span>
						</p>
						<p class="card-subtitle small text-primary-emphasis">
							{{ counts(course) }}
							<br />
							Темы курса:
							{{
								course.themes
									?.map((x) => themes?.find((t) => x.id == t.id)?.name)
									.join(', ')
							}}
						</p>
					</div>
				</div>
			</div>
		</div>

		<div v-else>
			<p class="text-center p-1">
				К сожалению, пока нет курсов
				<span v-if="searchOptions.themeId || searchOptions.text">по данному запросу</span>
			</p>
		</div>

		<PaginationView
			v-if="courses && courses.length > 0"
			:search-options="searchOptions"
			:total-count="totalCount"
			@selected-page="onSelectedPage"
		/>
	</div>
</template>

<style scoped></style>
