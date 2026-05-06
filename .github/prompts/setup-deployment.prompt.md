---
mode: agent
description: Provision the shared MCP environment and store credentials as GitHub Actions secrets. Triggered by "/setup-deployment".
---

# Setup Deployment

You are provisioning the shared MCP infrastructure for this repository and wiring it to GitHub Actions. Follow EVERY step exactly. Do NOT skip steps or reorder them.

---

## Step 1 — Collect inputs

Call the `vscode_askQuestions` tool with exactly these five questions:

```json
{
  "questions": [
    {
      "header": "DevEnvironmentName",
      "question": "Short name for the dev environment (e.g. mymcpdev). Rules: 5–22 characters, lowercase letters and digits only, no hyphens or underscores. Used as the Key Vault name, Container Apps environment name, Log Analytics name, and Storage Account prefix for the dev environment.",
      "allowFreeformInput": true
    },
    {
      "header": "ProdEnvironmentName",
      "question": "Short name for the prod environment (e.g. mymcpprod). Same rules as the dev name. Must be different from the dev name.",
      "allowFreeformInput": true
    },
    {
      "header": "AcrName",
      "question": "Short name for the shared Azure Container Registry (e.g. mymcpacr). Rules: 5–50 characters, alphanumeric only. Globally unique — this ACR is shared by both dev and prod. A 'rg-' prefix is used for the registry resource group name.",
      "allowFreeformInput": true
    },
    {
      "header": "Location",
      "question": "Azure region to deploy into.",
      "allowFreeformInput": true,
      "options": [
        { "label": "westeurope", "recommended": true },
        { "label": "northeurope" },
        { "label": "eastus" },
        { "label": "eastus2" },
        { "label": "swedencentral" }
      ]
    },
    {
      "header": "SubscriptionId",
      "question": "Azure subscription ID (GUID) to deploy resources into.",
      "allowFreeformInput": true
    }
  ]
}
```

Validate the inputs:
- **DevEnvironmentName** must match `^[a-z0-9]{5,22}$`. If it does not, stop and ask the user to correct it.
- **ProdEnvironmentName** must match `^[a-z0-9]{5,22}$` and must differ from **DevEnvironmentName**. If it does not, stop and ask the user to correct it.
- **AcrName** must match `^[a-z0-9]{5,50}$`. If it does not, stop and ask the user to correct it.
- **SubscriptionId** must be a valid GUID (`xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`). If it does not look like a GUID, stop and ask the user to correct it.

Derive the remaining resource names:

| Parameter | Value |
|---|---|
| `acrName` | `{AcrName}` (shared across dev and prod) |
| `acrResourceGroupName` | `rg-{AcrName}` |
| Dev `containerAppsEnvName` | `{DevEnvironmentName}` |
| Dev `keyVaultName` | `{DevEnvironmentName}` |
| Dev `logAnalyticsName` | `{DevEnvironmentName}` |
| Dev `storageAccountName` | `st{DevEnvironmentName}` |
| Dev `resourceGroupName` | `rg-{DevEnvironmentName}` |
| Prod `containerAppsEnvName` | `{ProdEnvironmentName}` |
| Prod `keyVaultName` | `{ProdEnvironmentName}` |
| Prod `logAnalyticsName` | `{ProdEnvironmentName}` |
| Prod `storageAccountName` | `st{ProdEnvironmentName}` |
| Prod `resourceGroupName` | `rg-{ProdEnvironmentName}` |

Echo the resolved values before proceeding:
> Provisioning **{DevEnvironmentName}** (dev) and **{ProdEnvironmentName}** (prod) with shared registry **{AcrName}** in **{Location}** — subscription `{SubscriptionId}`

---

## Step 2 — Verify prerequisites

Run `az version` and `gh --version`. If either command is not found, stop and tell the user which tool to install with a link:
- Azure CLI: https://learn.microsoft.com/cli/azure/install-azure-cli
- GitHub CLI: https://cli.github.com/

Verify an active Azure login by running:

```
az account show --query "name" -o tsv
```

If the command fails or returns nothing, stop and tell the user to run `az login` first.

Verify the GitHub CLI is authenticated by running:

```
gh auth status
```

If not authenticated, stop and tell the user to run `gh auth login` first.

---

## Step 3 — Set active subscription

```
az account set --subscription {SubscriptionId}
```

---

## Step 4 — Update Infrastructure/dev.bicepparam and Infrastructure/prod.bicepparam

Read `Infrastructure/dev.bicepparam`. Replace the entire file contents with the following, substituting the resolved values:

