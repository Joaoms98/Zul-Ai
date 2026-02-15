import { useRef, useEffect } from 'react';

interface Props {
  events: string[];
  maxHeight?: number;
}

const EVENT_COLORS: Record<string, string> = {
  'nasceu': '#00ff41',
  'morreu': '#ff3333',
  'conectados': '#00e5ff',
  'quebrada': '#ff6600',
  'Luminar': '#ffff00',
  'Umbral': '#8b00ff',
  'Nexus': '#00e5ff',
  'Pulsar': '#ff6600',
  'Void': '#555555',
};

function colorize(text: string): string {
  let result = text;
  for (const [keyword, color] of Object.entries(EVENT_COLORS)) {
    result = result.replaceAll(keyword, `<span style="color:${color}">${keyword}</span>`);
  }
  return result;
}

export function EventLog({ events, maxHeight = 300 }: Props) {
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [events]);

  return (
    <div style={{
      background: '#000',
      border: '1px solid #222',
      padding: '8px 12px',
      maxHeight,
      overflowY: 'auto',
      fontSize: '11px',
    }}>
      <div style={{ color: '#555', marginBottom: 4 }}>{'>'} EVENT LOG</div>
      {events.length === 0 && (
        <div style={{ color: '#333' }}>Waiting for events...</div>
      )}
      {events.map((event, i) => (
        <div
          key={i}
          style={{ color: '#00aa2a', padding: '1px 0' }}
          dangerouslySetInnerHTML={{ __html: `<span style="color:#333">[${String(i).padStart(3, '0')}]</span> ${colorize(event)}` }}
        />
      ))}
      <div ref={bottomRef} />
    </div>
  );
}
