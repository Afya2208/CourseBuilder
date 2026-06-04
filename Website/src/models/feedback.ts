export interface FeedbackSubmit {
	id?: number
	userId?: number
	email?: string
	title: string
	text: string
	dateTimeSent?: Date
	dateTimeSolved?: Date
	feedbackCategoryId: number
	feedbackCategory?: FeedbackCategory
}
export interface FeedbackCategory {
	id: number
	name: string
}

export interface FeedbackSubmitsAndTotalCount {
	feedbackSubmits: FeedbackSubmit[],
	totalCount: number
}
