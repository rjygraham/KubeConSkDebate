#!/usr/bin/env pwsh



# ###########################################################
# # Create the custom-values.yaml file
# ###########################################################
# @"
# namespace: ${env:AZURE_AKS_NAMESPACE}
# "@ | Out-File -FilePath custom-values.yaml -Encoding utf8

# ###########################################################
# # Add Azure Managed Identity and set to use AzureAD auth 
# ###########################################################
# if (![string]::IsNullOrEmpty($env:AZURE_IDENTITY_CLIENT_ID) -and ![string]::IsNullOrEmpty($env:AZURE_IDENTITY_NAME)) {
# @"
# useAzureAd: true
# managedIdentityName: $($env:AZURE_IDENTITY_NAME)
# managedIdentityClientId: $($env:AZURE_IDENTITY_CLIENT_ID)
# "@ | Out-File -Append custom-values.yaml
# }

# ###########################################################
# # Add base images
# ###########################################################
# @"
# namespace: ${env:AZURE_AKS_NAMESPACE}
# debate-host:
#   image:
#     repository: ${env:AZURE_REGISTRY_URI}/kubecon-sk-debate/debate-host
# leaderboard:
#   image:
#     repository: ${env:AZURE_REGISTRY_URI}/kubecon-sk-debate/leaderboard
# default-agents:
#   image:
#     repository: ${env:AZURE_REGISTRY_URI}/kubecon-sk-debate/default-agents
# "@ | Out-File -Append custom-values.yaml