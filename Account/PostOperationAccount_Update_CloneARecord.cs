// <copyright file="PostOperationAccount_Update_CloneARecord.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation Account Update Clone A Record Plugin.</summary>
namespace CRM.Plugins.Practice.Account
{
    using System;
    using System.Globalization;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation Account Update Clone A Record
    /// </summary>
    public class PostOperationAccount_Update_CloneARecord : IPlugin
    {
        /// <summary>
        /// Execute(IServiceProvider serviceProvider)
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider serviceProvider</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // Entity entity = (Entity)context.InputParameters["Target"];
                    try
                    {
                        Entity preImage = (Entity)context.PreEntityImages["PreImage"];
                        Entity oldState = new Entity("cdi_state");
                        oldState = preImage;
                        oldState.Id = Guid.Empty;
                        oldState.Attributes.Remove("cdi_stateid");
                        oldState.Attributes.Remove("createdon");
                        oldState.Attributes.Remove("modifiedon");
                        string originalState = oldState.Attributes.Contains("cdi_name") ? Convert.ToString(oldState.Attributes["cdi_name"], CultureInfo.InvariantCulture) : string.Empty;

                        Entity newState = oldState;

                        // newState.Id = Guid.Empty;
                        // newState.Attributes.Remove("cdi_stateid");
                        // newState.Attributes.Remove("createdon");
                        // newState.Attributes.Remove("modifiedon");
                        newState["cdi_name"] = "COPY" + originalState;
                        Guid newStateId = service.Create(newState);

                        QueryByAttribute queryByAttribute = new QueryByAttribute("cdi_district");
                        queryByAttribute.ColumnSet = new ColumnSet(true);
                        queryByAttribute.Attributes.AddRange("cdi_stateid");
                        queryByAttribute.Values.AddRange("e3c28f66-954b-ed11-bba1-000d3a31c3fa");

                        EntityCollection entityCollection = service.RetrieveMultiple(queryByAttribute);
                        if (entityCollection != null && entityCollection.Entities.Count > 0)
                        {
                            foreach (var oldDistricts in entityCollection.Entities)
                            {
                                oldDistricts.Attributes.Remove("cdi_districtid");
                                oldDistricts.Attributes.Remove("createdon");
                                oldDistricts.Attributes.Remove("modifiedon");

                                Entity newDistricts = oldDistricts;
                                newDistricts.Id = Guid.Empty;
                                newDistricts.Attributes.Remove("cdi_stateid");
                                newDistricts["cdi_stateid"] = new EntityReference("cdi_state", newStateId);
                                service.Create(newDistricts);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                }
            }
        }
    }
}
