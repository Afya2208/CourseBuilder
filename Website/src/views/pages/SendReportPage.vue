<script lang="ts" setup>
import type { FeedbackCategory, FeedbackSubmit } from '@/models/feedback.ts'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { Validator } from '@/util/validator'
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'

const complaint = ref<FeedbackSubmit>({
	title: '',
	text: '',
	feedbackCategoryId: 1,
})
const { user } = useUserStore()
if (user) {
	complaint.value.userId = user.id
	complaint.value.email = user.email
}
const router = useRouter()
const sendReport = async () => {
	if (checkFields()) {
		await api.post('feedback', complaint.value).then(() => {
			alert('Обращение успешно отправлено, спасибо за обратную связь!')
			router.push('/')
		})
	}
}
const checkFields = (): boolean => {
	const v = new Validator()
	v.validate(
		!complaint.value.email && !complaint.value.userId,
		'Требуется указать email или войдите в аккаунт для обеспечения обратной связи',
	)
	v.validate(!complaint.value.title, 'Требуется указать название/тему обращения')
	v.validate(!complaint.value.text, 'Требуется указать описание обращения')
	return v.returnResult()
}
const categories = ref<FeedbackCategory[]>()
onMounted(async () => {
	await api.get<FeedbackCategory[]>('feedback/categories').then((res) => {
		categories.value = res.data
	})
})
</script>

<template>
	<div class="container">
		<h3 class="my-3">Форма для обращений</h3>
		<p>
			Почта для обратной связи:
			<input
				class="form-control d-inline w-25"
				type="email"
				v-model="complaint.email"
				placeholder="example@email.com"
			/>
		</p>

		<p class="my-1">
			Выберите категорию обращения:
			<select class="form-select" v-model="complaint.feedbackCategoryId">
				<option v-for="cat in categories" :key="cat.id" :value="cat.id">
					{{ cat.name }}
				</option>
			</select>
		</p>

		<p class="my-1">Краткое название обращения:</p>
		<input
			class="form-control d-inline w-25"
			v-model="complaint.title"
			placeholder="Не работает ..."
		/>
		<p class="my-1">Напишите подробно, что случилось или какие есть предложения:</p>
		<textarea
			class="form-control w-75"
			v-model="complaint.text"
			placeholder="Проблема была с ..."
		></textarea>

		<button class="my-2 btn btn-primary" @click="sendReport">Отправить</button>
	</div>
</template>

<style scoped>
select {
	display: inline;
	width: 330px;
}
</style>
