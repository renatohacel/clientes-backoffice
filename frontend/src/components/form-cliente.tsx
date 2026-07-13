import { useState } from "react";
import { ApiError } from "@/services/api";
import { actualizarCliente, crearCliente } from "@/services/clientes";
import type { ClienteDto } from "@/types";
import {
    validarCliente,
    hayErrores,
    type ErroresFormulario,
    type DataFormulario,
} from "@/utils/validacion";
import { input } from "@/types/const";

interface FormularioClienteProps {
    clienteInicial: ClienteDto | null;
    onGuardado: (cliente: ClienteDto) => void;
    onCancelar: () => void;
    onConflicto: () => void;
}

export function FormularioCliente({ clienteInicial, onGuardado, onCancelar, onConflicto }: FormularioClienteProps) {
    const esEdicion = clienteInicial !== null;

    const [valores, setValores] = useState<DataFormulario>({
        nombre: clienteInicial?.nombre ?? "",
        rfc: clienteInicial?.rfc ?? "",
        email: clienteInicial?.email ?? "",
        telefono: clienteInicial?.telefono ?? "",
    });
    const [errores, setErrores] = useState<ErroresFormulario>({});
    const [errorApi, setErrorApi] = useState<string | null>(null);
    const [guardando, setGuardando] = useState(false);

    function cambiar(campo: keyof DataFormulario, valor: string) {
        setValores((v) => ({ ...v, [campo]: valor }));
        setErrores((e) => ({ ...e, [campo]: undefined }));
    }

    async function handleSubmit(e: React.SubmitEvent<HTMLFormElement>) {
        e.preventDefault();
        const erroresValidacion = validarCliente(valores);
        setErrores(erroresValidacion);
        if (hayErrores(erroresValidacion)) return;

        setGuardando(true);
        setErrorApi(null);

        const payload = {
            nombre: valores.nombre.trim(),
            rfc: valores.rfc.trim(),
            email: valores.email.trim(),
            telefono: valores.telefono.trim() || null,
        };

        try {
            const guardado = esEdicion
                ? await actualizarCliente(clienteInicial.id, { ...payload, version: clienteInicial.version })
                : await crearCliente(payload);
            onGuardado(guardado);
        } catch (err) {
            if (err instanceof ApiError) {
                if (err.status === 409) {
                    onConflicto();
                    return;
                }
                if (err.codigo === "rfc_duplicado") {
                    setErrores((e) => ({ ...e, rfc: err.message }));
                } else if (err.codigo === "email_duplicado") {
                    setErrores((e) => ({ ...e, email: err.message }));
                } else {
                    setErrorApi(err.message);
                }
            } else {
                setErrorApi("No se pudo conectar con el servidor.");
            }
        } finally {
            setGuardando(false);
        }
    }

    return (
        <form
            onSubmit={handleSubmit}
            noValidate
            className="max-w-lg bg-white rounded-xl shadow-sm p-6 flex flex-col gap-4"
        >
            <h2 className="text-lg font-semibold text-slate-800">
                {esEdicion ? `Editar: ${clienteInicial.nombre}` : "Nuevo cliente"}
            </h2>

            <label className="flex flex-col gap-1 text-sm text-slate-600">
                Nombre *
                <input
                    value={valores.nombre}
                    onChange={(e) => cambiar("nombre", e.target.value)}
                    autoFocus
                    className={input.className}
                />
                {errores.nombre && <span className="text-sm text-red-600">{errores.nombre}</span>}
            </label>

            <label className="flex flex-col gap-1 text-sm text-slate-600">
                RFC *
                <input
                    value={valores.rfc}
                    onChange={(e) => cambiar("rfc", e.target.value.toUpperCase())}
                    maxLength={13}
                    className={`${input.className} font-mono`}
                />
                {errores.rfc && <span className="text-sm text-red-600">{errores.rfc}</span>}
            </label>

            <label className="flex flex-col gap-1 text-sm text-slate-600">
                Email *
                <input
                    type="email"
                    value={valores.email}
                    onChange={(e) => cambiar("email", e.target.value)}
                    className={input.className}
                />
                {errores.email && <span className="text-sm text-red-600">{errores.email}</span>}
            </label>

            <label className="flex flex-col gap-1 text-sm text-slate-600">
                Teléfono
                <input
                    value={valores.telefono}
                    onChange={(e) => cambiar("telefono", e.target.value)}
                    className={input.className}
                />
                {errores.telefono && <span className="text-sm text-red-600">{errores.telefono}</span>}
            </label>

            {errorApi && (
                <p role="alert" className="text-sm text-red-600">{errorApi}</p>
            )}

            <div className="flex gap-2">
                <button
                    type="submit"
                    disabled={guardando}
                    className="rounded-md bg-slate-800 text-white px-4 py-2 font-medium hover:bg-slate-700 disabled:opacity-50"
                >
                    {guardando ? "Guardando..." : esEdicion ? "Guardar cambios" : "Crear cliente"}
                </button>
                <button
                    type="button"
                    onClick={onCancelar}
                    disabled={guardando}
                    className="rounded-md border border-slate-300 bg-white px-4 py-2 text-slate-700 hover:bg-slate-50 disabled:opacity-50"
                >
                    Cancelar
                </button>
            </div>
        </form>
    );
}