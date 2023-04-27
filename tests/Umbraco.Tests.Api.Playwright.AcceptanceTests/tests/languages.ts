import { test, expect } from '@playwright/test';

test('has title', async ({ request }) => {
  const testResult = await request.get(`https://localhost:44331/umbraco/management/api/v1/language/en-US`);
});
