using System.Globalization;
using System.Linq;
using Chameleon.App.Entities;
using System.Collections.Generic;

namespace Chameleon.EntityFrameworkCore.Seed.Host.App
{
    public class CountriesCreator : ApplicationBaseCreator
    {
        public CountriesCreator(ChameleonDbContext context)
            : base(context)
        {
        }

        public override void Run()
        {
            if (Context.Countries.Any())
            {
                return;
            }

            var countries = GetCountriesByIso3166();
            foreach (var item in countries)
            {
                Create(item);
            }
            SaveChanges();
        }

        private void Create(RegionInfo regionInfo)
        {
            var table = Context.Countries;
            if (table.Any(e => e.ISOCode3 == regionInfo.ThreeLetterISORegionName))
            {
                return;
            }

            var entity = new Country
            {
                Name = regionInfo.EnglishName,
                IsMetric = regionInfo.IsMetric,
                ISOCode2 = regionInfo.TwoLetterISORegionName,
                ISOCode3 = regionInfo.ThreeLetterISORegionName,
            };
            table.Add(entity);
        }

        /// <summary>
        /// Gets the list of countries based on ISO 3166-1
        /// </summary>
        /// <returns>Returns the list of countries based on ISO 3166-1</returns>
        private static List<RegionInfo> GetCountriesByIso3166()
        {
            var countries = new List<RegionInfo>();
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo country = new RegionInfo(culture.LCID);
                if (countries.Where(p => p.Name == country.Name).Count() == 0)
                    countries.Add(country);
            }
            return countries.OrderBy(p => p.EnglishName).ToList();
        }
    }
}
