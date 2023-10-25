param location string = resourceGroup().location
param serverName string = 'modernrecrut120'

param dbUser string = 'ModernRecrutAdmin'
@minLength(10)
@maxLength(20)
@secure()
param dbPassword string
param storageAccountName string = 'stdocuments120'
param containerName string = 'images'

var AppSpecs =[
  {
   name:'mvc'
   sku:'F1'
  }
  {
    name:'postulation'
    sku:'F1'
   }
   {
    name:'emplois'
    sku:'F1'
   }
   {
    name:'documents'
    sku:'F1'
   }
   {
    name:'favoris'
    sku:'F1'
   }
]

var dbSettings  = [
    {
    name:'postulation'
    sku:'Standard'
   }
   {
    name:'emplois'
    sku:'Basic'
   }
]

module AppServices 'Modules/appService.bicep' = [for AppSpec in AppSpecs: {
  name: AppSpec.name
  params: {
    appName : AppSpec.name
    location: location
    sku:AppSpec.sku
  }
}]

module Databases 'Modules/sqlDatabases.bicep' = {
    name: 'Databases'
    params:{
        location:location
        serverName:serverName
        dbSettings:dbSettings
        dbUser:dbUser
        dbPassword:dbPassword
    }
}

module StorageAcount 'Modules/StorageAccount.bicep' = {
    name:'Storage'
    params:{
        location:location
        storageAccountName:storageAccountName
        containerName:containerName
    }
}
