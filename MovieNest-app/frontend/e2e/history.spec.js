import { expect, test } from '@playwright/test';
import { signIn, signOut } from './helpers/auth.js';
import { uniqueTitle } from './helpers/titles.js';

test('watchlist to history flow: mark watched and verify on history page', async ({ page }) => {
  const movieTitle = uniqueTitle('E2E History Movie');

  await signIn(page, { suffix: `history-${Date.now()}` });

  await page.goto('/watchlist');
  await page.getByLabel('Movie title').fill(movieTitle);
  await page.getByLabel('Year (optional)').fill('2012');
  await page.getByRole('button', { name: 'Add to watchlist' }).click();
  await expect(page.getByText(movieTitle)).toBeVisible();

  await page.getByRole('button', { name: 'Mark as watched' }).click();
  await expect(page.getByText('No movies yet. Add one above.')).toBeVisible();

  await page.goto('/history');
  await expect(page.getByRole('heading', { name: 'Watched history' })).toBeVisible();
  await expect(page.getByText(movieTitle)).toBeVisible();

  await signOut(page);
  await expect(page.getByRole('button', { name: 'Log in with Google' })).toBeVisible();
});
