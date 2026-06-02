using 'main.bicep'

// Shared registry — keep acrName and acrResourceGroupName identical in prod.bicepparam
param acrName             = 'tarnmcpcontainer'
param acrResourceGroupName = 'rg-tarnmcpcontainer'

// Dev-environment resources
param containerAppsEnvName = 'tarnmcptest'
param keyVaultName        = 'tarnmcptest'
param logAnalyticsName    = 'tarnmcptest'
param location            = 'swedencentral'
param resourceGroupName   = 'rg-tarnmcptest'
param storageAccountName  = 'sttarnmcptest'
