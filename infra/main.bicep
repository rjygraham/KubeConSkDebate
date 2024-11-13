targetScope = 'subscription'

// The main bicep module to provision Azure resources.
// For a more complete walkthrough to understand how this file works with azd,
// see https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/make-azd-compatible?pivots=azd-create

@minLength(1)
@maxLength(64)
@description('Name of the the environment which is used to generate a short unique hash used in all resources.')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string

// Optional parameters to override the default azd resource naming conventions.
// Add the following to main.parameters.json to provide values:
// "resourceGroupName": {
//      "value": "myGroupName"
// }
param resourceGroupName string = ''

var abbrs = loadJsonContent('./abbreviations.json')

// tags that should be applied to all resources.
var tags = {
  // Tag all resources with the environment name.
  'azd-env-name': environmentName
}

// Generate a unique token to be used in naming resources.
// Remove linter suppression after using.
#disable-next-line no-unused-vars
var resourceToken = toLower(uniqueString(subscription().id, environmentName, location))

// Name of the service defined in azure.yaml
// A tag named azd-service-name with this value should be applied to the service host resource, such as:
//   Microsoft.Web/sites for appservice, function
// Example usage:
//   tags: union(tags, { 'azd-service-name': apiServiceName })
#disable-next-line no-unused-vars
var debateHostName = 'debate-host'
var leaderboarName = 'leaderboard'
var defaultAgentsName = 'default-agents'

// Organize resources in a resource group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: !empty(resourceGroupName) ? resourceGroupName : '${abbrs.resourcesResourceGroups}${environmentName}'
  location: location
  tags: tags
}

// Add resources to be provisioned below.
module monitoring 'core/monitor/monitoring.bicep' = {
  name: 'monitoring'
  scope: rg
  params: {
    location: location
    tags: tags
    logAnalyticsName: '${abbrs.logAnalyticsWorkspace}${resourceToken}'
    applicationInsightsName: '${abbrs.applicationInsights}${resourceToken}'
  }
}

// module keyVault 'core/security/keyvault.bicep' = {
//   name: 'keyVault'
//   scope: rg
//   params: {
//     location: location
//     tags: tags
//     name: '${abbrs.keyVaultVaults}${resourceToken}'
//   }
// }

module storage 'core/storage/storage-account.bicep' = {
  name: 'storage'
  scope: rg
  params: {
    location: location
    tags: tags
    name: '${abbrs.storageStorageAccounts}${resourceToken}'
  }
}

module aoai 'core/ai/cognitiveservices.bicep' = {
  name: 'aoai'
  scope: rg
  params: {
    location: location
    tags: tags
    name: '${abbrs.cognitiveServicesAccounts}${resourceToken}'
    deployments: [
      {
        name: 'gpt-40'
        model: {
          name: 'gpt-4o'
          version: '2024-08-06'
          format: 'OpenAI'
        }
        sku: {
          name: 'GlobalStandard'
          capacity: 450
        }
      }
    ]
  }
}

module containerRegistry 'core/host/container-registry.bicep' = {
  name: 'containerRegistry'
  scope: rg
  params: {
    location: location
    tags: tags
    name: '${abbrs.containerRegistryRegistries}${resourceToken}'
  }
}

// module aks 'core/host/aks-managed-cluster.bicep' = {
//   scope: rg
//   name: 'aks'
//   params: {
//     name: '${abbrs.kubernetesManagedClusters}${resourceToken}'
//     skuName: 'Automatic'
//     skuTier: 'Standard'
//     kubernetesVersion: '1.31.1'
//     enableRbac: true
//     enableAad: true
//     enableAzureRbac: true
//     networkDataplane: 'cilium'
//     networkPlugin: 'azure'
//     networkPolicy: 'calico'
//     workspaceId: monitoring.outputs.logAnalyticsWorkspaceId
//     webAppRoutingAddon: true
//     systemPoolConfig: {
//       name: 'systempool'
//       count: 3
//       mode: 'System'
//       vmSize: 'Standard_D4ds_v5'
//       availabilityZones: ['1', '3']
//     }
//   }
// }

module aks 'core/host/aks-automatic-cluster.bicep' = {
  scope: rg
  name: 'aks'
  params: {
    name: '${abbrs.kubernetesManagedClusters}${resourceToken}'
    location: location
    tags: tags
    clusterSku: {
      name: 'Automatic'
      tier: 'Standard'
    }
    enableRBAC: true
    nodeProvisioningProfile: {
      mode: 'Auto'
    }
    disableLocalAccounts: true
    azureRbac: true
    upgradeChannel: 'stable'
    nodeOSUpgradeChannel: 'NodeImage'
    supportPlan: 'KubernetesOfficial'
    enableContainerInsights: true
    omsAgentAddon: {
      enabled: true
      config: {
          logAnalyticsWorkspaceResourceID: monitoring.outputs.logAnalyticsWorkspaceId
          useAADAuth: 'true'
      }
    }
  }
}

module aksAcrAccess 'core/security/registry-access.bicep' = {
  scope: rg
  name: 'aksAcrAccess'
  params: {
    containerRegistryName: containerRegistry.outputs.name
    principalId: aks.outputs.clusterIdentity.objectId
  }
}

// module cloudNativeMonitoring 'core/monitor/cloud-native.bicep' = {
//   scope: rg
//   name: 'cloudNativeMonitoring'
//   params: {
//     grafanaDashbboardName: '${abbrs.grafanaDashboards}${resourceToken}'
//     monitorWorkspaceName: '${abbrs.monitorWorkspaces}${resourceToken}'
//   }
// }

// Add outputs from the deployment here, if needed.
//
// This allows the outputs to be referenced by other bicep deployments in the deployment pipeline,
// or by the local machine as a way to reference created resources in Azure for local development.
// Secrets should not be added here.
//
// Outputs are automatically saved in the local azd environment .env file.
// To see these outputs, run `azd env get-values`,  or `azd env get-values --output json` for json output.
output AZURE_LOCATION string = location
output AZURE_TENANT_ID string = tenant().tenantId
output AZURE_REGISTRY_NAME string = containerRegistry.outputs.name
output AZURE_REGISTRY_URI string = containerRegistry.outputs.loginServer
output AZURE_AKS_CLUSTER_NAME string = aks.outputs.clusterName
