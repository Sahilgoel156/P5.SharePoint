using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace P5.SharePoint.Core;

public static class ModuleConstants
{
    public static class Security
    {
        public static class Permissions
        {
            public const string Access = "SharePoint:access";
            public const string Create = "SharePoint:create";
            public const string Read = "SharePoint:read";
            public const string Update = "SharePoint:update";
            public const string Delete = "SharePoint:delete";

            public static string[] AllPermissions { get; } =
            {
                Access,
                Create,
                Read,
                Update,
                Delete,
            };
        }
    }

    public static class Settings
    {
        public static class General
        {
            public static SettingDescriptor SharePointEnabled { get; } = new SettingDescriptor
            {
                Name = "SharePoint.SharePointEnabled",
                GroupName = "SharePoint|General",
                ValueType = SettingValueType.Boolean,
                DefaultValue = false,
            };

            public static SettingDescriptor SharePointPassword { get; } = new SettingDescriptor
            {
                Name = "SharePoint.SharePointPassword",
                GroupName = "SharePoint|Advanced",
                ValueType = SettingValueType.SecureString,
                DefaultValue = "qwerty",
            };

            public static IEnumerable<SettingDescriptor> AllGeneralSettings
            {
                get
                {
                    yield return SharePointEnabled;
                    yield return SharePointPassword;
                }
            }
        }

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                return General.AllGeneralSettings;
            }
        }
    }
}
