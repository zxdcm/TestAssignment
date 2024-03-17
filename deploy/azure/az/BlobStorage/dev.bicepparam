using 'main.bicep'

param location = 'polandcentral'
param storageAccountName = 'sttestassignment001'
param accountType = 'Standard_LRS'
param kind = 'StorageV2'
param sourceImagesContainerName = 'source-images'


param minimumTlsVersion = 'TLS1_2'
param supportsHttpsTrafficOnly = true
param allowBlobPublicAccess = false
param allowSharedKeyAccess = true
param defaultOAuth = false
param accessTier = 'Hot'
param publicNetworkAccess = 'Enabled'
param allowCrossTenantReplication = false
param networkAclsBypass = 'AzureServices'
param networkAclsDefaultAction = 'Allow'
param dnsEndpointType = 'Standard'
param keySource = 'Microsoft.Storage'
param encryptionEnabled = true
param infrastructureEncryptionEnabled = false
param isContainerRestoreEnabled = false
param isBlobSoftDeleteEnabled = false
param isContainerSoftDeleteEnabled = false
param changeFeed = false
param isVersioningEnabled = false
