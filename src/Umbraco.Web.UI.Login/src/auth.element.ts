import { html, customElement, property, ifDefined } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { umbExtensionsRegistry } from "@umbraco-cms/backoffice/extension-registry";

import { UMB_AUTH_CONTEXT, UmbAuthContext } from "./contexts";
import { UmbSlimBackofficeController } from "./controllers";

// Import the main bundle
import { extensions } from './umbraco-package.js';

@customElement('umb-auth')
export default class UmbAuthElement extends UmbLitElement {
  /**
   * Disables the local login form and only allows external login providers.
   *
   * @attr disable-local-login
   */
  @property({type: Boolean, attribute: 'disable-local-login'})
  disableLocalLogin = false;

  @property({attribute: 'background-image'})
  backgroundImage?: string;

  @property({attribute: 'logo-image'})
  logoImage?: string;

  @property({attribute: 'logo-image-alternative'})
  logoImageAlternative?: string;

  @property({type: Boolean, attribute: 'username-is-email'})
  usernameIsEmail = false;

  @property({type: Boolean, attribute: 'allow-password-reset'})
  allowPasswordReset = false;

  @property({type: Boolean, attribute: 'allow-user-invite'})
  allowUserInvite = false;

  @property({attribute: 'return-url'})
  set returnPath(value: string) {
    this.#authContext.returnPath = value;
  }
  get returnPath() {
    return this.#authContext.returnPath;
  }

  /**
   * Override the default flow.
   */
  protected flow?: 'mfa' | 'reset-password' | 'invite-user';

  #authContext = new UmbAuthContext(this, UMB_AUTH_CONTEXT);

  constructor() {
    super();

    (this as unknown as EventTarget).addEventListener('umb-login-flow', (e) => {
      if (e instanceof CustomEvent) {
        this.flow = e.detail.flow || undefined;
      }
      this.requestUpdate();
    });

    // Bind the (slim) Backoffice controller to this element so that we can use utilities from the Backoffice app.
    new UmbSlimBackofficeController(this);

    // Register the main package for Umbraco.Auth
    umbExtensionsRegistry.registerMany(extensions);
  }

  render() {
    return html`
      <umb-auth-layout
        background-image=${ifDefined(this.backgroundImage)}
        logo-image=${ifDefined(this.logoImage)}
        logo-image-alternative=${ifDefined(this.logoImageAlternative)}>
        ${this._renderFlowAndStatus()}
      </umb-auth-layout>
    `;
  }

  private _renderFlowAndStatus() {
    if (this.disableLocalLogin) {
      return html`
        <umb-error-layout no-back-link>
          <umb-localize key="auth_localLoginDisabled">Unfortunately, it is not possible to log in directly. It has been disabled by a login provider.</umb-localize>
        </umb-error-layout>
      `;
    }

    const searchParams = new URLSearchParams(window.location.search);
    let flow = this.flow || searchParams.get('flow')?.toLowerCase();
    const status = searchParams.get('status');

    if (status === 'resetCodeExpired') {
      return html`
        <umb-error-layout
          message=${this.localize.term('auth_resetCodeExpired')}>
        </umb-error-layout>`;
    }

    if (flow === 'invite-user' && status === 'false') {
      return html`
        <umb-error-layout
          message=${this.localize.term('auth_userInviteExpiredMessage')}>
        </umb-error-layout>`;
    }

    // validate
    if (flow) {
      if (flow === 'mfa' && !this.#authContext.isMfaEnabled) {
        flow = undefined;
      }
    }

    switch (flow) {
      case 'mfa':
        return html`
          <umb-mfa-page></umb-mfa-page>`;
      case 'reset':
        return html`
          <umb-reset-password-page></umb-reset-password-page>`;
      case 'reset-password':
        return html`
          <umb-new-password-page></umb-new-password-page>`;
      case 'invite-user':
        return html`
          <umb-invite-page></umb-invite-page>`;

      default:
        return html`
          <umb-login-page
            ?allow-password-reset=${this.allowPasswordReset}
            ?username-is-email=${this.usernameIsEmail}>
            <slot name="subheadline" slot="subheadline"></slot>
          </umb-login-page>`;
    }
  }
}

declare global {
  interface HTMLElementTagNameMap {
    'umb-auth': UmbAuthElement;
  }
}
