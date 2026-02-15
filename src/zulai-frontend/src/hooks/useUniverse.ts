import { useState, useCallback, useRef } from 'react';
import { api } from '../services/api';
import type { UniverseStateDto, TickResultDto, AnalyticsDto } from '../services/api';

export function useUniverse() {
  const [universe, setUniverse] = useState<UniverseStateDto | null>(null);
  const [lastTick, setLastTick] = useState<TickResultDto | null>(null);
  const [analytics, setAnalytics] = useState<AnalyticsDto | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [autoPlaying, setAutoPlaying] = useState(false);
  const intervalRef = useRef<number | null>(null);

  const create = useCallback(async (width = 80, height = 40, atoms = 20) => {
    setLoading(true);
    setError(null);
    try {
      const state = await api.createUniverse(width, height, atoms);
      setUniverse(state);
      setLastTick(null);
      setAnalytics(null);
      return state;
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Failed to create universe');
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  const tick = useCallback(async () => {
    if (!universe) return null;
    setLoading(true);
    setError(null);
    try {
      const result = await api.tick(universe.id);
      setLastTick(result);
      setUniverse(prev => prev ? { ...prev, currentTick: result.tick, atomsAlive: result.atomsAlive } : prev);
      return result;
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Tick failed');
      return null;
    } finally {
      setLoading(false);
    }
  }, [universe]);

  const tickMultiple = useCallback(async (count: number) => {
    if (!universe) return null;
    setLoading(true);
    setError(null);
    try {
      const result = await api.tickMultiple(universe.id, count);
      setLastTick(result);
      setUniverse(prev => prev ? { ...prev, currentTick: result.tick, atomsAlive: result.atomsAlive } : prev);
      return result;
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Tick failed');
      return null;
    } finally {
      setLoading(false);
    }
  }, [universe]);

  const fetchAnalytics = useCallback(async () => {
    if (!universe) return;
    try {
      const data = await api.getAnalytics(universe.id);
      setAnalytics(data);
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : 'Failed to fetch analytics');
    }
  }, [universe]);

  const startAutoPlay = useCallback((speedMs = 800) => {
    if (autoPlaying || !universe) return;
    setAutoPlaying(true);
    const id = window.setInterval(async () => {
      try {
        const result = await api.tick(universe.id);
        setLastTick(result);
        setUniverse(prev => prev ? { ...prev, currentTick: result.tick, atomsAlive: result.atomsAlive } : prev);
      } catch {
        // stop on error
        if (intervalRef.current) window.clearInterval(intervalRef.current);
        setAutoPlaying(false);
      }
    }, speedMs);
    intervalRef.current = id;
  }, [autoPlaying, universe]);

  const stopAutoPlay = useCallback(() => {
    if (intervalRef.current) {
      window.clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
    setAutoPlaying(false);
  }, []);

  return {
    universe, lastTick, analytics, loading, error, autoPlaying,
    create, tick, tickMultiple, fetchAnalytics, startAutoPlay, stopAutoPlay,
  };
}
