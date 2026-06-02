using 'containerApp.bicep'

param imageName = 'kangaroomcp'
param appName = 'kangaroomcp'
param acrName = 'tarnmcpcontainer'
param environmentName = 'tarnmcptest'
param resourceGroupName = 'rg-tarnmcptest'
param keyVaultSecrets = [
  {
    key: 'kangaroomcpclientid' // Must be lowercase - used in secretRef
    value: 'KangarooMCPClientId' // PascalCase - actual Key Vault secret name
  }
  {
    key: 'kangaroomcpclientsecret' // Must be lowercase - used in secretRef
    value: 'KangarooMCPClientSecret' // PascalCase - actual Key Vault secret name
  }
  {
    key: 'kangaroomcptenantid' // Must be lowercase - used in secretRef
    value: 'KangarooMCPTenantId' // PascalCase - actual Key Vault secret name
  }
]
param environment = [
  {
    name: 'EntraIdAuth__TenantId'
    secretRef: 'kangaroomcptenantid'
  }
  {
    name: 'EntraIdAuth__ClientId'
    secretRef: 'kangaroomcpclientid'
  }
  {
    name: 'EntraIdAuth__ClientSecret'
    secretRef: 'kangaroomcpclientsecret'
  }
  {
    name: 'EntraIdAuth__PublicUrl'
    value: 'TODO-public-url-after-first-deploy'
  }
  {
    name: 'KangarooMCPApi__BaseUrl'
    value: 'https://api.ala.org.au/'
  }
  {
    name: 'IsTransportStateless'
    value: 'true'
  }
  // Application Insights connection string is automatically added by the template
]
