import {ConstantHelper, NotificationConstantHelper, test} from '@umbraco/playwright-testhelpers';
import {expect} from "@playwright/test";


const contentName = 'TestContent';
const documentTypeName = 'TestDocumentType';
const elementName = 'TestElement';
const elementDataTypeName = 'Textstring';
const blockGridName = 'TestBlockGridEditor';
const propertyName = 'TextStringProperty'
const propertyValue = 'This is a test';
let textStringDataTypeId = '';
let blockGridDataTypeId = '';
let elementId = '';
let documentTypeId = '';

test.beforeEach(async ({umbracoApi}) => {
  await umbracoApi.documentType.ensureNameNotExists(documentTypeName);
  await umbracoApi.document.ensureNameNotExists(contentName);
  await umbracoApi.dataType.ensureNameNotExists(blockGridName);
  const textStringDataType = await umbracoApi.dataType.getByName(elementDataTypeName);
  textStringDataTypeId = textStringDataType.id;
  elementId = await umbracoApi.documentType.createDefaultElementType(elementName, 'testGroup', propertyName, textStringDataTypeId);
});

test.afterEach(async ({umbracoApi}) => {
  await umbracoApi.document.ensureNameNotExists(contentName);
  await umbracoApi.documentType.ensureNameNotExists(documentTypeName);
  await umbracoApi.dataType.ensureNameNotExists(blockGridName);
});

test('can create content with block grid editor', {tag: '@smoke'}, async ({umbracoApi, umbracoUi}) => {
  // Arrange
  blockGridDataTypeId = await umbracoApi.dataType.createBlockGridWithPermissions(blockGridName, elementId, true);
  documentTypeId = await umbracoApi.documentType.createDocumentTypeWithPropertyEditor(documentTypeName, elementName, blockGridDataTypeId);
  await umbracoUi.goToBackOffice();
  await umbracoUi.content.goToSection(ConstantHelper.sections.content);

  // Act
  await umbracoUi.content.clickActionsMenuAtRoot();
  await umbracoUi.content.clickCreateButton();
  await umbracoUi.content.chooseDocumentType(documentTypeName);
  await umbracoUi.content.enterContentName(contentName);
  await umbracoUi.content.clickSaveButton();

  // Assert
  await umbracoUi.content.isSuccessNotificationVisible();
  expect(await umbracoApi.document.doesNameExist(contentName)).toBeTruthy();
});

test('can create content with block grid editor with a block', {tag: '@smoke'}, async ({umbracoApi, umbracoUi}) => {
  // Arrange
  blockGridDataTypeId = await umbracoApi.dataType.createBlockGridWithPermissions(blockGridName, elementId, true);
  documentTypeId = await umbracoApi.documentType.createDocumentTypeWithPropertyEditor(documentTypeName, elementName, blockGridDataTypeId);
  await umbracoApi.document.createDefaultDocument(contentName, documentTypeId);
  await umbracoUi.goToBackOffice();
  await umbracoUi.content.goToSection(ConstantHelper.sections.content);

  // Act
  await umbracoUi.content.goToContentWithName(contentName);
  await umbracoUi.content.addBlockWithName(elementName);
  await umbracoUi.content.clickExactLinkWithName(elementName);
  await umbracoUi.content.enterTextForPropertyWithName(propertyName, propertyValue);
  await umbracoUi.content.clickCreateButton();
  await umbracoUi.content.clickSaveButton();

  // Assert
  await umbracoUi.content.doesSuccessNotificationHaveText(NotificationConstantHelper.success.saved);
  expect(await umbracoApi.document.doesNameExist(contentName)).toBeTruthy();
  // Checks if the block contains the correct value
  await umbracoUi.reloadPage();
  await umbracoUi.waitForTimeout(2000);
  await umbracoUi.content.goToBlockWithName(elementName, true);
  await umbracoUi.content.doesPropertyWithNameHaveText(propertyName, propertyValue);
});

//TODO: Missing builder for blockGrid in a document
test.skip('can update content with block grid editor with a block', {tag: '@smoke'}, async ({umbracoApi, umbracoUi}) => {
  // Arrange
  blockGridDataTypeId = await umbracoApi.dataType.createBlockGridWithAllowAtRootAndInlineEditing(blockGridName, elementId, true, true);
  documentTypeId = await umbracoApi.documentType.createDocumentTypeWithPropertyEditor(documentTypeName, elementName, blockGridDataTypeId);
  await umbracoApi.document.createDefaultDocument(contentName, documentTypeId);
  await umbracoUi.goToBackOffice();
  await umbracoUi.content.goToSection(ConstantHelper.sections.content);

  // Act
  await umbracoUi.content.goToContentWithName(contentName);
});
