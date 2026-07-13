import { Paginacion } from "@/components/paginacion";
import { estadoClases, type ListadoClientesProps } from "@/types/const";



export function ListadoClientes({
    items, total, pagina, totalPaginas, busqueda, cargando,
    onBusqueda, onPagina, onEditar, onNuevo, seleccionado, onSeleccionar,
}: ListadoClientesProps) {
    return (
        <section>
            <div className="flex gap-2 mb-4">
                <input
                    type="search"
                    placeholder="Buscar por nombre, RFC o email..."
                    value={busqueda}
                    onChange={(e) => onBusqueda(e.target.value)}
                    className="flex-1 rounded-md border border-slate-300 px-3 py-2 text-base focus:outline-none focus:ring-2 focus:ring-slate-400"
                />
                <button
                    onClick={onNuevo}
                    className="rounded-md bg-slate-800 text-white px-4 py-2 font-medium hover:bg-slate-700"
                >
                    + Nuevo cliente
                </button>
            </div>

            {cargando && items.length === 0 ? (
                <p className="text-center text-slate-500 py-10">Cargando clientes...</p>
            ) : items.length === 0 ? (
                <p className="text-center text-slate-500 py-10">
                    {busqueda ? `Sin resultados para "${busqueda}".` : "No hay clientes registrados."}
                </p>
            ) : (
                <>
                    <div className="overflow-hidden rounded-xl bg-white shadow-sm">
                        <table className="w-full text-sm">
                            <thead>
                                <tr className="border-b border-slate-200 text-left text-slate-500">
                                    <th className="px-4 py-3 font-medium">Nombre</th>
                                    <th className="px-4 py-3 font-medium">RFC</th>
                                    <th className="px-4 py-3 font-medium">Email</th>
                                    <th className="px-4 py-3 font-medium">Estado</th>
                                    <th className="px-4 py-3" />
                                </tr>
                            </thead>
                            <tbody>
                                {items.map((c) => (
                                    <tr
                                        key={c.id}
                                        onClick={() => onSeleccionar(c)}
                                        className={`cursor-pointer border-b border-slate-100 last:border-0 hover:bg-slate-50 ${seleccionado?.id === c.id ? "bg-slate-100" : ""
                                            }`}
                                    >
                                        <td className="px-4 py-3 text-slate-800">{c.nombre}</td>
                                        <td className="px-4 py-3 font-mono text-slate-600">{c.rfc}</td>
                                        <td className="px-4 py-3 text-slate-600">{c.email}</td>
                                        <td className="px-4 py-3">
                                            <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${estadoClases[c.estado]}`}>
                                                {c.estado}
                                            </span>
                                        </td>
                                        <td className="px-4 py-3 text-right">
                                            <button
                                                onClick={(e) => { e.stopPropagation(); onEditar(c); }}
                                                className="text-slate-500 hover:text-slate-800 font-medium"
                                            >
                                                Editar
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                    <footer className="flex items-center justify-between mt-4 text-sm text-slate-500">
                        <span>{total} cliente{total === 1 ? "" : "s"}</span>
                        <Paginacion pagina={pagina} totalPaginas={totalPaginas} onCambio={onPagina} />
                    </footer>
                </>
            )}
        </section>
    );
}