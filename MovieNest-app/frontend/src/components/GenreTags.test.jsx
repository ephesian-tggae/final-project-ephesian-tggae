import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import GenreTags from './GenreTags';

describe('GenreTags', () => {
  it('shows genre names', () => {
    render(
      <GenreTags
        genres={[
          { tmdbGenreId: 18, name: 'Drama' },
          { tmdbGenreId: 878, name: 'Sci-Fi' },
        ]}
      />,
    );

    expect(screen.getByText('Drama')).toBeInTheDocument();
    expect(screen.getByText('Sci-Fi')).toBeInTheDocument();
  });

  it('shows nothing when there are no genres', () => {
    const { container } = render(<GenreTags genres={[]} />);

    expect(container).toBeEmptyDOMElement();
  });
});
