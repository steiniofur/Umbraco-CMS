import {test} from '@umbraco/playwright-testhelpers';
import {expect} from "@playwright/test";

test.describe('Relation Type tests', () => {
  const relationTypeName = 'Tester';
  const parentTypeId = 'c66ba18e-eaf3-4cff-8a22-41b16d66a972';
  const childTypeId = 'c66ba18e-eaf3-4cff-8a22-41b16d66a972';

  test.beforeEach(async ({page, umbracoApi}) => {
    await umbracoApi.relationType.ensureRelationTypeNameNotExistsAtRoot(relationTypeName);
  });

  test.afterEach(async ({page, umbracoApi}) => {
    await umbracoApi.relationType.ensureRelationTypeNameNotExistsAtRoot(relationTypeName);
  });

  test('can create a new relation type', async ({page, umbracoApi, umbracoUi}) => {
    await umbracoApi.relationType.createRelationType(relationTypeName, null, false, true, parentTypeId, childTypeId);

    // Assert
    await expect(umbracoApi.relationType.doesRelationTypeWithNameExistAtRoot(relationTypeName)).toBeTruthy();
  });

  test('can update name of relation type', async ({page, umbracoApi, umbracoUi}) => {
    await umbracoApi.relationType.createRelationType('WrongName', null, false, true, parentTypeId, childTypeId);

    const relationType = await umbracoApi.relationType.getRelationTypeByNameAtRoot('WrongName');

    // Updates the name of the relationType
    relationType.name = relationTypeName;
    await umbracoApi.relationType.updateRelationType(relationType.id, relationType);

    // Assert
    // Checks if the updated relationType contains the update name
    const updatedRelationType = await umbracoApi.relationType.getRelationTypeById(relationType.id);
    await expect(updatedRelationType.name == relationTypeName).toBeTruthy();
  });

  test('can delete a relation type', async ({page, umbracoApi, umbracoUi}) => {
    await umbracoApi.relationType.createRelationType(relationTypeName, null, false, true, parentTypeId, childTypeId);

    const relationType = await umbracoApi.relationType.getRelationTypeByNameAtRoot(relationTypeName);

    await umbracoApi.relationType.deleteRelationType(relationType.id);

    // Assert
    await expect(await umbracoApi.relationType.doesRelationTypeWithNameExistAtRoot(relationTypeName)).toBeFalsy();
  });

  test('can get relation type items', async ({page, umbracoApi, umbracoUi}) => {
    const anotherRelationTypeName = 'AnotherRelationType';

    await umbracoApi.relationType.ensureRelationTypeNameNotExistsAtRoot(anotherRelationTypeName);

    await umbracoApi.relationType.createRelationType(relationTypeName, null, false, true, parentTypeId, childTypeId);

    await umbracoApi.relationType.createRelationType(anotherRelationTypeName, null, false, true, parentTypeId, childTypeId);

    // Gets the first created relation type
    const relationType = await umbracoApi.relationType.getRelationTypeByNameAtRoot(relationTypeName);

    // Gets another created relation type
    const anotherRelationType = await umbracoApi.relationType.getRelationTypeByNameAtRoot(anotherRelationTypeName);

    const items = [relationType.id, anotherRelationType.id];

    // Gets the relation type items
    const getITheItems = await umbracoApi.relationType.getRelationTypeItems(items);

    // Assert
    // Checks if the items contain the correct names;
    await expect(getITheItems[0].name == relationTypeName && getITheItems[1].name == anotherRelationTypeName).toBeTruthy();

    // Clean
    await umbracoApi.relationType.ensureRelationTypeNameNotExistsAtRoot(anotherRelationTypeName);
  });
});
