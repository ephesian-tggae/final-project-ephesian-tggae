import { describe, expect, it } from 'vitest';
import { validateReviewEntry, validateWatchlistEntry } from './validateMovieEntry';

describe('validateWatchlistEntry', () => {
  it('requires a title and rejects invalid years', () => {
    expect(validateWatchlistEntry({ title: '', releaseYear: '' })).toEqual({
      title: 'Movie title is required.',
    });

    expect(
      validateWatchlistEntry({ title: 'Inception', releaseYear: '1800' }),
    ).toEqual({
      releaseYear: 'Enter a valid year (1888–2100).',
    });
  });
});

describe('validateReviewEntry', () => {
  it('requires a rating from 1 to 5', () => {
    expect(
      validateReviewEntry({
        title: 'Inception',
        releaseYear: '2010',
        rating: '',
      }),
    ).toEqual({
      rating: 'Choose a rating from 1 to 5.',
    });
  });
});
