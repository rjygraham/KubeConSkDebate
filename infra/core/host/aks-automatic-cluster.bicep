metadata description = 'Creates an Azure Kubernetes Service (AKS) Automatic cluster.'

@description('The name for the AKS managed cluster')
param name string

@description('The location of AKS resource.')
param location string

param tags object = {}

@description('The managed cluster SKU tier.')
param clusterSku object = { 
  name: 'Automatic'
  tier: 'Standard'
}

@description('Enable Microsoft Entra ID Profile.')
param enableAadProfile bool = false

@description('Whether to enable Kubernetes Role-Based Access Control.')
param enableRBAC bool = true

@description('The name of the resource group containing agent pool nodes.')
param nodeResourceGroup string = ''

@description('Node resource group lockdown profile for a managed cluster.')
param nodeResourceGroupProfile object = {
  restrictionLevel: 'ReadOnly'
}

@description('The node provisioning mode.')
param nodeProvisioningProfile object = {
  mode: 'Auto'
}

@description('An array of Microsoft Entra group object ids to give administrative access.')
param adminGroupObjectIDs array = []

@description('Enable or disable Azure RBAC.')
param azureRbac bool = false

@description('Enable or disable local accounts.')
param disableLocalAccounts bool = false

@description('Auto upgrade channel for a managed cluster.')
@allowed(['node-image', 'none', 'patch', 'rapid', 'stable'])
param upgradeChannel string = 'stable'

@description('Auto upgrade channel for node OS security.')
@allowed(['NodeImage', 'None', 'SecurityPatch', 'Unmanaged'])
param nodeOSUpgradeChannel string = 'NodeImage'

@description('The support plan for a managed cluster.')
@allowed([	'AKSLongTermSupport', 'KubernetesOfficial'])
param supportPlan string = 'KubernetesOfficial'

param workspaceId string = ''

param enableContainerInsights bool = false
param omsAgentAddon object = {}

var defaultAadProfile = {
  managed: true
  enableAzureRBAC: azureRbac
  adminGroupObjectIDs: adminGroupObjectIDs
}

resource aks 'Microsoft.ContainerService/managedClusters@2024-03-02-preview' = {
  location: location
  tags: tags
  name: name
  sku: clusterSku
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    enableRBAC: enableRBAC
    nodeResourceGroup: nodeResourceGroup
    nodeResourceGroupProfile: nodeResourceGroupProfile
    nodeProvisioningProfile: nodeProvisioningProfile
    disableLocalAccounts: disableLocalAccounts
    aadProfile: (enableAadProfile ? defaultAadProfile : null)
    autoUpgradeProfile: {
      upgradeChannel: upgradeChannel
      nodeOSUpgradeChannel: nodeOSUpgradeChannel
    }
    agentPoolProfiles: [
      {
        name: 'systempool'
        mode: 'System'
        count: 3
      }
    ]
    addonProfiles: {
      omsagent: enableContainerInsights ? omsAgentAddon : {}
    }
    supportPlan: supportPlan
  }
}

var aksDiagCategories = [
  'cluster-autoscaler'
  'kube-controller-manager'
  'kube-audit-admin'
  'guard'
]

// TODO: Update diagnostics to be its own module
// Blocking issue: https://github.com/Azure/bicep/issues/622
// Unable to pass in a `resource` scope or unable to use string interpolation in resource types
resource diagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = if (!empty(workspaceId)) {
  name: 'aks-diagnostics'
  scope: aks
  properties: {
    workspaceId: workspaceId
    logs: [for category in aksDiagCategories: {
      category: category
      enabled: true
    }]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

@description('The resource name of the AKS cluster')
output clusterName string = aks.name

@description('The resource ID of the AKS cluster')
output clusterId string = aks.id

@description('The AKS cluster identity')
output clusterIdentity object = {
  clientId: aks.properties.identityProfile.kubeletidentity.clientId
  objectId: aks.properties.identityProfile.kubeletidentity.objectId
  resourceId: aks.properties.identityProfile.kubeletidentity.resourceId
}
