const BASE_URL = '/api';

export interface AtomDto {
  id: string;
  type: number;
  typeName: string;
  positionX: number;
  positionY: number;
  energy: number;
  age: number;
  isAlive: boolean;
  symbol: string;
  connectionCount: number;
}

export interface UniverseStateDto {
  id: string;
  currentTick: number;
  gridWidth: number;
  gridHeight: number;
  isActive: boolean;
  atomsAlive: number;
  totalConnections: number;
  createdAt: string;
  atoms: AtomDto[];
}

export interface TickResultDto {
  universeId: string;
  tick: number;
  atomsAlive: number;
  atomsBorn: number;
  atomsDied: number;
  connectionsFormed: number;
  connectionsBroken: number;
  totalEnergy: number;
  asciiArt: string;
  eventLog: string[];
}

export interface AsciiOutputDto {
  tick: number;
  asciiArt: string;
  atomCount: number;
  connectionCount: number;
  generatedAt: string;
}

export interface AnalyticsDto {
  universeId: string;
  currentTick: number;
  totalAtomsBorn: number;
  totalAtomsDied: number;
  atomsAlive: number;
  totalConnectionsFormed: number;
  totalConnectionsBroken: number;
  activeConnections: number;
  averageEnergy: number;
  atomsByType: Record<string, number>;
}

export interface HistoryResponse {
  total: number;
  skip: number;
  take: number;
  items: {
    id: string;
    type: string;
    atomId: string | null;
    connectionId: string | null;
    tick: number;
    description: string;
    occurredAt: string;
  }[];
}

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${url}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });
  if (!res.ok) throw new Error(`${res.status}: ${res.statusText}`);
  return res.json();
}

async function requestText(url: string): Promise<string> {
  const res = await fetch(`${BASE_URL}${url}`);
  if (!res.ok) throw new Error(`${res.status}: ${res.statusText}`);
  return res.text();
}

export const api = {
  createUniverse: (width = 80, height = 40, initialAtoms = 20) =>
    request<UniverseStateDto>('/universe', {
      method: 'POST',
      body: JSON.stringify({ width, height, initialAtoms }),
    }),

  getState: (id: string) =>
    request<UniverseStateDto>(`/universe/${id}`),

  tick: (id: string) =>
    request<TickResultDto>(`/universe/${id}/tick`, { method: 'POST' }),

  tickMultiple: (id: string, count: number) =>
    request<TickResultDto>(`/universe/${id}/tick/${count}`, { method: 'POST' }),

  getAscii: (id: string) =>
    requestText(`/universe/${id}/ascii`),

  getAsciiJson: (id: string) =>
    request<AsciiOutputDto>(`/universe/${id}/ascii/json`),

  getHistory: (id: string, skip = 0, take = 50) =>
    request<HistoryResponse>(`/universe/${id}/history?skip=${skip}&take=${take}`),

  getAnalytics: (id: string) =>
    request<AnalyticsDto>(`/universe/${id}/analytics`),

  getAtoms: (id: string) =>
    request<AtomDto[]>(`/universe/${id}/atoms`),
};
