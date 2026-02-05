<script lang="ts" setup>
import type { Role, User, UserEditable } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import {
	BButton,
    BForm,
    BModal,
	BFormFloatingLabel,
    BFormSelect,
    BFormInput,
    useToggle,
} from 'bootstrap-vue-next'
import { onMounted, ref } from 'vue'

const users = ref<User[]>([])
const roles = ref<Role[]>([])
const userStore = useUserStore()

onMounted(async () => {
	await fetch()
})

const fetch = async () => {
	await api.get<User[]>('users').then((res) => {
		users.value = res.data
	})
	await api.get<Role[]>('roles').then((res) => {
		roles.value = res.data
	})
}
const changePassword = async (user: User) => {
    if (confirm(`Вы уверены, что хотите сменить пароль у ${user.email}?`)) {
        let newPassword = prompt("Введите новый пароль")
        if (newPassword) {
            await api.post("users/"+user.id + "/change-password", {password: newPassword, userId:user.id}).then(res => {
                alert("Пароль успешно изменен")
            })
        }
	}
}
const deleteUser = async (user: User, index: number) => {
	if (confirm(`Вы уверены, что хотите удалить пользователя ${user.email}?`)) {
        await api.delete("users/"+user.id).then(res => {
            users.value?.splice(index, 1)
        })
	}
}
const { hide: hideModal, show: showModal} = useToggle("user-modal")
const addUser = async () => {
	showModal()
}
const startEditingUser = (user:User, index:number) => {
    selectedUserIndex.value = index
    userEditable.value = {
        id: user.id,
        email: user.email,
        roleId: user.role.id,
        userInformation: {
            userId: user.id,
            lastName: user.userInformation.lastName,
            middleName: user.userInformation.middleName,
            firstName: user.userInformation.firstName,
            position: user.userInformation.position,
            phone: user.userInformation.phone,
        }
    }
    showModal()
}

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
        }
	}
}
const userEditable = ref<UserEditable>(getNullUser())
const close = () => {
	if (userEditable.value.id != 0) {
		selectedUserIndex.value = undefined
		userEditable.value = getNullUser()
	}
	hideModal()
}
const saveUser = async () => {

    if (!userEditable.value.email) {
        alert("Укажите email")
        return
    }
    if (userEditable.value.id == 0 && (!userEditable.value.password || userEditable.value.password.length < 8)) {
        alert("Укажите пароль как минимум 8 символов")
        return
    }

	if (userEditable.value.id == 0) {
		await api
            .post<User>('sign-up', { ...userEditable.value, ...userEditable.value.userInformation })
			.then((res) => {
				users.value.push(res.data)
				userEditable.value = getNullUser()
				hideModal()
				alert('Пользователь успешно создан')
			})
            .catch((err) => {
                if (err.status == 400) {
                    alert('Ошибка, данная почта уже занята')
                }
			})
    } else {
        console.log(JSON.stringify(userEditable.value))
		await api
			.put<User>('users', userEditable.value)
			.then((res) => {
				let oldModule = users.value[selectedUserIndex.value!!]
				let newModule = res.data
				users.value[selectedUserIndex.value!!] = newModule
				alert('Пользователь успешно изменен')
			})
			.catch((err) => {
				alert('Ошибка, повторите позже')
			})
		hideModal()
		userEditable.value = getNullUser()
		selectedUserIndex.value = undefined
	}
}
const selectedUserIndex = ref<number>()
</script>
<template>
	<div style="padding: 10px">
		<h2>
			Пользователи
			<BButton variant="primary" @click="addUser">Добавить пользователя</BButton>
		</h2>
		<div class="d-flex flex-wrap">
			<div class="table-responsive">
				<table class="table table-striped table-hover table-bordered">
					<thead>
						<tr>
							<th>Email</th>
							<th>Роль</th>
							<th>Фамилия</th>
							<th>Имя</th>
							<th>Отчество</th>
							<th>Телефон</th>
							<th>Должность</th>
							<th>Действия</th>
						</tr>
					</thead>
					<tbody>
						<tr v-for="(user, index) in users" :key="user.id">
							<td>
                                <p>
                                    {{ user.email }}
                                </p>
							</td>
							<td>
								 <p>
                                    {{ user.role.name }}
                                </p>
							</td>
							<td>
								<p>
                                    {{ user.userInformation.lastName }}
                                </p>
							</td>
							<td>
								<p>
                                    {{ user.userInformation.firstName }}
                                </p>
							</td>
							<td>
								<p v-if="user.userInformation.middleName">
                                    {{ user.userInformation.middleName }}
                                </p>
							</td>
							<td>
								<p>
                                    {{ user.userInformation.phone }}
                                </p>
							</td>
							<td>
								<p>
                                    {{ user.userInformation.position }}
                                </p>
							</td>
							<td>
								<div class="d-flex gap-1">
                                    <button @click="changePassword(user)" class="btn btn-outline-secondary btn-sm">
                                        Сменить пароль
                                    </button>
                                    <button @click="startEditingUser(user, index)" class="btn btn-outline-warning btn-sm">
                                        ✏️
                                    </button>
                                    <button @click="deleteUser(user, index)" class="btn btn-outline-danger btn-sm">
                                        ❌
                                    </button>
								</div>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
		</div>
	</div>

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
            <BFormFloatingLabel v-if="userEditable.id == 0" class="my-2" label="Email" label-for="user-email">
				<BFormInput
					id="user-email"
					v-model="userEditable.email"
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
                v-model="userEditable.roleId"
			/>
			<BFormFloatingLabel class="my-2" label="Фамилия" label-for="user-last">
				<BFormInput
					id="user-last"
					v-model="userEditable.userInformation.lastName"
					type="text"
					placeholder="Иванов"
				/>
			</BFormFloatingLabel>

            <BFormFloatingLabel class="my-2" label="Имя" label-for="user-first">
				<BFormInput
					id="user-first"
					v-model="userEditable.userInformation.firstName"
					type="text"
					placeholder="Иван"
				/>
			</BFormFloatingLabel>

            <BFormFloatingLabel class="my-2" label="Отчество" label-for="user-middle">
				<BFormInput
					id="user-middle"
					v-model="userEditable.userInformation.middleName"
					type="text"
					placeholder="Иванович"
				/>
			</BFormFloatingLabel>

             <BFormFloatingLabel class="my-2" label="Телефон" label-for="user-phone">
				<BFormInput
					id="user-phone"
					v-model="userEditable.userInformation.phone"
					type="text"
					placeholder="89992221221"
				/>
			</BFormFloatingLabel>

             <BFormFloatingLabel class="my-2" label="Должность" label-for="user-position">
				<BFormInput
					id="user-position"
					v-model="userEditable.userInformation.position"
					type="text"
					placeholder="Менеджер"
				/>
			</BFormFloatingLabel>

             <BFormFloatingLabel v-if="userEditable.id == 0" class="my-2" label="Пароль" label-for="user-password">
				<BFormInput
					id="user-password"
					v-model="userEditable.password"
					type="password"
					placeholder="Пароль"
				/>
			</BFormFloatingLabel>
		</BForm>
		<template #footer>
			<BButton variant="primary" @click="saveUser()">Сохранить</BButton>
			<button class="btn btn-dark" @click="close">Отмена</button>
		</template>
	</BModal>
</template>
<style scoped>

</style>
