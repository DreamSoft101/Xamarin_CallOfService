﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallOfService.Technician.Mobile.Domain;

namespace CallOfService.Technician.Mobile.Database
{
    public class DbInitializer
    {
        private readonly IDbSet<UserProfile> _userProfileDbSet;
        private readonly IDbSet<Appointment> _appointmentDbSet;
        private readonly IDbSet<JobDetails> _jobDetailsDbSet;

        public DbInitializer(IDbSet<UserProfile> userProfileDbSet,IDbSet<Appointment> appointmentDbSet, IDbSet<JobDetails> jobDetailsDbSet)
        {
            _userProfileDbSet = userProfileDbSet;
            _appointmentDbSet = appointmentDbSet;
            _jobDetailsDbSet = jobDetailsDbSet;
        }

        public async Task InitDb()
        {
            await _userProfileDbSet.CreateTable();
            await _appointmentDbSet.CreateTable();
            await _jobDetailsDbSet.CreateTable();
        }   
    }
}
