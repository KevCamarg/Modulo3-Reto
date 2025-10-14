using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibreriaDigital.Models
{
    public class UpdateLoanDto
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }
    }

}