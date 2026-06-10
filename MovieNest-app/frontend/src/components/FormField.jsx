import { cloneElement } from 'react';

export default function FormField({
  label,
  htmlFor,
  error = null,
  required = false,
  children,
}) {
  const errorId = `${htmlFor}-error`;

  const control = cloneElement(children, {
    id: htmlFor,
    'aria-invalid': Boolean(error),
    'aria-describedby': error ? errorId : undefined,
  });

  return (
    <div className="form-field">
      <label htmlFor={htmlFor}>
        {label}
        {required && <span className="required-mark"> (required)</span>}
      </label>
      {control}
      {error && (
        <p id={errorId} className="field-error" role="alert">
          {error}
        </p>
      )}
    </div>
  );
}
