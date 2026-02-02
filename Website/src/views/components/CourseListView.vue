<script setup lang="ts">
import type { Course, CourseEditable, Theme } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import {
    BButton, BModal, useToggle, BForm, BFormFloatingLabel, BFormInput, BFormSelect,
    BFormCheckbox
 } from 'bootstrap-vue-next'
import { storeToRefs } from 'pinia'
import { inject, ref } from 'vue'
import { RouterLink } from 'vue-router'


const { user } = storeToRefs(useUserStore())
const props = defineProps<{
    themes: Theme[],
    courses: Course[]
}>()
const themes = ref(props.themes ?? [])
const courses = ref(props.courses ?? [])

const deleteCourse = async (courseId: number, index: number) => {
	await api.delete(`courses/${courseId}`).then(res=> {
        courses.value.splice(index, 1)
	})
}
const { hide:hideCourseModal, show:showCourseModal } = useToggle('add-course-modal')

const addCourse = () => showCourseModal()
const getNullCourse = () : CourseEditable => {
    return {
        id: 0,
        name: '',
        description: '',
        authorId: user.value?.id ?? 0,
        themesIds: [],
        minimalCompletionPercentage: 100,
        modulesHaveOrder: true
    }
}
const close = () => {
    if (editableCourse.value.id != 0) {
        editableCourse.value = getNullCourse()
        hideCourseModal()
    }
}
const editableCourse = ref<CourseEditable>(getNullCourse())
const saveCourse = async () => {
    if (editableCourse.value.id == 0) {
        await api.post<Course>("courses", editableCourse.value)
        .then(res => {
            courses.value.push({...res.data, lessonsCount: 0, modulesCount: 0,})
            editableCourse.value = getNullCourse()
            hideCourseModal()
            alert("Курс успешно создан")
        })
        .catch(err => {
            hideCourseModal()
            alert("Ошибка, повторорите позже")
        }) 
    }
    else {
        await api.put<Course>("courses", editableCourse.value)
        .then(res => {
            editableCourse.value = getNullCourse()
            hideCourseModal()
            alert("Курс успешно изменен")
        })
        .catch(err => {
            hideCourseModal()
            alert("Ошибка, повторорите позже")
        }) 
    }
}
const updateCourse = async (course: Course) => {
    editableCourse.value = {
        id: course.id,
        name: course.name,
        price: course.price,
        description: course.description,
        authorId: course.authorId ?? user.value?.id ?? 0,
        themesIds: [],
        minimalCompletionPercentage: course.minimalCompletionPercentage ?? 100,
        modulesHaveOrder: course.modulesHaveOrder ?? true
    }
    course.themes?.forEach(x=> editableCourse.value.themesIds.push(x.id))
    showCourseModal()
}
</script>

<template>
    <form class="d-flex">
        <input type="search" class="form-control " placeholder="Программирование...">
        <button type="button" class="btn mx-2 btn-outline-primary">Поиск</button>

        <label>
            <input type="checkbox"/>
            Искать по темам
        </label>
    </form>
    <div>
        <BButton class="my-2" v-if="user?.role.id == 1" variant="primary" @click="addCourse">Добавить новый курс</BButton>
    </div>

    <BModal id="add-course-modal" centered 
        header-class="bg-dark text-white"
        header-close-class="bg-white"
        title="Создание нового курса" no-close-on-backdrop>
        <BForm>
            <BFormFloatingLabel
                class="my-2"
                label="Название"
                label-for="course-name">
                <BFormInput id="course-name"
                v-model="editableCourse.name"
                type="text" placeholder="Новый курс"/>
            </BFormFloatingLabel>

            <BFormFloatingLabel
                class="my-2"
                label="Описание"
                label-for="course-desc">
                <BFormInput id="course-desc"
                 v-model="editableCourse.description"
                type="text" placeholder="Описание..."/>
            </BFormFloatingLabel>

            <BFormFloatingLabel
                class="my-2"
                label="Стоимость (если необходимо)"
                label-for="course-price">
                <BFormInput  v-model="editableCourse.price" 
                id="course-price"
                type="number" />
            </BFormFloatingLabel>

            <BFormCheckbox 
                id="course-modules-have-order"
                v-model="editableCourse.modulesHaveOrder">
                У модулей есть порядок
            </BFormCheckbox>

            <BFormFloatingLabel
                class="my-2"
                label="Минимальный процент прохождения для сдачи"
                label-for="course-min-percentage"
                >
                <BFormInput id="course-min-percentage"
                type="number"
                v-model="editableCourse.minimalCompletionPercentage"/>
            </BFormFloatingLabel>

            <div>
                <p>Темы:</p>
                <BFormSelect :options="themes"
                select-size="7"
                v-model="editableCourse.themesIds"
                value-field="id"
                text-field="name"
                multiple id="course-themes" />
            </div>
            
        </BForm>
        <template #footer>
            <BButton variant="primary" @click="saveCourse()">Сохранить</BButton>
            <BButton variant="dark" @click="hideCourseModal()">Отмена</BButton>
        </template>
    </BModal>

    <div class="d-flex">
        <div v-for="course, index in courses" class="card m-2">
            <div class="card-body" style="max-width: 18rem;">
                <h5 class="card-title">
                    {{ course.name }}
                    <span v-if="user?.role.id == 1">
                        <BButton variant="danger" @click="deleteCourse(course.id, index)">❌</BButton>
                        <BButton variant="warning" @click="updateCourse(course)">✏️</BButton>
                    </span>
                </h5>
                <p class="card-text">{{ course.description }}</p>
                <p class="card-text">
				    Количество модулей: {{ course.modulesCount }}
				    <br/>
				    Количество занятий: {{ course.lessonsCount }}
			    </p>
                <p class="card-text" v-if="course.price && course.price > 0">Стоимость: {{ course.price }} руб.</p>
                <RouterLink class="btn btn-primary" :to="`courses/${course.id}`">Перейти к курсу</RouterLink>
                <h6 class="card-subtitle my-2 text-muted ">Темы курса:</h6>
                <ul>
                    <li v-for="theme in course.themes">
                        <p>{{ themes.find(x=>x.id == theme.id)?.name }}</p>
                    </li>
				</ul>
            </div>
        </div>
    </div>
</template>
