import { css, CSSResultGroup, html, LitElement, nothing, PropertyValueMap } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { ifDefined } from 'lit/directives/if-defined.js';
import { when } from 'lit/directives/when.js';

@customElement('umb-auth-layout')
export class UmbAuthLayoutElement extends LitElement {
	@property({ attribute: 'background-image' })
	backgroundImage?: string;

	@property({ attribute: 'logo-image' })
	logoImage?: string;

	protected updated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): void {
		super.updated(_changedProperties);

		if (_changedProperties.has('backgroundImage')) {
			this.style.setProperty('--image', `url('${this.backgroundImage}')`);
		}
	}

	#renderImageContainer() {
		if (!this.backgroundImage) {
			return when(
				this.logoImage,
				() =>
					html`<div id="logo-no-image" aria-hidden="true">
						<img src=${ifDefined(this.logoImage)} alt="umbraco-logo" />
					</div>`
			);
		}

		return html`
			<div id="image-container">
				<div id="image">
					${when(
						this.logoImage,
						() =>
							html`<div id="logo" aria-hidden="true">
								<img src=${ifDefined(this.logoImage)} alt="umbraco-logo" />
							</div>`
					)}
				</div>
			</div>
		`;
	}

	render() {
		return html`
			<div id="main">
				${this.#renderImageContainer()}
				<div id="content-container">
					<div id="content">
						<slot></slot>
					</div>
				</div>
			</div>
			${when(
				this.logoImage,
				() =>
					html`<div id="logo-no-image" aria-hidden="true">
						<img src=${ifDefined(this.logoImage)} alt="umbraco-logo" />
					</div>`
			)}
		`;
	}

	static styles: CSSResultGroup = [
		css`
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
			#logo {
				position: absolute;
				top: 24px;
				left: 24px;
				height: 30px;
				width: 200px;
			}
			#logo-no-image {
				position: fixed;
				top: 24px;
				left: 24px;
				height: 30px;
				width: 200px;
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
				#logo-no-image {
					display: none;
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
