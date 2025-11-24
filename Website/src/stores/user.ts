
import type { SignInResponse } from "../models/main"
import { defineStore } from "pinia"
import { computed, ref } from "vue"

export const useUserStore = defineStore('user', () => {
  const user = ref<SignInResponse>(undefined)
  const fullName = computed(() => {
    if (user.value) {
        return `${user.value.lastName} ${user.value.firstName} ${user.value.middleName}`;
    }
    return "Не авторизован";
  } )
  function signIn(newUser:SignInResponse) {
    user.value = newUser
  }

  return { user, fullName, signIn }
})