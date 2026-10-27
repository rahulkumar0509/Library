Here is the complete working flow we followed for your **Azure Function App (.NET C# isolated) on free Azure account using MacBook + VS Code**.

# 1. Create Azure Function App (Free Tier)

Azure Portal:

```
Create Resource
 → Function App
```

Selected:

* Hosting plan:

  * ✅ Consumption (Windows)
* Region:

  * ✅ South India
* OS:

  * ✅ Windows

Why:

* East US failed because quota was `0 VM`
* West US options did not allow the required setup
* South India + Windows Consumption worked on free account

Created:

```
Function App:
abctestfunc
```

---

# 2. Create local C# Azure Function project

On Mac:

Installed:

* .NET SDK
* Azure Functions Core Tools
* VS Code Azure Functions extension

Created function project:

```
Offline-message-function
```

Project structure:

```
Offline-message-function
│
├── Program.cs
├── Projects.csproj
├── host.json
│
├── offlineMessageProcessor.cs
└── testTimer.cs
```

---

# 2.1 Install Azure function tool in local
 * sudo npm install -g azure-functions-core-tools@4 --unsafe-perm true    
 * func new (to create new function)


# 3. HTTP Trigger Function

Created HTTP trigger:

```csharp
[Function("offlineMessageProcesser")]
public HttpResponseData Run(
    [HttpTrigger(
        AuthorizationLevel.Anonymous,
        "get",
        "post")] HttpRequestData req)
{
}
```

Local test:

```bash
func start
```

Output:

```
offlineMessageProcesser: [GET,POST]
```

---

# 4. Add Timer Trigger Function

Created second function:

```csharp
[Function("testTimer")]
public void Run(
    [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
{
}
```

This runs every minute.

For daily 2 PM later:

```csharp
[TimerTrigger("0 0 14 * * *")]
```

---

# 5. Fix local Timer Storage issue

Local error:

```
Could not create BlobContainerClient for ScheduleMonitor
```

Reason:

Timer needs Azure Storage.

Added:

```
local.settings.json
```

with:

```
AzureWebJobsStorage=<storage connection string>
```

After that:

```bash
func start
```

worked:

```
testTimer: timerTrigger
```

---

# 6. Deploy local project to Azure

First tried portal ZIP deployment.

Created publish:

```bash
dotnet publish -c Release
```

Created zip:

```bash
cd bin/Release/net8.0/publish

zip -r functionapp.zip .
```

Uploaded through:

```
Azure Portal
 → Function App
 → Deployment Center
 → Zip Deploy
```

---

# 7. Fix .NET isolated deployment issue

Initial problem:

```
Could not find the .azurefunctions folder
```

Reason:
ZIP package was missing Azure Functions metadata.

Fixed by deploying using Core Tools:

```bash
func azure functionapp publish abctestfunc
```

This created proper deployment artifacts:

```
.azurefunctions
worker.config.json
host.json
Projects.dll
```

---

# 8. Configure Azure Storage

Created Storage Account.

Checked:

```
Storage Account
 → Access keys
 → Connection string
```

Added in Function App:

```
abctestfunc
 → Environment variables
 → AzureWebJobsStorage
```

Value:

```
DefaultEndpointsProtocol=https;
AccountName=xxx;
AccountKey=xxx;
EndpointSuffix=core.windows.net
```

Restarted:

```
Function App
 → Overview
 → Restart
```

---

# 9. Verify Function App health

Logs showed:

Successful:

```
Host Status:
state: Running
```

Timer executed:

```
C# Timer trigger function executed
```

No more:

```
Unable to create client for AzureWebJobsStorage
```

---

# 10. Final architecture

Your current design:

```
abctestfunc (Function App)
│
├── offlineMessageProcesser
│       HTTP Trigger
│
├── testTimer
│       Timer Trigger
│
└── processBlobFile
        Blob Trigger
```

No need for separate Function Apps.

---

# 11. Next Blob Trigger step

Add another C# file:

```
ProcessBlobFile.cs
```

Example:

```csharp
[Function("processBlobFile")]
public void Run(
    [BlobTrigger(
        "uploads/{name}",
        Connection = "AzureWebJobsStorage")] Stream blob,
    string name)
{
}
```

Create container:

```
Storage Account
 → Containers
 → uploads
```

Upload file:

```
test.txt
```

Function fires automatically.

---

# Final working commands you used

Start locally:

```bash
func start
```

Publish/deploy:

```bash
func azure functionapp publish abctestfunc
```

Create release build:

```bash
dotnet publish -c Release
```

---

This is now a standard Azure Functions setup: one Function App hosting HTTP + Timer + Blob triggers.
