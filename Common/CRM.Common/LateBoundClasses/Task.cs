// *********************************************************************
// Created by : Latebound Constants Generator 1.2021.12.1 for XrmToolBox
// Author     : Jonas Rapp https://jonasr.app/
// GitHub     : https://github.com/rappen/LCG-UDG/
// Source Org : https://environmentdev.crm.dynamics.com/
// Filename   : F:\CRM\LateBoundClasses\Task.cs
// Created    : 2022-11-21 11:27:43
// *********************************************************************
namespace CRM.Common
{
    /// <summary>OwnershipType: UserOwned, IntroducedVersion: 5.0.0.0</summary>
    public static class Task
    {
        public const string EntityName = "task";
        public const string EntityCollectionName = "tasks";

        #region Attributes

        /// <summary>Type: Uniqueidentifier, RequiredLevel: SystemRequired</summary>
        public const string PrimaryKey = "activityid";
        /// <summary>Type: String, RequiredLevel: ApplicationRequired, MaxLength: 200, Format: Text</summary>
        public const string PrimaryName = "subject";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string DeprecatedProcessStage = "stageid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 1250, Format: Text</summary>
        public const string DeprecatedTraversedPath = "traversedpath";
        /// <summary>Type: State, RequiredLevel: SystemRequired, DisplayName: Activity Status, OptionSetType: State</summary>
        public const string ActivityStatus = "statecode";
        /// <summary>Type: EntityName, RequiredLevel: SystemRequired, DisplayName: Activity Type, OptionSetType: Picklist</summary>
        public const string ActivityType = "activitytypecode";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        public const string ActualEnd = "actualend";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateOnly, DateTimeBehavior: UserLocal</summary>
        public const string ActualStart = "actualstart";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 8192</summary>
        public const string AdditionalParameters = "activityadditionalparams";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string AssignedTaskUniqueId = "crmtaskassigneduniqueid";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 250, Format: Text</summary>
        public const string Category = "category";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string CreatedBy = "createdby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string CreatedOn = "createdon";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: transactioncurrency</summary>
        public const string Currency = "transactioncurrencyid";
        /// <summary>Type: Memo, RequiredLevel: None, MaxLength: 2000</summary>
        public const string Description = "description";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string DueDate = "scheduledend";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string Duration = "actualdurationminutes";
        /// <summary>Type: Decimal, RequiredLevel: None, MinValue: 0.000000000001, MaxValue: 100000000000, Precision: 12</summary>
        public const string ExchangeRate = "exchangerate";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string IsBilled = "isbilled";
        /// <summary>Type: Boolean, RequiredLevel: SystemRequired, True: 1, False: 0, DefaultValue: False</summary>
        public const string IsRegularActivity = "isregularactivity";
        /// <summary>Type: Boolean, RequiredLevel: None, True: 1, False: 0, DefaultValue: False</summary>
        public const string IsWorkflowCreated = "isworkflowcreated";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string LastOnHoldTime = "lastonholdtime";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: sla</summary>
        public const string LastSLAapplied = "slainvokedid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: systemuser</summary>
        public const string ModifiedBy = "modifiedby";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string ModifiedOn = "modifiedon";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: -2147483648, MaxValue: 2147483647</summary>
        public const string OnHoldTimeMinutes = "onholdtime";
        /// <summary>Type: Owner, RequiredLevel: SystemRequired, Targets: systemuser,team</summary>
        public const string Owner = "ownerid";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 100</summary>
        public const string PercentComplete = "percentcomplete";
        /// <summary>Type: Picklist, RequiredLevel: None, DisplayName: Priority, OptionSetType: Picklist, DefaultFormValue: 1</summary>
        public const string Priority = "prioritycode";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string Process = "processid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: account,bookableresourcebooking,bookableresourcebookingheader,bulkoperation,campaign,campaignactivity,cdi_profile,cdi_testreport,contact,contract,entitlement,entitlementtemplate,incident,invoice,knowledgearticle,knowledgebaserecord,lead,msdyn_customerasset,msdyn_playbookinstance,msdyn_postalbum,msdyn_salessuggestion,msdyn_swarm,opportunity,quote,salesorder,site</summary>
        public const string Regarding = "regardingobjectid";
        /// <summary>Type: Integer, RequiredLevel: None, MinValue: 0, MaxValue: 2147483647</summary>
        public const string ScheduledDuration = "scheduleddurationminutes";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: service</summary>
        public const string Service = "serviceid";
        /// <summary>Type: Lookup, RequiredLevel: None, Targets: sla</summary>
        public const string SLA = "slaid";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string SortDate = "sortdate";
        /// <summary>Type: DateTime, RequiredLevel: None, Format: DateAndTime, DateTimeBehavior: UserLocal</summary>
        public const string StartDate = "scheduledstart";
        /// <summary>Type: Status, RequiredLevel: None, DisplayName: Status Reason, OptionSetType: Status</summary>
        public const string StatusReason = "statuscode";
        /// <summary>Type: String, RequiredLevel: None, MaxLength: 250, Format: Text</summary>
        public const string Sub_Category = "subcategory";
        /// <summary>Type: Uniqueidentifier, RequiredLevel: None</summary>
        public const string Subscription = "subscriptionid";

        #endregion Attributes

        #region Relationships

        /// <summary>Parent: "User" Child: "Task" Lookup: "CreatedBy"</summary>
        public const string RelM1_TaskCreatedBy = "lk_task_createdby";
        /// <summary>Parent: "User" Child: "Task" Lookup: "OwningUser"</summary>
        public const string RelM1_TaskOwningUser = "user_task";
        /// <summary>Parent: "User" Child: "Task" Lookup: "CreatedByDelegate"</summary>
        public const string RelM1_TaskCreatedByDelegate = "lk_task_createdonbehalfby";
        /// <summary>Parent: "User" Child: "Task" Lookup: "ModifiedBy"</summary>
        public const string RelM1_TaskModifiedBy = "lk_task_modifiedby";
        /// <summary>Parent: "User" Child: "Task" Lookup: "ModifiedByDelegate"</summary>
        public const string RelM1_TaskModifiedByDelegate = "lk_task_modifiedonbehalfby";

        #endregion Relationships

        #region OptionSets

        public enum ActivityStatus_OptionSet
        {
            Open = 0,
            Completed = 1,
            Canceled = 2
        }
        public enum ActivityType_OptionSet
        {
            Fax = 4204,
            PhoneCall = 4210,
            Email = 4202,
            Letter = 4207,
            Appointment = 4201,
            Task = 4212,
            RecurringAppointment = 4251,
            Teamschat = 10093,
            QuickCampaign = 4406,
            CampaignActivity = 4402,
            CampaignResponse = 4401,
            CaseResolution = 4206,
            ServiceActivity = 4214,
            OpportunityClose = 4208,
            OrderClose = 4209,
            QuoteClose = 4211,
            CustomerVoicealert = 10334,
            CustomerVoicesurveyinvite = 10344,
            CustomerVoicesurveyresponse = 10346,
            Conversation = 10425,
            Session = 10442,
            StudentReport = 10647
        }
        public enum Priority_OptionSet
        {
            Low = 0,
            Normal = 1,
            High = 2
        }
        public enum StatusReason_OptionSet
        {
            NotStarted = 2,
            InProgress = 3,
            Waitingonsomeoneelse = 4,
            Completed = 5,
            Canceled = 6,
            Deferred = 7
        }

        #endregion OptionSets
    }
}
