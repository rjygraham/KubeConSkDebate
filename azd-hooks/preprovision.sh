#!/bin/bash

echo "Ensuring Azure CLI extensions and dependencies are installed"

# add azure cli extensions
az extension add --upgrade --name aks-preview

az provider register --namespace "Microsoft.ContainerService"
# while [[ $(az provider show --namespace "Microsoft.ContainerService" --query "registrationState" -o tsv) != "Registered" ]]; do
#   echo "Waiting for Microsoft.ContainerService provider registration..."
#   sleep 3
# done

# # Register the preview features
# az feature register --namespace Microsoft.ContainerService --name EnableAPIServerVnetIntegrationPreview
# az feature register --namespace Microsoft.ContainerService --name NRGLockdownPreview
# az feature register --namespace Microsoft.ContainerService --name SafeguardsPreview
# az feature register --namespace Microsoft.ContainerService --name NodeAutoProvisioningPreview
# az feature register --namespace Microsoft.ContainerService --name DisableSSHPreview
# az feature register --namespace Microsoft.ContainerService --name AutomaticSKUPreview

# # Wait for the features to register
# while [[ $(az feature show --namespace Microsoft.ContainerService --name EnableAPIServerVnetIntegrationPreview --query "properties.state" -o tsv) != "Registered" ]]; do
#   echo "Waiting for EnableAPIServerVnetIntegrationPreview feature registration..."
#   sleep 3
# done

# while [[ $(az feature show --namespace Microsoft.ContainerService --name NRGLockdownPreview --query "properties.state" -o tsv) != "Registered" ]]; do
#   echo "Waiting for NRGLockdownPreview feature registration..."
#   sleep 3
# done

# while [[ $(az feature show --namespace Microsoft.ContainerService --name SafeguardsPreview --query "properties.state" -o tsv) != "Registered" ]]; do
#   echo "Waiting for SafeguardsPreview feature registration..."
#   sleep 3
# done

# while [[ $(az feature show --namespace Microsoft.ContainerService --name NodeAutoProvisioningPreview --query "properties.state" -o tsv) != "Registered" ]]; do
#   echo "Waiting for NodeAutoProvisioningPreview feature registration..."
#   sleep 3
# done

# while [[ $(az feature show --namespace Microsoft.ContainerService --name DisableSSHPreview --query "properties.state" -o tsv) != "Registered" ]]; do
#   echo "Waiting for DisableSSHPreview feature registration..."
#   sleep 3
# done

# while [[ $(az feature show --namespace Microsoft.ContainerService --name AutomaticSKUPreview --query "properties.state" -o tsv) != "Registered" ]]; do
#   echo "Waiting for AutomaticSKUPreview feature registration..."
#   sleep 3
# done

# # Propagate the feature registrations
# az provider register -n Microsoft.ContainerService
