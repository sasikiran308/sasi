// <copyright file="PreValidationContact_Delete_ActivitiesCheck.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Pre Validation Contact Delete Activities Check Plugin.</summary>
namespace CRM.Plugins.Practice
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Pre Validation Contact Delete Activities Check
    /// </summary>
    public class PreValidationContact_Delete_ActivitiesCheck : IPlugin
    {
        /// <summary>
        /// Execute(IServiceProvider serviceProvider)
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider serviceProvider</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // Entity entity = (Entity)context.InputParameters["Target"];
                    IOrganizationServiceFactory organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = organizationServiceFactory.CreateOrganizationService(context.UserId);
                    try
                    {
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, " Pre Validation Contact Delete Activities Check Execution Started: {0} ", DateTime.Now));

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                        {
                            EntityReference contactReference = (EntityReference)context.InputParameters["Target"];

                            QueryExpression queryexpression = new QueryExpression("activitypointer");
                            queryexpression.ColumnSet = new ColumnSet("subject");
                            queryexpression.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, contactReference.Id);
                            queryexpression.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                            EntityCollection activities = service.RetrieveMultiple(queryexpression);
                            if (activities.Entities.Count > 0)
                            {
                                throw new InvalidPluginExecutionException("You have open activities,Cannot delete contact record.");
                            }
                        }

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, " Pre Validation Contact Delete Activities Check Execution Ended: {0} ", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Pre Validation Contact Delete Activities Check.", ex);
                    }
                    catch (Exception ex)
                    {
                        tracingService.Trace("Pre Validation Contact Delete Activities Check: {0}", ex.ToString());
                        throw;
                    }
                }
            }
        }
    }
}
