import type { SignInResponse, User } from '../models/main'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useUserStore = defineStore('user-info', () => {
  const user = ref<User | null>(null)
  const fullName = computed(() => {
    if (user.value) {
      let info = user.value.userInformation
      return `${info.lastName} ${info.firstName} ${info.middleName}`
    }
    return 'Не авторизован'
  })
  function signIn(signInResponse: SignInResponse) {
    user.value = signInResponse.user
  }
  function saveUser(userToSave: User) {
    user.value = userToSave
  }
  return { user, fullName, signIn, saveUser }
})
