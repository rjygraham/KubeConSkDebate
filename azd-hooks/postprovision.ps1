#!/usr/bin/env pwsh

$services=@("KubeCon.Sk.Debate.Host", "KubeCon.Sk.Debate.Leaderboard", "KubeCon.Sk.Debate.DefaultAgents")

if (($env:DEPLOY_AZURE_CONTAINER_REGISTRY -like "true") -and ($env:BUILD_CONTAINERS -like "true")) {
  Write-Output "Build container images"
  foreach ($service in $services) {
    Write-Output "Building kubecon-sk-debate/${service}:latest"
    az acr build --registry $env:AZURE_REGISTRY_NAME --image kubecon-sk-debate/${service}:latest ./src/${service}/
  }
} 
else {
  Write-Output "No BUILD_CONTAINERS variable set, skipping container build"
}
