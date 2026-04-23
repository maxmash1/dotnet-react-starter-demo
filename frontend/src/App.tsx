import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { SystemHealthPage } from './routes/health/HealthPage';
import { DashboardPage } from './routes/dashboard/DashboardPage';

/**
 * Main application shell with routing and navigation.
 */
export function ApplicationShell() {
  return (
    <BrowserRouter>
      <div className="min-h-screen flex flex-col">
        <header className="bg-[var(--color-brand-header)] px-6 py-4 shadow-md">
          <nav className="max-w-6xl mx-auto flex items-center justify-between">
            <h1 className="text-white text-xl font-bold tracking-tight">
              Organization Starter
            </h1>
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
                  to="/dashboard"
                  className="text-white hover:text-blue-100 transition-colors"
                >
                  Dashboard
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
          </nav>
        </header>

        <main className="flex-1 max-w-6xl mx-auto w-full px-6 py-8">
          <Routes>
            <Route path="/" element={<WelcomeLanding />} />
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/health" element={<SystemHealthPage />} />
          </Routes>
        </main>

        <footer className="bg-gray-100 px-6 py-4 text-center text-sm text-gray-600">
          <p>Organization Starter Template &copy; {new Date().getFullYear()}</p>
        </footer>
      </div>
    </BrowserRouter>
  );
}

/** Landing page component */
function WelcomeLanding() {
  return (
    <div className="text-center py-12">
      <h2 className="text-3xl font-bold text-[var(--color-brand-text)] mb-4">
        Welcome to the Starter Template
      </h2>
      <p className="text-lg text-gray-600 mb-8 max-w-2xl mx-auto">
        This application demonstrates the organization development standards
        with a .NET 8 backend and React 18 frontend.
      </p>
      <Link
        to="/health"
        className="inline-block bg-[var(--color-brand-primary)] text-white px-6 py-3 rounded-lg font-medium hover:bg-blue-700 transition-colors"
      >
        Check System Health
      </Link>
    </div>
  );
}
