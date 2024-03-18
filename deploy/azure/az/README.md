# Deploying resources using AZ CLI

Execute from terminal

az deployment group create --name [deploymentName] --resource-group [RgName] --parameters [PathToBicepFile]

e.g 

az deployment group create --name DevBlobStorageDeployment --resource-group rg-testassignment-dev-001 --parameters BlobStorage/dev.bicepparam

az deployment group create --name BlobImageResizer --resource-group rg-testassignment-dev-001 --parameters FunctionApps/BlobImageResizer

# Local dev 

# CosmosDB read-write assignment example

az cosmosdb sql role assignment create -a testassignment-feed-dev -g rg-testassignment-dev-001 -s "/" -p <principal> -d 00000000-0000-0000-0000-000000000002
