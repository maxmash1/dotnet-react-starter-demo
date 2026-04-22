import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { SystemHealthPage } from './routes/health/HealthPage';

const GITHUB_REPO_URL = import.meta.env.VITE_GITHUB_REPO_URL || 'https://github.com/maxmash1/dotnet-react-starter-demo';

/**
 * Main application shell with routing and navigation.
 */
export function ApplicationShell() {
  return (
    <BrowserRouter>
      <div className="min-h-screen flex flex-col bg-gray-950 text-white antialiased">
        {/* Header */}
        <header className="border-b border-gray-800 px-6 py-4">
          <nav className="max-w-7xl mx-auto flex items-center justify-between">
            <div className="flex items-center gap-2">
              <div className="h-8 w-8 rounded-lg bg-violet-600 flex items-center justify-center">
                <span className="text-white text-xs font-bold">OS</span>
              </div>
              <span className="text-white text-lg font-semibold tracking-tight">
                Organization Starter
              </span>
            </div>
            <ul className="flex gap-6">
              <li>
                <Link
                  to="/"
                  className="text-gray-400 hover:text-white transition-colors text-sm font-medium"
                >
                  Home
                </Link>
              </li>
              <li>
                <Link
                  to="/health"
                  className="text-gray-400 hover:text-white transition-colors text-sm font-medium"
                >
                  System Health
                </Link>
              </li>
            </ul>
          </nav>
        </header>

        <main className="flex-1 max-w-7xl mx-auto w-full px-6 py-8">
          <Routes>
            <Route path="/" element={<WelcomeLanding />} />
            <Route path="/health" element={<SystemHealthPage />} />
          </Routes>
        </main>

        <footer className="border-t border-gray-800 px-6 py-4 text-center text-sm text-gray-500">
          <p>Organization Starter Template &copy; {new Date().getFullYear()}</p>
        </footer>
      </div>
    </BrowserRouter>
  );
}

/** Landing page component */
function WelcomeLanding() {
  return (
    <section className="relative py-24 sm:py-32">
      {/* Decorative background glow orbs */}
      <div className="pointer-events-none absolute inset-0 overflow-hidden">
        <div className="absolute -top-40 -right-40 h-80 w-80 rounded-full bg-violet-600/20 blur-3xl" />
        <div className="absolute -bottom-40 -left-40 h-80 w-80 rounded-full bg-fuchsia-600/20 blur-3xl" />
      </div>

      <div className="relative mx-auto max-w-4xl text-center">
        {/* Section label */}
        <p className="text-sm font-semibold tracking-wider text-violet-400 uppercase mb-4">
          Demo Template
        </p>

        {/* Hero headline with gradient */}
        <h2 className="text-5xl sm:text-7xl font-bold tracking-tight mb-6">
          Welcome to the{' '}
          <span className="bg-gradient-to-r from-violet-400 to-fuchsia-400 bg-clip-text text-transparent">
            Starter Template
          </span>
        </h2>

        {/* Sub-text */}
        <p className="text-lg leading-8 text-gray-400 mb-10 max-w-2xl mx-auto">
          This application demonstrates the organization development standards
          with a .NET 8 backend and React 18 frontend.
        </p>

        {/* CTA buttons */}
        <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
          <Link
            to="/health"
            className="rounded-md bg-violet-600 px-6 py-3 text-sm font-medium text-white hover:bg-violet-500 focus:outline-none focus:ring-2 focus:ring-violet-500 focus:ring-offset-2 focus:ring-offset-gray-950 transition-colors"
          >
            Check System Health
          </Link>
          <a
            href={GITHUB_REPO_URL}
            target="_blank"
            rel="noopener noreferrer"
            className="rounded-md border border-gray-700 px-6 py-3 text-sm font-medium text-gray-300 hover:text-white hover:border-gray-500 focus:outline-none focus:ring-2 focus:ring-violet-500 focus:ring-offset-2 focus:ring-offset-gray-950 transition-colors"
          >
            View on GitHub
          </a>
        </div>

        {/* Feature cards */}
        <div className="mt-16 grid gap-6 sm:grid-cols-2 lg:grid-cols-3 text-left">
          <FeatureCard
            emoji="⚙️"
            title=".NET 8 Backend"
            description="RESTful API with envelope pattern, EF Core, and organization-standard repository/service layers."
          />
          <FeatureCard
            emoji="⚛️"
            title="React 18 Frontend"
            description="TypeScript components with Tailwind CSS, React Router, and a typed API client."
          />
          <FeatureCard
            emoji="🔍"
            title="System Health"
            description="Live health check endpoint wired end-to-end from the .NET backend to this UI."
          />
        </div>
      </div>
    </section>
  );
}

interface FeatureCardProps {
  emoji: string;
  title: string;
  description: string;
}

function FeatureCard({ emoji, title, description }: FeatureCardProps) {
  return (
    <div className="group rounded-2xl border border-white/10 bg-white/5 p-6 transition-colors hover:border-violet-500/40 hover:bg-white/[0.07]">
      <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-lg bg-violet-600/10">
        <span className="text-2xl" role="img" aria-hidden="true">{emoji}</span>
      </div>
      <h3 className="text-lg font-semibold text-white mb-2">{title}</h3>
      <p className="text-sm text-gray-400 leading-relaxed">{description}</p>
    </div>
  );
}

