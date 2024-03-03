import { manifests as copyEntityActionManifests } from './common/copy/manifests.js';
import { manifests as deleteEntityActionManifests } from './common/delete/manifests.js';
import { manifests as renameEntityActionManifests } from './common/rename/manifests.js';

export const manifests = [...copyEntityActionManifests, ...renameEntityActionManifests, deleteEntityActionManifests];
