<script lang="ts" setup>
import type { CoursesAndTotalCount } from '@/models/coursesAndTotalCount'
import type { Kit } from '@/models/kit'
import type { KitsAndTotalCount } from '@/models/kitsAndTotalCount'
import type { Course, Theme } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { BButton, BFormSelect, BInputGroup, BInputGroupText } from 'bootstrap-vue-next'
import { computed, onMounted, ref, toRefs, watch } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import PaginationView from '@/views/pages/components/PaginationView.vue'
import { pluralizeRu } from '@/util/methods.ts'

const { user, addAvailableCourse, availableKitsIds, addAvailableKit } = useUserStore()
const kits = ref<Kit[]>()
const totalCount = ref(0)
const themes = ref<Theme[]>()
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
const searchChanged = ref(false)
watch(
	() => searchOptions.value.themeId,
	(newVal, oldVal) => {
		searchChanged.value = true
	},
)
watch(
	() => searchOptions.value.text,
	(newVal, oldVal) => {
		searchChanged.value = true
	},
)

const loadThemesAsync = async () => {
	await api.get<Theme[]>('themes').then((res) => (themes.value = res.data))
}

const searchKits = async () => {
	loadError.value = false
	let requestPath = 'kits/search?'
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
		.get<KitsAndTotalCount>(requestPath)
		.then((res) => {
			kits.value = res.data.kits
			totalCount.value = res.data.totalCount
		})
		.catch((err) => (loadError.value = true))
}

const router = useRouter()

const takeKit = async (k: Kit) => {
	if (!user) {
		alert('Для приобретения наборов и курсов необходимо авторизоваться на сайте')
		return
	}
	if (k.price && k.price > 0) {
		router.push(`/payment/kit/${k.id}`)
	} else {
		await api.post(`users/${user?.id}/add-kit/${k.id}`).then((res) => {
			router.push('/my-courses')
			alert('Набор и курсы успешно добавлены в список доступных')
			if (k.coursesInfo) {
				for (let c of k.coursesInfo) {
					addAvailableCourse(c.id)
				}
			}
			addAvailableKit(k.id)
		})
	}
}
onMounted(async () => {
	await loadThemesAsync()
	await searchKits()
})

const onSelectedPage = async () => {
	await searchKits()
}
</script>

<template>
	<div class="container">
		<h3 class="mt-3" id="header">Наборы курсов</h3>
		<p class="text-secondary small">
			Посмотреть отдельные курсы можно
			<RouterLink to="/courses">на странице образовательных курсов</RouterLink>
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

			<button type="button" class="btn m-1 btn-outline-primary" @click="searchKits">
				Поиск
			</button>
			<button type="button" class="btn m-1 btn-outline-secondary" @click="clearSearch">
				Очистить поля
			</button>
		</div>

		<div v-if="!kits && !loadError">
			<p>Загрузка данных, подождите, пожалуйста</p>
		</div>

		<div v-else-if="!kits && loadError">
			<p>Ошибка загрузки данных, попробуйте позже</p>
		</div>

		<div v-else-if="kits && kits.length > 0" class="row row-cols-1 row-cols-md-3 g-4 p-2">
			<div class="col" v-for="(kit, index) in kits" :key="kit.id">
				<div class="card h-100">
					<div class="card-body d-flex flex-column">
						<h5 class="card-title">{{ kit.name }}</h5>
						<p class="card-text my-1 overflow-auto" style="max-height: 120px">
							{{ kit.description }}
						</p>
						<div class="mt-auto pt-2 border-top">
							<div
								class="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-2"
							>
								<span class="text-primary fw-medium">
									{{
										kit.price > 0 ? `Стоимость: ${kit.price} руб.` : 'Бесплатно'
									}}
								</span>
								<button
									v-if="user?.role.id == 3 && !availableKitsIds?.includes(kit.id) ||
									!user"
									class="btn btn-outline-success btn-sm"
									@click="takeKit(kit)"
								>
									Приобрести
								</button>
							</div>
							<div v-if="kit.coursesInfo?.length" class="mt-2">
								<h6 class="card-subtitle small text-muted mb-2">Курсы в наборе:</h6>
								<div
									class="d-flex flex-wrap gap-2"
									style="max-height: 120px; overflow-y: auto"
								>
									<div
										v-for="course in kit.coursesInfo"
										:key="course.id"
										class="position-relative flex-fill bg-light rounded p-2 border"
										style="min-width: 140px"
									>
										<RouterLink
											class="stretched-link text-decoration-none"
											:to="`/courses/${course.id}`"
										>
											<span class="small fw-medium text-dark">{{
												course.name
											}}</span>
										</RouterLink>
									</div>
								</div>
							</div>
						</div>
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
			v-if="kits && kits.length > 0"
			:search-options="searchOptions"
			:total-count="totalCount"
			@selected-page="onSelectedPage"
		/>
	</div>
</template>
