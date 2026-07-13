import { button } from "@/types/const";

interface PaginacionProps {
    pagina: number;
    totalPaginas: number;
    onCambio: (pagina: number) => void;
}

export function Paginacion({ pagina, totalPaginas, onCambio }: PaginacionProps) {
    return (
        <div className="flex items-center gap-3 text-sm text-slate-600">
            <button disabled={pagina <= 1} onClick={() => onCambio(pagina - 1)} className={button.className}>
                ← Anterior
            </button>
            <span>Página {pagina} de {totalPaginas}</span>
            <button disabled={pagina >= totalPaginas} onClick={() => onCambio(pagina + 1)} className={button.className}>
                Siguiente →
            </button>
        </div>
    );
}