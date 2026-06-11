export default function StatusMessage({ type = 'status', message, className = '' }) {
  if (!message) {
    return null;
  }

  const role = type === 'error' ? 'alert' : 'status';
  const ariaLive = type === 'error' ? 'assertive' : 'polite';
  const typeClass =
    type === 'error' ? 'error' : type === 'success' ? 'success' : '';

  return (
    <p
      role={role}
      aria-live={ariaLive}
      className={[typeClass, className].filter(Boolean).join(' ')}
    >
      {message}
    </p>
  );
}
