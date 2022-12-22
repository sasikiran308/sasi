// <copyright file="PostOperationOrder_Update_CloneAOrderRecord.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation Order Update Clone A Order Record Plugin.</summary>
namespace CRM.Plugins.Practice.Order
{
    using System;
    using System.Globalization;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation Order Update Clone A Order Record
    /// </summary>
    public class PostOperationOrder_Update_CloneAOrderRecord : IPlugin
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
                    Entity entity = (Entity)context.InputParameters["Target"];
                    try
                    {
                        Entity preImage = (Entity)context.PreEntityImages["PreImage"];
                        Entity oldOrder = new Entity("salesorder");
                        oldOrder = preImage;
                        oldOrder.Id = Guid.Empty;
                        oldOrder.Attributes.Remove("salesorderid");
                        oldOrder.Attributes.Remove("ordernumber");
                        oldOrder.Attributes.Remove("createdon");
                        oldOrder.Attributes.Remove("modifiedon");
                        string originalState = oldOrder.Attributes.Contains("name") ? Convert.ToString(oldOrder.Attributes["name"], CultureInfo.InvariantCulture) : string.Empty;

                        Entity newOrder = oldOrder;
                        newOrder.Id = Guid.Empty;
                        newOrder.Attributes.Remove("salesorderid");
                        oldOrder.Attributes.Remove("ordernumber");
                        newOrder.Attributes.Remove("createdon");
                        newOrder.Attributes.Remove("modifiedon");
                        newOrder["name"] = "COPY" + originalState;
                        Guid newOrderId = service.Create(newOrder);

                        QueryByAttribute queryByAttribute = new QueryByAttribute("salesorderdetail");
                        queryByAttribute.ColumnSet = new ColumnSet(true);
                        queryByAttribute.Attributes.AddRange("salesorderid");
                        queryByAttribute.Values.AddRange(entity.Id);

                        EntityCollection entityCollection = service.RetrieveMultiple(queryByAttribute);
                        if (entityCollection != null && entityCollection.Entities.Count > 0)
                        {
                            foreach (var oldOrderProducts in entityCollection.Entities)
                            {
                                oldOrderProducts.Attributes.Remove("salesorderdetailid");
                                oldOrderProducts.Attributes.Remove("createdon");
                                oldOrderProducts.Attributes.Remove("modifiedon");

                                Entity newOrderProducts = oldOrderProducts;
                                newOrderProducts.Id = Guid.Empty;
                                newOrderProducts.Attributes.Remove("salesorderid");
                                newOrderProducts["salesorderid"] = new EntityReference("salesorder", newOrderId);
                                service.Create(newOrderProducts);
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