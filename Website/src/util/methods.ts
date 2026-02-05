import type { Correlation } from '@/models/main'
import type { AxiosError } from 'axios'

export function randomInt(minInclude: number, maxInclude: number): number {
	return Math.floor(Math.random() * (maxInclude - minInclude) + minInclude)
}

export function indexToChar(index: number): string {
	let code = 'a'.charCodeAt(0)
	return String.fromCharCode(code + index)
}
