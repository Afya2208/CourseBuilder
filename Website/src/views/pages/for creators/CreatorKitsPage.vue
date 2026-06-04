<script lang="ts" setup>
import type { Kit } from '@/models/kit';
import type { Course } from '@/models/main';
import api from '@/services/api';
import { useUserStore } from '@/stores/user';
import KitForm from '@/views/pages/forms/KitForm.vue';
import { storeToRefs } from 'pinia';
import { onMounted, ref } from 'vue';
import type { ComponentExposed } from 'vue-component-type-helpers';
import { RouterLink } from 'vue-router';


const kits = ref<Kit[]>()
const courses = ref<Course[]>()
const loadError = ref(false)
const kitForm = ref<ComponentExposed<typeof KitForm>>()
const {user} = useUserStore()

const loadDataAsync = async () => {
    await loadKitsAsync()
    await api.get<Course[]>(`courses/by-user/${user?.id}`)
    .then(res => {
        courses.value = res.data
    })
}

const editKit = (k:Kit) => {
    kitForm.value?.startEditingKit(k)
}
const addKit = () => {
    kitForm.value?.startCreatingKit()
}
const deleteKit = async (k:Kit, index:number) => {
    if (confirm("Вы уверены, что хотите удалить набор " + k.name + "?")) {
        await api.delete('kits/' + k.id).then(res => {
            alert("Набор успешно удален")
            kits.value?.splice(index, 1)
        });
    }
}
const onSaved = async () => {
    await loadKitsAsync()
}

const loadKitsAsync = async () => {
    await api.get<Kit[]>(`kits/by-user/${user?.id}`)
    .then(res => {
        kits.value = res.data
    })
    .catch(err => {
        loadError.value = false
    })
}

onMounted(async () => {
    await loadDataAsync()
})

</script>
<template>
    <div >
        <div class="my-3">
            <button class="btn btn-primary" @click="addKit">Создать новый набор курсов</button>
        </div>

        <KitForm ref="kitForm" :courses="courses!!" @saved="onSaved"/>

        <div v-if="!kits && !loadError">
            <p>Загрузка данных, подождите, пожалуйста</p>
        </div>

        <div v-else-if="!kits && loadError">
            <p>Ошибка загрузки данных, попробуйте позже</p>
        </div>

        <div v-else-if="kits && kits.length > 0">
            <div class="d-flex flex-wrap" >
                <div v-for="(kit, index) in kits" class="card m-2" style="width: 300px;">
                    <div class="card-body" >
                        <h5 class="card-title">
                            {{ kit.name }}
                        </h5>
                        <p class="card-text">
                            {{ kit.description }}
                            <br>
                            Стоимость: {{ kit.price }} руб.
                        </p>
                        <h6 class="card-subtitle my-2 text-muted">Курсы, входящие в набор:</h6>
                        <ul>
                            <li v-for="course in kit.coursesInfo">
                                <RouterLink :to="`/courses/${course.id}`">{{ course.name }}</RouterLink>
                            </li>
                        </ul>
                    </div>
                    <div class="card-footer">
                        <button class="btn btn-outline-danger " @click="deleteKit(kit, index)">❌</button>
                        <button class="btn btn-outline-warning mx-2" @click="editKit(kit)">✏️</button>
                    </div>
                </div>
            </div>
        </div>

        <div v-else>
            <p>К сожалению, пока нет Ваших наборов</p>
        </div>
    </div>
</template>
