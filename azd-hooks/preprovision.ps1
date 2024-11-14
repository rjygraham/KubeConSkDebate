#!/usr/bin/env pwsh

Write-Host "Ensuring Azure CLI extensions and dependencies are installed"

# add azure cli extensions
az extension add --upgrade --name "aks-preview"

az provider register --namespace "Microsoft.ContainerService"
while ((az provider show --namespace "Microsoft.ContainerService" --query "registrationState" -o tsv) -ne "Registered") {
  Write-Host "Waiting for Microsoft.ContainerService provider registration..."
  Start-Sleep -Seconds 3
}

# Register the preview features
az feature register --namespace "Microsoft.ContainerService" --name "EnableAPIServerVnetIntegrationPreview"
az feature register --namespace "Microsoft.ContainerService" --name "NRGLockdownPreview"
az feature register --namespace "Microsoft.ContainerService" --name "SafeguardsPreview"
az feature register --namespace "Microsoft.ContainerService" --name "NodeAutoProvisioningPreview"
az feature register --namespace "Microsoft.ContainerService" --name "DisableSSHPreview"
az feature register --namespace "Microsoft.ContainerService" --name "AutomaticSKUPreview"


while ((az feature show --namespace "Microsoft.ContainerService" --name "EnableAPIServerVnetIntegrationPreview" --query "properties.state" -o tsv) -ne "Registered") {
  Write-Host "Waiting for EnableAPIServerVnetIntegrationPreview feature registration..."
  Start-Sleep -Seconds 3
}

while ((az feature show --namespace "Microsoft.ContainerService" --name "NRGLockdownPreview" --query "properties.state" -o tsv) -ne "Registered") {
  Write-Host "Waiting for NRGLockdownPreview feature registration..."
  Start-Sleep -Seconds 3
}

while ((az feature show --namespace "Microsoft.ContainerService" --name "SafeguardsPreview" --query "properties.state" -o tsv) -ne "Registered") {
  Write-Host "Waiting for SafeguardsPreview feature registration..."
  Start-Sleep -Seconds 3
}

while ((az feature show --namespace "Microsoft.ContainerService" --name "NodeAutoProvisioningPreview" --query "properties.state" -o tsv) -ne "Registered") {
  Write-Host "Waiting for NodeAutoProvisioningPreview feature registration..."
  Start-Sleep -Seconds 3
}

while ((az feature show --namespace "Microsoft.ContainerService" --name "DisableSSHPreview" --query "properties.state" -o tsv) -ne "Registered") {
  Write-Host "Waiting for DisableSSHPreview feature registration..."
  Start-Sleep -Seconds 3
}

while ((az feature show --namespace "Microsoft.ContainerService" --name "AutomaticSKUPreview" --query "properties.state" -o tsv) -ne "Registered") {
  Write-Host "Waiting for AutomaticSKUPreview feature registration..."
  Start-Sleep -Seconds 3
}

# propagate the feature registrations
az provider register --namespace "Microsoft.ContainerService"

# Check kubelogin and install if not exists
if (-not (Get-Command kubelogin -ErrorAction SilentlyContinue)) {
  az aks install-cli
}