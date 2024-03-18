param accountName string = 'cosmos-${uniqueString(resourceGroup().id)}'

param location string = resourceGroup().location

param databaseName string

param principalId string 
param roleDefinitionId string

resource account 'Microsoft.DocumentDB/databaseAccounts@2023-11-15' = {
  name: toLower(accountName)
  location: location
  properties: {
    enableFreeTier: true
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        isZoneRedundant: false
      }
    ]
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2023-11-15' = {
  parent: account
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
    options: {
      throughput: 2000
    }
  }
}

resource cosmosDBRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2021-10-15' = {
  name: 'cosmosDBRoleAssignment'
  parent: account
  properties: {
    principalId: principalId
    roleDefinitionId: roleDefinitionId
    scope: '/'
  }
}



output location string = location
output resourceGroupName string = resourceGroup().name

