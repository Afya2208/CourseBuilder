export interface Group {
	id: number
	name: string
	curatorFeedback: string
	curatorName?: string
	curatorId: number
	dateStart: string,
	maxMembersCount?: number,
	dateEnd: string
	coursesInfo: {
		id: number
		name?: string
	}[]
	usersInfo?: {
		userId: number
		firstName: string
		lastName: string
		middleName?: string
	}[]
}
