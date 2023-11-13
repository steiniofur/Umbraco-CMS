import { css, CSSResultGroup, html, LitElement, nothing, PropertyValueMap } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { ifDefined } from 'lit/directives/if-defined.js';
import { styleMap } from 'lit/directives/style-map.js';
import { when } from 'lit/directives/when.js';

@customElement('umb-auth-layout')
export class UmbAuthLayoutElement extends LitElement {
	@property({ attribute: 'background-image' })
	backgroundImage?: string;

	@property({ attribute: 'logo-image' })
	logoImage?: string;

	protected updated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): void {
		super.updated(_changedProperties);
		// set --image variable

		if (_changedProperties.has('backgroundImage')) {
			this.style.setProperty('--image', `url('${this.backgroundImage}')`);
		}
	}

	render() {
		return html`
			<div id="main">
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
		return html`
			<div
				id="background"
				style=${styleMap({ backgroundImage: `url('${this.backgroundImage}')` })}
				aria-hidden="true"></div>

			${this.logoImage ? html`<div id="logo" aria-hidden="true"><img src=${this.logoImage} alt="" /></div>` : nothing}

			<div id="container">
				<div id="box">
					<slot></slot>
				</div>
			</div>
		`;
	}

	static styles: CSSResultGroup = [
		css`
			#main {
				max-width: 1600px;
				display: grid;
				grid-template-areas: 'content';
				height: 100vh;
				padding: 32px;
				box-sizing: border-box;
				margin: 0 auto;
				grid-auto-columns: 1fr;
			}
			#image-container {
				/* background-color: #e0a4a4; */
				display: none;
			}
			#content-container {
				/* background-color: #a4a4e0; */
				display: flex;
				padding: 16px;
			}
			#content {
				grid-area: content;
				max-width: 400px;
				margin: auto;
			}
			#image {
				grid-area: image;
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
					grid-template-areas: 'image content';
					padding-right: 0;
				}
				#image-container {
					display: block;
				}
				#logo-no-image {
					display: none;
				}
			}
			/* #background {
				position: fixed;
				overflow: hidden;
				background-position: 50%;
				background-repeat: no-repeat;
				background-size: cover;
				width: 100vw;
				height: 100vh;
			}

			#logo img {
				height: 100%;
			}

			#container {
				position: relative;
				display: flex;
				align-items: center;
				justify-content: center;
				width: 100vw;
				height: 100vh;
			}

			#box {
				width: 500px;
				padding: var(--uui-size-layout-3);
				background-color: var(--uui-color-surface-alt);
				box-sizing: border-box;
				box-shadow: var(--uui-shadow-depth-5);
				border-radius: calc(var(--uui-border-radius) * 2);
			}

			#email,
			#password {
				width: 100%;
			} */
		`,
	];
}

declare global {
	interface HTMLElementTagNameMap {
		'umb-auth-layout': UmbAuthLayoutElement;
	}
}
