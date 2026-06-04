export class Validator {
    errorList : string[] = [];
    validate(condition:boolean, errorMessage:string) {
        if (condition) this.errorList.push(errorMessage)
    }
    returnResult(): boolean {
        let result = true;
        if (this.errorList.length > 0) {
            alert(this.errorList.join("\n"))
            result = false;
        }
        return result
    }
}