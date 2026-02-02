<script lang="ts" setup>
import type { Role, User, UserEditable } from '@/models/main'
import api from '@/services/api';
import { useUserStore } from '@/stores/user';
import { BButton, BCard, BCardText, BCardTitle, BForm, BFormFloatingLabel,
    BInput, BCardBody, BFormSelect,
    BTable, BTbody
 } from 'bootstrap-vue-next';
import { storeToRefs } from 'pinia';
import { computed, onMounted, onUpdated, ref, watch } from 'vue'

const users = ref<UserEditable[]>()
const roles = ref<Role[]>()
const userStore = useUserStore()
const {user, fullName} = storeToRefs(userStore)

onMounted(async () => {
    await fetch()
})

const fetch = async () => {
    await api.get<UserEditable[]>("users").then(res => {
        users.value = res.data
    })
    await api.get<Role[]>("roles").then(res => {
        roles.value = res.data
    })
}
const changePassword = async () => {
    if (confirm("Вы уверены, что хотите сменить пароль этому пользователю?")) {

    }
}
const deleteUser = async (user:UserEditable, index:number) => {
    if (confirm(`Вы уверены, что хотите удалить пользователя ${user.email}?`)) {
        users.value?.splice(index, 1)
    }
}
const addUser = async () => {
    users.value?.push({
        id: 0,
        canRedact: true,
        email: '',
        roleId: 1,
        userInformation: {
            position: '',
            userId: 0,
            lastName: '',
            middleName: '',
            firstName: '',
            phone: '',
        }
    })
}
const redactUser = (user:UserEditable)  => {
    user.canRedact = !user.canRedact
}
const save = async () => {

}
</script>
<template>
    <div style="padding: 10px;">
        <p>{{ user?.email }}</p>
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
                        <BInput 
                        :disabled="!user.canRedact" 
                        v-model="user.email"
                        size="sm"
                        />
                    </td>
                    <td>
                        <BFormSelect 
                        v-model="user.roleId" 
                        :options="roles"
                        :disabled="!user.canRedact" 
                        size="sm"
                        value-field="id" 
                        text-field="name"
                        />
                    </td>
                    <td>
                        <BInput 
                        :disabled="!user.canRedact" 
                        v-model="user.userInformation.lastName"
                        size="sm"
                        />
                    </td>
                    <td>
                        <BInput 
                        :disabled="!user.canRedact" 
                        v-model="user.userInformation.firstName"
                        size="sm"
                        />
                    </td>
                    <td>
                        <BInput 
                        :disabled="!user.canRedact" 
                        v-model="user.userInformation.middleName"
                        size="sm"
                        />
                    </td>
                    <td>
                        <BInput 
                        :disabled="!user.canRedact" 
                        v-model="user.userInformation.phone"
                        size="sm"
                        />
                    </td>
                    <td>
                        <BInput 
                        :disabled="!user.canRedact" 
                        v-model="user.userInformation.position"
                        size="sm"
                        />
                    </td>
                    <td>
                        <div class="d-flex gap-1">
                            
                        <BButton v-if="user.id == 0"
                            size="sm" 
                            @click="setPassword()"
                            variant="outline-secondary"
                        >
                            Установить пароль
                        </BButton>

                        <BButton v-else
                            size="sm" 
                            @click="changePassword()"
                            variant="outline-secondary"
                        >
                            Сменить пароль
                        </BButton>
                        <BButton 
                            size="sm" 
                            @click="redactUser(user)"
                            variant="outline-warning"
                        >
                            ✏️
                        </BButton>
                        <BButton 
                            size="sm" 
                            @click="deleteUser(user, index)"
                            variant="outline-danger"
                        >
                            ❌
                        </BButton>
                        </div>
                    </td>
                    </tr>
                </tbody>
                </table>
            </div>
        </div>
        <BButton variant="primary" @click="save">Сохранить изменения</BButton>
    </div>
</template>
<style scoped></style>
