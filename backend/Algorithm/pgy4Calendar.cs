using System;
using System.Collections.Generic;
using System.Linq;
using MedicalDemo.Models;

namespace MedicalDemo.Algorithm
{
    public class PGY4Calendar
    {
        public Dictionary<string, Dictionary<string, string>> Schedule { get; private set; }

        private readonly Dictionary<string, List<string>> ElectivePreferences;

        public PGY4Calendar(List<Residents> residents)
        {
            var pgy4s = residents.FindAll(r => r.graduate_yr == 4);

            string[] months = new[]
            {
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
            };

            Schedule = new Dictionary<string, Dictionary<string, string>>();
            foreach (var r in pgy4s)
            {
                Schedule[$"{r.first_name} {r.last_name}"] = months.ToDictionary(m => m, m => "");
            }

            // Mock form submissions
            ElectivePreferences = new Dictionary<string, List<string>>
            {
                { "Alice Smith", new List<string> { "Chief", "TMS", "IOP", "VA", "Intp Psy", "HPC", "NFETC", "Addiction" } },
                { "Bob Jones",   new List<string> { "TMS", "Chief", "VA", "IOP", "Intp Psy", "HPC", "Comm", "NFETC" } },
                { "Charlie Brown", new List<string> { "IOP", "VA", "TMS", "Chief", "HPC", "Addiction", "Intp Psy", "Forensic" } },
                { "Dana White",  new List<string> { "VA", "TMS", "Chief", "IOP", "Intp Psy", "HPC", "NFETC", "Comm" } },
                { "Eve Black",   new List<string> { "Intp Psy", "TMS", "IOP", "Chief", "VA", "HPC", "Addiction", "Forensic" } }
            };

            // example of tracking how many residents are assigned to a task each month
            // in the final version each task has different rules, and we'll need to compare with pgy2 schedules as well
            // for now we just only allow two per month
            Dictionary<string, Dictionary<string, int>> monthTaskCount = months.ToDictionary(m => m, m => new Dictionary<string, int>());
            foreach (string month in months)
            {
                monthTaskCount[month] = new Dictionary<string, int>();
            }

            // track how many times each resident has been assigned each task. if the task has a higher priority this will be higher
            // each task has a limit of how many times they are allowed and required. all residents have to do two months of certain tasks so this will be important
            Dictionary<string, Dictionary<string, int>> residentTaskCount = pgy4s.ToDictionary(
                r => $"{r.first_name} {r.last_name}",
                r => new Dictionary<string, int>());

            foreach (Residents resident in pgy4s)
            {
                foreach (string task in ElectivePreferences[$"{resident.first_name} {resident.last_name}"])
                {
                    residentTaskCount[$"{resident.first_name} {resident.last_name}"][task] = 0;
                }
            }

            // Assign month-by-month
            foreach (var month in months)
            {
                foreach (var resident in pgy4s)
                {
                    string name = $"{resident.first_name} {resident.last_name}";
                    var prefs = ElectivePreferences[name];

                    // Find first task that:
                    // 1) Resident hasn't had more than 1 before (so max 2 total)
                    // 2) Month hasn't reached 3 residents for this task
                    string assigned = prefs.FirstOrDefault(task =>
                        residentTaskCount[name][task] < 2 &&
                        (!monthTaskCount[month].ContainsKey(task) || monthTaskCount[month][task] < 3)
                    );

                    if (assigned == null)
                    {
                        assigned = "Elective"; // fallback
                    }

                    Schedule[name][month] = assigned;

                    // Update counters
                    residentTaskCount[name][assigned]++;
                    if (!monthTaskCount[month].ContainsKey(assigned))
                    {
                        monthTaskCount[month][assigned] = 0;
                    }

                    monthTaskCount[month][assigned]++;
                }
            }
        }

        public void PrintSchedule()
        {
            foreach (KeyValuePair<string, Dictionary<string, string>> resident in Schedule)
            {
                Console.WriteLine($"--- {resident.Key} ---");
                foreach (KeyValuePair<string, string> month in resident.Value)
                {
                    Console.WriteLine($"{month.Key}: {month.Value}");
                }

                Console.WriteLine();
            }
        }
    }
}
