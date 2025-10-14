using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LibreriaDigital.Models
{
	public class Loan
	{
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID del libro es obligatorio.")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "La fecha de préstamo es obligatoria.")]
        public DateTime LoanDate { get; set; }

        [Required(ErrorMessage = "La fecha de entrega esperada es obligatoria.")]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; } // Puede ser nulo si no se ha devuelto
        public bool IsReturned { get; set; } = false; // Estado de devolución
    }
}