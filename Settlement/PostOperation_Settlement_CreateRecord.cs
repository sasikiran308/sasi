// <copyright file="PostOperation_Settlement_CreateRecord.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation Settlement Create Record Plugin.</summary>
namespace CRM.Plugins.Practice.Settlement
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// Post Operation Settlement Create Record
    /// </summary>
    public class PostOperation_Settlement_CreateRecord : IPlugin
    {
        /// <summary>
        /// Execute(IServiceProvider serviceProvider)
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
                        if (entity.Attributes.Contains("cdi_roletype"))
                        {
                            CreateSettlementPartyRecord(service, entity);
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

        /// <summary>
        /// Implements the Create Settlement Party Record
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="entity">Entity entity</param>
        /// <param name="tracingService">tracing Service</param>
        private static void CreateSettlementPartyRecord(IOrganizationService service, Entity entity)
        {
            var name = entity.Attributes.Contains("cdi_name") ? Convert.ToString(entity.Attributes["cdi_name"], CultureInfo.InvariantCulture) : string.Empty;
            var roleType = (OptionSetValue)entity.Attributes["cdi_roletype"];
            if (entity.Attributes.Contains("cdi_roletype"))
            {
                Entity settlementParty = new Entity("cdi_settlementparty");
                settlementParty["cdi_name"] = name;
                settlementParty["cdi_roletype"] = roleType;
                settlementParty["cdi_settlement"] = new EntityReference(entity.LogicalName, entity.Id);
                service.Create(settlementParty);
            }
        }
    }
}
