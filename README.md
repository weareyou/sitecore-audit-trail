# sitecore-audit-trail

Configuration before deployment:


Audit Trail Azure Functions Deploy
- Create a Function App in Azure, defaults are fine.
- Create a Signal R instance in Azure, again the defaults are fine.
- Create a CosmosDB account with shared provisioning. Database name "audit-trail", with a collection called "audit-records". Recommended partition key is "/Event".
- From Visual Studio, deploy the AuditTrail.Feature.AuditTrail.AzureFunctions project and set up a publish profile to the Function App. Set "deploy from zip" to true.
- Once the publish profile is made, open "Manage Application Settings..."
- Set variables "COSMOS_CONNECTION_STRING" and "AzureSignalRConnectionString" to their respective connection strings, found on Azure.


Audit Trail Sitecore Deploy



Audit Trail View (vue app)
- Set API domain and API key in the .env file in project root. (retrieved from Azure after Function deployment)
- npm install, npm run build, then upload dist folder to preferred hosting. (tested with Azure Blob Storage > Static Hosting)
