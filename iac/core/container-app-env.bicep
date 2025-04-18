@description('The name of the Container App Environment.')
param containerAppEnvironmentName string

@description('The name of the Container App that will be deployed.')
param containerAppName string

@description('The name of the Container App Managed Identity.')
param userIdentityName string

@description('The name of the Key Vault.')
param keyVaultName string

@description('The name of the Registry.')
param containerRegistryName string

@description('The name of the Log Analytics workspace that this Container App environment sends logs to.')
param logAnalyticsName string

@description('The location that the Container App Environment will be deployed.')
param location string

var tags = {
  env: 'production'
  own: 'dalapagos'
  app: 'tunneling'
}

var roleIds = [
  '7f951dda-4ed3-4680-a7ca-43fe172d538d'
  '00482a5a-887f-4fb3-b363-3b7fe8e74483'
]

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' existing = {
  name: logAnalyticsName
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: userIdentityName
  location: location 
}

resource roleAssignments 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for roleId in roleIds: {
  name: guid(resourceGroup().id, containerRegistryName, roleId)
  properties: {
    principalId: identity.properties.principalId  
    principalType: 'ServicePrincipal'
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleId)
  }
}]

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
  }
}

resource containerApp 'Microsoft.App/containerApps@2024-10-02-preview' = {
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
          keyVaultUrl: 'https://${keyVaultName}.vault.azure.net/secrets/db-connect'
          name: 'db-connect'
          identity: identity.id
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
