import { useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { SystemHealthPage } from './routes/health/HealthPage';
import { DashboardPage } from './routes/dashboard';

/** Reads the persisted theme from localStorage, defaulting to light. */
function getInitialTheme(): 'light' | 'dark' {
  try {
    const stored = localStorage.getItem('theme');
    if (stored === 'dark' || stored === 'light') return stored;
  } catch {
    // localStorage unavailable
  }
  return 'light';
}

/**
 * Main application shell with routing, navigation, and theme toggle.
 */
export function ApplicationShell() {
  const [theme, setTheme] = useState<'light' | 'dark'>(getInitialTheme);

  // Apply / remove the `dark` class on <html> whenever theme changes
  useEffect(() => {
    const root = document.documentElement;
    if (theme === 'dark') {
      root.classList.add('dark');
    } else {
      root.classList.remove('dark');
    }
    try {
      localStorage.setItem('theme', theme);
    } catch {
      // localStorage unavailable
    }
  }, [theme]);

  function toggleTheme() {
    setTheme((prev) => (prev === 'light' ? 'dark' : 'light'));
  }

  const isDark = theme === 'dark';

  return (
    <BrowserRouter>
      <div className="min-h-screen flex flex-col">
        <header className="bg-[var(--color-brand-header)] px-6 py-4 shadow-md">
          <nav className="max-w-6xl mx-auto flex items-center justify-between">
            <h1 className="text-white text-xl font-bold tracking-tight">
              Organization Starter
            </h1>
            <div className="flex items-center gap-6">
              <ul className="flex gap-6">
                <li>
                  <Link
                    to="/"
                    className="text-white hover:text-blue-100 transition-colors"
                  >
                    Home
                  </Link>
                </li>
                <li>
                  <Link
                    to="/health"
                    className="text-white hover:text-blue-100 transition-colors"
                  >
                    System Health
                  </Link>
                </li>
              </ul>

              {/* Theme toggle */}
              <button
                onClick={toggleTheme}
                aria-label={isDark ? 'Switch to light mode' : 'Switch to dark mode'}
                className="w-9 h-9 rounded-full flex items-center justify-center text-lg transition-colors duration-200 bg-white/20 hover:bg-white/30 focus:outline-none focus:ring-2 focus:ring-white/60"
              >
                {isDark ? '☀️' : '🌙'}
              </button>
            </div>
          </nav>
        </header>

        <main
          className="flex-1 max-w-6xl mx-auto w-full px-6 py-8"
          style={{ background: 'var(--ds-bg-page)' }}
        >
          <Routes>
            <Route path="/" element={<DashboardPage />} />
            <Route path="/health" element={<SystemHealthPage />} />
          </Routes>
        </main>

        <footer
          className="px-6 py-4 text-center text-sm transition-colors duration-200"
          style={{
            background: 'var(--ds-bg-card)',
            borderTop: '1px solid var(--ds-border)',
            color: 'var(--ds-text-muted)',
          }}
        >
          <p>Organization Starter Template &copy; {new Date().getFullYear()}</p>
        </footer>
      </div>
    </BrowserRouter>
  );
}
