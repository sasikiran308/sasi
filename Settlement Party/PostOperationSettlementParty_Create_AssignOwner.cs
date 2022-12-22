// <copyright file="PostOperationSettlementParty_Create_AssignOwner.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create Assign Owner Plugin.</summary>
namespace CRM.Plugins.Practice.Settlement_Party
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation SettlementParty Create Assign Owner
    /// </summary>
    public class PostOperationSettlementParty_Create_AssignOwner : IPlugin
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
                        UpdateOwner(service, entity);
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
        /// Implements the Update Owner
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="entity">Entity entity</param>
        private static void UpdateOwner(IOrganizationService service, Entity entity)
        {
            int value = ((OptionSetValue)entity["cdi_roletype"]).Value;
            if (value == 1)
            {
                AssignUser(service, entity, "Arbitrator");
            }
            else if (value == 2)
            {
                AssignUser(service, entity, "Commissioner ");
            }
            else if (value == 9)
            {
                AssignUser(service, entity, "Respondent");
            }
            else if (value == 8)
            {
                AssignUser(service, entity, "Petitioner"); 
            }
        }

        /// <summary>
        /// Implements the Assign User
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="entity">Entity entity</param>
        /// <param name="name">string name</param>
        private static void AssignUser(IOrganizationService service, Entity entity, string name)
        {
            QueryExpression queryExpression = new QueryExpression("cdi_setting");
            queryExpression.ColumnSet.AddColumns(new string[] { "cdi_values", "cdi_settingid" });
            queryExpression.Criteria.AddCondition("cdi_name", ConditionOperator.Equal, name);

            EntityCollection entityCollection = service.RetrieveMultiple(queryExpression);

            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
            {
                string id = entityCollection.Entities[0].Attributes.Contains("cdi_values") ? Convert.ToString(entityCollection.Entities[0].Attributes["cdi_values"], CultureInfo.InvariantCulture) : string.Empty;

                AssignRequest assign = new AssignRequest
                {
                    Assignee = new EntityReference("systemuser", new Guid(id)),
                    Target = new EntityReference(entity.LogicalName, entity.Id)
                };
                service.Execute(assign);
            }
        }
    }
}
