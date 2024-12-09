import type { UmbDocumentTypeEntityType } from './entity.js';
import type {
	UmbContentTypeCompositionCompatibleModel,
	UmbContentTypeCompositionReferenceModel,
	UmbContentTypeAvailableCompositionRequestModel,
	UmbContentTypeModel,
} from '@umbraco-cms/backoffice/content-type';

export interface UmbDocumentTypeDetailModel extends UmbContentTypeModel {
	entityType: UmbDocumentTypeEntityType;
	allowedTemplates: Array<{ id: string }>;
	defaultTemplate: { id: string } | null;
	cleanup: UmbDocumentTypeCleanupModel;
}

export type UmbDocumentTypeCleanupModel = {
	preventCleanup: boolean;
	keepAllVersionsNewerThanDays?: number | null;
	keepLatestVersionPerDayForDays?: number | null;
};

export interface UmbDocumentTypeAvailableCompositionRequestModel
	extends UmbContentTypeAvailableCompositionRequestModel {
	isElement: boolean;
}

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface UmbDocumentTypeCompositionCompatibleModel extends UmbContentTypeCompositionCompatibleModel {}

// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface UmbDocumentTypeCompositionReferenceModel extends UmbContentTypeCompositionReferenceModel {}