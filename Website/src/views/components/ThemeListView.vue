<script setup lang="ts">
import type { Theme } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { storeToRefs } from 'pinia'
import { inject, ref, type Ref } from 'vue'


const props = defineProps<{
    themes: Theme[]
}>()
const themes = ref(props.themes)
const { user } = storeToRefs(useUserStore())

const deleteTheme = async (themeId: number, index:number) => {
    await api.delete<Theme>(`themes/${themeId}`)
    .then((res) => {
        alert(`Успешное удаление темы ${res.data.name}`)
        themes.value.splice(index, 1)
    })
}
const addTheme = async () => {
    const newThemeName = prompt('Напишите, какую новую тему хотите добавить', 'Новая тема')
    if (newThemeName) {
        const newTheme: Theme = {
            id: 0,
            name: newThemeName,
        }
        await api.post<Theme>('themes', newTheme).then((res) => {
            alert('Новая тема успешно добавлена')
            themes.value.push(res.data)
        })
    }
}
const updateTheme = async (themeToUpdate: Theme) => {
    const newThemeName = prompt('Напишите, новое название для темы', themeToUpdate.name)
    if (newThemeName) {
        let oldName = themeToUpdate.name
        themeToUpdate.name = newThemeName
        await api.put<Theme>(`themes`, themeToUpdate).then((res) => {
            alert('Тема успешно обновлена')
        })
            .catch(err => {
                alert('Ошибка: тема не обновлена')
                themeToUpdate.name = oldName
        })
    }
}
</script>

<template>
    <h2>
        Темы
        <button v-if="user && user.role.id == 1" class="btn btn-primary" @click="addTheme">Добавить</button>
    </h2>
    <div class="d-flex flex-wrap">
        <div class="card mx-1" v-for="theme, index in themes">
          <div class="card-body p-2">
            <h5 class="card-title">{{ theme.name }}</h5>
            <p v-if="user?.role.id == 1">
                <button class="btn btn-sm btn-danger" @click="deleteTheme(theme.id, index)">🗑️</button>
                <button class="btn btn-sm btn-warning mx-1" @click="updateTheme(theme)">⚙️</button>
            </p>
          </div>
        </div>
    </div>
</template>

<style scoped>

</style>
