// <copyright file="PostOperation_Settlement_UpdateRecord.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation Settlement Update Record Plugin.</summary>
namespace CRM.Plugins.Practice.Settlement
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation Settlement Update Record
    /// </summary>
    public class PostOperation_Settlement_UpdateRecord : IPlugin
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
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation Settlement UpdateRecord Execution Started:{0}", DateTime.Now));
                        Entity postImage = (Entity)context.PostEntityImages["PostImage"];
                        if (postImage.Attributes.Contains("cdi_name"))
                        {
                            string name = postImage.Attributes.Contains("cdi_name") ? Convert.ToString(postImage.Attributes["cdi_name"], CultureInfo.InvariantCulture) : string.Empty;
                            string roleTypeText = postImage.Attributes.Contains("cdi_roletype") ? postImage.FormattedValues["cdi_roletype"] : string.Empty;
                            CreateSettlementPartyRecordByRoleType(service, entity, name, roleTypeText);
                        }

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation Settlement UpdateRecord Execution Ended:{0}", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Post Operation Settlement Update Record.", ex);
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
        /// Implements the Create Settlement Party Record By RoleType
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="entity">Entity entity</param>
        /// <param name="name">string name</param>
        /// <param name="roleTypeText">string roleTypeText</param>
        private static void CreateSettlementPartyRecordByRoleType(IOrganizationService service, Entity entity, string name, string roleTypeText)
        {
            int roleType = ((OptionSetValue)entity.Attributes["cdi_roletype"]).Value;
            QueryExpression queryExpression = new QueryExpression("cdi_settlementparty");
            queryExpression.ColumnSet.AddColumns(new string[] { "cdi_name", "cdi_roletype" });
            queryExpression.Criteria.AddCondition("cdi_roletype", ConditionOperator.Equal, roleType);
            queryExpression.Criteria.AddCondition("cdi_name", ConditionOperator.Equal, name);

            EntityCollection entityCollection = service.RetrieveMultiple(queryExpression);
            if (entityCollection.Entities.Count > 0)
            {
                throw new InvalidPluginExecutionException("Duplicate Parties are not allowed.");
            }
            else
            {
                CreateSettlementPartyRecord(service, entity, roleTypeText);
            }
        }

        /// <summary>
        /// Implements the Create Settlement Party Record
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="entity">Entity entity</param>
        /// <param name="roleTypeText">string roleTypeText</param>
        private static void CreateSettlementPartyRecord(IOrganizationService service, Entity entity, string roleTypeText)
        {
            var roleType = (OptionSetValue)entity.Attributes["cdi_roletype"];
            if (roleType != null)
            {
                Entity settlementParty = new Entity("cdi_settlementparty");
                settlementParty["cdi_name"] = roleTypeText;
                settlementParty["cdi_roletype"] = roleType;
                settlementParty["cdi_settlement"] = new EntityReference(entity.LogicalName, entity.Id);
                service.Create(settlementParty);

                Entity settlement = new Entity("cdi_settlement");
                settlement.Id = entity.Id;
                settlement["cdi_allsettlementpartiessigned"] = false;
                settlement["statuscode"] = new OptionSetValue(1);
                service.Update(settlement);
            }
        }
    }
}