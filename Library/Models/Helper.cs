using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    class UpdateHelper
    {

        static public string FromEmployee(string department, string FIO, string phone, string updateName)
        {
            DBManager manager = new DBManager();
            var empl = manager.GetContext().Employees.First(s => s.FIO == updateName);

            if (department != "Department")
                empl.Department = department;
            if (FIO != "FIO")
                empl.FIO = FIO;
            if (phone != "Phone")
                empl.Phone = phone;

            //return manager.Update<Employee>(manager.GetIdElement(TablesNames.Employees, updateName), empl);
            return manager.Update<Employee>(empl.Id, empl);
        }

        static public string FromBuyOrder(string orderedName, string bookTitle, string updateName)
        {
            DBManager manager = new DBManager();
            var idOrderBook = manager.GetContext().Lib.First(s => s.Book_title == updateName).Id;
            var buyOrder = manager.GetContext().BuyOrders.First(s => s.Id_book == idOrderBook);
            if (orderedName != "name ordered")
                buyOrder.Id_employee = manager.GetIdElement(TablesNames.Employees, orderedName);
            if (bookTitle != "book title")
                buyOrder.Id_book = idOrderBook;

            //return manager.Update<Buy_Order>(manager.GetIdElement(TablesNames.BuyOrders, updateName), buyOrder);
            return manager.Update<Buy_Order>(buyOrder.Id, buyOrder);
        }

        static public string FromReadingOrder(string orderedName, string bookTitle, string updateName)
        {
            DBManager manager = new DBManager();
            int idBook = manager.GetIdElement(TablesNames.Books, updateName);
            var readingOrder = manager.GetContext().ReadingOrders.First(s => s.Id_book == idBook);

            if (orderedName != "name ordered")
                readingOrder.Id_employee = manager.GetIdElement(TablesNames.Employees, orderedName);
            if (bookTitle != "book title")
                readingOrder.Id_book = manager.GetIdElement(TablesNames.Books, bookTitle);

            return manager.Update<Reading_Order>(readingOrder.Id, readingOrder);
        }

        static public string FromBook(string title, string desc, string numCop, string updateName)
        {
            DBManager manager = new DBManager();
            var book = manager.GetContext().Lib.First(s => s.Book_title == updateName);

            if (title != "book title")
                book.Book_title = title;
            if (desc != "short description")
                book.Short_description = desc;
            if (numCop != "number copies")
                book.Number_copies = Convert.ToInt32(numCop);

            //return manager.Update<Employee>(manager.GetIdElement(TablesNames.Books, updateName), book);
            return manager.Update<Employee>(book.Id, book);
        }

    }

    class InsertHelper
    {
        static public string FromEmployee(string department, string FIO, string phone)
        {
            DBManager manager = new DBManager();
            var empl = new Employee();

            if (department != "Department")
                empl.Department = department;
            if (FIO != "FIO")
                empl.FIO = FIO;
            if (phone != "Phone")
                empl.Phone = phone;

            return manager.Insert<Employee>(empl);
        }

        static public string FromBuyOrder(string orderedName, string bookTitle)
        {
            DBManager manager = new DBManager();
            var buyOrder = new Buy_Order();
            if (orderedName != "name ordered")
                buyOrder.Id_employee = manager.GetIdElement(TablesNames.Employees, orderedName);
            if (bookTitle != "book title")
                buyOrder.Id_book = manager.GetIdElement(TablesNames.Books, bookTitle);

            return manager.Insert<Buy_Order>(buyOrder);
        }

        static public string FromReadingOrder(string orderedName, string bookTitle)
        {
            DBManager manager = new DBManager();
            var readingOrder = new Reading_Order();

            if (orderedName != "name ordered")
                readingOrder.Id_employee = manager.GetIdElement(TablesNames.Employees, orderedName);
            if (bookTitle != "book title")
                readingOrder.Id_book = manager.GetIdElement(TablesNames.Books, bookTitle);

            return manager.Insert<Reading_Order>(readingOrder);
        }

        static public string FromBook(string title, string desc, string numCop)
        {
            DBManager manager = new DBManager();
            var book = new Book();

            if (title != "book title")
                book.Book_title = title;
            if (desc != "short description")
                book.Short_description = desc;
            if (numCop != "number copies")
                book.Number_copies = Convert.ToInt32(numCop);

            return manager.Insert<Book>(book);
        }

    }

    class DeleteHelper
    {
        static public string FromEmployee(string FIO)
        {
            DBManager manager = new DBManager();
            var empl = manager.GetContext().Employees.FirstOrDefault(s => s.FIO == FIO);

            return manager.Delete<Employee>(empl);
        }

        static public string FromBuyOrder(string bookTitle)
        {
            DBManager manager = new DBManager();
            var idOrderBook = manager.GetIdElement(TablesNames.Books, bookTitle);
            var buyOrder = manager.GetContext().BuyOrders.FirstOrDefault(s => s.Id_book== idOrderBook);

            return manager.Delete<Buy_Order>(buyOrder);
        }

        static public string FromReadingOrder(string ordered, string bookTitle)
        {
            DBManager manager = new DBManager();
            var readingOrder = new Reading_Order();

            Reading_Order order = new Reading_Order();
            int idBook = manager.GetIdElement(TablesNames.Books, bookTitle);
            var orders = manager.GetContext().ReadingOrders.Where(s => s.Id_book == idBook);
            int idOrdered = manager.GetIdElement(TablesNames.Employees, ordered);
            foreach (var b in orders)
            {
                if (b.Id_employee == idOrdered)
                {
                    order = b;
                    break;
                }
            }

            return manager.Delete<Reading_Order>(order);
        }

        static public string FromBook(string title)
        {
            DBManager manager = new DBManager();

            return manager.Delete<Book>(manager.GetContext().Lib.First(s => s.Book_title == title));
        }
    }

    class LibApp
    {
        static public List<BookForPage> GetListAvailableBooks()
        {
            DBManager manager = new DBManager();
            IEnumerable<Book> lib = manager.GetContext().Lib;
            List<BookForPage> books = new List<BookForPage>();
            int number = 0;
            List<Book> oldBooks = lib.ToList();
            foreach (var b in lib)
            {
                DBManager man2 = new DBManager();
                BookForPage newBook = new BookForPage
                {
                    Id = oldBooks[number].Id,
                    Book_title = oldBooks[number].Book_title,
                    Description = oldBooks[number].Short_description,
                    Number_copies_on_hands = oldBooks[number].Number_copies - LibApp.GetNumberBookOnHands(oldBooks[number], man2.GetContext())
                };
                books.Add(newBook);
                ++number;
            }
            return books;
        }

        static public int GetNumberBookOnHands(Book book, LibraryContext context)
        {
            var b = context.IssuedBooks.Where(s => s.Id_book == book.Id);
            return b.Count();

        }
        static public List<string> GetListNamesTables(string tableName)
        {
            switch (tableName)
            {
                case "books":
                    return new List<string> { "Id", "Book title", "Short description", "Number copies"};
                case "buy":
                    return new List<string> { "Id", "Id employee", "Id book"};
                case "reading":
                    return new List<string> { "Id", "Id book", "Id employee"};
                case "issued":
                    return new List<string> { "Id", "Id book", "Id employee", "Deadline" };
                default:
                    return new List<string> { "Id", "FIO", "Phone", "Department" };
            }
        }

        static public DbSet GetTable(string table)
        {
            DBManager manager = new DBManager();
            switch (table)
            {
                case "books":
                    return manager.GetContext().Lib;
                case "buy":
                    return manager.GetContext().BuyOrders;
                case "reading":
                    return manager.GetContext().ReadingOrders;
                case "issued":
                    return manager.GetContext().IssuedBooks;
                default:
                    return manager.GetContext().Employees;
            }
        }

    }

}
