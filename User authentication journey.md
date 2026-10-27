I have product API service, Order API service and UI APP runing in azure and I have defined three app registration under Azure Entra ID and I also have API gateway defined. in UI App registration I added API permision of both API. Now I want to login the application and get access token which is having user role as admin. Tell me this token will be able to access both the API or do i need to separately call the API to fethc different oken for both api and then use them? Tell me the API gateway apporach and without gateway.

In Microsoft Entra ID (Azure AD), the answer depends on **how OAuth scopes/audiences are configured** and whether you're calling APIs directly or through an API Gateway.

## Key Principle: Access Tokens Are Audience-Specific

An access token is issued for a specific **resource (audience)**.

For example:

* Product API App Registration

  * Application ID URI: `api://product-api`
* Order API App Registration

  * Application ID URI: `api://order-api`

When the UI requests a token, it specifies a scope:

```text
scope=api://product-api/Product.Read
```

The resulting token contains:

```json
{
  "aud": "api://product-api",
  "scp": "Product.Read",
  "roles": ["Admin"]
}
```

This token is valid only for Product API.

Even if the UI app registration has permissions to both APIs, the token's `aud` claim is still only one API.

---

# Without API Gateway

## Scenario

```text
UI
 ├── Product API
 └── Order API
```

### Login

User signs in:

```text
openid
profile
offline_access
```

You receive:

* ID Token
* Refresh Token (or auth code)

### Calling Product API

Acquire token for:

```text
api://product-api/Product.Read
```

Result:

```json
{
  "aud": "api://product-api",
  "roles": ["Admin"]
}
```

Use it against Product API.

---

### Calling Order API

Acquire another token for:

```text
api://order-api/Order.Read
```

Result:

```json
{
  "aud": "api://order-api",
  "roles": ["Admin"]
}
```

Use it against Order API.

---

### Conclusion (No Gateway)

You typically need **one access token per API resource**.

```text
Token A -> Product API
Token B -> Order API
```

The same logged-in user can obtain both tokens silently through MSAL, but they are different tokens.

---

# With API Gateway

Suppose:

```text
UI
  |
  v
API Gateway
  |
  +---- Product API
  |
  +---- Order API
```

and the Gateway itself has an App Registration.

## Recommended Approach

Expose scopes on Gateway:

```text
api://gateway/access
```

UI requests:

```text
scope=api://gateway/access
```

User receives:

```json
{
  "aud": "api://gateway",
  "roles": ["Admin"]
}
```

The UI sends only this token to the Gateway.

---

### Gateway Validation

Gateway validates:

* issuer
* audience
* signature
* roles/scopes

Example:

```json
{
  "roles": ["Admin"]
}
```

---

### Gateway to Backend APIs

Two common patterns:

#### Pattern 1: Token Pass-Through (Less Common)

Gateway forwards the same token.

Problem:

```json
"aud": "api://gateway"
```

Product API expects:

```json
"aud": "api://product-api"
```

Validation fails.

So this pattern generally doesn't work unless Product API is configured to trust Gateway audience.

---

#### Pattern 2: On-Behalf-Of (OBO) Flow (Recommended)

```text
User
  |
  | Token (aud=gateway)
  v
Gateway
  |
  | OBO
  v
Entra ID
  |
  +--> Product API Token
  |
  +--> Order API Token
```

Gateway exchanges the user's token for backend-specific tokens.

For Product API:

```json
{
  "aud": "api://product-api",
  "roles": ["Admin"]
}
```

For Order API:

```json
{
  "aud": "api://order-api",
  "roles": ["Admin"]
}
```

Gateway calls each API using the appropriate token.

This is the Microsoft-recommended architecture for API-to-API calls on behalf of a user.

---

# What About Roles?

Suppose you created an app role:

```text
Admin
```

and assigned the user.

The role appears in access tokens issued for the target API.

Example:

```json
{
  "roles": ["Admin"]
}
```

Product API can check:

```csharp
[Authorize(Roles = "Admin")]
```

Similarly for Order API.

---

# Best Practice Architecture

```text
UI SPA/Web App
     |
     | Login
     v
Entra ID
     |
     | Access Token (aud=gateway)
     v
API Gateway
     |
     | OBO
     |
     +---- Product API
     |
     +---- Order API
```

Benefits:

* UI manages only one API token.
* Backend APIs are hidden behind Gateway.
* Centralized authentication and authorization.
* Easier API versioning and throttling.
* Least exposure of internal APIs.

---

## Direct Answer

### Without API Gateway

No. A single access token cannot normally be used for both Product API and Order API because each token is issued for a specific audience (`aud`). You will acquire a separate access token for each API, although MSAL can do this silently after login.

### With API Gateway

The UI can obtain a single token for the Gateway. The Gateway then uses the **On-Behalf-Of (OBO)** flow to acquire separate backend tokens for Product API and Order API and forwards requests accordingly. This is the preferred enterprise pattern in Entra ID.
