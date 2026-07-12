import type { ErrorApi } from "@/types";

const BASE_URL = import.meta.env.VITE_API_URL;

export class ApiError extends Error {
    readonly status: number;
    readonly codigo: string;

    constructor(status: number, codigo: string, mensaje: string) {
        super(mensaje);
        this.status = status;
        this.codigo = codigo;
    }
}

let token: string | null = localStorage.getItem("token");

export function guardarToken(nuevoToken: string) {
    token = nuevoToken;
    localStorage.setItem("token", nuevoToken);
}

export function cerrarCesion() {
    token = null;
    localStorage.removeItem("token");
}

export function haySession(): boolean {
    return token !== null;
}

export async function request<T>(ruta: string, init?: RequestInit): Promise<T> {
    const respuesta = await fetch(`${BASE_URL}${ruta}`, {
        ...init,
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorizacion: `Bearer ${token}` } : {}),
            ...init?.headers,
        },
    });

    if (respuesta.status === 401) {
        cerrarCesion();
        throw new ApiError(401, "no_autorizado", "Tu sesión expiró. Inicia sesión de nuevo.");
    }

    if (!respuesta.ok) {
        const error = (await respuesta.json().catch(() => null)) as ErrorApi | null;
        throw new ApiError(
            respuesta.status,
            error?.codigo ?? "error_desconocido",
            error?.mensaje ?? "Ocurrió un error inesperado."
        );
    }
    return respuesta.json() as Promise<T>;
}