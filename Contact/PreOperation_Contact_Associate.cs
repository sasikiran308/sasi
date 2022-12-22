// <copyright file="PreOperation_Contact_Associate.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create DuplicateReocrd Check Plugin.</summary>
namespace CRM.Plugins.Practice
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Pre Operation Contact Associate
    /// </summary>
    public class PreOperation_Contact_Associate : IPlugin
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

                try
                {
                    EntityReference targetEntity = null;
                    string strRelationshipName = string.Empty;
                    EntityReferenceCollection relatedEntities = null;

                    if (context.MessageName == "Associate")
                    {
                        if (context.InputParameters.Contains("Relationship"))
                        {
                            strRelationshipName = context.InputParameters["Relationship"].ToString();
                            string[] words = strRelationshipName.Split('.');
                            strRelationshipName = words[0];
                        }

                        // Check the "Relationship Name" with your intended one
                        if (strRelationshipName != "cdi_Contact_cdi_district")
                        {
                            return;
                        }

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                        {
                            // Account
                            targetEntity = (EntityReference)context.InputParameters["Target"];
                        }

                        EntityReference contactStatelookupReference = null;
                        if (targetEntity.LogicalName.ToUpperInvariant() == "CONTACT")
                        {
                            // your logic
                            Entity contact = service.Retrieve(targetEntity.LogicalName, targetEntity.Id, new ColumnSet("cdi_state"));
                            if (contact != null)
                            {
                                contactStatelookupReference = contact.Contains("cdi_state") ? contact["cdi_state"] as EntityReference : null;
                            }
                        }

                        // Get Entity 2 reference from "RelatedEntities" Key from context
                        if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                        {
                            relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                        }

                        EntityReference districtStateLookupReference = null;
                        for (int i = 0; i < relatedEntities.Count; i++)
                        {
                            Entity district = service.Retrieve(relatedEntities[i].LogicalName.ToString(), new Guid(relatedEntities[i].Id.ToString()), new ColumnSet("cdi_stateid"));
                            if (district != null)
                            {
                                districtStateLookupReference = district.Contains("cdi_stateid") ? district["cdi_stateid"] as EntityReference : null;
                            }
                        }

                        if (contactStatelookupReference.Id != districtStateLookupReference.Id)
                        {
                            // Create Email Acivity
                            throw new InvalidPluginExecutionException("Other State Districts are not allowed.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }

                //if (context.InputParameters.Contains("Relationship"))
                //{
                //    EntityReference targetEntity = null;
                //    string relationshipName = string.Empty;
                //    EntityReferenceCollection relatedEntities = null;
                //    Guid isDistricEntityID = Guid.Empty;
                //    Guid stateid = Guid.Empty;
                //    Relationship relationship = (Relationship)context.InputParameters["Relationship"];
                //    if (relationship.SchemaName != "cdi_Contact_cdi_district")
                //    {
                //        return;
                //    }

                //    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                //    {
                //        targetEntity = (EntityReference)context.InputParameters["Target"];
                //        Entity opputunity = service.Retrieve("contact", targetEntity.Id, new ColumnSet("cdi_state"));
                //        if (opputunity.Attributes.Contains("cdi_state"))
                //        {
                //            var eventEnt = (EntityReference)opputunity["cdi_state"];
                //            isDistricEntityID = eventEnt.Id;
                //        }
                //    }

                //    if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                //    {
                //        relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                //        if (relatedEntities != null && relatedEntities.Count > 0)
                //        {
                //            foreach (var relatedentity in relatedEntities)
                //            {
                //                Entity distrcitentity = service.Retrieve("cdi_district", relatedentity.Id, new ColumnSet("cdi_stateid"));
                //                if (distrcitentity.Attributes.Contains("cdi_stateid"))
                //                {
                //                    stateid = ((EntityReference)distrcitentity.Attributes["cdi_stateid"]).Id;
                //                }
                //            }
                //        }
                //    }

                //    if (stateid != isDistricEntityID)
                //    {
                //        throw new InvalidPluginExecutionException("Related Entity you have selected is from another state.Please check before choose");
                //    }
                //}
            }
        }
    }
}
