import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import StatusMessage from './StatusMessage';

describe('StatusMessage', () => {
  it('shows a message', () => {
    render(<StatusMessage message="Saved to your watchlist." />);

    expect(screen.getByText('Saved to your watchlist.')).toBeInTheDocument();
  });

  it('uses alert behavior for errors', () => {
    render(<StatusMessage type="error" message="Something went wrong." />);

    expect(screen.getByRole('alert')).toHaveTextContent('Something went wrong.');
  });
});
