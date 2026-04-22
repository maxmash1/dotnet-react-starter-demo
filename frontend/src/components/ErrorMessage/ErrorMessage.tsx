interface ErrorDisplayProps {
  errorTitle?: string;
  errorDescription: string;
}

/**
 * Displays error messages with dark-theme brand styling.
 * Used for API failures and validation errors.
 */
export function ErrorMessage({ errorTitle, errorDescription }: ErrorDisplayProps) {
  return (
    <div
      className="rounded-md border border-red-800/50 bg-red-900/20 p-4 text-sm text-red-400"
      role="alert"
      aria-live="assertive"
    >
      {errorTitle && (
        <h3 className="font-semibold mb-1">{errorTitle}</h3>
      )}
      <p>{errorDescription}</p>
    </div>
  );
}
