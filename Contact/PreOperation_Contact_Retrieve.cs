// <copyright file="PreOperation_Contact_Retrieve.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Pre Operation Contact Retrieve Plugin.</summary>

namespace CRM.Plugins.Practice
{
    using System;
    using System.Globalization;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Pre Operation Contact Retrieve
    /// </summary>
    public class PreOperation_Contact_Retrieve : IPlugin
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
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                {
                    EntityReference entity = (EntityReference)context.InputParameters["Target"];
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    try
                    {
                        if (entity.LogicalName == "annotation")
                        {
                            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, " Pre Operation Contact Retrieve Execution started: {0} ", DateTime.Now));
                            Guid initiatingUserID = context.InitiatingUserId;
                            if (HasAdminRole(service, initiatingUserID) == false)
                            {
                                throw new InvalidPluginExecutionException("You don't have the permission.");
                            }

                            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, " Pre Operation Contact Retrieve Execution Ended: {0} ", DateTime.Now));
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("not contact.");
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
        /// hasAdminRole(IOrganizationService service, Guid systemUserId)
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="systemUserId">Guid systemUserId</param>
        /// <returns>query expression</returns>
        public static bool HasAdminRole(IOrganizationService service, Guid systemUserId)
        {
            bool isRoleExists = false;
            if (service != null)
            {
                Guid adminRoleTemplateId = new Guid("627090FF-40A3-4053-8790-584EDC5BE201");
                QueryExpression queryexpression = new QueryExpression("role");
                queryexpression.Criteria.AddCondition("roletemplateid", ConditionOperator.Equal, adminRoleTemplateId.ToString());
                LinkEntity link = queryexpression.AddLink("systemuserroles", "roleid", "roleid");
                link.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, systemUserId);
                EntityCollection entityCollection = service.RetrieveMultiple(queryexpression);
                if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                {
                    isRoleExists = true;
                    return isRoleExists;
                }
            }
            return isRoleExists;
        }
    }
}