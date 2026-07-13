import { ApiError } from "@/services/api";
import { login } from "@/services/auth";
import { input } from "@/types/const";
import { useState } from "react";

interface LoginProps {
    onLogin: () => void;
}

export const Login = ({ onLogin }: LoginProps) => {

    const [usuario, setUsuario] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>("");
    const [cargando, setCargando] = useState(false);

    async function handleSubmit(e: React.SubmitEvent<HTMLFormElement>) {
        e.preventDefault();
        setError(null);
        setCargando(true);
        try {
            await login(usuario, password);
            onLogin();
        } catch (err) {
            setError(
                err instanceof ApiError && err.status === 401
                    ? "Usuario o contraseña incorrectos."
                    : "No se pudo conectar al servidor.",
            );
        } finally {
            setCargando(false);
        }
    }


    return (
        <div className="min-h-screen flex items-center justify-center bg-slate-100">
            <form
                onSubmit={handleSubmit}
                className="w-full max-w-sm bg-white rounded-xl shadow-sm p-8 flex flex-col gap-4"
            >
                <h1 className="text-xl font-semibold text-slate-800">Mantenimiento de Clientes</h1>

                <label className="flex flex-col gap-1 text-sm text-slate-600">
                    Usuario
                    <input
                        value={usuario}
                        onChange={(e) => setUsuario(e.target.value)}
                        autoFocus
                        className={input.className}
                    />
                </label>

                <label className="flex flex-col gap-1 text-sm text-slate-600">
                    Contraseña
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        className={input.className}
                    />
                </label>

                {error && (
                    <p role="alert" className="text-sm text-red-600">{error}</p>
                )}

                <button
                    type="submit"
                    disabled={cargando || !usuario || !password}
                    className="rounded-md bg-slate-800 text-white py-2 font-medium hover:bg-slate-700 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                    {cargando ? "Entrando..." : "Entrar"}
                </button>
            </form>
        </div>
    )
}