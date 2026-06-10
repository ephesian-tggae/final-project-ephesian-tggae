import FormField from './FormField';

export default function MovieEntryForm({
  mode,
  values,
  errors = {},
  onChange,
  onSubmit,
  submitting = false,
  submitLabel,
  submittingLabel = 'Saving…',
}) {
  return (
    <form className="watchlist-form" onSubmit={onSubmit} noValidate>
      <FormField
        label="Movie title"
        htmlFor="movie-title"
        error={errors.title}
        required
      >
        <input
          type="text"
          value={values.title}
          onChange={(e) => onChange('title', e.target.value)}
          placeholder="e.g. Inception"
        />
      </FormField>

      <FormField
        label="Year (optional)"
        htmlFor="movie-year"
        error={errors.releaseYear}
      >
        <input
          type="number"
          value={values.releaseYear}
          onChange={(e) => onChange('releaseYear', e.target.value)}
          placeholder="2010"
        />
      </FormField>

      {mode === 'review' && (
        <>
          <FormField
            label="Rating (1–5)"
            htmlFor="review-rating"
            error={errors.rating}
            required
          >
            <select
              value={values.rating}
              onChange={(e) => onChange('rating', e.target.value)}
            >
              <option value="1">1</option>
              <option value="2">2</option>
              <option value="3">3</option>
              <option value="4">4</option>
              <option value="5">5</option>
            </select>
          </FormField>

          <FormField label="Review (optional)" htmlFor="review-text">
            <textarea
              value={values.text}
              onChange={(e) => onChange('text', e.target.value)}
              placeholder="What did you think?"
              rows={3}
            />
          </FormField>
        </>
      )}

      <button type="submit" disabled={submitting}>
        {submitting ? submittingLabel : submitLabel}
      </button>
    </form>
  );
}
