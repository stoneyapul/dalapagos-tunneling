@description('The location to deploy all my resources')
param location string = resourceGroup().location

@description('The name of the log analytics workspace')
param logAnalyticsWorkspaceName string

@description('The name of the Application Insights workspace')
param appInsightsName string

@description('The name of the Container App Environment.')
param containerAppEnvName string

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

module env 'container-app-env.bicep' = {
  name: 'env'
  params: {
    appInsightsName: appInsights.outputs.name
    containerAppEnvironmentName: containerAppEnvName
    location: location
    logAnalyticsName: logAnalytics.outputs.name
    tags: tags
  }
}
