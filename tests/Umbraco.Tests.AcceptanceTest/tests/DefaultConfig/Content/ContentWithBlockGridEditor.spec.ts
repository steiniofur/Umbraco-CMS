import {ConstantHelper, test} from '@umbraco/playwright-testhelpers';
import {expect} from "@playwright/test";


const documentName = 'TestContent';
const documentTypeName = 'TestDocumentTypeForContent';
const elementName = 'TestElement';
const elementDataType = 'Textstring';
const blockGridName = 'TestBlockGridEditor';
let textStringDataTypeId = '';
let blockGridDataTypeId = '';
let elementId = '';
let documentTypeId = '';
let documentId = '';

test.beforeEach(async ({umbracoApi}) => {
  await umbracoApi.documentType.ensureNameNotExists(documentTypeName);
  await umbracoApi.document.ensureNameNotExists(documentName);
  await umbracoApi.dataType.ensureNameNotExists(blockGridName);

  const textStringDataType = await umbracoApi.dataType.getByName(elementDataType);
  textStringDataTypeId = textStringDataType.id;
  elementId = await umbracoApi.documentType.createDefaultElementType(elementName, 'testGroup', elementDataType, textStringDataTypeId);
  blockGridDataTypeId = await umbracoApi.dataType.createBlockGridWithABlock(blockGridName, elementId);
});

test.afterEach(async ({umbracoApi}) => {
  await umbracoApi.document.ensureNameNotExists(documentName);
  await umbracoApi.documentType.ensureNameNotExists(documentTypeName);
  await umbracoApi.dataType.ensureNameNotExists(blockGridName);
});

test('can create content with block grid editor ', {tag: '@smoke'}, async ({page, umbracoApi, umbracoUi}) => {
  // Arrange
  const expectedState = 'Draft';
  documentTypeId = await umbracoApi.documentType.createDocumentTypeWithPropertyEditor(documentTypeName, blockGridName, blockGridDataTypeId);
  documentId  = await umbracoApi.document.createDefaultDocument(documentName, documentTypeId);
  await umbracoUi.goToBackOffice();
  await umbracoUi.content.goToSection(ConstantHelper.sections.content);

  // Act
  await page.pause();
  await umbracoUi.content.clickActionsMenuAtRoot();
  await umbracoUi.content.clickCreateButton();
  await umbracoUi.content.chooseDocumentType(documentTypeName);
  await umbracoUi.content.enterContentName(documentName);
  await umbracoUi.content.clickSaveButton();

});

