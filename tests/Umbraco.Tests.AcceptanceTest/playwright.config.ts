import {defineConfig, devices} from '@playwright/test';
import * as path from "path";

require('dotenv').config();

export const STORAGE_STATE = path.join(__dirname, 'playwright/.auth/user.json');

export default defineConfig({
  testDir: './tests/',
  /* Maximum time one test can run for. */
  timeout: 30 * 1000,
  expect: {
    /**
     * Maximum time expect() should wait for the condition to be met.
     * For example in `await expect(locator).toHaveText();`
     */
    timeout: 5000
  },
  /* Fail the build on CI if you accidentally left test.only in the source code. */
  forbidOnly: !!process.env.CI,
  /* Retry on CI only */
  retries: 2,
  // We don't want to run parallel, as tests might differ in state
  workers: 1,
  /* Reporter to use. See https://playwright.dev/docs/test-reporters */
  reporter: process.env.CI ? 'line' : 'html',
  outputDir: "./results",
  /* Shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
  use: {
    /* Maximum time each action such as `click()` can take. Defaults to 0 (no limit). */
    actionTimeout: 0,
    // When working locally it can be a good idea to use trace: 'on-first-retry' instead of 'retain-on-failure', it can cut the local test times in half.
    trace: 'retain-on-failure',
    ignoreHTTPSErrors: true,
  },

  /* Configure projects for major browsers */
  projects: [
    // Setup project
    {
      name: 'setup',
      testMatch: '**/*.setup.ts',
    },
    {
      name: 'defaultConfig',
      testMatch: 'DefaultConfig/**',
      dependencies: ['setup'],
      use: {
        ...devices['Desktop Chrome'],
        // Use prepared auth state.
        ignoreHTTPSErrors: true,
        storageState: STORAGE_STATE
      }
    },
    // This project is used to test the install steps, for that we do not need to authenticate.
    {
      name: 'unattendedInstallConfig',
      testMatch: 'UnattendedInstallConfig/**',
      use: {
        ...devices['Desktop Chrome']
      }
    }
  ],
});
