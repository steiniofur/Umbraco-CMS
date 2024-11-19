import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import { UmbPropertyActionBase, type UmbPropertyActionArgs } from '@umbraco-cms/backoffice/property-action';
import { UMB_CLIPBOARD_CONTEXT } from '@umbraco-cms/backoffice/clipboard';
import type { UmbClipboardEntry } from '@umbraco-cms/backoffice/clipboard';
import { UmbId } from '@umbraco-cms/backoffice/id';

export class UmbColorPickerCopyToClipboardPropertyAction extends UmbPropertyActionBase {
	#clipboardContext?: typeof UMB_CLIPBOARD_CONTEXT.TYPE;
	#propertyContext?: typeof UMB_PROPERTY_CONTEXT.TYPE;
	#notificationContext?: typeof UMB_NOTIFICATION_CONTEXT.TYPE;
	#init?: Promise<unknown>;

	constructor(host: UmbControllerHost, args: UmbPropertyActionArgs<never>) {
		super(host, args);

		this.#init = Promise.all([
			this.consumeContext(UMB_CLIPBOARD_CONTEXT, (context) => {
				this.#clipboardContext = context;
			}).asPromise(),

			this.consumeContext(UMB_PROPERTY_CONTEXT, (context) => {
				this.#propertyContext = context;
			}).asPromise(),

			this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
				this.#notificationContext = context;
			}).asPromise(),
		]);
	}

	override async execute() {
		await this.#init;
		const propertyValue = this.#propertyContext?.getValue();
		const propertyLabel = this.#propertyContext?.getLabel() ?? 'Color';

		if (!propertyValue) {
			// TODO: Add correct message + localization
			this.#notificationContext!.peek('danger', { data: { message: 'No value' } });
		}

		// TODO: Add correct meta data
		const clipboardEntry: UmbClipboardEntry = {
			unique: UmbId.new(),
			type: 'color',
			name: propertyLabel,
			icons: ['icon-color'],
			meta: {},
			data: [propertyValue],
		};

		await this.#clipboardContext!.create(clipboardEntry);
		// TODO: Add correct message + localization
		this.#notificationContext?.peek('positive', { data: { message: `${propertyLabel} copied to clipboard` } });
	}
}
export { UmbColorPickerCopyToClipboardPropertyAction as api };