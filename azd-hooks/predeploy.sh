#!/bin/bash

##########################################################
# Check kubelogin and install if not exists
##########################################################
if ! command -v kubelogin &> /dev/null; then
  echo "kubelogin could not be found. Installing kubelogin..."
  az aks install-cli
fi

##########################################################
# Create the custom-values.yaml file
##########################################################
cat << EOF > custom-values.yaml
namespace: ${AZURE_AKS_NAMESPACE}
EOF

###########################################################
# Add Azure Managed Identity and set to use AzureAD auth 
###########################################################
if [ -n "${AZURE_IDENTITY_CLIENT_ID}" ] && [ -n "${AZURE_IDENTITY_NAME}" ]; then
  cat << EOF >> custom-values.yaml
useAzureAd: true
managedIdentityName: ${AZURE_IDENTITY_NAME}
managedIdentityClientId: ${AZURE_IDENTITY_CLIENT_ID}
EOF
fi

##########################################################
# Add base images
##########################################################
cat << EOF >> custom-values.yaml
namespace: ${AZURE_AKS_NAMESPACE}
debate-host:
  image:
    repository: ${AZURE_REGISTRY_URI}/kubecon-sk-debate/debate-host
leaderboard:
  image:
    repository: ${AZURE_REGISTRY_URI}/kubecon-sk-debate/leaderboard
default-agents:
  image:
    repository: ${AZURE_REGISTRY_URI}/kubecon-sk-debate/default-agents
EOF
