using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CRM.Plugins.Practice.Account
{
    public class PostOperationAccount_UpdateEmailOnAccountCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                // Obtain the tracing service
                ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Obtain the execution context from the service provider.  
                IPluginExecutionContext context = (IPluginExecutionContext)
                    serviceProvider.GetService(typeof(IPluginExecutionContext));

                // The InputParameters collection contains all the data passed in the message request.  
                if (context.InputParameters.Contains("Target") &&
                    context.InputParameters["Target"] is Entity)
                {
                    // Obtain the target entity from the input parameters.  
                    // Entity entity = (Entity)context.InputParameters["Target"];

                    // Obtain the organization service reference which you will need for  
                    // web service calls.  
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    try
                    {
                        //tracingService.Trace($"Pre-Operation Contact DuplicateRecord Execution Started:{DateTime.Now}");

                        Entity account = new Entity("account");
                        //account["name"] = context.SharedVariables["firstname"];
                        account["emailaddress1"] = context.SharedVariables["emailaddress"];
                        service.Update(account);

                        //tracingService.Trace($"Pre-Operation Contact DuplicateRecord Execution Ended:{DateTime.Now}");
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Create Contact.", ex);
                    }
                    catch (Exception ex)
                    {
                        tracingService.Trace("Plugin: {0}", ex.ToString());
                        throw;
                    }
                }
            }
        }
    }
}