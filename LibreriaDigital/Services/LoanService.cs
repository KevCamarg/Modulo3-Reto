using LibreriaDigital.Data;
using LibreriaDigital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace LibreriaDigital.Services
{
	public class LoanService
	{
        private readonly ApplicationDbContext _context;

        public LoanService()
        {
            _context = new ApplicationDbContext();
        }

        public List<Loan> GetAllLoans()
        {
            return _context.Loans
               .Include(l => l.Book)
               .Include(l => l.User)
               .ToList();
        }

        public Loan CreateLoan(Loan loan)
        {
            loan.LoanDate = DateTime.Now; // Establecer la fecha de préstamo actual
            _context.Loans.Add(loan);
            _context.SaveChanges();
            return loan;
        }

        public Loan ReturnLoan(int id, Loan loan)
        {
            var existingLoan = _context.Loans.Find(id);
            if (existingLoan == null) return null;

            existingLoan.ReturnDate = DateTime.Now; // Establecer la fecha de devolución actual
            existingLoan.IsReturned = true;

            _context.SaveChanges();
            return existingLoan;
        }

        public Loan UpdateLoan(int id, UpdateLoanDto dto)
        {
            var loan = _context.Loans.FirstOrDefault(l => l.Id == id);
            if (loan == null)
                return null;

            // Mapear los datos del DTO al entity
            loan.BookId = dto.BookId;
            loan.UserId = dto.UserId;
            loan.LoanDate = dto.LoanDate;
            loan.DueDate = dto.DueDate;
            loan.IsReturned = dto.IsReturned;

            _context.SaveChanges();

            return loan;
        }

    }
}