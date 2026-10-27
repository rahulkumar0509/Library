Good idea. Let's build it from scratch using the **recommended setup**:

```text id="7s46p5"
Angular SPA
   ↓
Azure Entra ID
   ↓
Azure APIM (Gateway)
   ↓
Product API
```

We'll create **two App Registrations**:

1. **Angular SPA**
2. **Product API** (the audience APIM validates)

---

# Step 1: Create the Product API App Registration

In Azure Portal:

```text id="76d8ah"
Entra ID
→ App registrations
→ New registration
```

### Name

```text id="itjlwm"
ProductApi
```

### Supported account types

For learning:

```text id="2psr6q"
Accounts in this organizational directory only
```

### Redirect URI

Leave empty.

Click **Register**.

---

# Step 2: Expose the API

Open:

```text id="gwy5gx"
ProductApi
→ Expose an API
```

### Set Application ID URI

Click:

```text id="jdc7xu"
Add
```

Azure will suggest:

```text id="vvy3b6"
api://<product-api-client-id>
```

Example:

```text id="t20g31"
api://12345678-abcd-1234-abcd-123456789abc
```

Save it.

---

### Add Scope

Click:

```text id="x3c2rq"
Add a scope
```

Fill:

```text id="xpp5i8"
Scope name:
access_as_user

Who can consent:
Admins and users

Admin consent display name:
Access Product API

Admin consent description:
Allows access to Product API

User consent display name:
Access Product API

User consent description:
Allows access to Product API

State:
Enabled
```

Save.

---

# Step 3: Create Angular App Registration

```text id="gnqf0m"
Entra ID
→ App registrations
→ New registration
```

### Name

```text id="zhmjow"
AngularSPA
```

### Supported account types

```text id="ap57h0"
Accounts in this organizational directory only
```

### Redirect URI

Choose:

```text id="5nd4s2"
Single-page application (SPA)
```

and enter:

```text id="x88fx7"
http://localhost:4200
```

Register.

---

# Step 4: Grant API Permission

Open:

```text id="f7r1df"
AngularSPA
→ API Permissions
→ Add Permission
→ My APIs
```

You should now see:

```text id="k4uk7s"
ProductApi
```

Select it.

Choose:

```text id="b8jjxv"
Delegated Permissions
```

Select:

```text id="g4c5n2"
access_as_user
```

Add permission.

---

# Step 5: Grant Admin Consent

Still in:

```text id="pfyuxh"
API Permissions
```

Click:

```text id="v4nb2h"
Grant admin consent
```

Status should become:

```text id="gx5j5h"
Granted
```

---

# Step 6: Configure Angular

Install MSAL.

Use:

```typescript id="n36baf"
export function msalInstanceFactory() {
  return new PublicClientApplication({
    auth: {
      clientId: '<AngularSPA Client ID>',
      authority:
        'https://login.microsoftonline.com/<Tenant ID>/v2.0',
      redirectUri: 'http://localhost:4200'
    }
  });
}
```

---

# Step 7: Login Request

```typescript id="m8pcjk"
await this.msalService.instance.loginRedirect({
  scopes: [
    'openid',
    'profile',
    'email',
    'api://<ProductApi Client ID>/access_as_user'
  ]
});
```

Example:

```typescript id="9c6evn"
scopes: [
  'openid',
  'profile',
  'email',
  'api://12345678-abcd-1234-abcd-123456789abc/access_as_user'
]
```

---

# Step 8: Acquire Access Token

```typescript id="mr4a1c"
const token =
  await this.msalService.instance.acquireTokenSilent({
    scopes: [
      'api://12345678-abcd-1234-abcd-123456789abc/access_as_user'
    ],
    account:
      this.msalService.instance.getActiveAccount()!
  });

console.log(token.accessToken);
```

---

# Step 9: Configure APIM

Use the ProductApi OpenID issuer.

Example:

```xml id="s2udb9"
<validate-jwt header-name="Authorization">

  <openid-config
    url="https://login.microsoftonline.com/<tenant-id>/v2.0/.well-known/openid-configuration" />

  <audiences>
    <audience>
      api://12345678-abcd-1234-abcd-123456789abc
    </audience>
  </audiences>

</validate-jwt>
```

---

# Step 10: Test

Decode the token at:

[jwt.ms](https://jwt.ms?utm_source=chatgpt.com)

You should see:

```json id="hq1lz9"
{
  "aud": "api://12345678-abcd-1234-abcd-123456789abc"
}
```

Then call:

```http id="o6qkhn"
GET https://your-apim.azure-api.net/products
Authorization: Bearer <access_token>
```

APIM validates the token and forwards the request.

---

### Final Architecture

```text id="nyg2sw"
Angular SPA App Registration
    ↓ login
Azure Entra ID
    ↓ access token
ProductApi App Registration
    ↓ audience validation
APIM
    ↓
Product Service
```

This is the standard production pattern used with Angular + Entra ID + API Gateway.
