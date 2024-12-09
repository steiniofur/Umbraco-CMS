import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from '../workspace/document-workspace.context-token.js';
import { UmbDocumentPublishingRepository } from './repository/index.js';
import { UmbDocumentPublishedPendingChangesManager } from './document-published-pending-changes.manager.js';
import { UMB_DOCUMENT_PUBLISHING_WORKSPACE_CONTEXT } from './document-publishing.workspace-context.token.js';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import { UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import type { UmbDocumentDetailModel, UmbDocumentVariantPublishModel } from '../types.js';
import { UmbServerModelValidatorContext } from '@umbraco-cms/backoffice/validation';
import {
	UmbRequestReloadChildrenOfEntityEvent,
	UmbRequestReloadStructureForEntityEvent,
} from '@umbraco-cms/backoffice/entity-action';
import { UMB_ACTION_EVENT_CONTEXT } from '@umbraco-cms/backoffice/action';
import type { UmbDocumentValidationRepository } from '../repository/validation/index.js';
import { UMB_DOCUMENT_SCHEDULE_MODAL } from './schedule-publish/constants.js';
import { UmbVariantId } from '@umbraco-cms/backoffice/variant';
import { UMB_DOCUMENT_PUBLISH_WITH_DESCENDANTS_MODAL } from './publish-with-descendants/constants.js';

export class UmbDocumentPublishingWorkspaceContext extends UmbContextBase<UmbDocumentPublishingWorkspaceContext> {
	public readonly publishedPendingChanges = new UmbDocumentPublishedPendingChangesManager(this);

	#init: Promise<unknown>;
	#documentWorkspaceContext?: typeof UMB_DOCUMENT_WORKSPACE_CONTEXT.TYPE;
	#publishingRepository = new UmbDocumentPublishingRepository(this);
	#serverValidation = new UmbServerModelValidatorContext(this);
	#validationRepository?: UmbDocumentValidationRepository;

	constructor(host: UmbControllerHost) {
		super(host, UMB_DOCUMENT_PUBLISHING_WORKSPACE_CONTEXT);

		this.#init = Promise.all([
			this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, async (context) => {
				this.#documentWorkspaceContext = context;

				// No need to check pending changes for new documents
				if (context.getIsNew()) {
					return;
				}

				this.observe(context.unique, async (unique) => {
					if (unique) {
						const { data: publishedData } = await this.#publishingRepository.published(unique);
						const currentData = context.getData();

						if (!currentData || !publishedData) {
							return;
						}

						this.publishedPendingChanges.process({ currentData, publishedData });
					}
				});
			}).asPromise(),
		]);
	}

	public async saveAndPublish(): Promise<void> {
		return this.#handleSaveAndPublish();
	}

	public async schedule() {
		const { options, selected } = await this._determineVariantOptions();

		const modalManagerContext = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
		const result = await modalManagerContext
			.open(this, UMB_DOCUMENT_SCHEDULE_MODAL, {
				data: {
					options,
					pickableFilter: this._readOnlyLanguageVariantsFilter,
				},
				value: { selection: selected.map((unique) => ({ unique, schedule: {} })) },
			})
			.onSubmit()
			.catch(() => undefined);

		if (!result?.selection.length) return;

		// Map to the correct format for the API (UmbDocumentVariantPublishModel)
		const variants =
			result?.selection.map<UmbDocumentVariantPublishModel>((x) => ({
				variantId: UmbVariantId.FromString(x.unique),
				schedule: x.schedule,
			})) ?? [];

		if (!variants.length) return;

		// TODO: Validate content & Save changes for the selected variants — This was how it worked in v.13 [NL]

		const unique = this.getUnique();
		if (!unique) throw new Error('Unique is missing');
		await this.#publishingRepository.publish(unique, variants);
	}

	public async publishWithDescendants() {
		await this.#init;
		if (!this.#documentWorkspaceContext) throw new Error('Document workspace context is missing');

		const unique = this.#documentWorkspaceContext.getUnique();
		if (!unique) throw new Error('Unique is missing');

		const entityType = this.#documentWorkspaceContext.getEntityType();
		if (!entityType) throw new Error('Entity type is missing');

		const { options, selected } = await this._determineVariantOptions();

		const modalManagerContext = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
		const result = await modalManagerContext
			.open(this, UMB_DOCUMENT_PUBLISH_WITH_DESCENDANTS_MODAL, {
				data: {
					options,
					pickableFilter: this._readOnlyLanguageVariantsFilter,
				},
				value: { selection: selected },
			})
			.onSubmit()
			.catch(() => undefined);

		if (!result?.selection.length) return;

		// Map to variantIds
		const variantIds = result?.selection.map((x) => UmbVariantId.FromString(x)) ?? [];

		if (!variantIds.length) return;

		const { error } = await this.#publishingRepository.publishWithDescendants(
			unique,
			variantIds,
			result.includeUnpublishedDescendants ?? false,
		);

		if (!error) {
			const eventContext = await this.getContext(UMB_ACTION_EVENT_CONTEXT);

			// request reload of this entity
			const structureEvent = new UmbRequestReloadStructureForEntityEvent({
				entityType,
				unique,
			});

			// request reload of the children
			const childrenEvent = new UmbRequestReloadChildrenOfEntityEvent({
				entityType,
				unique,
			});

			eventContext.dispatchEvent(structureEvent);
			eventContext.dispatchEvent(childrenEvent);
		}
	}

	public async unpublish() {
		await this.#init;
		if (!this.#documentWorkspaceContext) throw new Error('Document workspace context is missing');

		const unique = this.#documentWorkspaceContext.getUnique();
		if (!unique) throw new Error('Unique is missing');

		const entityType = this.#documentWorkspaceContext.getEntityType();
		if (!entityType) throw new Error('Entity type is missing');

		// TODO: remove meta
		new UmbUnpublishDocumentEntityAction(this, { unique, entityType, meta: {} as never }).execute();
	}

	async #handleSaveAndPublish() {
		await this.#init;
		if (!this.#documentWorkspaceContext) throw new Error('Document workspace context is missing');

		const unique = this.#documentWorkspaceContext.getUnique();
		if (!unique) throw new Error('Unique is missing');

		let variantIds: Array<UmbVariantId> = [];

		const { options, selected } = await this._determineVariantOptions();

		// If there is only one variant, we don't need to open the modal.
		if (options.length === 0) {
			throw new Error('No variants are available');
		} else if (options.length === 1) {
			// If only one option we will skip ahead and save the document with the only variant available:
			variantIds.push(UmbVariantId.Create(options[0]));
		} else {
			// If there are multiple variants, we will open the modal to let the user pick which variants to publish.
			const modalManagerContext = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
			const result = await modalManagerContext
				.open(this, UMB_DOCUMENT_PUBLISH_MODAL, {
					data: {
						options,
						pickableFilter: this._readOnlyLanguageVariantsFilter,
					},
					value: { selection: selected },
				})
				.onSubmit()
				.catch(() => undefined);

			if (!result?.selection.length || !unique) return;

			variantIds = result?.selection.map((x) => UmbVariantId.FromString(x)) ?? [];
		}

		const saveData = await this._data.constructData(variantIds);
		await this._runMandatoryValidationForSaveData(saveData);

		// Create the validation repository if it does not exist. (we first create this here when we need it) [NL]
		this.#validationRepository ??= new UmbDocumentValidationRepository(this);

		// We ask the server first to get a concatenated set of validation messages. So we see both front-end and back-end validation messages [NL]
		if (this.getIsNew()) {
			const parent = this.getParent();
			if (!parent) throw new Error('Parent is not set');
			this.#serverValidation.askServerForValidation(
				saveData,
				this.#validationRepository.validateCreate(saveData, parent.unique),
			);
		} else {
			this.#serverValidation.askServerForValidation(
				saveData,
				this.#validationRepository.validateSave(saveData, variantIds),
			);
		}

		// TODO: Only validate the specified selection.. [NL]
		return this.validateAndSubmit(
			async () => {
				return this.#performSaveAndPublish(variantIds, saveData);
			},
			async () => {
				// If data of the selection is not valid Then just save:
				await this._performCreateOrUpdate(variantIds, saveData);
				// Notifying that the save was successful, but we did not publish, which is what we want to symbolize here. [NL]
				const notificationContext = await this.getContext(UMB_NOTIFICATION_CONTEXT);
				// TODO: Get rid of the save notification.
				// TODO: Translate this message [NL]
				notificationContext.peek('danger', {
					data: { message: 'Document was not published, but we saved it for you.' },
				});
				// Reject even thought the save was successful, but we did not publish, which is what we want to symbolize here. [NL]
				return await Promise.reject();
			},
		);
	}

	async #performSaveAndPublish(variantIds: Array<UmbVariantId>, saveData: UmbDocumentDetailModel): Promise<void> {
		await this.#init;
		if (!this.#documentWorkspaceContext) throw new Error('Document workspace context is missing');

		const unique = this.#documentWorkspaceContext.getUnique();
		if (!unique) throw new Error('Unique is missing');

		const entityType = this.#documentWorkspaceContext.getEntityType();
		if (!entityType) throw new Error('Entity type is missing');

		await this._performCreateOrUpdate(variantIds, saveData);

		const { error } = await this.#publishingRepository.publish(
			unique,
			variantIds.map((variantId) => ({ variantId })),
		);

		if (!error) {
			const eventContext = await this.getContext(UMB_ACTION_EVENT_CONTEXT);
			const event = new UmbRequestReloadStructureForEntityEvent({ unique, entityType });
			eventContext.dispatchEvent(event);
			this._closeModal();
		}
	}
}

export { UmbDocumentPublishingWorkspaceContext as api };
