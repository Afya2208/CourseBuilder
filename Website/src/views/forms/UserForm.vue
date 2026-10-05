<script lang="ts" setup>
import type { Role, User, UserEditable } from '@/models/main.ts'
import api from '@/services/api.ts'
import { Validator } from '@/util/validator.ts'
import {
	BForm,
	BFormFloatingLabel,
	BFormInput,
	BFormSelect,
	BModal,
	useToggle,
} from 'bootstrap-vue-next'
import { ref, toRefs } from 'vue'

const props = defineProps<{
	roles: Role[]
}>()

const { roles } = toRefs(props)

const { hide: hideModal, show: showModal } = useToggle('user-modal')
const startCreatingUser = async () => {
	user.value = getNullUser()
	showModal()
}
const startEditingUser = (u: User) => {
	user.value = {
		id: u.id,
		email: u.email,
		roleId: u.role.id,
		userInformation: {
			userId: u.id,
			lastName: u.userInformation.lastName,
			middleName: u.userInformation.middleName,
			firstName: u.userInformation.firstName,
			position: u.userInformation.position,
			phone: u.userInformation.phone,
		},
	}
	showModal()
}
defineExpose({ startCreatingUser, startEditingUser })

const getNullUser = (): UserEditable => {
	return {
		id: 0,
		email: '',
		password: '',
		roleId: 1,
		userInformation: {
			userId: 0,
			lastName: '',
			middleName: '',
			firstName: '',
			position: '',
			phone: '',
		},
	}
}

const user = ref<UserEditable>(getNullUser())
const close = () => {
	emits('closed')
	hideModal()
}
const emits = defineEmits<{
	saved: [u: User]
	closed: []
}>()

const checkFields = (): boolean => {
	const v = new Validator()
	v.validate(!user.value.email, 'Укажите email')
	v.validate(
		user.value.id == 0 && (!user.value.password || user.value.password.length < 8),
		'Укажите пароль как минимум 8 символов',
	)
	return v.returnResult()
}

const saveUser = async () => {
	if (checkFields()) {
		if (user.value.id == 0) {
			await api
				.post<User>('sign-up', { ...user.value, ...user.value.userInformation })
				.then((res) => {
					hideModal()
					alert('Пользователь успешно создан')
					emits('saved', res.data)
				})
				.catch((err) => {
					if (err.status == 400) {
						alert('Ошибка, данная почта уже занята')
					}
				})
		} else {
			await api
				.put<User>('users', user.value)
				.then((res) => {
					hideModal()
					alert('Пользователь успешно изменен')
					emits('saved', res.data)
				})
				.catch((err) => {
					alert('Ошибка, повторите позже')
				})
		}
	}
}
</script>

<template>
	<div>
		<BModal
			id="user-modal"
			centered
			header-class="bg-dark text-white"
			header-close-class="bg-white"
			@close="close"
			title="Новый пользователь"
			no-close-on-backdrop
		>
			<BForm>
				<BFormFloatingLabel
					v-if="user.id == 0"
					class="my-2"
					label="Email"
					label-for="user-email"
				>
					<BFormInput
						id="user-email"
						v-model="user.email"
						type="email"
						placeholder="email@t.com"
					/>
				</BFormFloatingLabel>
				<p>Роль:</p>
				<BFormSelect
					:options="roles"
					id="user-role"
					text-field="name"
					value-field="id"
					v-model="user.roleId"
				/>
				<BFormFloatingLabel class="my-2" label="Фамилия" label-for="user-last">
					<BFormInput
						id="user-last"
						v-model="user.userInformation.lastName"
						type="text"
						placeholder="Иванов"
					/>
				</BFormFloatingLabel>

				<BFormFloatingLabel class="my-2" label="Имя" label-for="user-first">
					<BFormInput
						id="user-first"
						v-model="user.userInformation.firstName"
						type="text"
						placeholder="Иван"
					/>
				</BFormFloatingLabel>

				<BFormFloatingLabel class="my-2" label="Отчество" label-for="user-middle">
					<BFormInput
						id="user-middle"
						v-model="user.userInformation.middleName"
						type="text"
						placeholder="Иванович"
					/>
				</BFormFloatingLabel>

				<BFormFloatingLabel class="my-2" label="Телефон" label-for="user-phone">
					<BFormInput
						id="user-phone"
						v-model="user.userInformation.phone"
						type="text"
						placeholder="89992221221"
					/>
				</BFormFloatingLabel>

				<BFormFloatingLabel class="my-2" label="Должность" label-for="user-position">
					<BFormInput
						id="user-position"
						v-model="user.userInformation.position"
						type="text"
						placeholder="Менеджер"
					/>
				</BFormFloatingLabel>

				<BFormFloatingLabel
					v-if="user.id == 0"
					class="my-2"
					label="Пароль"
					label-for="user-password"
				>
					<BFormInput
						id="user-password"
						v-model="user.password"
						type="password"
						placeholder="Пароль"
					/>
				</BFormFloatingLabel>
			</BForm>
			<template #footer>
				<button class="btn btn-primary" @click="saveUser">Сохранить</button>
				<button class="btn btn-dark" @click="close">Отмена</button>
			</template>
		</BModal>
	</div>
</template>
