import type { SignInResponse } from "@/models/main";
import api from "@/services/api";
import axios from "axios";
import { useRouter } from "vue-router";

export async function getUserData () : Promise<SignInResponse> {
    try {
        let user:SignInResponse
        let userId = Number.parseInt(sessionStorage.getItem("userId"));
        let router = useRouter()
        let token = sessionStorage.getItem("token")
        if(userId == undefined || token == undefined) {
            return undefined
        }
        await api.get<SignInResponse>("users/" + userId,
            {headers: {
                Authorization:"Bearer " + token
            }}
        ).then(res => {
            user = res.data
        })
            .catch(err => {
                if (err.status == 401) {
                    alert("Время сессии закончено, авторизируйтесь повторно, пожалуйста")
                    router.push({path:"/auth"});
                    return undefined
                }
            }) 
        return user
    } catch (err) {
        return undefined
    }
}