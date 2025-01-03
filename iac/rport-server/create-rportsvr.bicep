@description('The hub id.')
param hubId string

@description('The name of the Virtual Machine.')
param vmName string

@description('Location for all resources.')
param location string = resourceGroup().location

@description('The size of the VM.')
param vmSize string

@description('Username for the Virtual Machine.')
param adminUsername string = 'dlpgadmin'

@description('Password for the Virtual Machine.')
@secure()
param adminPassword string

module vmNetwork './deploy-vmnetwork.bicep' = {
  name: 'deployNetwork'
  params: {
    hubId: hubId
    vmName: vmName
    location: location
  }
}

module vm './deploy-vm.bicep' = {
  name: 'deployVM'
  params: {
    vmName: vmName
    location: location
    vmSize: vmSize
    networkInterfaceId: vmNetwork.outputs.networkInterfaceID
    adminUsername: adminUsername
    adminPassword: adminPassword
  }
}

@description('The public IP.')
output sshUserIp string = '${adminUsername}@${vmNetwork.outputs.publicIP}'
