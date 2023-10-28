resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'temp897398027095626a'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}
