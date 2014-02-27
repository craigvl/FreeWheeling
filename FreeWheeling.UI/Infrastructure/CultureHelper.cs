﻿using FreeWheeling.Domain.Abstract;
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

                if (_Location.Name == "Townsville" ||
                    _Location.Name == "Cairns")
                {
                    TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                    return TZone;
                }
                else
                {
                    TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                    return TZone;
                }
            }

        }

    }
}