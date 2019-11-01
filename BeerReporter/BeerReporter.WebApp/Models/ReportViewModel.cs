using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeerReporter.WebApp.Models
{
    public class ReportViewModel
    {
        public string Location { get; set; }

        public string SasUri { get; set; }

        public string GenerateMessage { get; set; }

        public string FetchMessage { get; set; }

        public void RemoveNulls()
        {
            if (string.IsNullOrEmpty(SasUri))
                SasUri = "";

            if (string.IsNullOrEmpty(Location))
                Location = "";

            if (string.IsNullOrEmpty(GenerateMessage))
                GenerateMessage = "";

            if (string.IsNullOrEmpty(FetchMessage))
                FetchMessage = "";
        }
    }
}
