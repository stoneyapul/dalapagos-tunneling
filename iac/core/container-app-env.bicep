@description('The name of the Container App Environment.')
param containerAppEnvironmentName string

@description('The name of the Container App that will be deployed.')
param containerAppName string

@description('The name of the Container App Managed Identity.')
param userIdentityName string

@description('The name of the Registry.')
param containerRegistryName string

@description('The name of the Log Analytics workspace that this Container App environment sends logs to.')
param logAnalyticsName string

@description('The name of the App Insights that this Container App Environment will send logs to.')
param appInsightsName string

@description('The location that the Container App Environment will be deployed.')
param location string

var tags = {
  env: 'production'
  own: 'dalapagos'
  app: 'tunneling'
}

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' existing = {
  name: logAnalyticsName
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: userIdentityName
  location: location 
}

resource roleAssignment1 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, containerRegistryName, 'AcrPullUserAssigned')
  properties: {
    principalId: identity.properties.principalId  
    principalType: 'ServicePrincipal'
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
  }
}

resource roleAssignment2 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, containerRegistryName, 'KeyVaultUserAssigned')
  properties: {
    principalId: identity.properties.principalId  
    principalType: 'ServicePrincipal'
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', '00482a5a-887f-4fb3-b363-3b7fe8e74483')
  }
}

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-11-02-preview' = {
  name: containerAppEnvironmentName
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
    appInsightsConfiguration: {
      connectionString: appInsights.properties.ConnectionString
    }
    openTelemetryConfiguration: {
      tracesConfiguration: {
        destinations: [
          'appInsights'
        ]
      }
      logsConfiguration: {
        destinations: [
          'appInsights'
        ]
      }
    }
  }
}

resource containerApp 'Microsoft.App/containerApps@2022-06-01-preview' = {
  name: containerAppName
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identity.id}': {}
    }
  }
  properties: {
    environmentId: containerAppEnvironment.id
    configuration: {
      secrets: [
        {
          value: 'https://dlpg-key-vault.vault.azure.net/secrets/db-connect'
          name: 'db-connect'
        }
      ]
      ingress: {
        targetPort: 8080
        external: true
      }
      registries: [
        {
          server: '${containerRegistryName}.azurecr.io'
          identity: identity.id
        }
      ]
    }
    template: {
      containers: [
        {
          image: '${containerRegistryName}.azurecr.io/${containerAppName}:latest'
          name: containerAppName
           env: [
            {
              name: 'ConnectionStrings__TunnelsDb'
              secretRef: 'db-connect'
            }
          ]
          resources: {
            cpu: 1
            memory: '2Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 2
      }
    }
  }
}
@description('The name of the Container App Environment.')
output containerAppEnvName string = containerAppEnvironment.name

@description('The resource Id of the Container App Environment.')
output containerAppEnvId string = containerAppEnvironment.id
