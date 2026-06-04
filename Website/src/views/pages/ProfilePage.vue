<script lang="ts" setup>
import { useUserStore } from '@/stores/user'
import { reactive, ref } from 'vue'
import api from '@/services/api.ts'
import { storeToRefs } from 'pinia'
import {
	BButton,
	BForm,
	BFormFloatingLabel,
	BFormInput,
	BModal,
	useToggle,
} from 'bootstrap-vue-next'
const isEditing = ref(false)
const { user: userFromStore } = useUserStore()
const { user: changeUserStore } = storeToRefs(useUserStore())
const user = reactive({
	email: userFromStore?.email,
	lastName: userFromStore?.userInformation.lastName,
	firstName: userFromStore?.userInformation.firstName,
	middleName: userFromStore?.userInformation.middleName,
	position: userFromStore?.userInformation.position,
	phone: userFromStore?.userInformation.phone,
})
const save = async () => {
	const newUserInformation = {
		userId: userFromStore?.id,
		firstName: user.firstName,
		middleName: user.firstName,
		lastName: user.lastName,
		position: user.position,
		phone: user.phone,
	}
	api.put('/users', {
		id: userFromStore?.id,
		email: user.email,
		firstName: user.firstName,
		roleId: userFromStore?.role.id,
		userInformation: newUserInformation,
	}).then((res) => {
		alert('Данные сохранены')
		isEditing.value = false
		if (changeUserStore.value) {
			changeUserStore.value.userInformation = newUserInformation
		}
	})
}
const { hide: closeModal, show: showModal } = useToggle('password-modal')
const close = () => {
	closeModal()
}
const newPassword = ref('')
const changePasswordAsync = async () => {
	await api
		.post('users/change-password', {
			userId: userFromStore?.id,
			password: newPassword.value,
		})
		.then((res) => {
			alert('Пароль успешно изменен')
		})
}
const showPasswordChangeInput = async () => {
	if (confirm('Вы уверены, что хотите поменять пароль?')) {
		await showModal()
	}
}
const errors = ref<string[]>([])
const onPasswordInput = () => {
	errors.value = []
	const password = newPassword.value
	const errs = []
	if (password.length < 8) errs.push('Минимум 8 символов')
	if (!/[a-z]/.test(password)) errs.push('Требуется строчная буква (a-z)')
	if (!/[A-Z]/.test(password)) errs.push('Требуется заглавная буква (A-Z)')
	if (!/\d/.test(password)) errs.push('Требуется цифра (0-9)')
	if (!/[^a-zA-Z0-9]/.test(password))
		errs.push('Требуется специальный символ (не буква и не цифра)')
	errors.value = errs
}
</script>

<template>
	<div class="container" v-if="user">
		<h3 class="my-3">Профиль</h3>

		<div class="d-flex flex-column gap-2 my-2">
			<span> Почта: <input type="email" disabled v-model="user.email" /> </span>
			<span> UID: {{ userFromStore?.id }} </span>
			<span>
				Пароль:
				<button class="btn btn-outline-dark" @click="showPasswordChangeInput">
					Сменить пароль
				</button>
			</span>
			<span>Фамилия: </span>
			<input type="text" :disabled="!isEditing" v-model="user.lastName" />
			<span>Имя: </span>
			<input type="text" :disabled="!isEditing" v-model="user.firstName" />
			<span>Отчество: </span>
			<input type="text" :disabled="!isEditing" v-model="user.middleName" />
			<span>Дополнительная информация: </span>
			<span>Телефон: </span>
			<input type="text" :disabled="!isEditing" v-model="user.phone" />

			<span>Должность: </span>
			<input type="text" :disabled="!isEditing" v-model="user.position" />
		</div>
		<button v-if="!isEditing" class="btn btn-outline-primary" @click="isEditing = true">
			Изменить профиль
		</button>
		<button v-else class="btn btn-outline-primary" @click="save">Сохранить изменения</button>

		<BModal
			id="password-modal"
			centered
			header-class="bg-dark text-white"
			header-close-class="bg-white"
			@close="close"
			title="Новый пароль"
		>
			<BForm>
				<BFormFloatingLabel class="my-2" label="Пароль" label-for="password-input">
					<BFormInput
						id="password-input"
						v-model.trim="newPassword"
						type="password"
                        @input="onPasswordInput"
						placeholder="Пароль"
					/>
				</BFormFloatingLabel>
				<p v-if="errors.length > 0">
					<span
						:key="index"
						class="d-block text-danger"
						v-for="(error, index) in errors"
						>{{ error }}</span
					>
				</p>
			</BForm>
			<template #footer>
				<BButton variant="primary" :disabled="errors.length > 0" @click="changePasswordAsync">Сохранить</BButton>
				<button class="btn btn-dark" @click="close">Отмена</button>
			</template>
		</BModal>
	</div>
</template>
<style scoped>
input {
	width: 200px;
}
</style>
