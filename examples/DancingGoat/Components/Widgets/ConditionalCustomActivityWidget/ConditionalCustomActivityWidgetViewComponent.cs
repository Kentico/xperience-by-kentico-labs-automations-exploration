using System;
using System.Threading.Tasks;
using CMS.Activities;
using CMS.ContactManagement;
using CMS.DataEngine;

using DancingGoat.Widgets;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Web.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

[assembly: RegisterWidget(
    ConditionalCustomActivityWidgetViewComponent.IDENTIFIER,
    typeof(ConditionalCustomActivityWidgetViewComponent),
    "Conditional custom activity",
    typeof(ConditionalCustomActivityWidgetProperties), 
    Description = "Logs a custom activity when a configured condition is met.", 
    IconClass = "icon-ekg-line")]

namespace DancingGoat.Widgets
{
    /// <summary>
    /// Widget that conditionally logs custom activities for contacts.
    /// </summary>
    public class ConditionalCustomActivityWidgetViewComponent(
        IInfoProvider<ActivityInfo> activityInfoProvider,
        ICustomActivityLogger customActivityLogger,
        IInfoProvider<ContactGroupInfo> contactGroupInfoProvider,
        IInfoProvider<ActivityTypeInfo> activityTypeInfoProvider) : ViewComponent
    {
        public const string IDENTIFIER = "DancingGoat.General.ConditionalCustomActivityWidget";

        public async Task<ViewViewComponentResult> InvokeAsync(ConditionalCustomActivityWidgetProperties properties)
        {
            var kenticoFeatures = HttpContext.Kentico();
            var isEditMode = kenticoFeatures.PageBuilder().EditMode;
            var isPreviewMode = kenticoFeatures.Preview().Enabled && !isEditMode;
            var isEditOrPreviewMode = isEditMode || isPreviewMode;

            // Activity logging is intentionally only executed for live requests.
            if (!isEditOrPreviewMode)
            {
                TryLogCustomActivity(properties);
            }

            var conditionGroupName = (await contactGroupInfoProvider.GetAsync(properties.ContactGroup))?.ContactGroupDisplayName ?? properties.ContactGroup;
            var conditionActivityName = (await activityTypeInfoProvider.GetAsync(properties.ConditionActivityType))?.ActivityTypeDisplayName ?? properties.ConditionActivityType;
            var actionActivityName = (await activityTypeInfoProvider.GetAsync(properties.ActionActivityType))?.ActivityTypeDisplayName ?? properties.ActionActivityType;

            var model = new ConditionalCustomActivityWidgetViewModel
            {
                ConditionType = properties.ConditionType,
                ConditionActivityType = conditionActivityName,
                ConditionActivityValue = properties.ConditionActivityValue,
                ContactGroup = conditionGroupName,
                ActionActivityType = actionActivityName,
                ActionActivityValue = properties.ActionActivityValue,
                ExecuteWhen = properties.ExecuteWhen,
                IsEditMode = isEditMode,
                IsPreviewMode = isPreviewMode
            };

            return View("~/Components/Widgets/ConditionalCustomActivityWidget/_ConditionalCustomActivityWidget.cshtml", model);
        }


        private void TryLogCustomActivity(ConditionalCustomActivityWidgetProperties properties)
        {
            if (string.Equals(properties.ExecuteWhen, ExecuteWhenModes.Disabled, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var contact = ContactManagementContext.GetCurrentContact(true);
            if ((contact is null) || (contact.ContactID <= 0))
            {
                return;
            }

            bool conditionMet = string.Equals(properties.ConditionType, ConditionTypes.CustomActivity, StringComparison.OrdinalIgnoreCase)
                ? DidContactPerformActivity(contact.ContactID, properties.ConditionActivityType, properties.ConditionActivityValue)
                : string.Equals(properties.ConditionType, ConditionTypes.ContactGroup, StringComparison.OrdinalIgnoreCase)
                    && IsContactInGroup(contact, properties.ContactGroup);

            if (!conditionMet)
            {
                return;
            }

            if (string.Equals(properties.ExecuteWhen, ExecuteWhenModes.Once, StringComparison.OrdinalIgnoreCase)
                && DidContactPerformActivity(contact.ContactID, properties.ActionActivityType, properties.ActionActivityValue))
            {
                return;
            }

            customActivityLogger.Log(properties.ActionActivityType, new CustomActivityData
            {
                ActivityValue = properties.ActionActivityValue
            });
        }

        private static bool IsContactInGroup(ContactInfo contact, string contactGroupCodeName) =>
            !string.IsNullOrWhiteSpace(contactGroupCodeName) && contact.IsInContactGroup(contactGroupCodeName);

        private bool DidContactPerformActivity(int contactId, string activityType, string activityValue) =>
            !string.IsNullOrWhiteSpace(activityType) && (
                string.IsNullOrWhiteSpace(activityValue)
                    ? activityInfoProvider.ContactDidActivity(contactId, activityType, null, 0, null)
                    : activityInfoProvider.ContactDidActivityWithValue(contactId, activityType, "equals", activityValue, 0));
    }
}
