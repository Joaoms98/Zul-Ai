const LOGO = `
 ______      _        _    _
|___  /     | |      / \\  (_)
   / /_   _ | |     / _ \\  _
  / /| | | || |    / ___ \\| |
 / /_| |_| || |___/ /   \\_\\_|
/_____|\\__,_||_____/       (_)
`;

export function Header() {
  return (
    <header style={{
      borderBottom: '1px solid #222',
      padding: '8px 20px',
      display: 'flex',
      alignItems: 'center',
      gap: 24,
      background: '#0a0a0a',
    }}>
      <pre style={{
        color: '#00ff41',
        fontSize: '8px',
        lineHeight: '9px',
        margin: 0,
        textShadow: '0 0 10px rgba(0,255,65,0.3)',
      }}>
        {LOGO}
      </pre>
      <div style={{ flex: 1 }}>
        <div style={{ color: '#555', fontSize: '10px', letterSpacing: 3 }}>
          MINI UNIVERSE SIMULATOR
        </div>
      </div>
      <div style={{ color: '#333', fontSize: '10px' }}>
        v1.0.0 | {new Date().toLocaleDateString()}
      </div>
    </header>
  );
}
