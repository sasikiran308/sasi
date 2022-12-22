// <copyright file="PostOperationContact_Update_DisassociateAndDisassociateDistricts.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create DuplicateReocrd Check Plugin.</summary>
namespace CRM.Plugins.Practice
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation Contact Update Disassociate And Disassociate Districts
    /// </summary>
    public class PostOperationContact_Update_DisassociateAndDisassociateDistricts : IPlugin
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
                    Entity entity = (Entity)context.InputParameters["Target"];
                    IOrganizationServiceFactory organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = organizationServiceFactory.CreateOrganizationService(context.UserId);
                    try
                    {
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, " Post Operation Contact Update Disassociate And Associate Districts Execution started: {0}", DateTime.Now));

                        // Dissociate logic
                        // old state logic
                        Entity preImageAccount = (Entity)context.PreEntityImages["Image"];
                        if (preImageAccount.Contains("cdi_state"))
                        {
                            EntityReference oldstateReference = preImageAccount.Attributes.Contains("cdi_state") ? preImageAccount.Attributes["cdi_state"] as EntityReference : null;
                            if (oldstateReference != null && oldstateReference.Id != Guid.Empty)
                            {
                                EntityCollection entityCollection = RetrieveDistrictRecordsByState(service, oldstateReference.Id);
                                if (entityCollection != null && entityCollection.Entities.Count > 0)
                                {
                                    // Creating EntityReferenceCollection for the Contact
                                    EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
                                    foreach (var item in entityCollection.Entities)
                                    {
                                        // Add the related entity district
                                        relatedEntities.Add(item.ToEntityReference());
                                    }

                                    // Add the District State relationship name
                                    Relationship relationship = new Relationship("cdi_Contact_cdi_district");

                                    // Associate the district record to state
                                    service.Disassociate(entity.LogicalName, entity.Id, relationship, relatedEntities);
                                }
                            }
                        }

                        // Current Logic
                        if (entity.Attributes.Contains("cdi_state"))
                        {
                            EntityReference stateReference = entity.Attributes.Contains("cdi_state") ? entity.Attributes["cdi_state"] as EntityReference : null;
                            if (stateReference != null && stateReference.Id != Guid.Empty)
                            {
                                EntityCollection entityCollection = RetrieveDistrictRecordsByState(service, stateReference.Id);
                                if (entityCollection != null && entityCollection.Entities.Count > 0)
                                {
                                    // Creating EntityReferenceCollection for the Contact
                                    EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
                                    foreach (var item in entityCollection.Entities)
                                    {
                                        // Add the related entity district
                                        relatedEntities.Add(item.ToEntityReference());
                                    }

                                    // Add the District State relationship name
                                    Relationship relationship = new Relationship("cdi_Contact_cdi_district");

                                    // Associate the district record to state
                                    service.Associate(entity.LogicalName, entity.Id, relationship, relatedEntities);
                                }
                            }
                        }

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, " Post Operation Contact Update Disassociate And Associate Districts Execution Ended: {0}", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Follow Up Plugin.", ex);
                    }
                    catch (Exception ex)
                    {
                        tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// RetrieveDistrictRecordsByState(organizationService, stateId)
        /// </summary>
        /// <param name="organizationService">IOrganizationService organizationService</param>
        /// <param name="stateId">Guid stateId</param>
        /// <returns>query Expression</returns>
        private static EntityCollection RetrieveDistrictRecordsByState(IOrganizationService organizationService, Guid stateId)
        {
            QueryExpression queryExpression = new QueryExpression("cdi_district");
            queryExpression.ColumnSet.AddColumns(new string[] { "cdi_name", "cdi_stateid" });
            queryExpression.Criteria.AddCondition("cdi_stateid", ConditionOperator.Equal, stateId);
            return organizationService.RetrieveMultiple(queryExpression);
        }
    }
}