import { ConstantHelper, test, AliasHelper } from '@umbraco/playwright-testhelpers';
import {expect} from "@playwright/test";

const dataTypeNames = ['Date Picker', 'Date Picker with time'];
for (const dataTypeName of dataTypeNames) {
  test.describe(`Content with the ${dataTypeName} tests`, () => {
    const contentName = 'TestDatePickerContent';
    const documentTypeName = 'TestDocumentTypeForContent';
    const date = dataTypeName === 'Date Picker' ? '2024-12-24' : '2024-12-24T10:00:59';
    const expectedDate = dataTypeName === 'Date Picker' ? '2024-12-24' : '2024-12-24 10:00:59';

    test.beforeEach(async ({umbracoApi, umbracoUi}) => {
      await umbracoApi.documentType.ensureNameNotExists(documentTypeName);
      await umbracoApi.document.ensureNameNotExists(contentName);
      await umbracoUi.goToBackOffice();
    });

    test.afterEach(async ({umbracoApi}) => {
      await umbracoApi.document.ensureNameNotExists(contentName); 
      await umbracoApi.documentType.ensureNameNotExists(documentTypeName);
    });

    test(`can create content with the ${dataTypeName} data type`, async ({umbracoApi, umbracoUi}) => {
      // Arrange
      const dataTypeData = await umbracoApi.dataType.getByName(dataTypeName);
      await umbracoApi.documentType.createDocumentTypeWithPropertyEditor(documentTypeName, dataTypeName, dataTypeData.id);
      await umbracoUi.content.goToSection(ConstantHelper.sections.content);

      // Act
      await umbracoUi.content.clickActionsMenuAtRoot();
      await umbracoUi.content.clickCreateButton();
      await umbracoUi.content.chooseDocumentType(documentTypeName);
      await umbracoUi.content.enterContentName(contentName);
      await umbracoUi.content.enterADate(date);
      await umbracoUi.content.clickSaveButton();

      // Assert
      await umbracoUi.content.isSuccessNotificationVisible();
      expect(await umbracoApi.document.doesNameExist(contentName)).toBeTruthy();
      const contentData = await umbracoApi.document.getByName(contentName);
      expect(contentData.values[0].alias).toEqual(AliasHelper.toAlias(dataTypeName));
      expect(contentData.values[0].value).toEqual(expectedDate);
    });

    test(`can publish content with the ${dataTypeName} data type`, async ({umbracoApi, umbracoUi}) => {
      // Arrange
      const dataTypeData = await umbracoApi.dataType.getByName(dataTypeName);
      await umbracoApi.documentType.createDocumentTypeWithPropertyEditor(documentTypeName, dataTypeName, dataTypeData.id);
      await umbracoUi.content.goToSection(ConstantHelper.sections.content);

      // Act
      await umbracoUi.content.clickActionsMenuAtRoot();
      await umbracoUi.content.clickCreateButton();
      await umbracoUi.content.chooseDocumentType(documentTypeName);
      await umbracoUi.content.enterContentName(contentName);
      await umbracoUi.content.enterADate(date);
      await umbracoUi.content.clickSaveAndPublishButton();

      // Assert
      await umbracoUi.content.doesSuccessNotificationsHaveCount(2);
      expect(await umbracoApi.document.doesNameExist(contentName)).toBeTruthy();
      const contentData = await umbracoApi.document.getByName(contentName);
      expect(contentData.values[0].alias).toEqual(AliasHelper.toAlias(dataTypeName));
      expect(contentData.values[0].value).toEqual(expectedDate);
    });

    test(`can create content with the custom ${dataTypeName} data type`, async ({umbracoApi, umbracoUi}) => {
      // Arrange
      const customDataTypeName = 'CustomDatePicker';
      const customDateFormat = dataTypeName === 'Date Picker' ? 'DD-MM-YYYY' : "DD-MM-YYYY hh:mm:ss";
      const expectedCustomDate = dataTypeName === 'Date Picker' ? '24-12-2024' : '24-12-2024 10:00:59';
      const customDataTypeId = await umbracoApi.dataType.createDatePickerDataType(customDataTypeName, customDateFormat);
      await umbracoApi.documentType.createDocumentTypeWithPropertyEditor(documentTypeName, customDataTypeName, customDataTypeId);
      await umbracoUi.content.goToSection(ConstantHelper.sections.content);

      // Act
      await umbracoUi.content.clickActionsMenuAtRoot();
      await umbracoUi.content.clickCreateButton();
      await umbracoUi.content.chooseDocumentType(documentTypeName);
      await umbracoUi.content.enterContentName(contentName);
      await umbracoUi.content.enterADate(date);
      await umbracoUi.content.clickSaveAndPublishButton();

      // Assert
      await umbracoUi.content.doesSuccessNotificationsHaveCount(2);
      expect(await umbracoApi.document.doesNameExist(contentName)).toBeTruthy();
      const contentData = await umbracoApi.document.getByName(contentName);
      expect(contentData.values[0].alias).toEqual(AliasHelper.toAlias(customDataTypeName));
      expect(contentData.values[0].value).toEqual(expectedCustomDate);

      // Clean
      await umbracoApi.dataType.ensureNameNotExists(customDataTypeName);
    });
  });
}
