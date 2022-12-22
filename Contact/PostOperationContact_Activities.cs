// <copyright file="PostOperationContact_Activities.cs" company="TECHXACT">
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
    /// Post Operation Contact Activities
    /// </summary>
    public class PostOperationContact_Activities : IPlugin
    {
        /// <summary>
        ///  Execute(IServiceProvider serviceProvider)
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
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                    try
                    {
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Pre-Operation Contact Create Activities Execution Started:{0}", DateTime.Now));
                        int value = 0;
                        if (entity.Attributes.Contains("cdi_activitytype"))
                        {
                            if (entity != null)
                            {
                                value = ((OptionSetValue)entity["cdi_activitytype"]).Value;
                            }
                        }

                        if (value == 754360002)
                        {
                            if (OpenActivityCheck(service, "task", 0, entity))
                            {
                                throw new InvalidPluginExecutionException("Activity already exists.");
                            }
                            else
                            {
                                Entity task = new Entity("task");
                                task["subject"] = "Task Created";
                                task["description"] = "Task Created";
                                task["regardingobjectid"] = new EntityReference(entity.LogicalName, entity.Id);
                                service.Create(task);
                            }
                        }
                        else if (value == 754360003)
                        {
                            if (OpenActivityCheck(service, "letter", 0, entity))
                            {
                                throw new InvalidPluginExecutionException("Activity already exists.");
                            }
                            else
                            {
                                Entity letter = new Entity("letter");
                                Entity fromActivityprty = new Entity("activityparty");
                                Entity toActivityParty = new Entity("activityparty");
                                fromActivityprty["partyid"] = new EntityReference("systemuser", context.UserId);
                                toActivityParty["partyid"] = new EntityReference(entity.LogicalName, entity.Id);
                                letter["from"] = new Entity[] { fromActivityprty };
                                letter["to"] = new Entity[] { toActivityParty };
                                letter["regardingobjectid"] = new EntityReference(entity.LogicalName, entity.Id);
                                letter["subject"] = "Letter Sent";
                                letter["description"] = "Letter Sent";
                                service.Create(letter);
                            }
                        }
                        else if (value == 754360000)
                        {
                            if (OpenActivityCheck(service, "email", 0, entity))
                            {
                                throw new InvalidPluginExecutionException("Activity already exists.");
                            }
                            else
                            {
                                Entity email = new Entity("email");
                                Entity fromActivityprty = new Entity("activityparty");
                                Entity toActivityParty = new Entity("activityparty");
                                fromActivityprty["partyid"] = new EntityReference("systemuser", context.UserId);
                                toActivityParty["partyid"] = new EntityReference(entity.LogicalName, entity.Id);
                                email["from"] = new Entity[] { fromActivityprty };
                                email["to"] = new Entity[] { toActivityParty };
                                email["regardingobjectid"] = new EntityReference(entity.LogicalName, entity.Id);
                                email["subject"] = "Email Created";
                                email["description"] = "Email Created";
                                email["directioncode"] = true;
                                service.Create(email);
                            }
                        }
                        else
                        {
                            if (OpenActivityCheck(service, "phonecall", 0, entity))
                            {
                            }
                            else
                            {
                                Entity phonecall = new Entity("phonecall");
                                Entity fromActivityprty = new Entity("activityparty");
                                Entity toActivityParty = new Entity("activityparty");
                                fromActivityprty["partyid"] = new EntityReference("systemuser", context.UserId);
                                toActivityParty["partyid"] = new EntityReference(entity.LogicalName, entity.Id);
                                phonecall["from"] = new Entity[] { fromActivityprty };
                                phonecall["to"] = new Entity[] { toActivityParty };
                                phonecall["regardingobjectid"] = new EntityReference(entity.LogicalName, entity.Id);
                                phonecall["subject"] = "Making PhoneCall.";
                                service.Create(phonecall);
                            }
                        }

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Pre-Operation Contact Create Activities Execution Ended:{0}", DateTime.Now));
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
        /// OpenActivityCheck(service, logicalName, statusvalue, entity)
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="logicalName">string logicalName</param>
        /// <param name="statusValue">int statusvalue</param>
        /// <param name="entity">Entity entity</param>
        /// <returns>a b c</returns>
        public static bool OpenActivityCheck(IOrganizationService service, string logicalName, int statusValue, Entity entity)
        {
            bool abc = false;
            if (service != null && entity != null)
            {
                QueryExpression queryExpression = new QueryExpression(logicalName);
                queryExpression.Criteria.AddCondition("regardingobjectid", ConditionOperator.Equal, entity.Id);
                queryExpression.Criteria.AddCondition("statecode", ConditionOperator.Equal, statusValue);
                EntityCollection entityCollection = service.RetrieveMultiple(queryExpression);
                if (entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                {
                    abc = true;
                    return abc;
                }               
              
            }
            return abc;

        }
    }
}
