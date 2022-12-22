// <copyright file="PostOperationAccount_Create_StatusUpdate.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create DuplicateReocrd Check Plugin.</summary>
namespace CRM.Plugins.Practice
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using System.Globalization;

    /// <summary>
    /// Post Operation Account Create Status Update
    /// </summary>
    public class PostOperationAccount_Create_StatusUpdate : IPlugin
    {
        /// <summary>
        /// Execute(service Provider)
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider serviceProvider</param>
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
                    Entity entity = (Entity)context.InputParameters["Target"];

                    // Obtain the organization service reference which you will need for  
                    // web service calls.  
                    IOrganizationServiceFactory serviceFactory =
                        (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    try
                    {
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation Account Create StatusUpdate Execution Started:{0}", DateTime.Now));

                        if (entity.Attributes.Contains("cdi_accountcurrentstatus"))
                        {
                            if (entity.Attributes.Contains("primarycontactid"))
                            {
                                Entity entity1 = new Entity("contact");
                                entity1.Id = entity.GetAttributeValue<EntityReference>("primarycontactid").Id;
                                entity1["cdi_contactcurrentstatus"] = (OptionSetValue)entity.Attributes["cdi_accountcurrentstatus"];
                                entity1["parentcustomerid"] = new EntityReference("account", entity.Id);
                                service.Update(entity1);
                            }
                            else
                            {
                                throw new InvalidPluginExecutionException("Primary Contact field should not be null.");
                            }
                        }

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation Account Create StatusUpdate Execution Ended:{0}", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Post Operation Account Create Status Update.", ex);
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