```bicep
using 'main.bicep'

// Shared registry — keep acrName and acrResourceGroupName identical in prod.bicepparam
param acrName             = '{AcrName}'
param acrResourceGroupName = 'rg-{AcrName}'

// Dev-environment resources
param containerAppsEnvName = '{DevEnvironmentName}'
param keyVaultName        = '{DevEnvironmentName}'
param logAnalyticsName    = '{DevEnvironmentName}'
param location            = '{Location}'
param resourceGroupName   = 'rg-{DevEnvironmentName}'
param storageAccountName  = 'st{DevEnvironmentName}'
```

Read `Infrastructure/prod.bicepparam`. Replace the entire file contents with the following, substituting the resolved values:

```bicep
using 'main.bicep'

// Shared registry — keep acrName and acrResourceGroupName identical to dev.bicepparam
param acrName             = '{AcrName}'
param acrResourceGroupName = 'rg-{AcrName}'

// Prod-environment resources
param containerAppsEnvName = '{ProdEnvironmentName}'
param keyVaultName        = '{ProdEnvironmentName}'
param logAnalyticsName    = '{ProdEnvironmentName}'
param location            = '{Location}'
param resourceGroupName   = 'rg-{ProdEnvironmentName}'
param storageAccountName  = 'st{ProdEnvironmentName}'
```

---

## Step 5 — Create service principal

Run the following command and **capture the JSON output into a variable**. Do NOT print the JSON to the terminal or display it in the chat — it contains the client secret.

```
az ad sp create-for-rbac --name "sp-mcp-{AcrName}" --json-auth --output json
```

Store the complete JSON output in a shell variable named `SP_JSON`. Do not write it to disk.

> **Note:** If your Azure CLI version does not support `--json-auth`, use `--sdk-auth` instead — it produces the same output.

---

## Step 6 — Assign Owner role to service principal

Extract the `clientId` field from `SP_JSON` and assign the `Owner` role to the service principal at subscription scope. This allows the GitHub Actions workflow to create role assignments for the MCP servers:

```
az role assignment create --assignee {clientId from SP_JSON} --role Owner --scope /subscriptions/{SubscriptionId}
```

The Bicep template will reference the deployment identity inline to assign the `AcrPush` role during the GitHub Actions deployment.

---

## Step 7 — Store credentials in GitHub

Pipe `SP_JSON` directly to the GitHub CLI without writing to disk or displaying in the terminal. This keeps the client secret entirely out of terminal history and the file system:

```
echo {SP_JSON} | gh secret set AZURE_CREDENTIALS --app actions
```

Set the shared ACR name as GitHub Actions repository variables for both environments (both point to the same registry):

```
gh variable set ACR_NAME_DEV --body "{AcrName}"
gh variable set ACR_NAME_PROD --body "{AcrName}"
```

Clear `SP_JSON` from the shell variable after use.

---

## Step 8 — Copy workflow templates to workflows folder

Copy the three GitHub Actions workflow templates from `.github/templates/` to `.github/workflows/`:

```
cp .github/templates/deploy-bicep.yml .github/workflows/
cp .github/templates/docker-deploy-containerapp-template.yml .github/workflows/
cp .github/templates/docker-publish-template.yml .github/workflows/
```

---

## Step 9 — Print completion checklist

Print a checklist of every action completed. Mark each item ✅:

```
✅ Infrastructure/dev.bicepparam updated
✅ Infrastructure/prod.bicepparam updated
✅ Service principal sp-mcp-{AcrName} created
✅ Owner role assigned to SP on subscription {SubscriptionId}
✅ AZURE_CREDENTIALS secret set in GitHub Actions
✅ ACR_NAME_DEV variable set to {AcrName}
✅ ACR_NAME_PROD variable set to {AcrName}
✅ Workflow templates copied to .github/workflows/
```

---

## Step 10 — Commit and push to trigger deployment

Review your changes:

```
git status
```

You should see `Infrastructure/dev.bicepparam` and `Infrastructure/prod.bicepparam` modified and the three workflow files in `.github/workflows/`. Commit and push the changes:

```
git add Infrastructure/dev.bicepparam Infrastructure/prod.bicepparam .github/workflows/
git commit -m "Configure MCP environments: {DevEnvironmentName} (dev) and {ProdEnvironmentName} (prod) with shared registry {AcrName}"
git push
```

This will trigger the **Deploy Bicep Template** workflow in GitHub Actions. The workflow will:
1. Deploy the shared infrastructure to Azure (ACR, Container Apps Environment, Key Vault, Log Analytics, etc.)

Monitor the workflow in the **Actions** tab of your repository.

Then print:
> **Next step:** Once the GitHub Actions workflow completes successfully, run **/new-mcp-server** to scaffold your first MCP server.
