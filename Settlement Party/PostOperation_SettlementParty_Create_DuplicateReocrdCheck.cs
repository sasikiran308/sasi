// <copyright file="PostOperation_SettlementParty_Create_DuplicateReocrdCheck.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create DuplicateReocrd Check Plugin.</summary>
namespace CRM.Plugins.Practice.Settlement_Party
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation SettlementParty Create DuplicateReocrd Check
    /// </summary>
    public class PostOperation_SettlementParty_Create_DuplicateReocrdCheck : IPlugin
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
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
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
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation SettlementParty Create Duplicate Record Execution Started:{0}", DateTime.Now));
                        Entity postImage = (Entity)context.PostEntityImages["PostImage"];
                        if (postImage.Contains("cdi_settlement"))
                        {
                            Guid id = ((EntityReference)postImage.Attributes["cdi_settlement"]).Id;
                            DuplicateSettlementPartyRecordCheck(service, entity, id);
                        }

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation SettlementParty Create Duplicate Record Execution Ended:{0}", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Post Operation Settlement Party Create Duplicate Record.", ex);
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
        /// Implements the Duplicate SettlementParty Record Check
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="entity">Entity entity</param>
        /// <param name="id">Guid id</param>
        private static void DuplicateSettlementPartyRecordCheck(IOrganizationService service, Entity entity, Guid id)
        {
            int roleType = ((OptionSetValue)entity.Attributes["cdi_roletype"]).Value;
            QueryExpression queryExpression = new QueryExpression("cdi_settlementparty");
            queryExpression.ColumnSet.AddColumns(new string[] { "cdi_name", "cdi_roletype", "cdi_settlement" });
            queryExpression.Criteria.AddCondition("cdi_roletype", ConditionOperator.Equal, roleType);
            queryExpression.Criteria.AddCondition("cdi_settlement", ConditionOperator.Equal, id);

            EntityCollection entityCollection = service.RetrieveMultiple(queryExpression);
            if (entityCollection.Entities.Count > 1)
            {
                throw new InvalidPluginExecutionException("Duplicate Parties are not allowed.");
            }
            else
            {
                Entity settlement = new Entity("cdi_settlement");
                settlement.Id = id;
                settlement["cdi_allsettlementpartiessigned"] = false;
                settlement["statuscode"] = new OptionSetValue(1);
                service.Update(settlement);
            }
        }
    }
}