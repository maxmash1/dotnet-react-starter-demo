import { render, screen } from '@testing-library/react';
import { LoadingSpinner } from './LoadingSpinner';

describe('LoadingSpinner', () => {
  it('renders with status role for accessibility', () => {
    // Arrange & Act
    render(<LoadingSpinner />);

    // Assert
    expect(screen.getByRole('status')).toBeInTheDocument();
  });

  it('has the accessible label "Loading content"', () => {
    // Arrange & Act
    render(<LoadingSpinner />);

    // Assert
    expect(screen.getByLabelText('Loading content')).toBeInTheDocument();
  });

  it('renders loading text visible to assistive technologies', () => {
    // Arrange & Act
    render(<LoadingSpinner />);

    // Assert
    expect(screen.getByText('Loading data...')).toBeInTheDocument();
  });

  it('renders a decorative spinner element hidden from screen readers', () => {
    // Arrange & Act
    const { container } = render(<LoadingSpinner />);

    // Assert — the spinning div carries aria-hidden="true"
    const spinnerDiv = container.querySelector('[aria-hidden="true"]');
    expect(spinnerDiv).toBeInTheDocument();
  });
});
