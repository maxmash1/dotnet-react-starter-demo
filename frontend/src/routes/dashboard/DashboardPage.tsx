import { useEffect, useState } from 'react';
import { executeApiRequest } from '../../lib/api';
import type { SingleItemEnvelope, SystemHealthInfo } from '../../lib/types';

type PageState = 'loading' | 'success' | 'error';

function SunIcon() {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      className="h-4 w-4"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
      strokeWidth={2}
      aria-hidden="true"
    >
      <circle cx="12" cy="12" r="5" />
      <path
        strokeLinecap="round"
        d="M12 1v2M12 21v2M4.22 4.22l1.42 1.42M18.36 18.36l1.42 1.42M1 12h2M21 12h2M4.22 19.78l1.42-1.42M18.36 5.64l1.42-1.42"
      />
    </svg>
  );
}

function MoonIcon() {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      className="h-4 w-4"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
      strokeWidth={2}
      aria-hidden="true"
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M21 12.79A9 9 0 1111.21 3 7 7 0 0021 12.79z"
      />
    </svg>
  );
}

function CheckCircleIcon() {
  return (
    <svg
      className="h-6 w-6 text-violet-600 dark:text-violet-400"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
      strokeWidth={2}
      aria-hidden="true"
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
      />
    </svg>
  );
}

function TagIcon() {
  return (
    <svg
      className="h-6 w-6 text-violet-600 dark:text-violet-400"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
      strokeWidth={2}
      aria-hidden="true"
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"
      />
    </svg>
  );
}

function ServerIcon() {
  return (
    <svg
      className="h-6 w-6 text-violet-600 dark:text-violet-400"
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
      strokeWidth={2}
      aria-hidden="true"
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M5 12h14M5 12a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v4a2 2 0 01-2 2M5 12a2 2 0 00-2 2v4a2 2 0 002 2h14a2 2 0 002-2v-4a2 2 0 00-2-2m-2-4h.01M17 16h.01"
      />
    </svg>
  );
}

/**
 * System dashboard page styled with the Agent Hub design system.
 * Supports both light and dark themes via a scoped toggle.
 */
