import { describe, expect, it } from 'vitest';
import { aggregateGenreCounts, computeAverageRating } from './dashboardStats';

describe('aggregateGenreCounts', () => {
  it('counts genres across watchlist and history items', () => {
    const watchlist = [
      { genres: [{ name: 'Drama' }, { name: 'Sci-Fi' }] },
      { genres: [{ name: 'Drama' }] },
    ];
    const history = [{ genres: [{ name: 'Sci-Fi' }] }];

    expect(aggregateGenreCounts([watchlist, history])).toEqual([
      { name: 'Drama', count: 2 },
      { name: 'Sci-Fi', count: 2 },
    ]);
  });
});

describe('computeAverageRating', () => {
  it('returns the rounded average rating', () => {
    const reviews = [{ rating: 4 }, { rating: 5 }, { rating: 3 }];

    expect(computeAverageRating(reviews)).toBe(4);
  });

  it('returns null when there are no reviews', () => {
    expect(computeAverageRating([])).toBeNull();
    expect(computeAverageRating(null)).toBeNull();
  });
});
