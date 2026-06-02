using 'main.bicep'

// Shared registry — keep acrName and acrResourceGroupName identical in prod.bicepparam
param acrName             = 'mcpcontainer'
param acrResourceGroupName = 'rg-mcpcontainer'

// Dev-environment resources
param containerAppsEnvName = 'mcptest'
param keyVaultName        = 'mcptest'
param logAnalyticsName    = 'mcptest'
param location            = 'swedencentral'
param resourceGroupName   = 'rg-mcptest'
param storageAccountName  = 'stmcptest'
