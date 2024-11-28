import { UMB_CREATE_DOCUMENT_TYPE_WORKSPACE_PATH_PATTERN } from '../../../paths.js';
import type { UmbDocumentTypeEntityTypeUnion } from '../../../entity.js';
import { UmbEntityCreateOptionActionBase } from '@umbraco-cms/backoffice/entity-create-option-action';
import type { MetaEntityCreateOptionAction } from '@umbraco-cms/backoffice/entity-create-option-action';

export class UmbDefaultDocumentTypeCreateOptionAction extends UmbEntityCreateOptionActionBase<MetaEntityCreateOptionAction> {
	override async execute() {
		const parentEntityType = this.args.entityType as UmbDocumentTypeEntityTypeUnion;
		if (!parentEntityType) throw new Error('Entity type is required to create a document type');

		const parentUnique = this.args.unique ?? null;

		const href = UMB_CREATE_DOCUMENT_TYPE_WORKSPACE_PATH_PATTERN.generateAbsolute({
			parentEntityType,
			parentUnique,
			presetAlias: null,
		});

		window.history.pushState({}, '', href);
	}
}

export { UmbDefaultDocumentTypeCreateOptionAction as api };
