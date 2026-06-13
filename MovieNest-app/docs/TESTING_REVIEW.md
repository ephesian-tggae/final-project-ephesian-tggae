# AI-Generated Test Review

# Test 1 - brittle

it('returns an object', () => {
const result = validateWatchlistEntry({});
expect(typeof result).toBe('object');
});

# Test 2 - missing coverage

it('accepts a non-empty title', () => {
const result = validateWatchlistEntry({ title: 'Inception' });
expect(result).toBeTruthy();
});

# Test 3 - useful with edits

it('rejects out-of-range ratings', () => {
const result = validateReviewEntry({
title: 'Inception',
releaseYear: '2010',
rating: '6',
});
expect(result.rating).toBeDefined();
});

# Test 3 after

it('rejects out-of-range ratings', () => {
const result = validateReviewEntry({
title: 'Inception',
releaseYear: '2010',
rating: '6',
});
expect(result).toEqual({
rating: 'Choose a rating from 1 to 5.',
});
});

Explaination: Before the test would only check that some error exists, but now the test checks the exact error message the app should show. The reason for this was because the test could pass even if the message was wrong.

# Test 4 - useful as-is

it('rejects non-numeric year', () => {
expect(validateWatchlistEntry({ title: 'Test', releaseYear: 'abc' })).toEqual({
releaseYear: 'Enter a valid year (1888–2100).',
});
});

# Test 5 - Useful with edits

it('flags missing title', () => {
const result = validateWatchlistEntry({ title: ' ' });
expect(result.title).toBeTruthy();
});

# Test 5 after

it('flags missing title', () => {
const result = validateWatchlistEntry({ title: ' ' });
expect(result).toEqual({
title: 'Movie title is required.',
});
});

Explaination: Instead of only checking that something is in result.title, the test checks the exact message for a blank only title. The reason is because spaces only should count as no title.
