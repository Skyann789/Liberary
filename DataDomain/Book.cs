using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDomain
{
    public class Book
    {
        public int BookID { get; set; }
        public string StatusID { get; set; }
        public string AuthorGivenName { get; set; }
        public string AuthorFamilyName { get; set; }
        public string GenreName { get; set; }
        public string Title { get; set; }
        public int PublishedYear { get; set; }
        public bool Active { get; set; }

        // Book's Author details
        public string AuthorID { get; set; }

        // Book's Genre details
        public string GenreID { get; set; }

        // Book's Reservation details
        public Member ReservedBy { get; set; }
        public Staff ReservedByStaff { get; set; }
        public DateTime ReservationDate { get; set; }

        // Book's Maintenance details
        public DateTime MaintenanceDate { get; set; }
        public Staff MaintenanceByStaff { get; set; }
        public string Description {get; set;}

        // Concatenates the given name and the family name to form the full name of the author
        public string AuthorFullName => $"{AuthorGivenName} {AuthorFamilyName}";


    }


}
