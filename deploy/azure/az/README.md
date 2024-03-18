# Deploying resources using AZ CLI

Execute from terminal

az deployment group create --name [deploymentName] --resource-group [RgName] --parameters [PathToBicepFile]

e.g 

az deployment group create --name DevBlobStorageDeployment --resource-group rg-testassignment-dev-001 --parameters BlobStorage/dev.bicepparam

az deployment group create --name BlobImageFunctions --resource-group rg-testassignment-dev-001 --parameters FunctionApps/BlobImageFunctions/dev.bicepparam