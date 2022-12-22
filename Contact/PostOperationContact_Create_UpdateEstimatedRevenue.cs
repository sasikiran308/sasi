// <copyright file="PostOperationContact_Create_UpdateEstimatedRevenue.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create DuplicateReocrd Check Plugin.</summary>
namespace CRM.Plugins.Practice.Contact
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Globalization;

    /// <summary>
    /// Post Operation Contact Create Update Estimated Revenue
    /// </summary>
    public class PostOperationContact_Create_UpdateEstimatedRevenue : IPlugin
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
                    Entity entity = (Entity)context.InputParameters["Target"];

                    // Obtain the organization service reference which you will need for  
                    // web service calls.  
                    IOrganizationServiceFactory serviceFactory =
                        (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    try
                    {
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation Contact Create Update Estimated Revenue Execution Started:{0}", DateTime.Now));
                        UpdateEstimatedRevenue(service, entity);
                        UpdatePriorityEstimatedRevenue(service, entity);
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation Contact Create Update Estimated Revenue Execution Started:{0}", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Post Operation Contact Create Update Estimated Revenue.", ex);
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
        /// UpdateEstimatedRevenue(organizationService, entity)
        /// </summary>
        /// <param name="organizationService">IOrganizationService organizationService</param>
        /// <param name="entity">Entity entity</param>
        private static void UpdateEstimatedRevenue(IOrganizationService organizationService, Entity entity)
        {
            Entity entityObj = null;

            decimal revenue = entity.Attributes.Contains("cdi_revenue") ? (entity.Attributes["cdi_revenue"] as Money).Value : 0;
            Guid parentcustomerId = (entity.Attributes["parentcustomerid"] as EntityReference).Id;
            if (parentcustomerId != null)
            {
                entityObj = organizationService.Retrieve("account", parentcustomerId, new ColumnSet("cdi_revenuepercentage"));
            }

            int revenuePercentage = Convert.ToInt32(entityObj.Attributes["cdi_revenuepercentage"], CultureInfo.InvariantCulture);
            entity["cdi_estimatedrevenue"] = revenue * Convert.ToDecimal(Convert.ToDecimal(revenuePercentage) / 100);
            organizationService.Update(entity);
        }

        /// <summary>
        /// UpdatePriorityEstimatedRevenue(organizationService, entity)
        /// </summary>
        /// <param name="organizationService">IOrganizationService organizationService</param>
        /// <param name="entity">Entity entity</param>
        private static void UpdatePriorityEstimatedRevenue(IOrganizationService organizationService, Entity entity)
        {
            Entity entityObj = null;
            decimal priorityrevenue = entity.Attributes.Contains("cdi_priorityrevenue") ? (entity.Attributes["cdi_priorityrevenue"] as Money).Value : 0;
            int prioroty = entity.Attributes.Contains("cdi__priority") ? ((OptionSetValue)entity.Attributes["cdi__priority"]).Value : 0;
            Guid parentcustomerId = (entity.Attributes["parentcustomerid"] as EntityReference).Id;
            if (parentcustomerId != null)
            {
                entityObj = organizationService.Retrieve("account", parentcustomerId, new ColumnSet("cdi_majorpercentage", "cdi_mediumpercentage", "cdi_minorpercentage"));

                int majorrevenuePercentage = Convert.ToInt32(entityObj.Attributes["cdi_majorpercentage"], CultureInfo.InvariantCulture);
                int mediumrevenuePercentage = Convert.ToInt32(entityObj.Attributes["cdi_mediumpercentage"], CultureInfo.InvariantCulture);
                int minorrevenuePercentage = Convert.ToInt32(entityObj.Attributes["cdi_minorpercentage"], CultureInfo.InvariantCulture);

                PriorotySwitchCase(organizationService, entity, prioroty, priorityrevenue, majorrevenuePercentage, mediumrevenuePercentage, minorrevenuePercentage);
            }
        }

        /// <summary>
        /// PriorotySwitchCase(organizationService, entity, prioroty, priorityrevenue, majorrevenuePercentage, mediumrevenuePercentage, minorrevenuePercentage)
        /// </summary>
        /// <param name="organizationService">IOrganizationService organizationService</param>
        /// <param name="entity">Entity entity</param>
        /// <param name="prioroty">int prioroty</param>
        /// <param name="priorityrevenue">decimal priorityrevenue</param>
        /// <param name="majorrevenuePercentage">int majorrevenuePercentage</param>
        /// <param name="mediumrevenuePercentage">int mediumrevenuePercentage</param>
        /// <param name="minorrevenuePercentage">int minorrevenuePercentage</param>
        private static void PriorotySwitchCase(IOrganizationService organizationService, Entity entity, int prioroty, decimal priorityrevenue, int majorrevenuePercentage, int mediumrevenuePercentage, int minorrevenuePercentage)
        {
            switch (prioroty)
            {
                case 1:
                    entity["cdi_priorityestimatedrevenue"] = priorityrevenue * Convert.ToDecimal(Convert.ToDecimal(majorrevenuePercentage) / 100);
                    organizationService.Update(entity);
                    break;
                case 2:
                    entity["cdi_priorityestimatedrevenue"] = priorityrevenue * Convert.ToDecimal(Convert.ToDecimal(mediumrevenuePercentage) / 100);
                    organizationService.Update(entity);
                    break;
                case 3:
                    entity["cdi_priorityestimatedrevenue"] = priorityrevenue * Convert.ToDecimal(Convert.ToDecimal(minorrevenuePercentage) / 100);
                    organizationService.Update(entity);
                    break;
            }
        }
    }
}
