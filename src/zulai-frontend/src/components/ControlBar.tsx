interface Props {
  hasUniverse: boolean;
  loading: boolean;
  autoPlaying: boolean;
  onCreateUniverse: () => void;
  onTick: () => void;
  onTick10: () => void;
  onAutoPlay: () => void;
  onStopAutoPlay: () => void;
  onRefreshAnalytics: () => void;
}

export function ControlBar({
  hasUniverse, loading, autoPlaying,
  onCreateUniverse, onTick, onTick10,
  onAutoPlay, onStopAutoPlay, onRefreshAnalytics,
}: Props) {
  return (
    <div style={{
      borderBottom: '1px solid #222',
      padding: '8px 20px',
      display: 'flex',
      alignItems: 'center',
      gap: 8,
      background: '#0d0d0d',
      flexWrap: 'wrap',
    }}>
      <span style={{ color: '#555', marginRight: 8, fontSize: '11px' }}>{'>'}</span>

      <button onClick={onCreateUniverse} disabled={loading}>
        {hasUniverse ? 'NEW BANG' : 'BIG BANG'}
      </button>

      {hasUniverse && (
        <>
          <div style={{ width: 1, height: 20, background: '#222', margin: '0 4px' }} />

          <button onClick={onTick} disabled={loading || autoPlaying}>
            TICK
          </button>

          <button onClick={onTick10} disabled={loading || autoPlaying}>
            TICK x10
          </button>

          <div style={{ width: 1, height: 20, background: '#222', margin: '0 4px' }} />

          {!autoPlaying ? (
            <button className="cyan" onClick={onAutoPlay} disabled={loading}>
              AUTO PLAY
            </button>
          ) : (
            <button className="danger" onClick={onStopAutoPlay}>
              STOP
            </button>
          )}

          <div style={{ width: 1, height: 20, background: '#222', margin: '0 4px' }} />

          <button onClick={onRefreshAnalytics} disabled={loading || autoPlaying} style={{ fontSize: '11px' }}>
            ANALYTICS
          </button>
        </>
      )}

      {loading && !autoPlaying && (
        <span style={{ color: '#00ff41', marginLeft: 'auto', fontSize: '11px', animation: 'blink 1s infinite' }}>
          PROCESSING...
        </span>
      )}

      {autoPlaying && (
        <span style={{ color: '#00e5ff', marginLeft: 'auto', fontSize: '11px' }}>
          SIMULATING...
        </span>
      )}

      <style>{`
        @keyframes blink {
          0%, 100% { opacity: 1; }
          50% { opacity: 0.3; }
        }
      `}</style>
    </div>
  );
}
