import { useCallback, useEffect, useRef, useState } from "react";
import { listarClientes } from "@/services/clientes";
import { ApiError } from "@/services/api";
import type { ClienteDto } from "@/types";

const TAMANO_PAGINA = 10;

interface Resultado {
    clave: string; // a qué búsqueda+página corresponden estos datos
    items: ClienteDto[];
    total: number;
}

interface ErrorCarga {
    clave: string;
    mensaje: string;
}

export function useClientes() {
    const [pagina, setPagina] = useState(1);
    const [busqueda, setBusqueda] = useState("");
    const [busquedaAplicada, setBusquedaAplicada] = useState("");
    const [resultado, setResultado] = useState<Resultado | null>(null);
    const [errorCarga, setErrorCarga] = useState<ErrorCarga | null>(null);

    useEffect(() => {
        const timer = setTimeout(() => {
            setBusquedaAplicada(busqueda);
            setPagina(1);
        }, 300);
        return () => clearTimeout(timer);
    }, [busqueda]);

    const clave = `${busquedaAplicada}|${pagina}`;

    const versionPeticion = useRef(0);

    const cargar = useCallback(async () => {
        const miVersion = ++versionPeticion.current;
        try {
            const r = await listarClientes(busquedaAplicada, pagina, TAMANO_PAGINA);
            if (miVersion !== versionPeticion.current) return;
            setResultado({ clave, items: r.items, total: r.total });
            setErrorCarga(null);
        } catch (err) {
            if (miVersion !== versionPeticion.current) return;
            setErrorCarga({
                clave,
                mensaje: err instanceof ApiError ? err.message : "No se pudo conectar con el servidor.",
            });
        }
    }, [busquedaAplicada, pagina, clave]);

    useEffect(() => {
        // eslint-disable-next-line react-hooks/set-state-in-effect
        void cargar();
    }, [cargar]);

    const items = resultado?.items ?? [];
    const total = resultado?.total ?? 0;
    const cargando = resultado?.clave !== clave && errorCarga?.clave !== clave;
    const error = errorCarga?.clave === clave ? errorCarga.mensaje : null;
    const totalPaginas = Math.max(1, Math.ceil(total / TAMANO_PAGINA));

    return {
        items, total, pagina, totalPaginas, busqueda, cargando, error,
        setBusqueda, setPagina,
        recargar: cargar,
    };
}