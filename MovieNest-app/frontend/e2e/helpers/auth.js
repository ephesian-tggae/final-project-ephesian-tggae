import { expect } from '@playwright/test';

export const apiBaseUrl = process.env.PLAYWRIGHT_API_URL ?? 'http://localhost:5102';

export async function signIn(page, options = {}) {
  const suffix = options.suffix ?? Date.now();
  const subjectId = options.subjectId ?? `e2e:user:${suffix}`;
  const email = options.email ?? `e2e-${suffix}@movienest.local`;
  const name = options.name ?? 'E2E Test User';

  const response = await page.request.post(`${apiBaseUrl}/api/auth/e2e-login`, {
    data: { subjectId, email, name },
  });

  expect(response.ok()).toBeTruthy();

  await page.goto('/');
  await expect(page.getByRole('button', { name: 'Log out' })).toBeVisible();

  return { subjectId, email, name };
}

export async function signOut(page) {
  await page.goto('/');
  await page.getByRole('button', { name: 'Log out' }).click();
  await expect(page.getByRole('button', { name: 'Log in with Google' })).toBeVisible();
}

export async function expectSignedOut(page) {
  await page.goto('/watchlist');
  await expect(page).toHaveURL('/');
  await page.goto('/');
  await expect(page.getByRole('button', { name: 'Log in with Google' })).toBeVisible();
}
