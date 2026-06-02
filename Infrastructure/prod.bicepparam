using 'main.bicep'

// Shared registry — keep acrName and acrResourceGroupName identical to dev.bicepparam
param acrName             = 'mcpcontainer'
param acrResourceGroupName = 'rg-mcpcontainer'

// Prod-environment resources
param containerAppsEnvName = 'mcpprod'
param keyVaultName        = 'mcpprod'
param logAnalyticsName    = 'mcpprod'
param location            = 'swedencentral'
param resourceGroupName   = 'rg-mcpprod'
param storageAccountName  = 'stmcpprod'
