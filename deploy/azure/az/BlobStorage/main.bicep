param location string
param storageAccountName string
param accountType string
param kind string
param minimumTlsVersion string
param supportsHttpsTrafficOnly bool
param allowBlobPublicAccess bool
param allowSharedKeyAccess bool
param defaultOAuth bool
param accessTier string
param publicNetworkAccess string
param allowCrossTenantReplication bool
param networkAclsBypass string
param networkAclsDefaultAction string
param dnsEndpointType string
param keySource string
param encryptionEnabled bool
param infrastructureEncryptionEnabled bool
param isContainerRestoreEnabled bool
param isBlobSoftDeleteEnabled bool
param isContainerSoftDeleteEnabled bool
param changeFeed bool
param isVersioningEnabled bool

param sourceImagesContainerName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: storageAccountName
  location: location
  tags: {}
  sku: {
    name: accountType
  }
  kind: kind
  properties: {
    minimumTlsVersion: minimumTlsVersion
    supportsHttpsTrafficOnly: supportsHttpsTrafficOnly
    allowBlobPublicAccess: allowBlobPublicAccess
    allowSharedKeyAccess: allowSharedKeyAccess
    defaultToOAuthAuthentication: defaultOAuth
    accessTier: accessTier
    publicNetworkAccess: publicNetworkAccess
    allowCrossTenantReplication: allowCrossTenantReplication
    networkAcls: {
      bypass: networkAclsBypass
      defaultAction: networkAclsDefaultAction
      ipRules: []
    }
    dnsEndpointType: dnsEndpointType
    encryption: {
      keySource: keySource
      services: {
        blob: {
          enabled: encryptionEnabled
        }
        file: {
          enabled: encryptionEnabled
        }
        table: {
          enabled: encryptionEnabled
        }
        queue: {
          enabled: encryptionEnabled
        }
      }
      requireInfrastructureEncryption: infrastructureEncryptionEnabled
    }
  }
  dependsOn: []
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2022-05-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    restorePolicy: {
      enabled: isContainerRestoreEnabled
    }
    deleteRetentionPolicy: {
      enabled: isBlobSoftDeleteEnabled
    }
    containerDeleteRetentionPolicy: {
      enabled: isContainerSoftDeleteEnabled
    }
    changeFeed: {
      enabled: changeFeed
    }
    isVersioningEnabled: isVersioningEnabled
  }
}

resource sourceContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-09-01' = {
  name: sourceImagesContainerName
  parent: blobService
  properties: {
    publicAccess: 'None'
    metadata: {}
  }
}
