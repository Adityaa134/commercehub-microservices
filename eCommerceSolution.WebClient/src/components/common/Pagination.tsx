export interface PaginationProps {
  page: number;
  totalPages: number;
  totalItems: number;
  pageSize: number;
  onChange: (page: number) => void;
}

function getPageItems(current: number, total: number): (number | string)[] {
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
  const items: (number | string)[] = [1];
  const start = Math.max(2, current - 1);
  const end = Math.min(total - 1, current + 1);
  if (start > 2) items.push('…');
  for (let i = start; i <= end; i++) items.push(i);
  if (end < total - 1) items.push('…');
  items.push(total);
  return items;
}

export function Pagination({
  page,
  totalPages,
  totalItems,
  pageSize,
  onChange,
}: PaginationProps) {
  if (totalPages <= 1) return null;
  const shown = Math.min(page * pageSize, totalItems);
  const from = totalItems > 0 ? (page - 1) * pageSize + 1 : 0;

  return (
    <nav
      aria-label="Pagination"
      className="mt-10 flex flex-col items-center gap-3.5 select-none"
    >
      <div className="flex items-center gap-1 p-[5px] bg-white border border-[#E9E6DD] rounded-full shadow-[0_6px_20px_rgba(20,22,26,0.06)]">
        <button
          type="button"
          onClick={() => onChange(page - 1)}
          disabled={page === 1}
          aria-label="Previous page"
          className={`w-[38px] h-[38px] rounded-full border-none flex-shrink-0 inline-flex items-center justify-center transition-colors ${
            page === 1
              ? 'text-[#C9C7BE] cursor-not-allowed bg-transparent'
              : 'text-[#16181C] hover:bg-[#EAEEFF] hover:text-[#3355FF] cursor-pointer bg-transparent'
          }`}
        >
          <svg className="w-4.5 h-4.5" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 19.5L8.25 12l7.5-7.5" />
          </svg>
        </button>

        <div className="hidden sm:flex items-center gap-0.5">
          {getPageItems(page, totalPages).map((it, i) =>
            it === '…' ? (
              <span
                key={`e${i}`}
                className="w-[30px] text-center text-[#B4B2A9] text-sm font-medium"
              >
                …
              </span>
            ) : (
              <button
                key={it}
                type="button"
                onClick={() => onChange(Number(it))}
                aria-current={it === page ? 'page' : undefined}
                className={`w-[38px] h-[38px] rounded-full border-none flex-shrink-0 inline-flex items-center justify-center text-[13.5px] font-semibold transition-all cursor-pointer ${
                  it === page
                    ? 'bg-[#3355FF] text-white shadow-[0_4px_12px_rgba(51,85,255,0.35)]'
                    : 'bg-transparent text-[#16181C] hover:bg-[#EAEEFF] hover:text-[#3355FF]'
                }`}
              >
                {it}
              </button>
            )
          )}
        </div>

        <span className="sm:hidden px-3.5 text-[13.5px] font-semibold text-[#16181C] whitespace-nowrap">
          Page {page} <span className="text-[#83887B] font-medium">of {totalPages}</span>
        </span>

        <button
          type="button"
          onClick={() => onChange(page + 1)}
          disabled={page === totalPages}
          aria-label="Next page"
          className={`w-[38px] h-[38px] rounded-full border-none flex-shrink-0 inline-flex items-center justify-center transition-colors ${
            page === totalPages
              ? 'text-[#C9C7BE] cursor-not-allowed bg-transparent'
              : 'text-[#16181C] hover:bg-[#EAEEFF] hover:text-[#3355FF] cursor-pointer bg-transparent'
          }`}
        >
          <svg className="w-4.5 h-4.5" fill="none" stroke="currentColor" strokeWidth="2.2" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" d="M8.25 4.5l7.5 7.5-7.5 7.5" />
          </svg>
        </button>
      </div>

      <div className="w-[200px] max-w-[60vw] h-[3px] rounded-[2px] bg-[#E9E6DD] overflow-hidden">
        <div
          className="h-full bg-[#3355FF] transition-all duration-250 ease-out"
          style={{ width: `${totalItems > 0 ? (shown / totalItems) * 100 : 0}%` }}
        />
      </div>

      <p className="m-0 text-[12.5px] text-[#83887B]">
        Showing {from}–{shown} of {totalItems} products
      </p>
    </nav>
  );
}

export default Pagination;

