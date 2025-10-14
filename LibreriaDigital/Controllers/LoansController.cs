using LibreriaDigital.Models;
using LibreriaDigital.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace LibreriaDigital.Controllers
{
    [RoutePrefix("api/loans")]
    [Authorize]
    public class LoansController:ApiController
	{
        private readonly LoanService _loanService;

        public LoansController()
        {
            _loanService = new LoanService();
        }

        // GET: api/loans
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllLoans()
        {
            var loans = _loanService.GetAllLoans(); // Ya incluye Book y User

            var loanDtos = loans.Select(l => new LoanDto
            {
                Id = l.Id,
                BookId = l.BookId, // ← Asegúrate de incluirlo
                BookTitle = l.Book?.Title ?? "Libro desconocido",

                UserId = l.UserId, // ← Incluirlo
                Name = l.User?.Name ?? "Usuario desconocido",

                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                IsReturned = l.IsReturned
            }).ToList();

            return Ok(loanDtos);
        }

        // POST: api/loans
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateLoan(Loan loan)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdLoan = _loanService.CreateLoan(loan);
            return CreatedAtRoute("GetLoan", new { id = createdLoan.Id }, createdLoan);
        }

        //// PUT: api/loans/{id}
        //[HttpPut]
        //[Route("{id:int}")]
        //public IHttpActionResult ReturnLoan(int id, Loan loan)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var updatedLoan = _loanService.ReturnLoan(id, loan);
        //    if (updatedLoan == null)
        //        return NotFound();

        //    return Ok(updatedLoan);
        //}

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult UpdateLoan(int id, UpdateLoanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedLoan = _loanService.UpdateLoan(id, dto);
            if (updatedLoan == null)
                return NotFound();

            return Ok(updatedLoan);
        }

    }
}