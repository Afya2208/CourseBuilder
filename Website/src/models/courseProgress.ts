export interface CourseProgress {
	progressPercent : number,
	solvedCount: number,
	totalCount: number,
	isComplete: boolean,
	status: "NoRequiredTasks" | "Completed" | "InProgress"
}
