using 'main.bicep'

// Shared registry — keep acrName and acrResourceGroupName identical to dev.bicepparam
param acrName             = 'tarnmcpcontainer'
param acrResourceGroupName = 'rg-tarnmcpcontainer'

// Prod-environment resources
param containerAppsEnvName = 'tarnmcpprod'
param keyVaultName        = 'tarnmcpprod'
param logAnalyticsName    = 'tarnmcpprod'
param location            = 'swedencentral'
param resourceGroupName   = 'rg-tarnmcpprod'
param storageAccountName  = 'sttarnmcpprod'
