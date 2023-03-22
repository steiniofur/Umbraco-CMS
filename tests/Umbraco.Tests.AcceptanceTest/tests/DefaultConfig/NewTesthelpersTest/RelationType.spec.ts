import {test} from '@umbraco/playwright-testhelpers';
import {expect} from "@playwright/test";

test.describe('New test file description', () => {

  const relationTypeKey = '9f93e414-5abe-36ec-a6bb-516e56f12258';
  const parentTypeKey = 'c66ba18e-eaf3-4cff-8a22-41b16d66a972';
  const childTypeKey = 'c66ba18e-eaf3-4cff-8a22-41b16d66a972';

  test.beforeEach(async ({page, umbracoApi}) => {
    await umbracoApi.relationType.ensureRelationTypeNotExists(relationTypeKey);
   });

  test.afterEach(async ({page, umbracoApi}) => {
    await umbracoApi.relationType.ensureRelationTypeNotExists(relationTypeKey);
  });
  
  test('can create a new relation type', async ({page, umbracoApi, umbracoUi}) => {
    await umbracoApi.relationType.createRelationType('Tester', relationTypeKey, false, true, parentTypeKey, childTypeKey);

    // Assert
    // Checks if the relationType was created
    await expect(umbracoApi.relationType.getRelationTypeByKey(relationTypeKey)).toBeTruthy();
  });

  test('can update name of relation type', async ({page, umbracoApi, umbracoUi}) => {
    await umbracoApi.relationType.createRelationType('Tester', relationTypeKey, false, true, parentTypeKey, childTypeKey);

    const updatedName = 'TestRelationTypeName';

    // Updates the name of the relationType
    const relationType = await umbracoApi.relationType.getRelationTypeByKey(relationTypeKey);
    relationType.name = updatedName;
    await umbracoApi.relationType.updateRelationType(relationTypeKey, relationType);
  
    // Assert
    const updatedRelationType = await umbracoApi.relationType.getRelationTypeByKey(relationTypeKey);
    // Checks if the updated relationType contains the update name
    await expect(updatedRelationType.name == updatedName).toBeTruthy();
  });

  test('can delete a relation type', async ({page, umbracoApi, umbracoUi}) => {
    await umbracoApi.relationType.createRelationType('Tester', relationTypeKey, false, true, parentTypeKey, childTypeKey);

    // Deletes the relationType
    await umbracoApi.relationType.deleteRelationType(relationTypeKey);
    
    // Assert
    // Checks if the relationType was actually deleted
    const doesExist = await umbracoApi.relationType.doesRelationTypeWithKeyExist(relationTypeKey);
    await expect(doesExist).not.toBeTruthy();
  });
});