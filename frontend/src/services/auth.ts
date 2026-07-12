import { guardarToken, request } from "./api";

export async function login(usuario: string, password: string): Promise<void> {
    const { token } = await request<{ token: string }>("/auth/login", {
        method: "POST",
        body: JSON.stringify({ usuario, password }),
    });
    guardarToken(token);
}