import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import type { UmbPartialViewTreeStore } from './partial-view-tree.store.js';

export const UMB_PARTIAL_VIEW_TREE_STORE_CONTEXT = new UmbContextToken<UmbPartialViewTreeStore>(
	'UmbPartialViewTreeStore',
);
