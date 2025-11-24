<script lang="ts" setup>
import { ref } from 'vue'

let email = ref('')
let password = ref('')
let authClick = async () => {

}
</script>

<template>
  <div>
    <h1>Авторизация</h1>
    <fieldset>
      <label>
        <span>Почта</span>
        <input v-model="email" placeholder="Почта" />
      </label>
      <label>
        <span>Пароль</span>
        <input v-model="password" type="password" placeholder="Пароль..." />
      </label>
      <br />
      <button @click="authClick"></button>
    </fieldset>
  </div>
</template>

<style></style>
