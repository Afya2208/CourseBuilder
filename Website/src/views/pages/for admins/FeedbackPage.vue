<script lang="ts" setup>
import type { FeedbackSubmit, FeedbackSubmitsAndTotalCount } from '@/models/feedback.ts'
import api from '@/services/api'

import PaginationView from '@/views/pages/components/PaginationView.vue'
import { onBeforeMount, onMounted, ref, watch } from 'vue'

const feedbackSubmits = ref<FeedbackSubmit[]>()
const totalCount = ref<number>(0)
const loadError = ref(false)

const loadFeedbackAsync = async () => {
	loadError.value = false
	const isSolved = type.value == 'Solved'
	await api
		.get<FeedbackSubmitsAndTotalCount>(
			`feedback/paged?pageSize=${searchOptions.value.pageSize}&pageNumber=${searchOptions.value.pageNumber}&solved=${isSolved}`,
		)
		.then((res) => {
			totalCount.value = res.data.totalCount
			feedbackSubmits.value = res.data.feedbackSubmits
		})
		.catch((err) => {
			loadError.value = true
		})
}

const solve = async (f: FeedbackSubmit) => {
	await api.post(`feedback/${f.id}/solve`).then((res) => {
		f.dateTimeSolved = new Date()
	})
}

let timer: number | undefined
onMounted(async () => {
	await loadFeedbackAsync()
	timer = setInterval(async () => {
		await loadFeedbackAsync()
	}, 1000 * 60)
})
onBeforeMount(() => {
	clearInterval(timer!)
})

const getDefaultSearchOptions = () => {
	return {
		pageNumber: 1,
		pageSize: 20,
		type: 'Current',
	}
}
const searchOptions = ref<{
	pageNumber: number
	pageSize: number
}>(getDefaultSearchOptions())
const type = ref<'Current' | 'Solved'>('Current')
watch(type, async () => {
	searchOptions.value.pageNumber = 1
	await loadFeedbackAsync()
})

const onSelectedPage = async () => {
	await loadFeedbackAsync()
}
</script>

<template>
	<div class="container">
		<h3 class="m-3">Обращения</h3>

		<div class="d-flex flex-wrap justify-content-center">
			<select v-model="type" class="form-control w-25">
				<option value="Current">Текущие</option>
				<option value="Solved">Решенные</option>
			</select>
		</div>

		<div v-if="!feedbackSubmits && !loadError">
			<p>Загрузка данных, подождите, пожалуйста</p>
		</div>

		<div v-else-if="!feedbackSubmits && loadError">
			<p>Ошибка загрузки данных, попробуйте позже</p>
		</div>

		<div
			v-else-if="feedbackSubmits && feedbackSubmits.length > 0"
			class="row row-cols-1 row-cols-md-1 g-4 p-2"
		>
			<div class="col" v-for="(feedback, index) in feedbackSubmits" :key="feedback.id">
				<div class="card h-100" style="min-height: 330px">
					<div class="card-body d-flex flex-column">
						<h5 class="card-title">Обращение №{{ feedback.id }}</h5>
						<p>
							От {{ feedback.dateTimeSent?.toString().split('T')[0] }}
							{{ feedback.dateTimeSent?.toString().split('T')[1]?.split('.')[0] }}
							<br />
							<span v-if="feedback.dateTimeSolved"
								>Отмечено:
								{{ feedback.dateTimeSolved?.toString().split('T')[0] }}
								{{
									feedback.dateTimeSolved?.toString().split('T')[1]?.split('.')[0]
								}}</span
							>
						</p>
						<p>
							Тип: {{ feedback.feedbackCategory?.name }}<br />
							{{ feedback.title }}<br />
							{{ feedback.text }}
						</p>
						<p>
							Почта: {{ feedback.email }} <br />
							<span v-if="feedback.userId">UID: {{ feedback.userId }}</span>
						</p>
						<p>
							<button
								v-if="!feedback.dateTimeSolved"
								class="btn btn-outline-primary"
								@click="solve(feedback)"
							>
								Отметить решенным
							</button>
						</p>
					</div>
				</div>
			</div>
		</div>

		<div v-else>
			<p class="text-center p-1">Пока нет обращений</p>
		</div>

		<PaginationView
			v-if="feedbackSubmits && feedbackSubmits.length > 0"
			:search-options="searchOptions"
			:total-count="totalCount"
			@selected-page="onSelectedPage"
		/>
	</div>
</template>
