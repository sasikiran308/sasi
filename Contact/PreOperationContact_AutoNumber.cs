// <copyright file="PreOperationContact_AutoNumber.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Pre Operation Contact AutoNumber Plugin.</summary>

namespace CRM.Plugins.Practice
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Globalization;

    /// <summary>
    /// Pre Operation Contact AutoNumber
    /// </summary>
    public class PreOperationContact_AutoNumber : IPlugin
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
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Pre-Operation Contact AutoNumber Execution Started:{0}", DateTime.Now));

                        var number = GenerateAutoNumber(service);
                        entity["cdi_autonumber"] = number;

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Pre-Operation Contact AutoNumber Execution Ended:{0}", DateTime.Now));
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

        /// <summary>
        /// GenerateAutoNumber(IOrganizationService organizationService)
        /// </summary>
        /// <param name="organizationService">IOrganizationService organizationService</param>
        /// <returns>formatted Number</returns>
        private static string GenerateAutoNumber(IOrganizationService organizationService)
        {
            QueryExpression queryExpression = new QueryExpression("cdi_setting");
            queryExpression.ColumnSet.AddColumns(new string[] { "cdi_values", "cdi_settingid" });
            queryExpression.Criteria.AddCondition("cdi_name", ConditionOperator.Equal, "Contact Auto");

            EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);

            string currentNumber = string.Empty;
            var nextNumber = 0;
            string formattedNumber = string.Empty;
            if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
            {
                currentNumber = entityCollection.Entities[0].Attributes.Contains("cdi_values") ? Convert.ToString(entityCollection.Entities[0].Attributes["cdi_values"], CultureInfo.InvariantCulture) : string.Empty;
                if (!string.IsNullOrEmpty(currentNumber))
                {
                    nextNumber = Convert.ToInt32(currentNumber, CultureInfo.InvariantCulture) + 1;
                }

                formattedNumber = DateTime.Now.Year.ToString(CultureInfo.CurrentCulture) + "-TechXact-" + nextNumber;
                if (nextNumber > 0)
                {
                    Entity setting = new Entity("cdi_setting");
                    setting.Id = entityCollection.Entities[0].Id;
                    setting["cdi_values"] = Convert.ToString(nextNumber, CultureInfo.InvariantCulture);
                    organizationService.Update(setting);
                }                  
            }

            return formattedNumber;
        }
    }
}
