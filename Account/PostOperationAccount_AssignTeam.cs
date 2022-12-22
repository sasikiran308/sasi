// <copyright file="PostOperationAccount_AssignTeam.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create DuplicateReocrd Check Plugin.</summary>
namespace CRM.Plugins.Practice
{
    using System;
    using System.ServiceModel;
    using System.Globalization;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation Account Assign Team
    /// </summary>
    public class PostOperationAccount_AssignTeam : IPlugin
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
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

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
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post-Operation Account Execution Started:{0}", DateTime.Now));
                        if (entity.LogicalName == "account")
                        {
                            QueryExpression queryExpression = new QueryExpression("cdi_setting");
                            queryExpression.ColumnSet.AddColumns(new string[] { "cdi_values", "cdi_settingid" });
                            queryExpression.Criteria.AddCondition("cdi_name", ConditionOperator.Equal, "Team");

                            EntityCollection entityCollection = service.RetrieveMultiple(queryExpression);

                            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                            {
                                string id = entityCollection.Entities[0].Attributes.Contains("cdi_values") ? Convert.ToString(entityCollection.Entities[0].Attributes["cdi_values"],CultureInfo.InvariantCulture) : string.Empty;

                                AssignRequest assign = new AssignRequest
                                {
                                    Assignee = new EntityReference("team", new Guid(id)),
                                    Target = new EntityReference(entity.LogicalName, entity.Id)
                                };
                                service.Execute(assign);
                            }

                            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post-Operation Account Execution Ended:{0}", DateTime.Now));
                        }
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