export function DashboardPage() {
  const [pageState, setPageState] = useState<PageState>('loading');
  const [healthData, setHealthData] = useState<SingleItemEnvelope<SystemHealthInfo> | null>(null);
  const [errorText, setErrorText] = useState<string>('');
  const [isDark, setIsDark] = useState<boolean>(() => {
    try {
      const stored = localStorage.getItem('dashboard-theme');
      if (stored !== null) return stored === 'dark';
      return window.matchMedia('(prefers-color-scheme: dark)').matches;
    } catch {
      // localStorage or matchMedia may be unavailable in restricted environments (e.g. SSR, sandboxed iframes)
      return false;
    }
  });

  const toggleTheme = () => {
    const next = !isDark;
    setIsDark(next);
    try {
      localStorage.setItem('dashboard-theme', next ? 'dark' : 'light');
    } catch {
      // localStorage may be blocked (private browsing, storage quota exceeded).
      // The toggle still works for the current session; preference just won't persist.
    }
  };

  useEffect(() => {
    let isMounted = true;

    async function fetchHealthStatus() {
      try {
        const apiResponse = await executeApiRequest<SingleItemEnvelope<SystemHealthInfo>>(
          '/v1/health'
        );
        if (isMounted) {
          setHealthData(apiResponse);
          setPageState('success');
        }
      } catch (err) {
        if (isMounted) {
          const errorMessage = err instanceof Error ? err.message : 'An unexpected error occurred while loading dashboard data';
          setErrorText(errorMessage);
          setPageState('error');
        }
      }
    }

    fetchHealthStatus();

    return () => {
      isMounted = false;
    };
  }, []);

  if (pageState === 'loading') {
    return (
      <div className={isDark ? 'dark' : ''}>
        <div className="flex flex-col items-center justify-center py-16 bg-gray-50 dark:bg-gray-950 rounded-2xl transition-colors">
          <div
            className="w-12 h-12 border-4 border-violet-600 dark:border-violet-500 border-t-transparent rounded-full animate-spin"
            role="status"
            aria-label="Loading dashboard"
          />
          <span className="mt-4 text-gray-500 dark:text-gray-400 font-medium">
            Loading dashboard...
          </span>
        </div>
      </div>
    );
  }

  if (pageState === 'error') {
    return (
      <div className={isDark ? 'dark' : ''}>
        <div className="bg-gray-50 dark:bg-gray-950 rounded-2xl p-6 transition-colors">
          <div
            className="rounded-md border border-red-200 dark:border-red-800/50 bg-red-50 dark:bg-red-900/20 p-4 text-sm text-red-700 dark:text-red-400"
            role="alert"
            aria-live="assertive"
          >
            <p className="font-semibold mb-1">Dashboard Error</p>
            <p>{errorText}</p>
          </div>
        </div>
      </div>
    );
  }

  if (!healthData) {
    return null;
  }

  const { item: healthInfo, metadata } = healthData;
  const isHealthy = healthInfo.status === 'healthy';

  return (
    <div className={isDark ? 'dark' : ''}>
      <div className="relative bg-gray-50 dark:bg-gray-950 rounded-2xl p-6 transition-colors overflow-hidden">

        {/* Decorative background orbs — dark mode only */}
        <div className="pointer-events-none absolute inset-0 overflow-hidden" aria-hidden="true">
          <div className="absolute -top-20 -right-20 h-72 w-72 rounded-full bg-violet-600/20 blur-3xl hidden dark:block" />
          <div className="absolute -bottom-20 -left-20 h-72 w-72 rounded-full bg-fuchsia-600/20 blur-3xl hidden dark:block" />
        </div>

        {/* Content */}
        <div className="relative space-y-6">

          {/* Page header */}
          <div className="flex items-start justify-between gap-4">
            <div>
              <p className="text-sm font-semibold tracking-wider uppercase text-violet-600 dark:text-violet-400 mb-1">
                System
              </p>
              <h2 className="text-3xl font-bold tracking-tight">
                <span className="bg-gradient-to-r from-violet-600 to-fuchsia-600 dark:from-violet-400 dark:to-fuchsia-400 bg-clip-text text-transparent">
                  Dashboard
                </span>
              </h2>
            </div>

            {/* Theme toggle */}
            <button
              onClick={toggleTheme}
              className="flex items-center gap-2 rounded-md border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 px-3 py-2 text-sm font-medium text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors focus:outline-none focus:ring-2 focus:ring-violet-500 focus:ring-offset-1 dark:focus:ring-offset-gray-950 shrink-0"
              aria-label={isDark ? 'Switch to light mode' : 'Switch to dark mode'}
            >
              {isDark ? <SunIcon /> : <MoonIcon />}
              <span>{isDark ? 'Light' : 'Dark'}</span>
            </button>
          </div>

          {/* Stat cards */}
          <div className="grid gap-4 sm:grid-cols-3">

            {/* Status card */}
            <div className="group rounded-2xl border border-gray-200 dark:border-white/10 bg-white dark:bg-white/5 p-6 transition-colors hover:border-violet-500/40 dark:hover:border-violet-500/40 hover:bg-violet-50 dark:hover:bg-white/[0.07]">
              <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-lg bg-violet-100 dark:bg-violet-600/10">
                <CheckCircleIcon />
              </div>
              <p className="text-xs text-gray-600 dark:text-gray-400 mb-1">Health Status</p>
              <p
                className={`text-lg font-semibold ${
                  isHealthy
                    ? 'text-green-600 dark:text-green-400'
                    : 'text-yellow-600 dark:text-yellow-400'
                }`}
              >
                {healthInfo.status.toUpperCase()}
              </p>
            </div>

            {/* Version card */}
            <div className="group rounded-2xl border border-gray-200 dark:border-white/10 bg-white dark:bg-white/5 p-6 transition-colors hover:border-violet-500/40 dark:hover:border-violet-500/40 hover:bg-violet-50 dark:hover:bg-white/[0.07]">
              <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-lg bg-violet-100 dark:bg-violet-600/10">
                <TagIcon />
              </div>
              <p className="text-xs text-gray-600 dark:text-gray-400 mb-1">Version</p>
              <p className="text-lg font-semibold text-gray-900 dark:text-white">
                {healthInfo.version}
              </p>
            </div>

            {/* Environment card */}
            <div className="group rounded-2xl border border-gray-200 dark:border-white/10 bg-white dark:bg-white/5 p-6 transition-colors hover:border-violet-500/40 dark:hover:border-violet-500/40 hover:bg-violet-50 dark:hover:bg-white/[0.07]">
              <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-lg bg-violet-100 dark:bg-violet-600/10">
                <ServerIcon />
              </div>
              <p className="text-xs text-gray-600 dark:text-gray-400 mb-1">Environment</p>
              <p className="text-lg font-semibold text-gray-900 dark:text-white">
                {healthInfo.environment}
              </p>
            </div>
          </div>

          {/* Metadata section */}
          <div className="rounded-xl border border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-900/50 p-5">
            <h3 className="text-sm font-semibold text-gray-500 dark:text-gray-400 mb-4">
              Response Metadata
            </h3>
            <div className="space-y-3">
              <div className="flex items-start gap-3">
                <span className="text-xs font-medium text-gray-600 dark:text-gray-400 w-28 pt-0.5 shrink-0">
                  Transaction ID
                </span>
                <span className="text-sm text-gray-700 dark:text-gray-300 font-mono break-all">
                  {metadata.transactionId}
                </span>
              </div>
              <div className="flex items-start gap-3">
                <span className="text-xs font-medium text-gray-600 dark:text-gray-400 w-28 pt-0.5 shrink-0">
                  Checked At
                </span>
                <span className="text-sm text-gray-700 dark:text-gray-300">
                  {new Date(healthInfo.checkedAtDate).toLocaleString()}
                </span>
              </div>
              <div className="flex items-start gap-3">
                <span className="text-xs font-medium text-gray-600 dark:text-gray-400 w-28 pt-0.5 shrink-0">
                  Timestamp
                </span>
                <span className="text-sm text-gray-700 dark:text-gray-300">
                  {metadata.timestamp}
                </span>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}
