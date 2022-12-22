// <copyright file="PreOperationContact_Update_PriorityEstimatedRevenue.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create DuplicateReocrd Check Plugin.</summary>
namespace CRM.Plugins.Practice.Contact
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Globalization;

    /// <summary>
    /// Pre Operation Contact Update Priority Estimated Revenue
    /// </summary>
    public class PreOperationContact_Update_PriorityEstimatedRevenue : IPlugin
    {
        /// <summary>
        /// Execute(IServiceProvider serviceProvider)
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider serviceProvider</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                // ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    Entity postImage = (Entity)context.PostEntityImages["PostImage"];
                    try
                    {
                        if (postImage.Contains("cdi__priority"))
                        {
                            if (context.Depth > 1)
                            {
                                return;
                            }

                            decimal priorityrevenue = postImage.Attributes.Contains("cdi_priorityrevenue") ? (postImage.Attributes["cdi_priorityrevenue"] as Money).Value : 0;
                            int priority = postImage.Attributes.Contains("cdi__priority") ? ((OptionSetValue)postImage.Attributes["cdi__priority"]).Value : 0;
                            Guid parentcustomerId = ((EntityReference)postImage.Attributes["parentcustomerid"]).Id;

                            UpdatePriorityEstimatedRevenue(service, entity, priority, parentcustomerId, priorityrevenue);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        ///  Implements the Update Priority Estimated Revenue
        /// </summary>
        /// <param name="organizationService">IOrganizationService organizationService</param>
        /// <param name="entity">Entity entity</param>
        /// <param name="priority">int priority</param>
        /// <param name="parentcustomerId">parent customerId</param>
        /// <param name="priorityrevenue">priority revenue</param>
        private static void UpdatePriorityEstimatedRevenue(IOrganizationService organizationService, Entity entity, int priority, Guid parentcustomerId, decimal priorityrevenue)
        {
            Entity entityObj = null;
            
            if (parentcustomerId != null)
            {
                entityObj = organizationService.Retrieve("account", parentcustomerId, new ColumnSet("cdi_majorpercentage", "cdi_mediumpercentage", "cdi_minorpercentage"));

                int majorrevenuePercentage = Convert.ToInt32(entityObj.Attributes["cdi_majorpercentage"], CultureInfo.InvariantCulture);
                int mediumrevenuePercentage = Convert.ToInt32(entityObj.Attributes["cdi_mediumpercentage"], CultureInfo.InvariantCulture);
                int minorrevenuePercentage = Convert.ToInt32(entityObj.Attributes["cdi_minorpercentage"], CultureInfo.InvariantCulture);

                PriorotySwitchCase(organizationService, entity, priority, priorityrevenue, majorrevenuePercentage, mediumrevenuePercentage, minorrevenuePercentage);
            }
        }

        /// <summary>
        /// Implements the priority switch case
        /// </summary>
        /// <param name="organizationService">IOrganizationService organizationService</param>
        /// <param name="entity">Entity entity</param>
        /// <param name="priority">int priority</param>
        /// <param name="priorityrevenue">priority revenue</param>
        /// <param name="majorrevenuePercentage">major revenue Percentage</param>
        /// <param name="mediumrevenuePercentage">medium revenue Percentage</param>
        /// <param name="minorrevenuePercentage">minor revenue Percentage</param>
        private static void PriorotySwitchCase(IOrganizationService organizationService, Entity entity, int priority, decimal priorityrevenue, int majorrevenuePercentage, int mediumrevenuePercentage, int minorrevenuePercentage)
        {
            switch (priority)
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
