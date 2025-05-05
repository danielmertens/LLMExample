# Deploy.ps1 - Script to deploy the application to Azure

# Step 1: Build the application
Write-Host "Building the application..."
dotnet publish LLMExample.Web/LLMExample.Web.csproj -c Release -o ./publish

# Step 2: Zip the publish folder
Write-Host "Zipping the publish folder..."
$publishPath = "z:\source\LLMExample\publish"
$zipPath = "z:\source\LLMExample\publish.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath
}
Compress-Archive -Path "$publishPath\*" -DestinationPath $zipPath

# Step 3: Deploy the zip file to Azure
Write-Host "Deploying to Azure..."
az webapp deploy --resource-group rg-daniel-ai --name protime-llm-example --src-path $zipPath

# Step 4: Verify the deployment
Write-Host "Verifying the deployment..."
$webAppUrl = az webapp show --resource-group rg-daniel-ai --name protime-llm-example --query "defaultHostName" -o tsv
Write-Host "Application deployed successfully. Access it at: https://$webAppUrl"
