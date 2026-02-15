import { useMemo } from 'react';

interface Props {
  ascii: string;
  tick?: number;
}

const SYMBOL_COLORS: Record<string, string> = {
  '*': '#ffff00',  // Luminar — yellow glow
  '#': '#8b00ff',  // Umbral — deep purple
  '@': '#00e5ff',  // Nexus — cyan hub
  '~': '#ff6600',  // Pulsar — orange wave
  '.': '#333333',  // Void — barely visible
  '=': '#00ff41',  // Strong connection
  '-': '#00aa2a',  // Medium connection
  ':': '#005500',  // Weak connection
  '+': '#333333',  // Border corner
  '|': '#333333',  // Border vertical
};

export function AsciiViewer({ ascii, tick }: Props) {
  const coloredHtml = useMemo(() => {
    return ascii.split('\n').map((line) => {
      return line.split('').map((ch) => {
        const color = SYMBOL_COLORS[ch];
        if (color) {
          return `<span style="color:${color}">${ch === '<' ? '&lt;' : ch === '>' ? '&gt;' : ch}</span>`;
        }
        return ch === '<' ? '&lt;' : ch === '>' ? '&gt;' : ch;
      }).join('');
    }).join('\n');
  }, [ascii]);

  return (
    <div style={{
      background: '#000000',
      border: '1px solid #222',
      padding: '12px',
      position: 'relative',
      overflow: 'auto',
      maxHeight: '100%',
    }}>
      {tick !== undefined && (
        <div style={{
          position: 'absolute',
          top: 4,
          right: 12,
          color: '#00ff41',
          fontSize: '11px',
          opacity: 0.6,
        }}>
          TICK {tick}
        </div>
      )}
      <pre
        style={{
          margin: 0,
          fontSize: '12px',
          lineHeight: '14px',
          letterSpacing: '1px',
          whiteSpace: 'pre',
        }}
        dangerouslySetInnerHTML={{ __html: coloredHtml }}
      />
    </div>
  );
}
