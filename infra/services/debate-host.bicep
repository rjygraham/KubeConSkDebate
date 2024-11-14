param location string = resourceGroup().location
param tags object = {}
param managedIdentityName string
param storageAccountName string
param cognitiveServicesAccountName string

var storageBlobDataContributorRoleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
var storageTableDataContributorRoleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3')
var cognitiveServicesDataContributorRoleDefinitionId = resourceId('Microsoft.Authorization/roleDefinitions', '19c28022-e58e-450d-a464-0b2a53034789')

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: storageAccountName
}

resource aoai 'Microsoft.CognitiveServices/accounts@2024-06-01-preview' existing = {
  name: cognitiveServicesAccountName
}

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-07-31-preview' = {
  name: managedIdentityName
  location: location
  tags: tags
}

// Assign the storage blob data contributor role to the managed identity
resource storageBlobDataContributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(managedIdentity.id, storage.id, storageBlobDataContributorRoleDefinitionId)
  scope: storage
  properties: {
    principalId: managedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: storageBlobDataContributorRoleDefinitionId
  }
}

// Assign the storage table data contributor role to the managed identity
resource storageTableDataContributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(managedIdentity.id, storage.id, storageTableDataContributorRoleDefinitionId)
  scope: storage
  properties: {
    principalId: managedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: storageTableDataContributorRoleDefinitionId
  }
}

// Assign the cognitive services data contributor role to the managed identity
resource cognitiveServicesDataContributorRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(managedIdentity.id, aoai.id, cognitiveServicesDataContributorRoleDefinitionId)
  scope: aoai
  properties: {
    principalId: managedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: cognitiveServicesDataContributorRoleDefinitionId
  }
}

output managedIdentityPrincipalId string = managedIdentity.properties.principalId
