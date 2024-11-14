#!/bin/bash

# Assign AKS RBAC Cluster Admin role to current executing principal
az role assignment create --assignee "$AZURE_PRINCIPAL_ID" \
  --role "Azure Kubernetes Service RBAC Cluster Admin" \
  --scope "$AZURE_AKS_CLUSTER_ID"

services=("KubeCon.Sk.Debate.Host" "KubeCon.Sk.Debate.Leaderboard" "KubeCon.Sk.Debate.DefaultAgents")

if [ "$DEPLOY_AZURE_CONTAINER_REGISTRY" == "true" ] && [ "$BUILD_CONTAINERS" == "true" ]; then
  echo "Build container images"
  for service in "${services[@]}"; do
    echo "Building kubecon-sk-debate/${service}:latest"
    az acr build --registry ${AZURE_REGISTRY_NAME} --image kubecon-sk-debate/${service}:latest ./src/${service}/
  done
else 
  echo "No BUILD_CONTAINERS variable set, skipping container build"
fi
