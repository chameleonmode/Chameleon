using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Configuration;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Net.Mail;

namespace Chameleon.EntityFrameworkCore.Seed.Host
{
    public class DefaultSettingsCreator
    {
        private readonly ChameleonDbContext _context;

        public DefaultSettingsCreator(ChameleonDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            int? tenantId = null;

            if (ChameleonConsts.MultiTenancyEnabled == false)
            {
#pragma warning disable CS0162 // Unreachable code detected
                tenantId = MultiTenancyConsts.DefaultTenantId;
#pragma warning restore CS0162 // Unreachable code detected
            }

            // Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@mydomain.com", tenantId);
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "mydomain.com mailer", tenantId);

            // Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "en", tenantId);
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.IgnoreQueryFilters().Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
    }
}
