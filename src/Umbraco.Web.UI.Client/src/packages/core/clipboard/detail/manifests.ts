import { UMB_CLIPBOARD_DETAIL_REPOSITORY_ALIAS, UMB_CLIPBOARD_DETAIL_STORE_ALIAS } from './constants.js';

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'repository',
		alias: UMB_CLIPBOARD_DETAIL_REPOSITORY_ALIAS,
		name: 'Clipboard Detail Repository',
		api: () => import('./clipboard-detail.repository.js'),
	},
	{
		type: 'store',
		alias: UMB_CLIPBOARD_DETAIL_STORE_ALIAS,
		name: 'Clipboard Detail Store',
		api: () => import('./clipboard-detail.store.js'),
	},
];