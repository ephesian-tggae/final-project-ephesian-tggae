import { expect, test } from '@playwright/test';
import { expectSignedOut, signIn, signOut } from './helpers/auth.js';
import { uniqueTitle } from './helpers/titles.js';

test('watchlist flow: add movie, verify persistence, log out', async ({ page }) => {
  const movieTitle = uniqueTitle('E2E Watchlist Movie');

  await signIn(page, { suffix: `watchlist-${Date.now()}` });

  await page.goto('/watchlist');
  await expect(page.getByRole('heading', { name: 'Watchlist' })).toBeVisible();

  await page.getByLabel('Movie title').fill(movieTitle);
  await page.getByLabel('Year (optional)').fill('2010');
  await page.getByRole('button', { name: 'Add to watchlist' }).click();

  await expect(page.getByText(movieTitle)).toBeVisible();

  await page.getByRole('button', { name: 'Reload from database' }).click();
  await expect(page.getByText(movieTitle)).toBeVisible();

  await page.goto('/');
  await page.getByRole('link', { name: 'Watchlist' }).click();
  await expect(page.getByText(movieTitle)).toBeVisible();

  await signOut(page);
  await expectSignedOut(page);
});
