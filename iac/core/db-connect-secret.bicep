param keyVaultName string
param name string
@secure()
param value string

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: keyVaultName
  resource secret 'secrets' = {
    name: name
    properties: {
      value: value
    }
  }
}
