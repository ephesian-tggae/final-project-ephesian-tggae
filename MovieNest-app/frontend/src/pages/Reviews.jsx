import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { createReview, deleteReview, fetchReviews, updateReview } from '../api';
import GenreTags from '../components/GenreTags';
import MovieEntryForm from '../components/MovieEntryForm';
import TmdbAttribution from '../components/TmdbAttribution';
import TmdbImageCredit from '../components/TmdbImageCredit';
import UserDataNote from '../components/UserDataNote';
import { validateReviewEntry } from '../utils/validateMovieEntry';

export default function Reviews() {
  const [items, setItems] = useState([]);
  const [title, setTitle] = useState('');
  const [releaseYear, setReleaseYear] = useState('');
  const [rating, setRating] = useState('5');
  const [text, setText] = useState('');
  const [fieldErrors, setFieldErrors] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [saving, setSaving] = useState(false);
  const [editingId, setEditingId] = useState(null);
  const [editRating, setEditRating] = useState('5');
  const [editText, setEditText] = useState('');
  const [updatingId, setUpdatingId] = useState(null);
  const [removingId, setRemovingId] = useState(null);

  async function loadReviews() {
    const data = await fetchReviews();
    if (data === null) {
      setError('Not signed in');
      setItems([]);
    } else {
      setItems(data);
      setError(null);
    }
    return data;
  }

  useEffect(() => {
    loadReviews()
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  function handleChange(field, value) {
    if (field === 'title') {
      setTitle(value);
    } else if (field === 'releaseYear') {
      setReleaseYear(value);
    } else if (field === 'rating') {
      setRating(value);
    } else if (field === 'text') {
      setText(value);
    }

    if (fieldErrors[field]) {
      setFieldErrors((prev) => {
        const next = { ...prev };
        delete next[field];
        return next;
      });
    }
  }

  async function handleSubmit(event) {
    event.preventDefault();

    const values = { title, releaseYear, rating, text };
    const errors = validateReviewEntry(values);
    if (Object.keys(errors).length > 0) {
      setFieldErrors(errors);
      return;
    }

    setFieldErrors({});
    setSaving(true);
    setError(null);

    try {
      const year = releaseYear ? parseInt(releaseYear, 10) : null;
      await createReview(title.trim(), parseInt(rating, 10), text.trim() || null, year);
      setTitle('');
      setReleaseYear('');
      setRating('5');
      setText('');
      setLoading(true);
      await loadReviews();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
      setLoading(false);
    }
  }

  function startEdit(item) {
    setEditingId(item.id);
    setEditRating(String(item.rating));
    setEditText(item.text || '');
    setError(null);
  }

  function cancelEdit() {
    setEditingId(null);
  }

  async function handleUpdate(id) {
    setUpdatingId(id);
    setError(null);

    try {
      await updateReview(id, parseInt(editRating, 10), editText.trim() || null);
      setEditingId(null);
      await loadReviews();
    } catch (err) {
      setError(err.message);
    } finally {
      setUpdatingId(null);
    }
  }

  async function handleRemove(id) {
    setRemovingId(id);
    setError(null);

    try {
      await deleteReview(id);
      if (editingId === id) {
        setEditingId(null);
      }
      await loadReviews();
    } catch (err) {
      setError(err.message);
    } finally {
      setRemovingId(null);
    }
  }

  return (
    <main className="page">
      <h1>Reviews</h1>
      <UserDataNote context="reviews" />
      <TmdbAttribution />

      <MovieEntryForm
        mode="review"
        values={{ title, releaseYear, rating, text }}
        errors={fieldErrors}
        onChange={handleChange}
        onSubmit={handleSubmit}
        submitting={saving}
        submitLabel="Add review"
        submittingLabel="Saving…"
      />

      {loading && <p>Loading your reviews…</p>}
      {error && <p className="error">{error}</p>}

      {!loading && items.length > 0 && (
        <>
        <TmdbImageCredit />
        <ul className="watchlist-items">
          {items.map((item) => (
            <li key={item.id} className="watchlist-item">
              {item.posterUrl ? (
                <img
                  src={item.posterUrl}
                  alt={`${item.title} poster`}
                  className="search-poster"
                />
              ) : (
                <div className="search-poster search-poster--empty">No poster</div>
              )}
              <div className="watchlist-item-details">
                <strong>{item.title}</strong>
                {item.releaseYear && <span> ({item.releaseYear})</span>}
                <GenreTags genres={item.genres} />
                <span className="meta">Your rating: {item.rating} / 5</span>
                {editingId === item.id ? (
                  <div className="review-edit">
                    <label>
                      Rating
                      <select
                        value={editRating}
                        onChange={(e) => setEditRating(e.target.value)}
                      >
                        <option value="1">1</option>
                        <option value="2">2</option>
                        <option value="3">3</option>
                        <option value="4">4</option>
                        <option value="5">5</option>
                      </select>
                    </label>
                    <label>
                      Review
                      <textarea
                        value={editText}
                        onChange={(e) => setEditText(e.target.value)}
                        rows={3}
                      />
                    </label>
                    <button
                      type="button"
                      onClick={() => handleUpdate(item.id)}
                      disabled={updatingId === item.id}
                    >
                      {updatingId === item.id ? 'Saving…' : 'Save'}
                    </button>
                    <button type="button" onClick={cancelEdit}>
                      Cancel
                    </button>
                  </div>
                ) : (
                  <>
                    {item.text && (
                      <>
                        <span className="meta">Your review:</span>
                        <p className="review-text">{item.text}</p>
                      </>
                    )}
                    <span className="meta">
                      Posted {new Date(item.createdAt).toLocaleString()}
                      {item.updatedAt !== item.createdAt &&
                        ` · updated ${new Date(item.updatedAt).toLocaleString()}`}
                    </span>
                    <button
                      type="button"
                      onClick={() => startEdit(item)}
                      disabled={removingId === item.id}
                    >
                      Edit
                    </button>
                    <button
                      type="button"
                      onClick={() => handleRemove(item.id)}
                      disabled={removingId === item.id || updatingId === item.id}
                    >
                      {removingId === item.id ? 'Removing…' : 'Delete'}
                    </button>
                  </>
                )}
              </div>
            </li>
          ))}
        </ul>
        </>
      )}

      {!loading && !error && items.length === 0 && (
        <p>No reviews yet.</p>
      )}

      <Link to="/">← Back to home</Link>
    </main>
  );
}
