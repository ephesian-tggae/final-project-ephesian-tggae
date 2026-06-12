import { expect, test } from '@playwright/test';
import { expectSignedOut, signIn, signOut } from './helpers/auth.js';
import { uniqueTitle } from './helpers/titles.js';

test('reviews flow: add review, verify persistence, log out', async ({ page }) => {
  const movieTitle = uniqueTitle('E2E Review Movie');
  const reviewText = 'A solid E2E test review.';

  await signIn(page, { suffix: `reviews-${Date.now()}` });

  await page.goto('/reviews');
  await expect(page.getByRole('heading', { name: 'Reviews' })).toBeVisible();

  await page.getByLabel('Movie title').fill(movieTitle);
  await page.getByLabel('Year (optional)').fill('2015');
  await page.getByLabel('Rating (1–5)').selectOption('4');
  await page.getByLabel('Review (optional)').fill(reviewText);
  await page.getByRole('button', { name: 'Add review' }).click();

  await expect(page.getByText(movieTitle)).toBeVisible();
  await expect(page.getByText(reviewText)).toBeVisible();

  await page.goto('/');
  await page.getByRole('link', { name: 'Reviews' }).click();
  await expect(page.getByText(movieTitle)).toBeVisible();
  await expect(page.getByText(reviewText)).toBeVisible();

  await signOut(page);
  await expectSignedOut(page);
});
