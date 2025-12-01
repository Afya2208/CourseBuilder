<script setup lang="ts">
import type { Theme } from '@/models/main'
import api from '@/services/api'
import { useUserStore } from '@/stores/user'
import { storeToRefs } from 'pinia'
import { inject, ref, type Ref } from 'vue'

const themes = ref(inject<Theme[]>('themes') ?? [])

const { user } = storeToRefs(useUserStore())
let roleId:Ref<Number> = ref(0)
if (user.value) {
  roleId.value = user.value.roleId
}

const deleteThemeClick = async (themeId: number) => {
  await api.delete<Theme>(`themes/${themeId}`).then((res) => {
    alert(`Успешное удаление темы ${res.data.name}`)
  })
}
const addThemeClick = async () => {
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
const updateThemeClick = async (themeToUpdate: Theme) => {
  const newThemeName = prompt('Напишите, новое название для темы', themeToUpdate.name)
  if (newThemeName) {
    themeToUpdate.name = newThemeName
    await api.put<Theme>(`themes/${themeToUpdate.id}`, themeToUpdate).then((res) => {
      alert('Тема успешно обновлена')
      themes.value.push(res.data)
    })
  }
}
</script>

<template>
  <div v-if="roleId == 1">
    <span @click="addThemeClick">➕</span>
  </div>
  <ul>
    <li v-for="theme in themes">
      <p>{{ theme.name }}</p>
      <p v-if="roleId == 1">
        <span @click="deleteThemeClick(theme.id)">🗑️</span>
        <span @click="updateThemeClick(theme)">⚙️</span>
      </p>
    </li>
  </ul>
</template>

<style></style>
