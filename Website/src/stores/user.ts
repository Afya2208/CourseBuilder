import api from '@/services/api'
import type { SignInResponse, User } from '../models/main'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useUserStore = defineStore('user-info', {
	state: () => ({
		user: null as User | null,
		isInitialized: false,
		token: null as String | null,
		_initPromise: null as Promise<void> | null,
	}),
	getters: {
		fullName: (state) => {
			if (state.user) {
				let info = state.user.userInformation
				return `${info?.lastName} ${info?.firstName} ${info?.middleName}`
			}
			return 'Не авторизован'
		},
	},
	actions: {
		async init() {
			if (this.isInitialized) return Promise.resolve()
			if (this._initPromise) return this._initPromise

			this._initPromise = this.updateUserData().finally(() => (this._initPromise = null))
			return this._initPromise
		},
		async updateUserData() {
			let id = sessionStorage.getItem('userId')
			let token = sessionStorage.getItem('token')
			if (!token || !id || Number.isNaN(Number.parseInt(id))) {
				return
			}
			await api
				.get<User>(`users/${id}`, {
					headers: { Authorization: `Bearer ${token}` },
				})
				.then((res) => {
					this.user = res.data
					this.token = token
					api.defaults.headers.common.Authorization = `Bearer ${token}`
				})
				.catch((err) => {
					this.logOut()
				})
			this.isInitialized = true
		},

		logOut() {
			sessionStorage.removeItem('userId')
			sessionStorage.removeItem('token')
			this.user = null
			this.token = null
		},
	},
})
