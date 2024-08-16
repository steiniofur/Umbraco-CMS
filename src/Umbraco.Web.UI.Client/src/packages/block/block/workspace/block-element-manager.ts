import type { UmbBlockDataType } from '../types.js';
import { UmbBlockElementPropertyDatasetContext } from './block-element-property-dataset.context.js';
import type { UmbContentTypeModel } from '@umbraco-cms/backoffice/content-type';
import { UmbContentTypeStructureManager } from '@umbraco-cms/backoffice/content-type';
import { UmbClassState, UmbObjectState } from '@umbraco-cms/backoffice/observable-api';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import { UmbDocumentTypeDetailRepository } from '@umbraco-cms/backoffice/document-type';
import type { UmbVariantId } from '@umbraco-cms/backoffice/variant';

export class UmbBlockElementManager extends UmbControllerBase {
	//
	#data = new UmbObjectState<UmbBlockDataType | undefined>(undefined);
	readonly data = this.#data.asObservable();
	#getDataPromise = new Promise<void>((resolve) => {
		this.#getDataResolver = resolve;
	});
	#getDataResolver!: () => void;

	#variantId = new UmbClassState<UmbVariantId | undefined>(undefined);
	readonly variantId = this.#variantId.asObservable();

	readonly unique = this.#data.asObservablePart((data) => data?.udi);
	readonly contentTypeId = this.#data.asObservablePart((data) => data?.contentTypeKey);

	readonly structure = new UmbContentTypeStructureManager<UmbContentTypeModel>(
		this,
		new UmbDocumentTypeDetailRepository(this),
	);

	constructor(host: UmbControllerHost) {
		// TODO: Get Workspace Alias via Manifest.
		super(host);

		this.observe(this.contentTypeId, (id) => this.structure.loadType(id));
	}

	reset() {
		this.#data.setValue(undefined);
	}

	setVariantId(variantId: UmbVariantId | undefined) {
		this.#variantId.setValue(variantId);
	}

	setData(data: UmbBlockDataType | undefined) {
		this.#data.setValue(data);
		this.#getDataResolver();
	}

	getData() {
		return this.#data.getValue();
	}

	getUnique() {
		return this.getData()?.udi;
	}

	getEntityType() {
		return 'element';
	}

	getContentTypeId() {
		return this.getData()?.contentTypeKey;
	}

	// We will implement propertyAlias in the future, when implementing Varying Blocks. [NL]
	// eslint-disable-next-line @typescript-eslint/no-unused-vars
	async propertyVariantId(propertyAlias: string) {
		return this.variantId;
	}

	async propertyValueByAlias<ReturnType = unknown>(propertyAlias: string) {
		await this.#getDataPromise;

		return this.#data.asObservablePart((data) => data?.[propertyAlias] as ReturnType);
	}

	async getPropertyValue<ReturnType = unknown>(propertyAlias: string) {
		await this.#getDataPromise;

		return this.#data.getValue()?.[propertyAlias] as ReturnType;
	}

	async setPropertyValue(alias: string, value: unknown) {
		this.initiatePropertyValueChange();
		await this.#getDataPromise;

		this.#data.update({ [alias]: value });
		this.finishPropertyValueChange();
	}

	#updateLock = 0;
	initiatePropertyValueChange() {
		this.#updateLock++;
		this.#data.mute();
		// TODO: When ready enable this code will enable handling a finish automatically by this implementation 'using myState.initiatePropertyValueChange()' (Relies on TS support of Using) [NL]
		/*return {
			[Symbol.dispose]: this.finishPropertyValueChange,
		};*/
	}
	finishPropertyValueChange = () => {
		this.#updateLock--;
		this.#triggerPropertyValueChanges();
	};
	#triggerPropertyValueChanges() {
		if (this.#updateLock === 0) {
			this.#data.unmute();
		}
	}

	public createPropertyDatasetContext(host: UmbControllerHost) {
		return new UmbBlockElementPropertyDatasetContext(host, this);
	}

	public override destroy(): void {
		this.#data.destroy();
		this.structure.destroy();
		super.destroy();
	}
}

export default UmbBlockElementManager;
