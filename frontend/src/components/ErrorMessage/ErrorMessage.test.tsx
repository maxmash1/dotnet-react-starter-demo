import { render, screen } from '@testing-library/react';
import { ErrorMessage } from './ErrorMessage';

describe('ErrorMessage', () => {
  describe('renders with required props', () => {
    it('displays the error description', () => {
      // Arrange
      const description = 'Something went wrong with the request';

      // Act
      render(<ErrorMessage errorDescription={description} />);

      // Assert
      expect(screen.getByText(description)).toBeInTheDocument();
    });

    it('renders with alert role for accessibility', () => {
      // Arrange & Act
      render(<ErrorMessage errorDescription="Error occurred" />);

      // Assert
      expect(screen.getByRole('alert')).toBeInTheDocument();
    });

    it('does not render a title when errorTitle is omitted', () => {
      // Arrange & Act
      render(<ErrorMessage errorDescription="Error details" />);

      // Assert — no heading-level element should appear
      expect(screen.queryByRole('heading')).not.toBeInTheDocument();
    });
  });

  describe('renders with optional title', () => {
    it('displays the error title when provided', () => {
      // Arrange
      const title = 'Connection Failed';
      const description = 'Could not reach the server';

      // Act
      render(<ErrorMessage errorTitle={title} errorDescription={description} />);

      // Assert
      expect(screen.getByText(title)).toBeInTheDocument();
    });

    it('displays both title and description together', () => {
      // Arrange
      const title = 'Health Check Failed';
      const description = 'Backend request failed: 503 Service Unavailable';

      // Act
      render(<ErrorMessage errorTitle={title} errorDescription={description} />);

      // Assert
      expect(screen.getByText(title)).toBeInTheDocument();
      expect(screen.getByText(description)).toBeInTheDocument();
    });
  });
});
