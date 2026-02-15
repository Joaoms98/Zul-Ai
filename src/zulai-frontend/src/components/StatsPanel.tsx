import type { TickResultDto, AnalyticsDto } from '../services/api';

interface Props {
  tick: TickResultDto | null;
  analytics: AnalyticsDto | null;
}

const TYPE_SYMBOLS: Record<string, { symbol: string; color: string }> = {
  Luminar: { symbol: '*', color: '#ffff00' },
  Umbral:  { symbol: '#', color: '#8b00ff' },
  Nexus:   { symbol: '@', color: '#00e5ff' },
  Pulsar:  { symbol: '~', color: '#ff6600' },
  Void:    { symbol: '.', color: '#555555' },
};

export function StatsPanel({ tick, analytics }: Props) {
  return (
    <div style={{
      background: '#000',
      border: '1px solid #222',
      padding: '12px',
      fontSize: '12px',
    }}>
      <div style={{ color: '#555', marginBottom: 8 }}>{'>'} UNIVERSE STATUS</div>

      {tick && (
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '4px 16px' }}>
          <Stat label="TICK" value={tick.tick} />
          <Stat label="ALIVE" value={tick.atomsAlive} color="#00ff41" />
          <Stat label="BORN" value={`+${tick.atomsBorn}`} color="#00ff41" />
          <Stat label="DIED" value={`-${tick.atomsDied}`} color="#ff3333" />
          <Stat label="LINKED" value={`+${tick.connectionsFormed}`} color="#00e5ff" />
          <Stat label="BROKEN" value={`-${tick.connectionsBroken}`} color="#ff6600" />
          <Stat label="ENERGY" value={tick.totalEnergy.toFixed(0)} color="#ffff00" />
        </div>
      )}

      {analytics && (
        <>
          <div style={{ borderTop: '1px solid #222', margin: '10px 0' }} />
          <div style={{ color: '#555', marginBottom: 8 }}>{'>'} ANALYTICS</div>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '4px 16px' }}>
            <Stat label="TOTAL BORN" value={analytics.totalAtomsBorn} />
            <Stat label="TOTAL DIED" value={analytics.totalAtomsDied} />
            <Stat label="AVG ENERGY" value={analytics.averageEnergy.toFixed(1)} color="#ffff00" />
            <Stat label="CONNECTIONS" value={analytics.activeConnections} color="#00e5ff" />
          </div>

          <div style={{ borderTop: '1px solid #222', margin: '10px 0' }} />
          <div style={{ color: '#555', marginBottom: 8 }}>{'>'} POPULATION</div>
          {Object.entries(analytics.atomsByType).map(([type, count]) => {
            const info = TYPE_SYMBOLS[type] || { symbol: '?', color: '#555' };
            return (
              <div key={type} style={{ display: 'flex', alignItems: 'center', gap: 8, padding: '2px 0' }}>
                <span style={{ color: info.color, fontWeight: 'bold' }}>{info.symbol}</span>
                <span style={{ color: '#888', width: 60 }}>{type}</span>
                <span style={{ color: info.color }}>{count}</span>
                <span style={{ color: info.color, opacity: 0.4 }}>
                  {'\u2588'.repeat(Math.min(count, 20))}
                </span>
              </div>
            );
          })}
        </>
      )}

      {!tick && !analytics && (
        <div style={{ color: '#333' }}>No data yet. Create a universe.</div>
      )}
    </div>
  );
}

function Stat({ label, value, color = '#00aa2a' }: { label: string; value: string | number; color?: string }) {
  return (
    <>
      <span style={{ color: '#555' }}>{label}</span>
      <span style={{ color, textAlign: 'right' }}>{value}</span>
    </>
  );
}
