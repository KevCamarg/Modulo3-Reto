using LibreriaDigital.Models;
using LibreriaDigital.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LibreriaDigital.Controllers
{
    [RoutePrefix("api/books")]
    [Authorize]
    public class BooksController : ApiController
    {
        private readonly BookService _bookService;

        public BooksController()
        {
            _bookService = new BookService();
        }

        // GET: api/books
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllBooks()
        {
            var books = _bookService.GetAllBooks();
            return Ok(books);
        }

        // GET: api/books/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetBook(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        // POST: api/books
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateBook(Book book)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBook = _bookService.CreateBook(book);
            return CreatedAtRoute("GetBook", new { id = createdBook.Id }, createdBook);
        }

        // PUT: api/books/{id}
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult UpdateBook(int id, Book book)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedBook = _bookService.UpdateBook(id, book);
            if (updatedBook == null)
                return NotFound();

            return Ok(updatedBook);
        }

        // DELETE: api/books/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult DeleteBook(int id)
        {
            var result = _bookService.DeleteBook(id);
            if (!result)
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
