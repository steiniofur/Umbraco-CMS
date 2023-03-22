import {test} from '@umbraco/playwright-testhelpers';

test.describe('New test file description', () => {
  test.beforeEach(async ({page, umbracoApi}) => {
   
  });

  test('New test', async ({page, umbracoApi, umbracoUi}) => {
    // const tester =  await umbracoApi.dictionary.ensureDictionaryNameNotExists("string");
    // console.log(tester);
    //
    // await umbracoApi.dictionary.createDictionary('Ordd', "da-DK", "Word", "3629b983-7e87-4883-bfa4-f549bfe098db");
    
    
    const dictionary = await umbracoApi.dictionary.getDictionaryByKey("3629b983-7e87-4883-bfa4-f549bfe098db");
    console.log(dictionary);
    
    dictionary.translations.translation = "NewOne";
    
    await umbracoApi.dictionary.updateDictionaryByKey("3629b983-7e87-4883-bfa4-f549bfe098db", dictionary);
    
  });
  
});