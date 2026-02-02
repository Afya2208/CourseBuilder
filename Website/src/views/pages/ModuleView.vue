<script setup lang="ts">
import type { Course, Module, Lesson, LessonType } from '@/models/main'
import api from '@/services/api'
import { onMounted, ref, type Ref } from 'vue'
import { useRoute } from 'vue-router'
import { BButton, BCard, BCardText, useToggle } from 'bootstrap-vue-next'
import { useUserStore } from '@/stores/user'

const userStore = useUserStore()
const user = ref(userStore.user)

const module = ref<Module>()
const lessons = ref<Lesson[]>()
const lessonTypes = ref<LessonType[]>()
const courseId = parseInt(useRoute().params.courseId as string)
const moduleId = parseInt(useRoute().params.moduleId as string)
const getData = async () => {
    await api.get<Module>(`modules/${moduleId}`).then((res) => {
        module.value = res.data
    })
    await api.get<Lesson[]>(`modules/${moduleId}/lessons`).then((res) => {
        lessons.value = res.data
    })
    await api.get<LessonType[]>(`lesson-types`).then((res) => {
        lessonTypes.value = res.data
    })
}
const saveSelectedLesson = async () => {
    if (selectedLesson.value.id == 0) {
        await api.post("").then(res => {
            alert("Занятие успешно сохранено")
            lessons.value?.push(res.data)
            selectedLesson.value = getNullLesson()
        }).catch(err => {
            alert("Ошибка, повторите позже")
        })
    }
    else {
        await api.put("").then(res => {
            alert("Занятие успешно сохранено")
            lessons.value?.push(res.data)
            
        }).catch (err => {
            alert("Ошибка, повторите позже")
        }) 
        selectedLesson.value = getNullLesson()
    }
    hideModal()
}
onMounted(async () => {
    await getData()
})
const getNullLesson = () : Lesson => {
    return {
        id: 0,
        name: '',
        description:'',
        moduleId: moduleId,
        lessonTypeId: 1,
        order: lessons.value?.length ?? 0 + 1
    }
}
const addLesson = () => {
    selectedLesson.value = getNullLesson()
    showModal()
}
const redactLesson = (lesson: Lesson) => {
    selectedLesson.value = lesson;
    showModal()
}
const deleteLesson = async (lesson:Lesson, index: number) => {
    if (confirm(`Вы уверены, что хотите удалить ${lesson.name} занятие?`)) {

    }
}
const {hide:hideModal, show:showModal} = useToggle('lesson-modal')
const selectedLesson = ref<Lesson>(getNullLesson())

</script>

<template>
    <div class="container">
        <div v-if="module">
            <h1>{{ module.name }}</h1>
            <p v-if="module.description">{{ module.description }}</p>
            <p v-if="module.lessonsCount">{{ module.lessonsCount }} занятий</p>
            <RouterLink :to="`/courses/${module.courseId}`">Вернуться к списку модулей</RouterLink>
        </div>
        <h3>
            Занятия
            <BButton @click="addLesson" v-if="user?.role.id == 1">Добавить новое занятие</BButton>
        </h3>
        <div class="d-flex flex-wrap" v-if="lessons" >
            <BCard :title="lesson.name" class="m-2" style="max-width: 20rem;"
            :class="{ is_control: lesson.lessonTypeId == 2, is_study: lesson.lessonTypeId == 1 }"
            v-for="lesson in lessons" :style="{ order: lesson.order }">
                <BCardText>    
                    <p>{{ lesson.description }}</p>
                    <p><RouterLink :to="`/courses/modules/lessons/${lesson.id}`">Перейти к занятию</RouterLink></p>
                </BCardText>
            </BCard>
        </div>
    </div>

    <BModal id="lesson-modal" centered 
        header-class="bg-dark text-white"
        header-close-class="bg-white"
        title="Новое занятие" no-close-on-backdrop>
        <BForm>
            <BFormFloatingLabel
                class="my-2"
                label="Название"
                label-for="lesson-name">
                <BFormInput id="lesson-name"
                v-model="selectedLesson.name"
                type="text" placeholder="Новое занятие"/>
            </BFormFloatingLabel>

            <BFormFloatingLabel
                class="my-2"
                label="Описание"
                label-for="lesson-desc">
                <BFormInput id="lesson-desc"
                 v-model="selectedLesson.description"
                type="text" placeholder="Описание..."/>
            </BFormFloatingLabel>

            <div>
                <p>Тип занятия:</p>
                <BFormSelect :options="lessonTypes"
                select-size="7"
                v-model="selectedLesson.lessonTypeId"
                value-field="id"
                text-field="name"
                id="lesson-type" />
            </div>
            
        </BForm>
        <template #footer>
            <BButton variant="primary" @click="saveSelectedLesson()">Сохранить</BButton>
            <BButton variant="dark" @click="hideModal()">Отмена</BButton>
        </template>
    </BModal>
</template>

<style scoped>
.is_control {
    background-color: red;
}
.is_study {
    background-color: blue;
}
</style>
