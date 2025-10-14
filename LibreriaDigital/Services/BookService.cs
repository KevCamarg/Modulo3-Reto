using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibreriaDigital.Models;
using LibreriaDigital.Data;

namespace LibreriaDigital.Services
{
	public class BookService
	{
        private readonly ApplicationDbContext _context;

        public BookService()
        {
            _context = new ApplicationDbContext();
        }

        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public Book GetBookById(int id)
        {
            return _context.Books.Find(id);
        }

        public Book CreateBook(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return book;
        }

        public Book UpdateBook(int id, Book book)
        {
            var existingBook = _context.Books.Find(id);
            if (existingBook == null) return null;

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.ISBN = book.ISBN;
            existingBook.Quantity = book.Quantity;

            _context.SaveChanges();
            return existingBook;
        }

        public bool DeleteBook(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            _context.SaveChanges();
            return true;
        }
    }
}