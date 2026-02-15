import { useState, useEffect } from 'react';
import { api } from '../services/api';
import type { HistoryResponse } from '../services/api';

interface Props {
  universeId: string;
  currentTick: number;
}

export function HistoryPanel({ universeId, currentTick }: Props) {
  const [history, setHistory] = useState<HistoryResponse | null>(null);
  const [page, setPage] = useState(0);
  const pageSize = 30;

  useEffect(() => {
    api.getHistory(universeId, page * pageSize, pageSize)
      .then(setHistory)
      .catch(() => {});
  }, [universeId, currentTick, page]);

  if (!history) return null;

  return (
    <div style={{
      background: '#000',
      border: '1px solid #222',
      padding: '12px',
      fontSize: '11px',
      overflowY: 'auto',
      maxHeight: 400,
    }}>
      <div style={{ color: '#555', marginBottom: 8 }}>
        {'>'} INTERACTION LOG ({history.total} total)
      </div>

      {history.items.map((item) => (
        <div key={item.id} style={{ padding: '2px 0', display: 'flex', gap: 8 }}>
          <span style={{ color: '#333', minWidth: 40 }}>T{item.tick}</span>
          <span style={{
            color: item.type === 'Born' ? '#00ff41'
              : item.type === 'Died' ? '#ff3333'
              : item.type === 'Connected' ? '#00e5ff'
              : item.type === 'Disconnected' ? '#ff6600'
              : '#888',
            minWidth: 90,
          }}>
            {item.type.toUpperCase()}
          </span>
          <span style={{ color: '#666' }}>{item.description}</span>
        </div>
      ))}

      <div style={{ display: 'flex', gap: 8, marginTop: 8 }}>
        <button
          onClick={() => setPage(p => Math.max(0, p - 1))}
          disabled={page === 0}
          style={{ fontSize: '10px', padding: '3px 8px' }}
        >
          PREV
        </button>
        <span style={{ color: '#555', alignSelf: 'center' }}>
          PAGE {page + 1}
        </span>
        <button
          onClick={() => setPage(p => p + 1)}
          disabled={(page + 1) * pageSize >= history.total}
          style={{ fontSize: '10px', padding: '3px 8px' }}
        >
          NEXT
        </button>
      </div>
    </div>
  );
}
