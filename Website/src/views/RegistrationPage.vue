<script lang="ts" setup>
import { BButton, BForm, BFormFloatingLabel, BFormInput } from 'bootstrap-vue-next'

import type { SignUpRequest } from '@/models/signUpRequest'
import api from '@/services/api'
import { Validator } from '@/util/validator.ts'
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const signUp = async () => {
	if (checkFields()) {
		await api
			.post('sign-up', data.value)
			.then(() => {
				alert('Вы успешно зарегистрированы')
				router.push('/auth')
			})
			.catch((err) => {
				if (err.status == 400) {
					alert('Введенная почта уже занята')
				}
			})
	}
}

const errors = ref<string[]>([])

const checkFields = (): boolean => {
	const v = new Validator()
	v.validate(!data.value.firstName, 'Требуется указать имя')
	v.validate(!data.value.email, 'Требуется указать email')
	v.validate(data.value.roleId == 0, 'Требуется указать, чем Вы хотите заниматься')
	v.validate(!data.value.lastName, 'Требуется указать фамилию')
	return v.returnResult()
}
const data = ref<SignUpRequest>({
	phone: '',
	lastName: '',
	firstName: '',
	middleName: '',
	position: '',
	email: '',
	password: '',
	roleId: 0,
})

const onPasswordInput = () => {
	errors.value = []
	const password = data.value.password
	const errs = []
	if (password.length < 8) errs.push('Минимум 8 символов')
	if (!/[a-z]/.test(password)) errs.push('Требуется строчная буква (a-z)')
	if (!/[A-Z]/.test(password)) errs.push('Требуется заглавная буква (A-Z)')
	if (!/\d/.test(password)) errs.push('Требуется цифра (0-9)')
	if (!/[^a-zA-Z0-9]/.test(password)) errs.push('Требуется специальный символ (не буква и не цифра)')
	errors.value = errs
}
</script>

<template>
	<div class="container">
		<h3 class="m-3">Регистрация</h3>
		<BForm>
			<BFormFloatingLabel class="m-2" label="Email" label-for="email">
				<BFormInput id="email" v-model.trim="data.email" type="email" placeholder="" />
			</BFormFloatingLabel>
			<BFormFloatingLabel class="m-2" label="Пароль" label-for="password">
				<BFormInput
					id="password"
					v-model="data.password"
					@input="onPasswordInput"
					placeholder=""
					type="password"
				/>
			</BFormFloatingLabel>

			<p v-if="errors.length > 0">
				<span :key="index" class="d-block text-danger" v-for="(error, index) in errors">{{ error }}</span>
			</p>

			<BFormFloatingLabel class="m-2" label="Фамилия" label-for="lastName">
				<BFormInput id="lastName" v-model.trim="data.lastName" type="text" placeholder="" />
			</BFormFloatingLabel>
			<BFormFloatingLabel class="m-2" label="Имя" label-for="firstName">
				<BFormInput
					id="firstName"
					v-model.trim="data.firstName"
					type="text"
					placeholder=""
				/>
			</BFormFloatingLabel>
			<BFormFloatingLabel class="m-2" label="Отчество" label-for="middleName">
				<BFormInput
					id="middleName"
					v-model.trim="data.middleName"
					type="text"
					placeholder=""
				/>
			</BFormFloatingLabel>
			<div class="input-group m-2">
				<label class="input-group-text">Я хочу </label>
				<select v-model="data.roleId" class="form-select">
					<option selected value="0">[Выберите опцию]</option>
					<option value="3">Изучать курсы</option>
					<option value="1">Создавать курсы</option>
				</select>
			</div>
			<p>Дополнительная информация (необязательно):</p>
			<BFormFloatingLabel class="m-2" label="Телефон" label-for="phone">
				<BFormInput id="phone" v-model.trim="data.phone" type="text" placeholder=""  />
			</BFormFloatingLabel>
			<BFormFloatingLabel class="m-2" label="Должность/положение" label-for="position">
				<BFormInput id="position" v-model.trim="data.position" type="text" placeholder="" />
			</BFormFloatingLabel>
		</BForm>
		<BButton
			type="button"
			class="m-2"
			:disabled="errors.length > 0"
			variant="primary"
			@click="signUp"
			>Создать профиль</BButton
		>
	</div>
</template>

<style scoped>
input,
.input-group {
	width: 300px;
}
</style>
