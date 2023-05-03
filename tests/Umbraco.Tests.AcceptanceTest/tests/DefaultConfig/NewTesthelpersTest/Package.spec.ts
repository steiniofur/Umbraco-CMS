import {test} from '@umbraco/playwright-testhelpers';
import {expect} from "@playwright/test";

test.describe('Package tests', () => {
  const packageName = 'packageTestName';

  test.beforeEach(async ({page, umbracoApi}) => {
    await umbracoApi.package.ensurePackageNameNotExists(packageName);
  });

  test.afterEach(async ({page, umbracoApi}) => {
    await umbracoApi.package.ensurePackageNameNotExists(packageName);
  });

  test('can create new package', async ({page, umbracoApi, umbracoUi}) => {
    await umbracoApi.package.createPackage(packageName);

    // Assert
    await umbracoApi.package.doesPackageWithNameExist(packageName);
  });

  test('can update a package', async ({page, umbracoApi, umbracoUi}) => {
    const packageTypes = ["theForgottenDoc"];

    await umbracoApi.package.createPackage(packageName, true, true);

    const packageData = await umbracoApi.package.getPackageByName(packageName);

    packageData.documentTypes = packageTypes;
    await umbracoApi.package.updatePackageById(packageData.id, packageData);

    const updatedPackageData = await umbracoApi.package.getPackageByName(packageName);

    // Assert
    // Checks if the updated value is correct
    await expect(updatedPackageData.documentTypes[0] == packageTypes[0]).toBeTruthy();
  });

  test('can delete a package', async ({page, umbracoApi, umbracoUi}) => {
    await umbracoApi.package.createPackage(packageName, true, true);

    const packageData = await umbracoApi.package.getPackageByName(packageName);

    await expect(await umbracoApi.package.doesPackageWithNameExist(packageName)).toBeTruthy();

    await umbracoApi.package.deletePackageById(packageData.id);

    // Assert
    await expect(await umbracoApi.package.doesPackageWithNameExist(packageName)).toBeFalsy();
  });

  test('can download a package', async ({page, umbracoApi, umbracoUi}) => {
    await umbracoApi.package.createPackage(packageName, true, true);

    const packageData = await umbracoApi.package.getPackageByName(packageName);

    const downloadedPackage = await umbracoApi.package.downloadPackageById(packageData.id);

    // Assert
    // Checks if the downloaded package contains the name defined in the created package
    await expect(downloadedPackage.includes(packageName)).toBeTruthy();
  });
});
