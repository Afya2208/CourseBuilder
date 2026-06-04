<script lang="ts" setup>
import type { Kit } from '@/models/kit'
import type { Course } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Validator } from '@/util/validator.ts'

const dataId = parseInt(useRoute().params.dataId as string)
const type = useRoute().params.type as string

const { user } = useUserStore()
const router = useRouter()
const course = ref<Course>()
const price = ref<number>()
const kit = ref<Kit>()
const { addAvailableCourse, addAvailableKit } = useUserStore()

const loadDataAsync = async () => {
	if (dataId && type == 'course') {
		await api.get<Course>('courses/' + dataId).then((res) => {
			course.value = res.data
			price.value = course.value.price
		})
	} else if (dataId && type == 'kit') {
		await api.get<Kit>('kits/' + dataId).then((res) => {
			kit.value = res.data
			price.value = kit.value.price
		})
	} else {
		alert(
			'Ошибка: неверные данные формы. Попробуйте найти курс или набор на сайте и перейдите на страницу оплаты именно по их ссылке',
		)
		router.push('/')
	}
}
const checkFields = (): boolean => {
	const val: Validator = new Validator()
	val.validate(!cvc.value || cvc.value.length < 3, 'Укажите CVC')
	val.validate(!card.value || card.value.length < 19, 'Укажите номер карты')
	val.validate(!expiry.value || expiry.value.length < 5, 'Укажите срок действия карты')
	return val.returnResult()
}
const tryPay = async () => {
	if (!checkFields()) {
		return
	}
	if (dataId && type == 'course') {
		if (course.value?.linkedGroupId) {
			await api
				.post(`users/${user?.id}/add-course-group/${course.value?.linkedGroupId}`)
				.then((res) => {
					router.push('/my-groups')
					alert(
						'Курс успешно добавлен в список доступных. Вы добавлены в группу/поток по этому курсу',
					)
					addAvailableCourse(dataId)
				})
				.catch((err) => {
					if (err.status == 400) {
						alert('Данный курс уже есть у Вас')
					}
				})
		} else {
			await api
				.post(`users/${user?.id}/add-course/${dataId}`)
				.then((res) => {
					router.push('/my-courses')
					alert('Курс успешно добавлен в список доступных')
					addAvailableCourse(dataId)
				})
				.catch((err) => {
					if (err.status == 400) {
						alert('Данный курс уже есть у Вас')
					}
				})
		}
	} else if (dataId && type == 'kit') {
		await api
			.post(`users/${user?.id}/add-kit/${dataId}`)
			.then((res) => {
				router.push('/my-courses')
				alert('Набор и курсы успешно добавлены в список доступных')
				if (kit.value?.coursesInfo) {
					for (let c of kit.value?.coursesInfo) {
						addAvailableCourse(c.id)
					}
				}
				addAvailableKit(dataId)
			})
			.catch((err) => {
				if (err.status == 400) {
					alert('Данный набор уже есть у Вас')
				}
			})
	}
}

onMounted(async () => {
	await loadDataAsync()
})

const card = ref()
const expiry = ref()
const cvc = ref()

const formatCard = (e) => {
	let val = e.target.value.replace(/\D/g, '').slice(0, 16)
	val = val.replace(/(\d{4})(?=\d)/g, '$1 ')
	card.value = val
}

const formatExpiry = (e) => {
	let val = e.target.value.replace(/\D/g, '').slice(0, 4)
	if (val.length > 2) val = val.slice(0, 2) + '/' + val.slice(2)

	const month = parseInt(val.slice(0, 2))
	if (month > 12) val = '12' + val.slice(2)
	if (month === 0 && val[0] !== '0') val = '0' + val.slice(1)

	expiry.value = val
}

const formatCvc = (e) => {
	cvc.value = e.target.value.replace(/\D/g, '').slice(0, 3)
}
</script>

<template>
	<div class="container">
		<div class="text-center mx-auto p-4" style="max-width: 500px">
			<h3 class="my-2">Оплата</h3>
			<p>
				Покупка
				<span v-if="type == 'kit'">набора {{ kit?.name }}</span>
				<span v-else-if="type == 'course'">курса {{ course?.name }}</span>
				за {{ price }} руб. для пользователя {{ user?.email }}
			</p>
			<p>
				Номер карты
				<input
					class="form-control d-inline long-in mb-2"
					v-model="card"
					maxlength="19"
					placeholder="1234 1234 1234 1234"
					@input="formatCard"
				/><br />
				Срок действия карты
				<input
					maxlength="5"
					v-model="expiry"
					class="form-control d-inline very-small"
					placeholder="ММ/ГГ"
					@input="formatExpiry"
				/>
				CVC
				<input class="form-control d-inline very-small" v-model="cvc" @input="formatCvc" />
			</p>
			<button class="btn btn-primary w-100" @click="tryPay">Купить</button>
		</div>
	</div>
</template>

<style scoped>
.very-small {
	width: 80px;
}
.long-in {
	width: 200px;
}
</style>
