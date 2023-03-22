import {test} from '@umbraco/playwright-testhelpers';
import {expect} from "@playwright/test";

test.describe('Telemetry tests', () => {

  test.beforeEach(async ({ page, umbracoApi }, testInfo) => {
    await umbracoApi.telemetry.setTelemetryLevel("Basic");
  });

  test('can change telemetry level', async ({page, umbracoApi, umbracoUi}) => {
    await page.pause();
    
    const expectedLevel = "Minimal";
    
    await page.goto(umbracoApi.baseUrl + '/umbraco');
    
    // Selects minimal as the telemetry level 
    await page.locator('[label="Settings"]').click();
    await page.locator('[label="Telemetry Data"]').click();
    await page.locator('[name="telemetryLevel"] >> input[id=input]').fill('1');
    await page.locator('[label="Save telemetry settings"]').click();
    
    // Assert
    // Checks if minimal is the set telemetry level in the api
    await expect(await umbracoApi.telemetry.checkTelemetryLevel(expectedLevel)).toBeTruthy();
  });
});