// <copyright file="PostOperation_SettlementParty_Update.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation Settlement Party Update Plugin.</summary>
namespace CRM.Plugins.Practice.Settlement_Party
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation Settlement Party Update
    /// </summary>
    public class PostOperation_SettlementParty_Update : IPlugin
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
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // Obtain the target entity from the input parameters.  
                    // Entity entity = (Entity)context.InputParameters["Target"];

                    // Obtain the organization service reference which you will need for  
                    // web service calls.  
                    IOrganizationServiceFactory serviceFactory =
                        (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    try
                    {
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation SettlementParty Update Execution Started:{0}", DateTime.Now));
                        Entity postImage = (Entity)context.PostEntityImages["PostImage"];
                        if (postImage.Contains("cdi_settlement"))
                        {
                            Guid id = ((EntityReference)postImage.Attributes["cdi_settlement"]).Id;
                            UpdateSettlementRecord(service, id);
                        }

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation SettlementParty Update Execution Ended:{0}", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Post Operation Settlement Party Update.", ex);
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
        /// Implements the Update Settlement Record
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="id">Guid id</param>
        private static void UpdateSettlementRecord(IOrganizationService service, Guid id)
        {
            string fetchxml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                  "<entity name='cdi_settlementparty'>" +
                                    "<attribute name='cdi_settlementpartyid' />" +
                                    "<attribute name='cdi_name' />" +
                                    "<attribute name='createdon' />" +
                                    "<attribute name='cdi_emailaddress' />" +
                                    "<attribute name='cdi_datesigned' />" +
                                    "<order attribute='cdi_name' descending='false' />" +
                                    "<filter type='and'>" +
                                      "<condition attribute='cdi_settlement' operator='eq' uiname='' uitype='cdi_settlement' value='" + id + "' />" +
                                    "</filter>" +
                                  "</entity>" +
                                "</fetch>";
            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchxml));
            bool isDataExists = false;
            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
            {
                foreach (var entity in entityCollection.Entities)
                {
                    if (entity.Contains("cdi_emailaddress") && entity.Contains("cdi_datesigned"))
                    {
                        isDataExists = true;
                    }
                }
            }

            if (isDataExists == true)
            {
                Entity settlement = new Entity("cdi_settlement");
                settlement.Id = id;
                settlement["cdi_allsettlementpartiessigned"] = true;
                settlement["statuscode"] = new OptionSetValue(3);
                service.Update(settlement);              
            }            
        }
    }
}