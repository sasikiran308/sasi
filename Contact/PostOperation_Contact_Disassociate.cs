// <copyright file="PostOperation_Contact_Disassociate.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation Contact Disassociate Plugin.</summary>
namespace CRM.Plugins.Practice
{
    using System;
    using System.Globalization;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation Contact Disassociate
    /// </summary>
    public class PostOperation_Contact_Disassociate : IPlugin
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
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation Contact Disassociate Execution Started: {0}", DateTime.Now));
                        EntityReference targetEntity = null;
                        string strRelationshipName = string.Empty;
                        EntityReferenceCollection relatedEntities = null;
                        if (context.MessageName == "Disassociate")
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
                                Entity account = service.Retrieve(targetEntity.LogicalName, targetEntity.Id, new ColumnSet("cdi_state"));
                                if (account != null)
                                {
                                    contactStatelookupReference = account.Contains("cdi_state") ? account["cdi_state"] as EntityReference : null;
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

                        //if (context.InputParameters.Contains("Relationship"))
                        //{
                        //    EntityReference targetEntity = null;
                        //    EntityReferenceCollection relatedEntities = null;
                        //    Guid isDistrcEntID = Guid.Empty;
                        //    Guid isDistricEntityID = Guid.Empty;
                        //    Relationship relationship = (Relationship)context.InputParameters["Relationship"];

                        //    bool hasRole = this.HasUserRole(service, context);
                        //    if (hasRole == true)
                        //    {
                        //        if (relationship.SchemaName != "cdi_Contact_cdi_district")
                        //        {
                        //            return;
                        //        }

                        //        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                        //        {
                        //            targetEntity = (EntityReference)context.InputParameters["Target"];
                        //            Entity contact = service.Retrieve("contact", targetEntity.Id, new ColumnSet("cdi_state"));
                        //            if (contact.Attributes.Contains("cdi_state"))
                        //            {
                        //                var eventEnt = (EntityReference)contact["cdi_state"];
                        //                isDistricEntityID = eventEnt.Id;
                        //            }
                        //        }

                        //        if (context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                        //        {
                        //            relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                        //            if (relatedEntities != null && relatedEntities.Count > 0)
                        //            {
                        //                EntityReference distrcitentity = relatedEntities[0];
                        //                isDistrcEntID = distrcitentity.Id;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        throw new InvalidPluginExecutionException("Dont have sufficient role to diassociate the records");
                        //    }
                        //}

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation Contact Disassociate Execution Ended: {0}", DateTime.Now));
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

//        /// <summary>
//        /// HasUserRole(service, context)
//        /// </summary>
//        /// <param name="service">IOrganizationService service</param>
//        /// <param name="context">IPluginExecutionContext context</param>
//        /// <returns>has admin role</returns>
//        public bool HasUserRole(IOrganizationService service, IPluginExecutionContext context)
//        {
//            string fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
//                                 "<entity name='role'>" +
//                                  "<attribute name='name' />" +
//                                "<attribute name='businessunitid' />" +
//                               "<attribute name='roleid' />" +
//                              "<order attribute='name' descending='false' />" +
//                            "<link-entity name='systemuserroles' from='roleid' to='roleid' visible='false' intersect='true'>" +
//                          "<link-entity name='systemuser' from='systemuserid' to='systemuserid' alias='ab'>" +
//                              "<filter type='and'>" +
//                           "<condition attribute='systemuserid' operator='eq' value = '" + context.UserId.ToString() + "' />" +
//                           "</filter>" +
//                         "</link-entity>" +
//                        "</link-entity>" +
//                          "</entity>" +
//                         "</fetch>";
//            EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
//            bool hasadminrole = false;
//            if (entityCollection.Entities.Count > 0)
//            {
//                string rolename = string.Empty;
//                foreach (var role in entityCollection.Entities)
//                {
//                    rolename = role.Attributes.Contains("name") ? Convert.ToString(role.Attributes["name"]) : string.Empty;
//                    if (rolename.ToLower() == "system administrator")
//                    {
//                        hasadminrole = false;
//                        break;
//                    }
//                }
//            }

//            return hasadminrole;
//        }
//    }
//}