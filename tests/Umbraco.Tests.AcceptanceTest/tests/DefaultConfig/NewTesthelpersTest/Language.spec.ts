import {test} from '@umbraco/playwright-testhelpers';
import {expect} from "@playwright/test";

test.describe('New test file description', () => {
  
  const isoCodeDanish = "da-Dk"
  
  test.beforeEach(async ({page, umbracoApi}) => {
    await umbracoApi.language.ensureIsoCodeNotExists(isoCodeDanish);
  });
  
  test.afterEach(async ({page, umbracoApi}) => {
    await umbracoApi.language.ensureIsoCodeNotExists(isoCodeDanish);
  });

  test('can create new language', async ({page, umbracoApi, umbracoUi}) => {
    
    // Creates the language
    await umbracoApi.language.createLanguage("Dansk", false, false, isoCodeDanish);
    
    // Asserts that the language exists
    await expect(await umbracoApi.language.doesLanguageWithIsoCodeExist(isoCodeDanish)).toBeTruthy();
  });

  test('can update language', async ({page, umbracoApi, umbracoUi}) => {
    // Gets the language object
    const Lang = await umbracoApi.language.getLanguageByIsoCode(isoCodeDanish);
    console.log(Lang);
    
    // Updates the name of the language
    Lang.name = "Test";
    await umbracoApi.language.updateLanguageWithIsoCode(isoCodeDanish, Lang);
    const Lang2 = await umbracoApi.language.getLanguageByIsoCode(isoCodeDanish);
    console.log(Lang2);
    
    // Assert that the language still exists
    await expect(await umbracoApi.language.doesLanguageWithIsoCodeExist(isoCodeDanish)).toBeTruthy();
    
    
  });
});