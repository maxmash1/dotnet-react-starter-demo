import { render, screen, waitFor } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { SystemHealthPage } from './HealthPage';
import type { SingleItemEnvelope, SystemHealthInfo } from '../../lib/types';

/** Wraps the component in a router context required by any Link/NavLink usage */
function renderHealthPage() {
  return render(
    <MemoryRouter>
      <SystemHealthPage />
    </MemoryRouter>,
  );
}

/** Builds a minimal valid health envelope for mocking */
function buildHealthEnvelope(overrides?: Partial<SystemHealthInfo>): SingleItemEnvelope<SystemHealthInfo> {
  return {
    item: {
      status: 'healthy',
      version: '1.0.0.0',
      checkedAtDate: new Date().toISOString(),
      environment: 'Testing',
      ...overrides,
    },
    metadata: {
      timestamp: new Date().toISOString(),
      transactionId: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
    },
    links: {
      self: 'http://localhost/v1/health',
    },
  };
}

describe('SystemHealthPage', () => {
  beforeEach(() => {
    vi.resetAllMocks();
  });

  describe('loading state', () => {
    it('shows the loading spinner while the request is in flight', () => {
      // Arrange — fetch never resolves during this test
      global.fetch = vi.fn().mockReturnValue(new Promise(() => {}));

      // Act
      renderHealthPage();

      // Assert
      expect(screen.getByRole('status')).toBeInTheDocument();
    });
  });

  describe('success state', () => {
    it('renders the health status badge after a successful response', async () => {
      // Arrange
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(buildHealthEnvelope()),
      } as unknown as Response);

      // Act
      renderHealthPage();

      // Assert
      await waitFor(() => {
        expect(screen.getByText('HEALTHY')).toBeInTheDocument();
      });
    });

    it('renders the API version returned from the backend', async () => {
      // Arrange
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(buildHealthEnvelope({ version: '2.3.1.0' })),
      } as unknown as Response);

      // Act
      renderHealthPage();

      // Assert
      await waitFor(() => {
        expect(screen.getByText('2.3.1.0')).toBeInTheDocument();
      });
    });

    it('renders the runtime environment', async () => {
      // Arrange
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(buildHealthEnvelope({ environment: 'Production' })),
      } as unknown as Response);

      // Act
      renderHealthPage();

      // Assert
      await waitFor(() => {
        expect(screen.getByText('Production')).toBeInTheDocument();
      });
    });

    it('renders the transaction ID in the metadata section', async () => {
      // Arrange
      const transactionId = 'test-txn-id-12345';
      const envelope = buildHealthEnvelope();
      envelope.metadata.transactionId = transactionId;

      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        json: vi.fn().mockResolvedValue(envelope),
      } as unknown as Response);

      // Act
      renderHealthPage();

      // Assert
      await waitFor(() => {
        expect(screen.getByText(transactionId)).toBeInTheDocument();
      });
    });
  });

  describe('error state', () => {
    it('shows the error component when the API call fails', async () => {
      // Arrange
      global.fetch = vi.fn().mockResolvedValue({
        ok: false,
        status: 503,
        statusText: 'Service Unavailable',
        text: vi.fn().mockResolvedValue(''),
      } as unknown as Response);

      // Act
      renderHealthPage();

      // Assert
      await waitFor(() => {
        expect(screen.getByRole('alert')).toBeInTheDocument();
      });
    });

    it('shows "Health Check Failed" as the error title', async () => {
      // Arrange
      global.fetch = vi.fn().mockRejectedValue(new Error('Network error'));

      // Act
      renderHealthPage();

      // Assert
      await waitFor(() => {
        expect(screen.getByText('Health Check Failed')).toBeInTheDocument();
      });
    });

    it('includes the error message in the alert text', async () => {
      // Arrange
      global.fetch = vi.fn().mockRejectedValue(new Error('Failed to fetch'));

      // Act
      renderHealthPage();

      // Assert
      await waitFor(() => {
        expect(screen.getByText(/Failed to fetch/)).toBeInTheDocument();
      });
    });
  });
});
