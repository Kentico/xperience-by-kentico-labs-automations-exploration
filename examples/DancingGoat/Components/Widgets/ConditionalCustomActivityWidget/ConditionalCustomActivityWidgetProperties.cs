using System.ComponentModel;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace DancingGoat.Widgets
{
    /// <summary>
    /// Properties for widget that conditionally logs a custom activity.
    /// </summary>
    [FormCategory(Label = "Condition", Order = 1)]
    [FormCategory(Label = "Action", Order = 10)]
    [FormCategory(Label = "Execution", Order = 20, Collapsible = true)]
    public class ConditionalCustomActivityWidgetProperties : IWidgetProperties
    {
        /// <summary>
        /// Type of condition: based on custom activity or contact group membership.
        /// </summary>
        [RadioGroupComponent(
            Label = "Condition type",
            ExplanationText = "Choose the condition type:<br><strong>Custom activity:</strong> Triggered when contact performs a specific activity.<br><strong>Contact group:</strong> Triggered when contact is a member of a specific group.",
            ExplanationTextAsHtml = true,
            Options = ConditionTypeOptions,
            Order = 1)]
        public string ConditionType { get; set; } = ConditionTypes.CustomActivity;


        /// <summary>
        /// Custom activity type that must be performed before action is executed.
        /// </summary>
        [VisibleIfEqualTo(nameof(ConditionType), ConditionTypes.CustomActivity)]
        [RequiredValidationRule]
        [DropDownComponent(
            Label = "Condition activity type",
            ExplanationText = "The activity that must be performed to trigger this widget.<br><a href=\"https://docs.kentico.com/documentation/developers-and-admins/development/marketing-automation-and-contact-management/website-activity-tracking\" target=\"_blank\" rel=\"noopener noreferrer\">Learn about website activity tracking →</a>",
            ExplanationTextAsHtml = true,
            DataProviderType = typeof(CustomActivityTypeDropDownOptionsProvider),
            Order = 2,
            Tooltip = "Only shown when Condition type is set to Custom activity")]
        public string ConditionActivityType { get; set; } = string.Empty;


        /// <summary>
        /// Optional value to match against ActivityValue for condition activity.
        /// </summary>
        [VisibleIfEqualTo(nameof(ConditionType), ConditionTypes.CustomActivity)]
        [TextInputComponent(
            Label = "Condition activity value",
            ExplanationText = "Optional: filter to contacts who performed the activity with this specific value.",
            Order = 3)]
        public string ConditionActivityValue { get; set; } = string.Empty;


        /// <summary>
        /// Contact group to check for membership.
        /// </summary>
        [VisibleIfEqualTo(nameof(ConditionType), ConditionTypes.ContactGroup)]
        [RequiredValidationRule]
        [DropDownComponent(
            Label = "Contact group",
            ExplanationText = "The contact group membership to check.",
            DataProviderType = typeof(ContactGroupDropDownOptionsProvider),
            Order = 4)]
        public string ContactGroup { get; set; } = string.Empty;


        /// <summary>
        /// Custom activity type to log when condition is met.
        /// </summary>
        [RequiredValidationRule]
        [DropDownComponent(
            Label = "Action activity type",
            ExplanationText = "The activity to log when the condition is met.",
            DataProviderType = typeof(CustomActivityTypeDropDownOptionsProvider),
            Order = 11)]
        public string ActionActivityType { get; set; } = string.Empty;


        /// <summary>
        /// Optional value to store in ActivityValue for action activity.
        /// </summary>
        [TextInputComponent(
            Label = "Activity action value",
            ExplanationText = "Optional: attach a value to the logged activity (e.g., campaign ID, segment, source).",
            Order = 12)]
        public string ActionActivityValue { get; set; } = string.Empty;


        /// <summary>
        /// Controls whether the action executes always, once, or never.
        /// </summary>
        [RadioGroupComponent(
            Label = "Execute when",
            ExplanationText = "<strong>Always:</strong> log every time the condition is met.<br><strong>Once:</strong> log only the first time (then never again).<br><strong>Disabled:</strong> do not log anything.",
            ExplanationTextAsHtml = true,
            Options = ExecuteWhenOptions,
            Order = 21)]
        public string ExecuteWhen { get; set; } = ExecuteWhenModes.Always;


        private const string ExecuteWhenOptions =
            ExecuteWhenModes.Always + ";Always\n" +
            ExecuteWhenModes.Once + ";Once\n" +
            ExecuteWhenModes.Disabled + ";Disabled";

        private const string ConditionTypeOptions =
            ConditionTypes.CustomActivity + ";Custom activity\n" +
            ConditionTypes.ContactGroup + ";Contact group";
    }


    public static class ExecuteWhenModes
    {
        public const string Always = "Always";
        public const string Once = "Once";
        public const string Disabled = "Disabled";
    }

    public static class ConditionTypes
    {
        public const string CustomActivity = "CustomActivity";
        public const string ContactGroup = "ContactGroup";
    }
}
