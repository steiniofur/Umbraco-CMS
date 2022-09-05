import { UUITextStyles } from '@umbraco-ui/uui-css';
import { css, LitElement, nothing, PropertyValueMap } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { UmbContextProviderMixin } from '../../../core/context';
import UmbActionElement, { ActionPageEntity } from './action.element';
import { BehaviorSubject, Observable } from 'rxjs';

// TODO how do we dynamically import this so we don't have to import every page that could potentially be used?

@customElement('umb-action-page-service')
export class UmbActionPageService extends UmbContextProviderMixin(LitElement) {
	static styles = [UUITextStyles, css``];

	@property({ type: Object })
	public actionEntity: ActionPageEntity = { key: '', name: '' };

	private _entity: BehaviorSubject<ActionPageEntity> = new BehaviorSubject({ key: '', name: '' });
	public readonly entity: Observable<ActionPageEntity> = this._entity.asObservable();

	@state()
	private _pages: Array<HTMLElement> = [];

	connectedCallback() {
		super.connectedCallback();
		this.provideContext('umbActionPageService', this);
		this.openFreshPage('umb-action-list-page');
	}

	protected updated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): void {
		super.updated(_changedProperties);

		if (_changedProperties.has('actionEntity')) {
			this._entity.next(this.actionEntity);
			//TODO: Move back to first page
			this.openFreshPage('umb-action-list-page');
		}
	}

	public openPage(elementName: string) {
		const element = document.createElement(elementName) as UmbActionElement;
		this._pages.push(element);
		this.requestUpdate('_pages');
	}

	public openFreshPage(elementName: string) {
		this._pages = [];
		this.openPage(elementName);
	}

	public closeTopPage() {
		this._pages.pop();
		this.requestUpdate('_pages');
	}

	private _renderTopPage() {
		if (this._pages.length === 0) {
			return nothing;
		}

		return this._pages[this._pages.length - 1];
	}

	render() {
		return this._renderTopPage();
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'umb-action-page-service': UmbActionPageService;
	}
}
