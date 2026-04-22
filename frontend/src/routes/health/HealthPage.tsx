import { useEffect, useState } from 'react';
import { executeApiRequest } from '../../lib/api';
import type { SingleItemEnvelope, SystemHealthInfo } from '../../lib/types';
import { LoadingSpinner } from '../../components/LoadingSpinner';
import { ErrorMessage } from '../../components/ErrorMessage';

type PageState = 'loading' | 'success' | 'error';

/**
 * System health monitoring page.
 * Fetches and displays the current health status from the backend API.
 */
export function SystemHealthPage() {
  const [pageState, setPageState] = useState<PageState>('loading');
  const [healthData, setHealthData] = useState<SingleItemEnvelope<SystemHealthInfo> | null>(null);
  const [errorText, setErrorText] = useState<string>('');

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
          const errorMessage = err instanceof Error ? err.message : 'Unknown error occurred';
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
    return <LoadingSpinner />;
  }

  if (pageState === 'error') {
    return (
      <ErrorMessage
        errorTitle="Health Check Failed"
        errorDescription={errorText}
      />
    );
  }

  if (!healthData) {
    return null;
  }

  const { item: healthInfo, metadata } = healthData;
  const isHealthy = healthInfo.status === 'healthy';

  return (
    <div className="relative space-y-6">
      {/* Decorative background glow orbs */}
      <div className="pointer-events-none absolute inset-0 overflow-hidden -z-10">
        <div className="absolute -top-20 -right-20 h-60 w-60 rounded-full bg-violet-600/20 blur-3xl" />
        <div className="absolute -bottom-20 -left-20 h-60 w-60 rounded-full bg-fuchsia-600/20 blur-3xl" />
      </div>

      {/* Page heading */}
      <div>
        <p className="text-sm font-semibold tracking-wider text-violet-400 uppercase mb-1">
          Monitoring
        </p>
        <h2 className="text-3xl font-bold tracking-tight text-white">
          System Health Status
        </h2>
      </div>

      {/* Main status card */}
      <div className="rounded-2xl border border-white/10 bg-white/5 p-6 space-y-5 backdrop-blur-sm">
        {/* Status row */}
        <div className="flex items-center gap-4">
          <span className="text-gray-400 font-medium w-36 text-sm">Status</span>
          <div className="flex items-center gap-2">
            {/* Animated ping dot */}
            <span className="relative flex h-3 w-3">
              {isHealthy && (
                <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-green-400 opacity-75" />
              )}
              <span
                className={`relative inline-flex rounded-full h-3 w-3 ${
                  isHealthy ? 'bg-green-400' : 'bg-yellow-400'
                }`}
              />
            </span>
            <span
              className={`text-sm font-semibold ${
                isHealthy ? 'text-green-400' : 'text-yellow-400'
              }`}
            >
              {healthInfo.status.toUpperCase()}
            </span>
          </div>
        </div>

        <div className="border-t border-white/5" />

        {/* Version row */}
        <div className="flex items-center gap-4">
          <span className="text-gray-400 font-medium w-36 text-sm">Version</span>
          <span className="text-white text-sm font-mono">{healthInfo.version}</span>
        </div>

        <div className="border-t border-white/5" />

        {/* Environment row */}
        <div className="flex items-center gap-4">
          <span className="text-gray-400 font-medium w-36 text-sm">Environment</span>
          <span className="inline-flex items-center rounded-md bg-violet-600/10 px-2.5 py-0.5 text-xs font-medium text-violet-400 ring-1 ring-inset ring-violet-500/20">
            {healthInfo.environment}
          </span>
        </div>

        <div className="border-t border-white/5" />

        {/* Checked at row */}
        <div className="flex items-center gap-4">
          <span className="text-gray-400 font-medium w-36 text-sm">Checked At</span>
          <span className="text-white text-sm">
            {new Date(healthInfo.checkedAtDate).toLocaleString()}
          </span>
        </div>
      </div>

      {/* Metadata panel */}
      <div className="rounded-xl border border-white/10 bg-white/5 p-4">
        <h3 className="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3">
          Response Metadata
        </h3>
        <div className="space-y-1.5">
          <p className="text-xs text-gray-400">
            <span className="font-medium text-gray-300">Transaction ID:</span>{' '}
            <span className="font-mono">{metadata.transactionId}</span>
          </p>
          <p className="text-xs text-gray-400">
            <span className="font-medium text-gray-300">Timestamp:</span>{' '}
            {metadata.timestamp}
          </p>
        </div>
      </div>
    </div>
  );
}

