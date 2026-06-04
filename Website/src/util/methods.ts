import type { Correlation } from '@/models/main'
import type { AxiosError } from 'axios'

export function randomInt(minInclude: number, maxInclude: number): number {
	return Math.floor(Math.random() * (maxInclude - minInclude) + minInclude)
}

export function indexToChar(index: number): string {
	const code = 'a'.charCodeAt(0)
	return String.fromCharCode(code + index)
}

export function getColorForCard(index: number) : object {
    const cssStyles = {}
    cssStyles[`card-color-${index%10}`] = true;
    return cssStyles;
}

// forms: [ед.ч. (1), род.п. ед.ч. (2-4), род.п. мн.ч. (5-0)]
export function pluralizeRu(count:number, forms:Array<string>) {
    const n = Math.abs(count) % 100;
    const n1 = n % 10;

    if (n >= 11 && n <= 19) return forms[2];
    if (n1 === 1) return forms[0];
    if (n1 >= 2 && n1 <= 4) return forms[1];
    return forms[2];
}