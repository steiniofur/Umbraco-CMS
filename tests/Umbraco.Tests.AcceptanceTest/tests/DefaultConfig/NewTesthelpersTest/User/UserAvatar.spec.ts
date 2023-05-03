import {test} from '@umbraco/playwright-testhelpers';
import {expect} from "@playwright/test";

test.describe('User Avatar Tests', () => {
  // User
  const userEmail = "userAvatar@email.com";
  const userName = "UserAvatarTests";
  // Avatar
  const avatarFileId = '3fa85f64-1717-4562-b3fc-2c963f11afa1';
  const avatarName = 'Umbraco.png';
  const mimeType = 'image/png';
  const avatarFilePath = './fixtures/mediaLibrary/Umbraco.png';

  test.beforeEach(async ({page, umbracoApi}) => {
    await umbracoApi.user.ensureUserNameNotExists(userName);
    await umbracoApi.temporaryFile.ensureTemporaryFileWithIdNotExists(avatarFileId);
  });

  test.afterEach(async ({page, umbracoApi}) => {
    await umbracoApi.user.ensureUserNameNotExists(userName);
    await umbracoApi.temporaryFile.ensureTemporaryFileWithIdNotExists(avatarFileId);
  });

  test('can add an avatar to a user ', async ({page, umbracoApi, umbracoUi}) => {
    const userGroup = await umbracoApi.userGroup.getUserGroupByName("Writers");

    const userGroupData = [userGroup.id];

    await umbracoApi.user.createUser(userEmail, userName, userGroupData);

    const userData = await umbracoApi.user.getUserByName(userName);

    await umbracoApi.temporaryFile.createTemporaryFile(avatarFileId, avatarName, mimeType, avatarFilePath);

    await umbracoApi.user.addAvatarToUserWithId(userData.id, avatarFileId);

    // Assert
    // Checks if the avatar was added to the user
    const userDataWithAvatar = await umbracoApi.user.getUserByName(userName);
    await expect(userDataWithAvatar.avatarUrls.length !== 0).toBeTruthy();
  });

  test('can remove an avatar from a user ', async ({page, umbracoApi, umbracoUi}) => {
    const userGroup = await umbracoApi.userGroup.getUserGroupByName("Writers");

    const userGroupData = [userGroup.id];

    await umbracoApi.user.createUser(userEmail, userName, userGroupData);

    const userData = await umbracoApi.user.getUserByName(userName);

    await umbracoApi.temporaryFile.createTemporaryFile(avatarFileId, avatarName, mimeType, avatarFilePath);

    await umbracoApi.user.addAvatarToUserWithId(userData.id, avatarFileId);

    // Checks if the avatar was added to the user
    const userDataWithAvatar = await umbracoApi.user.getUserByName(userName);
    await expect(userDataWithAvatar.avatarUrls.length !== 0).toBeTruthy();

    await umbracoApi.user.removeAvatarFromUserWithId(userData.id);

    // Checks if the avatar was removed from the user
    const userDataWithoutAvatar = await umbracoApi.user.getUserByName(userName);
    await expect(userDataWithoutAvatar.avatarUrls.length == 0).toBeTruthy();
  });
});
