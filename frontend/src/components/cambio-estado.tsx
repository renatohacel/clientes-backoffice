import { useState } from "react";
import { ApiError } from "@/services/api";
import { cambiarEstado } from "@/services/clientes";
import type { ClienteDto, EstadoCliente } from "@/types";
import { estadoClasesButton } from "@/types/const";

interface CambioEstadoProps {
    cliente: ClienteDto;
    onCambiado: (actualizado: ClienteDto) => void;
    onConflicto: () => void;
}

export function CambioEstado({ cliente, onCambiado, onConflicto }: CambioEstadoProps) {
    const [enviando, setEnviando] = useState<EstadoCliente | null>(null);
    const [error, setError] = useState<string | null>(null);

    async function handleCambio(nuevoEstado: EstadoCliente) {
        setEnviando(nuevoEstado);
        setError(null);
        try {
            const actualizado = await cambiarEstado(cliente.id, nuevoEstado, cliente.version);
            onCambiado(actualizado);
        } catch (err) {
            if (err instanceof ApiError && err.status === 409) {
                onConflicto();
            } else {
                setError(err instanceof ApiError ? err.message : "Error de conexión.");
            }
        } finally {
            setEnviando(null);
        }
    }

    return (
        <div className="flex items-center gap-2">
            <strong className="text-slate-700">Cambiar a:</strong>
            {cliente.transicionesPermitidas.map((destino) => (
                <button
                    key={destino}
                    disabled={enviando !== null}
                    onClick={() => handleCambio(destino)}
                    className={`rounded-md border border-slate-300 bg-white px-3 py-1.5 text-sm font-medium text-slate-700 disabled:opacity-50 transition-all duration-300 cursor-pointer ${estadoClasesButton[destino]}`}
                >

                    {enviando === destino ? "Cambiando..." : `${destino}`}
                </button>
            ))}
            {error && <span className="text-sm text-red-600">{error}</span>}
        </div>
    );
}