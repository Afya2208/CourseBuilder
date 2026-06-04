import api from '@/services/api'
import type { SignInResponse, User } from '../models/main'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useUserStore = defineStore('user-info', {
	state: () => ({
        user: null as User | null,
        availableCoursesIds: null as number[] | null,
        availableKitsIds: null as number[] | null,
		isInitialized: false,
		token: null as string | null,
		_initPromise: null as Promise<void> | null,
	}),
	getters: {
		fullName: (state) => {
			if (state.user) {
				const info = state.user.userInformation
				return `${info?.lastName} ${info?.firstName} ${info?.middleName}`
			}
			return 'Не авторизован'
		},
    },
    actions: {
        addAvailableCourse(courseId:number) {
            this.availableCoursesIds?.push(courseId)
        },
        addAvailableKit(kitId:number) {
            this.availableKitsIds?.push(kitId)
        },
		async init() {
			if (this.isInitialized) return Promise.resolve()
			if (this._initPromise) return this._initPromise

			this._initPromise = this.updateUserData().finally(() => (this._initPromise = null))
			return this._initPromise
		},
		async updateUserData() {
			const id = localStorage.getItem('userId')
			const token = localStorage.getItem('token')
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
            
            await api.get<number[]>(`courses/available-for/${this.user?.id}/ids`)
            .then(res => {
                this.availableCoursesIds = res.data    
            })
            await api.get<number[]>(`kits/available-for/${this.user?.id}/ids`)
            .then(res => {
                this.availableKitsIds = res.data    
            })
			this.isInitialized = true
		},

		logOut() {
			localStorage.removeItem('userId')
			localStorage.removeItem('token')
			this.user = null
			this.token = null
		},
	},
})
