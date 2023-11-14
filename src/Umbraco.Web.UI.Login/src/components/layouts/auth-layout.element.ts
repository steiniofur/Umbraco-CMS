import { css, CSSResultGroup, html, LitElement, nothing, PropertyValueMap } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { ifDefined } from 'lit/directives/if-defined.js';
import { when } from 'lit/directives/when.js';

@customElement('umb-auth-layout')
export class UmbAuthLayoutElement extends LitElement {
	@property({ attribute: 'image', reflect: true })
	image?: string;

	@property({ attribute: 'logo-on-image' })
	logoOnImage?: string;

	@property({ attribute: 'logo-on-background' })
	logoOnBackground?: string;

	protected updated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): void {
		super.updated(_changedProperties);

		if (_changedProperties.has('image')) {
			this.style.setProperty('--logo-on-background-display', this.image ? 'none' : 'unset');
			this.style.setProperty('--image', `url('${this.image}')`);
		}
	}

	#renderImageContainer() {
		if (!this.image) return nothing;

		return html`
			<div id="image-container">
				<div id="image">
					${when(
						this.logoOnImage,
						() => html`<img id="logo-on-image" src=${this.logoOnImage!} alt="umbraco-logo" aria-hidden="true" />`
					)}
				</div>
			</div>
		`;
	}

	#renderContent() {
		return html`
			<div id="content-container">
				<div id="content">
					<slot></slot>
				</div>
			</div>
		`;
	}

	render() {
		return html`
			<div id=${this.image ? 'main' : 'main-no-image'}>${this.#renderImageContainer()} ${this.#renderContent()}</div>
			<img id="logo-on-background" src=${ifDefined(this.logoOnBackground)} alt="umbraco-logo" aria-hidden="true" />
		`;
	}

	static styles: CSSResultGroup = [
		css`
			#main-no-image,
			#main {
				max-width: 1920px;
				display: flex;
				height: 100vh;
				padding: 8px;
				box-sizing: border-box;
				margin: 0 auto;
			}
			#image-container {
				display: none;
				width: 100%;
			}
			#content-container {
				display: flex;
				width: 100%;
				box-sizing: border-box;
			}
			#content {
				max-width: 400px;
				margin: auto;
			}
			#image {
				background-image: var(--image);
				background-position: 50%;
				background-repeat: no-repeat;
				background-size: cover;
				width: 100%;
				height: 100%;
				border-radius: 38px;
				position: relative;
			}
			#logo-on-image {
				position: absolute;
				top: 24px;
				left: 24px;
				height: 30px;
			}
			#logo-on-background {
				position: fixed;
				top: 24px;
				left: 24px;
				height: 30px;
				background-color: black;
			}
			@media only screen and (min-width: 900px) {
				#main {
					padding: 32px;
					padding-right: 0;
				}
				#image-container {
					display: block;
				}
				#content-container {
					display: flex;
					padding: 16px;
				}
				#logo-on-background {
					display: var(--logo-on-background-display);
				}
			}
		`,
	];
}

declare global {
	interface HTMLElementTagNameMap {
		'umb-auth-layout': UmbAuthLayoutElement;
	}
}
