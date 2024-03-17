param namePrefix string

param blobStorageName string

param azureFunctionName string

param containerName string

param location string = resourceGroup().location

resource eventTopic 'Microsoft.EventGrid/systemTopics@2022-06-15' = {
  name: '${namePrefix}-topic'
  location: location
  properties: {
    source: resourceId('Microsoft.Storage/storageAccounts', blobStorageName)
    topicType: 'Microsoft.Storage.StorageAccounts'
  }
}

// TODO: fix resourceId ?

// resource topicEvent 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2022-06-15' = {
//   parent: eventTopic
//   name: '${namePrefix}-event'
//   properties: {
//     destination: {
//       endpointType: 'AzureFunction'
//       properties: {
//         resourceId: resourceId('Microsoft.Web/sites', azureFunctionName)
//       }
//     }
//     filter: {
//       includedEventTypes: [
//         'Microsoft.Storage.BlobCreated'
//       ]
//       subjectBeginsWith: '/blobServices/default/containers/${containerName}'
//       enableAdvancedFilteringOnArrays: false
//     }
//     eventDeliverySchema: 'EventGridSchema'
//     retryPolicy: {
//       maxDeliveryAttempts: 5
//       eventTimeToLiveInMinutes: 1440
//     }
//   }
// }
