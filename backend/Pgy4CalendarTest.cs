using System;
using System.Collections.Generic;
using MedicalDemo.Algorithm;
using MedicalDemo.Models;

namespace MedicalDemo
{
    public class Pgy4CalendarTest
    {
        public static void Run()
        {
            // Pretend we pulled these from the database
            var residents = new List<Residents>
            {
                new Residents { first_name="Alice", last_name="Smith", graduate_yr=4 },
                new Residents { first_name="Bob", last_name="Jones", graduate_yr=4 },
                new Residents { first_name="Charlie", last_name="Brown", graduate_yr=4 },
                new Residents { first_name="Dana", last_name="White", graduate_yr=4 },
                new Residents { first_name="Eve", last_name="Black", graduate_yr=4 },
                new Residents { first_name="Chris", last_name="Evans", graduate_yr=3 } // ignored
            };

            // Create the PGY4 calendar using premade priority preferences
            var calendar = new PGY4Calendar(residents);

            // Print the automatically assigned schedule
            calendar.PrintSchedule();
        }
    }
}
