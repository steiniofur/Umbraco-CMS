import { type UmbDataSourceResponse, type UmbItemDataSource } from '@umbraco-cms/backoffice/repository';
import { UmbClipboardLocalStorageManager } from '../../clipboard-local-storage.manager.js';
import type { UmbClipboardEntryItemModel } from './types.js';

/**
 * A local storage data source for Clipboard Entry items
 * @class UmbClipboardEntryItemServerDataSource
 * @implements {UmbItemServerDataSourceBase}
 */
export class UmbClipboardEntryItemLocalStorageDataSource implements UmbItemDataSource<UmbClipboardEntryItemModel> {
	#localStorageManager = new UmbClipboardLocalStorageManager();

	/**
	 * Gets items from local storage
	 * @param {Array<string>} unique
	 * @memberof UmbClipboardEntryItemLocalStorageDataSource
	 */
	async getItems(unique: Array<string>) {
		const { entries } = this.#localStorageManager.getEntries();
		const items = entries
			.filter((entry) => unique.includes(entry.unique))
			.map((entry) => {
				const item: UmbClipboardEntryItemModel = {
					entityType: entry.entityType,
					unique: entry.unique,
					type: entry.type,
					name: entry.name,
					icons: entry.icons,
					meta: entry.meta,
				};
				return item;
			});
		return { data: items };
	}
}
