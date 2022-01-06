using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONE.Utilities
{
    
    public class Season
    {
        public Season(DateTime CurrDate)
        {
            int CurrYear = CurrDate.Year;
            if (CurrDate >= new DateTime(CurrYear, 9, 20) && CurrDate <= new DateTime(CurrYear, 12, 19))
            {
                StartDate = new DateTime(CurrYear, 9, 20);
                EndDate = new DateTime(CurrYear, 12, 19);
                Description = "Fall";
            }
            else if ((CurrDate >= new DateTime(CurrYear, 12, 20) && CurrDate <= new DateTime(CurrYear, 12, 31)) || (CurrDate >= new DateTime(CurrYear, 1, 1) && CurrDate <= new DateTime(CurrYear, 3, 20)))
            {
                if (CurrDate.Month == 12)
                {
                    StartDate = new DateTime(CurrYear, 12, 20);
                    EndDate = new DateTime(CurrYear + 1, 3, 20);
                }
                else
                {
                    StartDate = new DateTime(CurrYear - 1, 12, 20);
                    EndDate = new DateTime(CurrYear, 3, 20);
                }

                Description = "Winter";
            }
            else if (CurrDate >= new DateTime(CurrYear, 3, 21) && CurrDate <= new DateTime(CurrYear, 6, 19))
            {
                StartDate = new DateTime(CurrYear, 3, 21);
                EndDate = new DateTime(CurrYear, 6, 19);
                Description = "Spring";
            }
            else if (CurrDate >= new DateTime(CurrYear, 6, 20) && CurrDate <= new DateTime(CurrYear, 9, 19))
            {
                StartDate = new DateTime(CurrYear, 6, 20);
                EndDate = new DateTime(CurrYear, 9, 19);
                Description = "Summer";
            }
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
    }
}
