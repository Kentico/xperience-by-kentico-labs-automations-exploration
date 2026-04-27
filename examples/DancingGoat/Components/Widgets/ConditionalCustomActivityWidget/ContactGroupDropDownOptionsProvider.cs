using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CMS.ContactManagement;
using CMS.DataEngine;

using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace DancingGoat.Widgets
{
    /// <summary>
    /// Provides enabled contact groups (excluding recipient lists) for widget dropdowns.
    /// </summary>
    public class ContactGroupDropDownOptionsProvider : IDropDownOptionsProvider
    {
        private readonly IInfoProvider<ContactGroupInfo> contactGroupInfoProvider;


        public ContactGroupDropDownOptionsProvider(IInfoProvider<ContactGroupInfo> contactGroupInfoProvider)
        {
            this.contactGroupInfoProvider = contactGroupInfoProvider;
        }


        public Task<IEnumerable<DropDownOptionItem>> GetOptionItems()
        {
            var items = contactGroupInfoProvider.Get()
                .WhereEquals(nameof(ContactGroupInfo.ContactGroupEnabled), true)
                .WhereEquals(nameof(ContactGroupInfo.ContactGroupIsRecipientList), false)
                .OrderBy(nameof(ContactGroupInfo.ContactGroupDisplayName))
                .ToList()
                .Select(groupInfo => new DropDownOptionItem
                {
                    Value = groupInfo.ContactGroupName,
                    Text = string.IsNullOrWhiteSpace(groupInfo.ContactGroupDisplayName)
                        ? groupInfo.ContactGroupName
                        : groupInfo.ContactGroupDisplayName
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
