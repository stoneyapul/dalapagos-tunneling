@description('The location to deploy all my resources')
param location string = resourceGroup().location

@description('The name of the log analytics workspace')
param logAnalyticsWorkspaceName string

@description('The name of the Application Insights workspace')
param appInsightsName string

@description('The name of the Container Registry.')
param containerRegistryName string

@description('The name of the Key Vault.')
param keyVaultName string

@description('The name of the Sql Server.')
param sqlServerName string

@description('The password of the Sql Server.')
param sqlServerPass string

@description('The name of the Sql DB.')
param sqlDBName string

var tags = {
  env: 'production'
  own: 'dalapagos'
  app: 'tunneling'
}

var sqlConn = 'Server=tcp:${sqlServerName}.database.windows.net,1433;Initial Catalog=${sqlDBName};Persist Security Info=False;User ID=sqladmin;Password=${sqlServerPass};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

module logAnalytics 'log-analytics.bicep' = {
  name: 'law'
  params: {
    location: location 
    logAnalyticsWorkspaceName: logAnalyticsWorkspaceName
    tags: tags
  }
}

module keyVault 'key-vault.bicep' = {
  name: 'kv'
  params: {
    keyVaultName: keyVaultName
    location: location
    tags: tags
  }
}

module keyVaultRoles 'key-vault-role-assignment.bicep' = {
  name: 'kvroles'
  params: {
    keyVaultName: keyVaultName
    principalId: '9a362683-12ec-4061-ab49-00183a3a9d00'
    roleIds: ['2e2d6524-3c3f-4973-957c-bfeb7ba86a34']
  }
}

module dbSecret 'db-connect-secret.bicep' = {
  name: 'kv-secret-dbconnect'
  params: {
    keyVaultName: keyVault.outputs.name
    name: 'db-connect'
    value: sqlConn
  }
}

module appInsights 'app-insights.bicep' = {
  name: 'appins'
  params: {
    appInsightsName: appInsightsName
    keyVaultName: keyVault.outputs.name
    location: location
    logAnalyticsName: logAnalytics.outputs.name
    tags: tags
  }
}

module containerRegistry 'container-registry.bicep' = {
  name: 'acr'
  params: {
    containerRegistryName: containerRegistryName
    location: location
    tags: tags
  }
}

module sqlServer 'sql-server.bicep' = {
  name: 'sql'
  params: {
    location: location
    serverName: sqlServerName
    sqlDBName: sqlDBName
    administratorLogin: 'sqladmin'
    administratorLoginPassword: sqlServerPass  
  }
}
