import { useState } from 'react';
import FormField from './FormField';

export default function SearchForm({
  query,
  onQueryChange,
  onSubmit,
  loading = false,
  submitLabel = 'Search',
  loadingLabel = 'Searching…',
  inputLabel = 'Movie title',
  inputId = 'search-query',
  queryError = null,
  showPopularButton = false,
  onShowPopular,
  popularDisabled = false,
  allowEmptySubmit = false,
}) {
  const [validationError, setValidationError] = useState(null);
  const displayError = queryError ?? validationError;

  function handleQueryChange(event) {
    setValidationError(null);
    onQueryChange(event);
  }

  function handleSubmit(event) {
    event.preventDefault();

    if (!query.trim() && !allowEmptySubmit) {
      setValidationError('Enter a movie title to search.');
      return;
    }

    setValidationError(null);
    onSubmit(event);
  }

  return (
    <form className="search-form" onSubmit={handleSubmit} noValidate>
      <FormField
        label={inputLabel}
        htmlFor={inputId}
        error={displayError}
      >
        <input
          type="text"
          value={query}
          onChange={handleQueryChange}
          placeholder="e.g. Inception"
        />
      </FormField>
      <div className={showPopularButton ? 'discover-actions' : undefined}>
        <button type="submit" disabled={loading}>
          {loading ? loadingLabel : submitLabel}
        </button>
        {showPopularButton && (
          <button
            type="button"
            onClick={onShowPopular}
            disabled={popularDisabled || loading}
          >
            Show popular
          </button>
        )}
      </div>
    </form>
  );
}
