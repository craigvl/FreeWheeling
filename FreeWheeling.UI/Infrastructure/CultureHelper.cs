using FreeWheeling.Domain.Abstract;
using FreeWheeling.Domain.Entities;
using FreeWheeling.UI.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.Threading;

namespace FreeWheeling.UI.Infrastructure
{
    [Authorize]
    public class CultureHelper
    {
        private ICycleRepository repository;

        public CultureHelper(ICycleRepository repoParam)
        {
            repository = repoParam;
        }

        public CultureInfo GetCulture(int UserLocationId)
        {
            Location _Location = repository.GetLocations().Where(l => l.id == UserLocationId).FirstOrDefault();

            if (_Location.Name == "Townsville" ||
                _Location.Name == "Cairns")
            {
                return  new CultureInfo("en-AU");
            }else
            {
                return new CultureInfo("en-AU");
            }
        }

        public TimeZoneInfo GetTimeZoneInfo(int? UserLocationId)
        {
            if (UserLocationId == null)
            {
                TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                return TZone;
            }
            else
            {
                Location _Location = repository.GetLocations().Where(l => l.id == UserLocationId).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(_Location.TimeZoneInfo))
                {
                    TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById(_Location.TimeZoneInfo);
                    return TZone;                  
                }
                else
                {
                    TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                    return TZone;
                }
            }
        }

        public string IanaToWindows(string ianaZoneId)
        {
            var utcZones = new[] { "Etc/UTC", "Etc/UCT" };
            if (utcZones.Contains(ianaZoneId, StringComparer.OrdinalIgnoreCase))
                return "UTC";

            var tzdbSource = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default;

            // resolve any link, since the CLDR doesn't necessarily use canonical IDs
            var links = tzdbSource.CanonicalIdMap
              .Where(x => x.Value.Equals(ianaZoneId, StringComparison.OrdinalIgnoreCase))
              .Select(x => x.Key);

            var mappings = tzdbSource.WindowsMapping.MapZones;
            var item = mappings.FirstOrDefault(x => x.TzdbIds.Any(links.Contains));
            if (item == null) return null;
            return item.WindowsId;
        }

        // This will return the "primary" IANA zone that matches the given windows zone.
        // If the primary zone is a link, it then resolves it to the canonical ID.
        public string WindowsToIana(string windowsZoneId)
        {
            if (windowsZoneId.Equals("UTC", StringComparison.OrdinalIgnoreCase))
                return "Etc/UTC";

            var tzdbSource = NodaTime.TimeZones.TzdbDateTimeZoneSource.Default;
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(windowsZoneId);
            var tzid = tzdbSource.MapTimeZoneId(tzi);
            return tzdbSource.CanonicalIdMap[tzid];
        }
    }
}