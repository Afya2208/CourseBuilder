<script lang="ts" setup>
import type { Role, User, UserEditable } from '@/models/main'
import api from '@/services/api';
import { useUserStore } from '@/stores/user';
import { BButton, BCard, BCardText, BCardTitle, BForm, BFormFloatingLabel,
    BInput, BCardBody, BFormSelect
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
    <div class="container">
        <p>{{ user?.email }}</p>
        <h2>
            Пользователи
            <BButton variant="primary" @click="addUser">Добавить пользователя</BButton>
        </h2>
        <div class="d-flex flex-wrap">
            <BCard v-for="user, index in users" class="m-1">
                <BCardText>
                    <BForm>
                        

                        <BFormFloatingLabel class="my-1"
                            label-for="user-email"
                            label="Email">
                            <BInput :disabled="!user.canRedact" v-model="user.email" id="user-email"/>
                        </BFormFloatingLabel>
                        
                        <BFormSelect v-model="user.roleId" :options="roles"
                        :disabled="!user.canRedact"
                         value-field="id" text-field="name">
                        </BFormSelect>

                        <div v-if="user.userInformation">
                            <BFormFloatingLabel class="my-1"
                                label-for="user-lastName"
                                label="Фамилия">
                                <BInput :disabled="!user.canRedact" v-model="user.userInformation.lastName" id="user-lastName"/>
                            </BFormFloatingLabel>
                            <BFormFloatingLabel  class="my-1"
                                label-for="user-firstName"
                                label="Имя">
                                <BInput :disabled="!user.canRedact" v-model="user.userInformation.firstName" id="user-firstName"/>
                            </BFormFloatingLabel>
                            <BFormFloatingLabel class="my-1"
                                label-for="user-middleName"
                                label="Отчество">
                                <BInput :disabled="!user.canRedact" v-model="user.userInformation.middleName" id="user-middleName"/>
                            </BFormFloatingLabel>
                            <BFormFloatingLabel class="my-1"
                                label-for="user-phone"
                                label="Телефон">
                                <BInput :disabled="!user.canRedact" v-model="user.userInformation.phone" id="user-phone"/>
                            </BFormFloatingLabel>
                            <BFormFloatingLabel class="my-1"
                                label-for="user-phone"
                                label="Должность">
                                <BInput :disabled="!user.canRedact" v-model="user.userInformation.position" id="user-position"/>
                            </BFormFloatingLabel>
                            <BButton class="m-1 d-block" @click="changePassword">Сменить пароль</BButton>
                            <BButton class="m-1 d-block" @click="redactUser(user)">Редактировать</BButton>
                            <BButton class="m-1 d-block" @click="deleteUser(user, index)">Удалить</BButton>
                        </div>
                    </BForm>
                </BCardText>
            </BCard>
        </div>
        <BButton variant="primary" @click="save">Сохранить изменения</BButton>
    </div>
</template>
<style scoped></style>
