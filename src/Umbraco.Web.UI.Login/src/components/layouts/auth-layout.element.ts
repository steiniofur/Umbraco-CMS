import { css, CSSResultGroup, html, LitElement, nothing, PropertyValueMap } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { ifDefined } from 'lit/directives/if-defined.js';
import { styleMap } from 'lit/directives/style-map.js';

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
					<div id="image"></div>
				</div>
				<div id="content-container">
					<div id="content">
						<slot></slot>
					</div>
				</div>
			</div>
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
				grid-template-columns: 1fr 1fr;
				height: 100vh;
				padding: 32px;
				padding-right: 0;
				box-sizing: border-box;
				margin: 0 auto;
			}
			#image-container {
				/* background-color: #e0a4a4; */
			}
			#content-container {
				/* background-color: #a4a4e0; */
				display: flex;
				padding: 16px;
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
				border-radius: 32px;
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

			#logo {
				position: fixed;
				top: var(--uui-size-space-5);
				left: var(--uui-size-space-5);
				height: 30px;
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
