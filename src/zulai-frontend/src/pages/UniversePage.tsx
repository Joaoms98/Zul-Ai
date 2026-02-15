import { useState, useCallback } from 'react';
import { useUniverse } from '../hooks/useUniverse';
import { Header } from '../components/Header';
import { ControlBar } from '../components/ControlBar';
import { AsciiViewer } from '../components/AsciiViewer';
import { EventLog } from '../components/EventLog';
import { StatsPanel } from '../components/StatsPanel';
import { HistoryPanel } from '../components/HistoryPanel';

type Tab = 'events' | 'history';

export function UniversePage() {
  const {
    universe, lastTick, analytics, loading, error, autoPlaying,
    create, tick, tickMultiple, fetchAnalytics, startAutoPlay, stopAutoPlay,
  } = useUniverse();

  const [allEvents, setAllEvents] = useState<string[]>([]);
  const [activeTab, setActiveTab] = useState<Tab>('events');

  const handleCreate = useCallback(async () => {
    stopAutoPlay();
    setAllEvents([]);
    const state = await create();
    if (state) {
      setAllEvents([`Universe created: ${state.atomsAlive} atoms born`]);
    }
  }, [create, stopAutoPlay]);

  const handleTick = useCallback(async () => {
    const result = await tick();
    if (result) {
      setAllEvents(prev => [...prev, ...result.eventLog]);
    }
  }, [tick]);

  const handleTick10 = useCallback(async () => {
    const result = await tickMultiple(10);
    if (result) {
      setAllEvents(prev => [...prev, `[Skipped to tick ${result.tick}]`, ...result.eventLog]);
    }
  }, [tickMultiple]);

  // For auto play, we need a different approach since the hook handles it
  const handleAutoPlay = useCallback(() => {
    const originalSetLastTick = lastTick;
    startAutoPlay(600);
    // We'll track events through the lastTick changes
    void originalSetLastTick;
  }, [startAutoPlay, lastTick]);

  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100vh' }}>
      <Header />
      <ControlBar
        hasUniverse={!!universe}
        loading={loading}
        autoPlaying={autoPlaying}
        onCreateUniverse={handleCreate}
        onTick={handleTick}
        onTick10={handleTick10}
        onAutoPlay={handleAutoPlay}
        onStopAutoPlay={stopAutoPlay}
        onRefreshAnalytics={fetchAnalytics}
      />

      {error && (
        <div style={{
          background: '#1a0000',
          border: '1px solid #ff3333',
          color: '#ff3333',
          padding: '8px 20px',
          fontSize: '12px',
        }}>
          ERROR: {error}
        </div>
      )}

      {!universe ? (
        <WelcomeScreen />
      ) : (
        <div style={{
          flex: 1,
          display: 'grid',
          gridTemplateColumns: '1fr 320px',
          gridTemplateRows: '1fr auto',
          gap: 0,
          overflow: 'hidden',
        }}>
          {/* Main ASCII viewer */}
          <div style={{ overflow: 'auto', padding: 12 }}>
            <AsciiViewer
              ascii={lastTick?.asciiArt || 'Waiting for first tick...'}
              tick={lastTick?.tick ?? universe.currentTick}
            />
          </div>

          {/* Right panel */}
          <div style={{
            borderLeft: '1px solid #222',
            overflow: 'auto',
            padding: 12,
            display: 'flex',
            flexDirection: 'column',
            gap: 12,
          }}>
            <StatsPanel tick={lastTick} analytics={analytics} />

            {/* Tab switcher */}
            <div style={{ display: 'flex', gap: 0 }}>
              <TabButton active={activeTab === 'events'} onClick={() => setActiveTab('events')}>
                EVENTS
              </TabButton>
              <TabButton active={activeTab === 'history'} onClick={() => setActiveTab('history')}>
                HISTORY
              </TabButton>
            </div>

            {activeTab === 'events' ? (
              <EventLog events={allEvents} maxHeight={250} />
            ) : (
              <HistoryPanel universeId={universe.id} currentTick={universe.currentTick} />
            )}
          </div>
        </div>
      )}
    </div>
  );
}

function TabButton({ active, onClick, children }: { active: boolean; onClick: () => void; children: React.ReactNode }) {
  return (
    <button
      onClick={onClick}
      style={{
        flex: 1,
        border: '1px solid #222',
        borderBottom: active ? '1px solid #000' : '1px solid #222',
        background: active ? '#000' : '#0a0a0a',
        color: active ? '#00ff41' : '#555',
        padding: '6px 12px',
        fontSize: '10px',
        letterSpacing: 2,
      }}
    >
      {children}
    </button>
  );
}

function WelcomeScreen() {
  return (
    <div style={{
      flex: 1,
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 20,
    }}>
      <pre style={{
        color: '#00ff41',
        fontSize: '10px',
        lineHeight: '12px',
        textAlign: 'center',
        textShadow: '0 0 20px rgba(0,255,65,0.2)',
        opacity: 0.8,
      }}>{`
         .     *        .       .
    .       .       *        .       .
        .       .       .       .
   *       .   ____   .       *
      .       /    \\       .       .
    .    .   | VOID |   .       *
         .   \\____/   .    .
   .       .       .       .       .
      *       .       .       *
    .       .    *        .       .
      `}</pre>
      <div style={{ color: '#555', textAlign: 'center', maxWidth: 400 }}>
        <p style={{ marginBottom: 8 }}>O vazio espera.</p>
        <p style={{ marginBottom: 8, color: '#333' }}>
          Pressione <span style={{ color: '#00ff41' }}>BIG BANG</span> para iniciar o universo.
        </p>
        <p style={{ color: '#222', fontSize: '10px' }}>
          Atomos nascerão, se conectarão, viverão e morrerão.
        </p>
      </div>
    </div>
  );
}
