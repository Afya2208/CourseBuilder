import type { User } from '../models/main'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useUserStore = defineStore('user', () => {
  const user = ref<User | null>(null)
  const fullName = computed(() => {
    if (user.value) {
      return `${user.value.lastName} ${user.value.firstName} ${user.value.middleName}`
    }
    return 'Не авторизован'
  })
  function signIn(newUser: User) {
    user.value = newUser
  }

  return { user, fullName, signIn }
})
