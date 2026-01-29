<script lang="ts" setup>
import type { User } from '@/models/main';
import { ref } from 'vue';

const users = ref<User[]>([])
const fetch = async () => {

}
</script>
<template>
    <div>
        <h2>Пользователи</h2>
        <div v-if="users.length > 0">
            <div v-for="user in users">
                <form>
                    <div class="form-floating">
                        <input class="form-control" id="lastName" v-model="user.userInformation.lastName"
                        placeholder="Иванов"/>
                        <label for="lastName">Фамилия</label>
                    </div>
                    <div class="form-floating">
                        <input class="form-control" id="firstName" v-model="user.userInformation.firstName"
                        placeholder="Иван"/>
                        <label for="firstName">Имя</label>
                    </div>
                    <div class="form-floating">
                        <input class="form-control" id="lastName" v-model="user.userInformation.middleName"
                        placeholder="Иванов"/>
                        <label for="lastName">Фамилия</label>
                    </div>
                    <div class="form-floating">
                        <input class="form-control" id="lastName" v-model="user.email"
                        placeholder="Иванов"/>
                        <label for="lastName">Фамилия</label>
                    </div>
                    <div class="form-floating">
                        <input class="form-control" id="lastName" v-model="user.rol"
                        placeholder="Иванов"/>
                        <label for="lastName">Пар</label>
                    </div>
                </form>
                
            </div>
        </div>
    </div>
</template>
<style scoped>

</style>