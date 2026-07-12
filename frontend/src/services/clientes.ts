import { request } from "./api";

import type {
    ActualizarClientePayload,
    ClienteDto,
    CrearClientePayload,
    EstadoCliente,
    ResultadoPaginado,
} from "@/types";

export function listarClientes(
    busqueda: string,
    pagina: number,
    tamanoPagina: number,
): Promise<ResultadoPaginado<ClienteDto>> {
    const params = new URLSearchParams({
        pagina: String(pagina),
        tamanoPagina: String(tamanoPagina),
    });
    if (busqueda.trim()) params.set("busqueda", busqueda.trim());

    return request(`/clientes?${params}`);
}

export function obtenerCliente(id: string): Promise<ClienteDto> {
    return request(`/clientes/${id}`);
}

export function crearCliente(payload: CrearClientePayload): Promise<ClienteDto> {
    return request("/clientes", { method: "POST", body: JSON.stringify(payload) });
}

export function actualizarCliente(
    id: string,
    payload: ActualizarClientePayload,
): Promise<ClienteDto> {
    return request(`/clientes/${id}`, { method: "PUT", body: JSON.stringify(payload) })
}

export function cambiarEstado(
    id: string,
    nuevoEstado: EstadoCliente,
    version: number,
): Promise<ClienteDto> {
    return request(`/clientes/${id}/estado`, {
        method: "POST",
        body: JSON.stringify({ nuevoEstado, version }),
    })
}