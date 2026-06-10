const MIN_YEAR = 1888;
const MAX_YEAR = 2100;

function validateYear(releaseYear) {
  if (!releaseYear || String(releaseYear).trim() === '') {
    return null;
  }

  const year = parseInt(releaseYear, 10);
  if (Number.isNaN(year) || year < MIN_YEAR || year > MAX_YEAR) {
    return 'Enter a valid year (1888–2100).';
  }

  return null;
}

export function validateWatchlistEntry(values) {
  const errors = {};

  if (!values.title?.trim()) {
    errors.title = 'Movie title is required.';
  }

  const yearError = validateYear(values.releaseYear);
  if (yearError) {
    errors.releaseYear = yearError;
  }

  return errors;
}

export function validateReviewEntry(values) {
  const errors = validateWatchlistEntry(values);

  const rating = parseInt(values.rating, 10);
  if (!values.rating || Number.isNaN(rating) || rating < 1 || rating > 5) {
    errors.rating = 'Choose a rating from 1 to 5.';
  }

  return errors;
}
