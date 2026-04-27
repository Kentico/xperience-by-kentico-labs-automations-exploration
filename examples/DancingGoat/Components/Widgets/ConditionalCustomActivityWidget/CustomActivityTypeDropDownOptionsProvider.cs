using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CMS.Activities;
using CMS.DataEngine;

using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace DancingGoat.Widgets
{
    /// <summary>
    /// Provides custom and enabled activity types for widget dropdowns.
    /// </summary>
    public class CustomActivityTypeDropDownOptionsProvider : IDropDownOptionsProvider
    {
        private readonly IInfoProvider<ActivityTypeInfo> activityTypeInfoProvider;


        public CustomActivityTypeDropDownOptionsProvider(IInfoProvider<ActivityTypeInfo> activityTypeInfoProvider)
        {
            this.activityTypeInfoProvider = activityTypeInfoProvider;
        }


        public Task<IEnumerable<DropDownOptionItem>> GetOptionItems()
        {
            var activityTypes = activityTypeInfoProvider.Get()
                .WhereEquals(nameof(ActivityTypeInfo.ActivityTypeIsCustom), true)
                .WhereEquals(nameof(ActivityTypeInfo.ActivityTypeEnabled), true)
                .OrderBy(nameof(ActivityTypeInfo.ActivityTypeDisplayName))
                .ToList();

            var items = activityTypes
                .Select(typeInfo => new DropDownOptionItem
                {
                    Value = typeInfo.ActivityTypeName,
                    Text = string.IsNullOrWhiteSpace(typeInfo.ActivityTypeDisplayName)
                        ? typeInfo.ActivityTypeName
                        : typeInfo.ActivityTypeDisplayName
                })
                .ToList();

            items.Insert(0, new DropDownOptionItem
            {
                Value = string.Empty,
                Text = "(select)"
            });

            return Task.FromResult(items.AsEnumerable());
        }
    }
}
