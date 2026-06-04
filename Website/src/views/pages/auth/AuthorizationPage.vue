<script lang="ts" setup>
import { authorizationLogic } from './AuthorizationPage';

const {email, password, authClick, continueClick } = authorizationLogic();
</script>

<template>
	<div class="container text-center" style="max-width: 300px">
		<form>
			<h3 class="m-3 fw-normal">Авторизация</h3>
			<div class="form-floating my-1">
				<input
					v-model="email"
					type="email"
					id="email"
					class="form-control"
					placeholder="example@domain.com"
				/>
				<label for="email">Email</label>
			</div>
			<div class="form-floating my-1">
				<input
					type="password"
					id="password"
					v-model="password"
					class="form-control"
					placeholder="Пароль"
				/>
				<label for="password">Пароль</label>
			</div>
			<button @click="authClick" type="button" class="my-button btn btn-lg btn-primary my-2">
				Войти
			</button>
            <p class="text-secondary small mt-2 m-1">
                Нет профиля?
                <RouterLink  to="/reg">Регистрируйся</RouterLink>
            </p>
            <p class="text-secondary small pointer" @click="continueClick">
                Продолжить без профиля
            </p>

		</form>
	</div>
</template>

<style>
    .pointer {
        cursor: pointer;
    }
</style>
