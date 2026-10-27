# Azure Managed Identity + App Role Authentication Flow Notes

## Architecture

Flow:

```
User / UI
   |
   | User Access Token
   | roles: Admin/User
   |
   v
Order Service API
   |
   | Managed Identity Token
   | roles: OrderServiceAccess
   |
   v
Product API
```

There are **two different authentication flows**:

1. User → Order Service
2. Order Service → Product API (service-to-service)

---

# 1. Order Service API (OrderApiRahul)

Purpose:

* Receives requests from UI/Postman users.
* Validates user identity.

## App Registration

`OrderApiRahul`

Expose API:

```
Application ID URI:
api://33a872d2-40e7-4441-bed5-dd6007129997
```

Scope:

```
access_as_user
```

Full scope:

```
api://33a872d2-40e7-4441-bed5-dd6007129997/access_as_user
```

---

## App Roles

Order Service has user roles:

```
Admin
User
```

Allowed member types:

```
Users/Groups
```

Example:

Controller:

```csharp
[Authorize(Roles="Admin")]
public IActionResult CreateOrder()
```

Incoming user token:

```json
{
  "roles": [
    "Admin"
  ]
}
```

This is correct.

---

# 2. Product API

Purpose:

* Receives calls from Order Service.
* Does not care about users.
* Uses Managed Identity authentication.

---

## Product API App Registration

Example:

```
ProductApi
```

Application ID:

```
87e5423f-f256-45eb-ac68-b417de6d4da5
```

Audience:

```
api://87e5423f-f256-45eb-ac68-b417de6d4da5
```

---

# 3. Create App Role on Product API

Important rule:

> App roles belong to the API that receives the token.

Since Order Service calls Product API:

`OrderServiceAccess` must exist on **Product API**, not Order Service.

Create:

```
Name:
OrderServiceAccess

Value:
OrderServiceAccess

Allowed member types:
Applications

Enabled:
Yes
```

---

# 4. Assign Managed Identity to Product API Role

Managed Identity:

```
order-rahul
```

gets permission:

```
ProductApi
    |
    |
    OrderServiceAccess
```

Not:

```
OrderApiRahul
```

---

# 5. Get IDs using Azure CLI

## Product API Service Principal

```bash
az ad sp list --display-name ProductApi
```

Get:

```
PRODUCT_API_SP_OBJECT_ID
```

---

## Managed Identity Object ID

```bash
az ad sp list --display-name order-rahul
```

Get:

```
ORDER_MI_OBJECT_ID
```

---

## Get App Role ID

```bash
az ad app list --display-name ProductApi
```

Get:

```
PRODUCT_API_APP_ID
```

Then:

```bash
az ad app show \
--id PRODUCT_API_APP_ID \
--query "appRoles[?value=='OrderServiceAccess'].id"
```

Get:

```
ORDER_SERVICE_ACCESS_ROLE_ID
```

---

# 6. Assign Role Using Graph API

Command:

```bash
az rest \
--method POST \
--uri https://graph.microsoft.com/v1.0/servicePrincipals/ORDER_MI_OBJECT_ID/appRoleAssignments \
--body '{
  "principalId": "ORDER_MI_OBJECT_ID",
  "resourceId": "PRODUCT_API_SP_OBJECT_ID",
  "appRoleId": "ORDER_SERVICE_ACCESS_ROLE_ID"
}'
```

---

# 7. Order Service Token Request

When Order Service calls Product API:

Do NOT request Order API token.

Wrong:

```
api://33a872d2-40e7-4441-bed5-dd6007129997/.default
```

Correct:

```
api://87e5423f-f256-45eb-ac68-b417de6d4da5/.default
```

Code:

```csharp
var credential = new DefaultAzureCredential();

var token = await credential.GetTokenAsync(
    new TokenRequestContext(
        new[]
        {
            "api://87e5423f-f256-45eb-ac68-b417de6d4da5/.default"
        }));
```

---

# 8. Local Testing

Managed Identity does not exist on localhost.

For local testing:

```csharp
var credential = new AzureCliCredential();
```

Login:

```bash
az login --tenant TENANT_ID
```

Production:

```csharp
DefaultAzureCredential()
```

works automatically in Azure App Service / Function / VM.

---

# 9. Product API Authorization

Product API controller:

Correct:

```csharp
[Authorize(Roles="OrderServiceAccess")]
```

Not:

```csharp
[Authorize(Roles="Admin")]
```

because the caller is the managed identity.

Expected Product API token:

```json
{
  "aud": "87e5423f-f256-45eb-ac68-b417de6d4da5",
  "roles": [
    "OrderServiceAccess"
  ]
}
```

---

# Final Checklist

✅ OrderApiRahul

* `access_as_user` scope
* `Admin/User` roles for users
* Protect CreateOrder with `[Authorize(Roles="Admin")]`

✅ ProductApi

* Create `OrderServiceAccess` app role
* Allowed type: `Applications`

✅ Managed Identity

* `order-rahul` assigned `OrderServiceAccess` on ProductApi

✅ Order Service

* Request token using Product API `.default`

✅ Product API

* Validate `OrderServiceAccess` role

This gives the correct Azure AD service-to-service Managed Identity pattern.
