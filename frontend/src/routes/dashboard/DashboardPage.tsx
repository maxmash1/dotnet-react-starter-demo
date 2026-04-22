import { Link } from 'react-router-dom';

const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

interface StatCardProps {
  label: string;
  value: string;
  icon: string;
  accentClass: string;
}

/** A single metric / stat card for the dashboard overview strip */
function StatCard({ label, value, icon, accentClass }: StatCardProps) {
  return (
    <div
      className="rounded-xl p-5 flex items-center gap-4 transition-shadow duration-200"
      style={{
        background: 'var(--ds-bg-card)',
        boxShadow: 'var(--ds-shadow)',
        border: '1px solid var(--ds-border)',
      }}
    >
      <div
        className={`w-12 h-12 rounded-lg flex items-center justify-center text-2xl flex-shrink-0 ${accentClass}`}
      >
        {icon}
      </div>
      <div>
        <p className="text-sm font-medium" style={{ color: 'var(--ds-text-muted)' }}>
          {label}
        </p>
        <p className="text-xl font-bold mt-0.5" style={{ color: 'var(--ds-text-heading)' }}>
          {value}
        </p>
      </div>
    </div>
  );
}

interface QuickActionProps {
  title: string;
  description: string;
  href: string;
  icon: string;
  external?: boolean;
}

/** A quick-action card that navigates to a route or external URL */
function QuickAction({ title, description, href, icon, external }: QuickActionProps) {
  const sharedClassName =
    'group block rounded-xl p-6 transition-all duration-200 cursor-pointer ' +
    'hover:shadow-lg hover:bg-[var(--ds-bg-card-hover)]';
  const sharedStyle = {
    background: 'var(--ds-bg-card)',
    boxShadow: 'var(--ds-shadow)',
    border: '1px solid var(--ds-border)',
  };

  const inner = (
    <div className="flex items-start gap-4">
      <span className="text-3xl">{icon}</span>
      <div>
        <h3
          className="font-semibold text-base mb-1"
          style={{ color: 'var(--ds-accent-primary)' }}
        >
          {title}
        </h3>
        <p className="text-sm leading-relaxed" style={{ color: 'var(--ds-text-body)' }}>
          {description}
        </p>
      </div>
    </div>
  );

  if (external) {
    return (
      <a
        href={href}
        target="_blank"
        rel="noopener noreferrer"
        className={sharedClassName}
        style={sharedStyle}
      >
        {inner}
      </a>
    );
  }

  return (
    <Link to={href} className={sharedClassName} style={sharedStyle}>
      {inner}
    </Link>
  );
}

/**
 * Main dashboard page — shows an overview of the application
 * with stat cards and quick-action navigation links.
 * Supports both light and dark themes via CSS custom properties.
 */
export function DashboardPage() {
  return (
    <div className="space-y-8">
      {/* Hero */}
      <section>
        <h2
          className="text-3xl font-bold tracking-tight mb-2"
          style={{ color: 'var(--ds-text-heading)' }}
        >
          Welcome back 👋
        </h2>
        <p className="text-base max-w-2xl leading-relaxed" style={{ color: 'var(--ds-text-body)' }}>
          This is the organization starter template — a full-stack demo built with{' '}
          <span className="font-medium" style={{ color: 'var(--ds-accent-primary)' }}>
            .NET 8
          </span>{' '}
          and{' '}
          <span className="font-medium" style={{ color: 'var(--ds-accent-primary)' }}>
            React 18
          </span>
          . Use the navigation above to explore the available features.
        </p>
      </section>

      {/* Stat strip */}
      <section>
        <h3
          className="text-xs font-semibold uppercase tracking-widest mb-4"
          style={{ color: 'var(--ds-text-muted)' }}
        >
          Overview
        </h3>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <StatCard
            label="Stack"
            value=".NET 8 + React"
            icon="⚡"
            accentClass="bg-blue-100 dark:bg-blue-900/40"
          />
          <StatCard
            label="API Style"
            value="RESTful"
            icon="🔗"
            accentClass="bg-purple-100 dark:bg-purple-900/40"
          />
          <StatCard
            label="Frontend"
            value="TypeScript"
            icon="🛡️"
            accentClass="bg-sky-100 dark:bg-sky-900/40"
          />
          <StatCard
            label="Styling"
            value="Tailwind CSS"
            icon="🎨"
            accentClass="bg-teal-100 dark:bg-teal-900/40"
          />
        </div>
      </section>

      {/* Quick actions */}
      <section>
        <h3
          className="text-xs font-semibold uppercase tracking-widest mb-4"
          style={{ color: 'var(--ds-text-muted)' }}
        >
          Quick Actions
        </h3>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <QuickAction
            href="/health"
            icon="🏥"
            title="System Health"
            description="Check the current status of the backend API, version information, and environment details."
          />
          <QuickAction
            href={`${API_BASE}/swagger`}
            icon="📋"
            title="API Documentation"
            description="Browse the Swagger UI to explore and test all available API endpoints interactively."
            external
          />
        </div>
      </section>

      {/* Tech info footer */}
      <section>
        <div
          className="rounded-xl p-5"
          style={{
            background: 'var(--ds-bg-card)',
            border: '1px solid var(--ds-border)',
          }}
        >
          <h3
            className="text-sm font-semibold mb-3"
            style={{ color: 'var(--ds-text-heading)' }}
          >
            About this template
          </h3>
          <ul className="grid grid-cols-1 sm:grid-cols-2 gap-2">
            {[
              'Organization development standards enforced',
              'RESTful API with envelope response pattern',
              'EF Core + Repository + Service architecture',
              'xUnit + Moq backend testing suite',
              'React 18 with TypeScript strict mode',
              'Tailwind CSS utility-first styling',
            ].map((item) => (
              <li key={item} className="flex items-center gap-2 text-sm">
                <span style={{ color: 'var(--ds-accent-success)' }}>✓</span>
                <span style={{ color: 'var(--ds-text-body)' }}>{item}</span>
              </li>
            ))}
          </ul>
        </div>
      </section>
    </div>
  );
}
