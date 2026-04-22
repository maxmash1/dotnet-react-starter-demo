/**
 * Animated loading indicator for async operations.
 * Displays a spinning visual cue while data is being fetched.
 */
export function LoadingSpinner() {
  return (
    <div className="flex flex-col items-center justify-center py-24" role="status" aria-label="Loading content">
      <div
        className="w-12 h-12 border-4 border-violet-600 border-t-transparent rounded-full animate-spin"
        aria-hidden="true"
      />
      <span className="mt-4 text-gray-400 text-sm font-medium">
        Loading data...
      </span>
    </div>
  );
}
