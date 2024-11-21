import type { UmbExtensionManifestKind } from '@umbraco-cms/backoffice/extension-registry';
import { UMB_PROPERTY_ACTION_DEFAULT_KIND_MANIFEST } from '@umbraco-cms/backoffice/property-action';

export const manifests: Array<UmbExtensionManifest | UmbExtensionManifestKind> = [
	{
		type: 'kind',
		alias: 'Umb.Kind.PropertyAction.pasteFromClipboard',
		matchKind: 'pasteFromClipboard',
		matchType: 'propertyAction',
		manifest: {
			...UMB_PROPERTY_ACTION_DEFAULT_KIND_MANIFEST.manifest,
			type: 'propertyAction',
			kind: 'pasteFromClipboard',
			api: () => import('./paste-from-clipboard.property-action.js'),
			weight: 1200,
			meta: {
				icon: 'icon-paste-in',
				label: 'Paste',
			},
		},
	},
];
