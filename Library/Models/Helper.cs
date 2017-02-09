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
        static public string FromIssued(int Id, int Id_employee, int Id_books, string Deadline)
        {
            try
            {
                DBManager manager = new DBManager();
                var book = manager.GetContext().IssuedBooks.Find(Id);

                book.Id_book = Id_books;
                book.Id_employee = Id_employee;
                book.Deadline = System.DateTime.Parse(Deadline);

                //return manager.Update<Employee>(manager.GetIdElement(TablesNames.Employees, updateName), empl);
                return manager.Update<Issued_book>(book.Id, book);
            }
            catch(Exception ex)
            {
                return "Не удалось обновить запись. Ошибка: " + ex.Message;
            }
        }

        static public string FromEmployee(int Id, string department, string FIO, string phone, int Read_books)
        {
            

            try
            {
                DBManager manager = new DBManager();
                var empl = manager.GetContext().Employees.Find(Id);

                empl.Department = department;
                empl.FIO = FIO;
                empl.Phone = phone;
                empl.Read_books = Read_books;

                //return manager.Update<Employee>(manager.GetIdElement(TablesNames.Employees, updateName), empl);
                return manager.Update<Employee>(empl.Id, empl);
            }
            catch (Exception ex)
            {
                return "Не удалось обновить запись. Ошибка: " + ex.Message;
            }
        }

        //static public string FromBuyOrder(string orderedName, string bookTitle, string updateName)
        static public string FromBuyOrder(int Id, int Id_employee, int Id_book)
        {
            

            try
            {
                DBManager manager = new DBManager();
                var buyOrder = manager.GetContext().BuyOrders.First(s => s.Id_book == Id);
                buyOrder.Id_book = Id_book;
                buyOrder.Id_employee = Id_employee;

                //return manager.Update<Buy_Order>(manager.GetIdElement(TablesNames.BuyOrders, updateName), buyOrder);
                return manager.Update<Buy_Order>(buyOrder.Id, buyOrder);
            }
            catch (Exception ex)
            {
                return "Не удалось обновить запись. Ошибка: " + ex.Message;
            }
        }

        static public string FromReadingOrder(int Id, int Id_employee, int Id_book)
        {
            try
            {
                DBManager manager = new DBManager();
                var readingOrder = manager.GetContext().ReadingOrders.Find(Id);

                readingOrder.Id_employee = Id_employee;
                readingOrder.Id_book = Id_book;

                return manager.Update<Reading_Order>(readingOrder.Id, readingOrder);
            }
            catch (Exception ex)
            {
                return "Не удалось обновить запись. Ошибка: " + ex.Message;
            }
        }

        //TODO: сделать проверку на то что количество всех копий книги при обновлении не будет меньше чем количество книг на руках
        static public string FromBook(int Id, string title, string desc, string numCop, string howMany)
        {
            
            try
            {
                DBManager manager = new DBManager();
                var book = manager.GetContext().Lib.Find(Id);

                book.Book_title = title;
                book.Short_description = desc;
                book.Number_copies = Convert.ToInt32(numCop);
                book.How_many_times = Convert.ToInt32(howMany);

                //return manager.Update<Employee>(manager.GetIdElement(TablesNames.Books, updateName), book);
                return manager.Update<Employee>(book.Id, book);
            }
            catch (Exception ex)
            {
                return "Не удалось обновить запись. Ошибка: " + ex.Message;
            }
        }

    }

    class InsertHelper
    {
        static public string FromEmployee(string department, string FIO, string phone, string read_books)
        {
            try
            {
                DBManager manager = new DBManager();
                var empl = new Employee();

                empl.Department = department;
                empl.FIO = FIO;
                empl.Phone = phone;
                empl.Read_books = Convert.ToInt32(read_books);

                return manager.Insert<Employee>(empl);
            }
            catch (Exception ex)
            {
                return "Не удалось добавить новую запись. Ошибка: " + ex.Message;
            }
        }

        static public string FromBuyOrder(int Id_employee, int Id_book)
        {
            try
            {
                DBManager manager = new DBManager();
                var buyOrder = new Buy_Order();
                buyOrder.Id_employee = Id_employee;
                buyOrder.Id_book = Id_book;

                return manager.Insert<Buy_Order>(buyOrder);
            }
            catch (Exception ex)
            {
                return "Не удалось добавить новую запись. Ошибка: " + ex.Message;
            }
        }

        static public string FromReadingOrder(int Id_employee, int Id_book)
        {
            try
            {
                DBManager manager = new DBManager();
                var readingOrder = new Reading_Order();

                readingOrder.Id_employee = Id_employee;
                readingOrder.Id_book = Id_book;

                return manager.Insert<Reading_Order>(readingOrder);
            }
            catch (Exception ex)
            {
                return "Не удалось добавить новую запись. Ошибка: " + ex.Message;
            }
        }

        static public string FromBook(string title, string desc, string numCop, string howMany)
        {
            try
            {
                DBManager manager = new DBManager();
                var book = new Book();

                book.Book_title = title;
                book.Short_description = desc;
                book.Number_copies = Convert.ToInt32(numCop);
                book.How_many_times = Convert.ToInt32(howMany);

                return manager.Insert<Book>(book);
            }
            catch (Exception ex)
            {
                return "Не удалось добавить новую запись. Ошибка: " + ex.Message;
            }
        }

        static public string FromIssued(int Id_book, int Id_employee, string Deadline)
        {

            try
            {
                DBManager manager = new DBManager();
                var book = new Issued_book();

                book.Id_book = Id_book;
                book.Id_employee = Id_employee;
                book.Deadline = System.DateTime.Parse(Deadline);

                //return manager.Update<Employee>(manager.GetIdElement(TablesNames.Employees, updateName), empl);
                return manager.Insert<Issued_book>(book);
            }
            catch(Exception ex)
            {
                return "Не удалось добавить новую запись. Ошибка: " + ex.Message;
            }
        }

    }

    class DeleteHelper
    {
        static public string FromIssued(int Id)
        {
            try
            {
                DBManager manager = new DBManager();
                var book = new Issued_book { Id = Id };

                return manager.Delete<Issued_book>(book);
            }
            catch (Exception ex)
            {
                return "Не удалось удалить запись. Ошибка: " + ex.Message;
            }
        }

        static public string FromEmployee(int Id)
        {
            try
            {
                DBManager manager = new DBManager();
                var empl = manager.GetContext().Employees.Find(Id);

                return manager.Delete<Employee>(empl);
            }
            catch(Exception ex)
            {
                return "Не удалось удалить запись. Ошибка: " + ex.Message;
            }
        }

        static public string FromBuyOrder(int Id)
        {
            try
            {
                DBManager manager = new DBManager();
                var buyOrder = manager.GetContext().BuyOrders.Find(Id);

                return manager.Delete<Buy_Order>(buyOrder);
            }
            catch (Exception ex)
            {
                return "Не удалось удалить запись. Ошибка: " + ex.Message;
            }
           
        }

        static public string FromReadingOrder(int Id)
        {
            try
            {
                DBManager manager = new DBManager();
                var readingOrder = new Reading_Order();

                Reading_Order order = new Reading_Order { Id = Id };

                return manager.Delete<Reading_Order>(order);
            }
            catch (Exception ex)
            {
                return "Не удалось удалить запись. Ошибка: " + ex.Message;
            }

        }
        static public string FromBook(int Id)
        {
            try
            {
                DBManager manager = new DBManager();

                return manager.Delete<Book>(manager.GetContext().Lib.Find(Id));
            }
            catch (Exception ex)
            {
                return "Не удалось удалить запись. Ошибка: " + ex.Message;
            }
            
        }
    }

    class LibApp
    {
        public InsertHelper insertHelper = new InsertHelper();

        public UpdateHelper updateHelper = new UpdateHelper();

        public DeleteHelper deleteHelper = new DeleteHelper();


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
                    return new List<string> { "Id", "Book title", "Short description", "Number copies", "How many times" };
                case "buy":
                    return new List<string> { "Id", "Id employee", "Id book"};
                case "reading":
                    return new List<string> { "Id", "Id book", "Id employee"};
                case "issued":
                    return new List<string> { "Id", "Id book", "Id employee", "Deadline" };
                default:
                    return new List<string> { "Id", "FIO", "Phone", "Department", "Read_books" };
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

        static public string Update(string tableName, Library.Controllers.HomeController controller)
        {
            switch (tableName)
            {
                case "buy":
                    return UpdateHelper.FromBuyOrder(Convert.ToInt32(controller.Request.Params["Id"]),
                        Convert.ToInt32(controller.Request.Params["Id_employee"]),
                        Convert.ToInt32(controller.Request.Params["Id_book"]));
                case "books":
                    return UpdateHelper.FromBook(Convert.ToInt32(controller.Request.Params["Id"]),
                        controller.Request.Params["Book_title"],
                        controller.Request.Params["Short_description"],
                        controller.Request.Params["Number_copies"],
                        controller.Request.Params["How_many"]);
                case "reading":
                    return UpdateHelper.FromReadingOrder(Convert.ToInt32(controller.Request.Params["Id"]),
                        Convert.ToInt32(controller.Request.Params["Id_employee"]),
                        Convert.ToInt32(controller.Request.Params["Id_book"]));
                case "employees":
                    return UpdateHelper.FromEmployee(Convert.ToInt32(controller.Request.Params["Id"]),
                        (controller.Request.Params["Department"]),
                        (controller.Request.Params["FIO"]),
                        (controller.Request.Params["Phone"]),
                        Convert.ToInt32(controller.Request.Params["Read_books"]));
                default:
                    return UpdateHelper.FromIssued(Convert.ToInt32(controller.Request.Params["Id"]),
                        Convert.ToInt32(controller.Request.Params["Id_employee"]),
                        Convert.ToInt32(controller.Request.Params["Id_book"]),
                        controller.Request.Params["Deadline"]);
            }
        }

        static public string Insert(string tableName, Library.Controllers.HomeController controller)
        {
            switch (tableName)
            {
                case "buy":
                    return InsertHelper.FromBuyOrder(Convert.ToInt32(controller.Request.Params["Id_employee"]),
                        Convert.ToInt32(controller.Request.Params["Id_book"]));
                case "books":
                    return InsertHelper.FromBook(controller.Request.Params["Book_title"],
                        controller.Request.Params["Short_description"],
                        controller.Request.Params["Number_copies"],
                        controller.Request.Params["How_many"]);
                case "reading":
                    return InsertHelper.FromReadingOrder(Convert.ToInt32(controller.Request.Params["Id_employee"]),
                        Convert.ToInt32(controller.Request.Params["Id_book"]));
                case "employees":
                    return InsertHelper.FromEmployee(controller.Request.Params["Department"],
                        controller.Request.Params["FIO"],
                        controller.Request.Params["Phone"],
                        controller.Request.Params["Read_books"]);
                default:
                    return InsertHelper.FromIssued(Convert.ToInt32(controller.Request.Params["Id_book"]),
                        Convert.ToInt32(controller.Request.Params["Id_employee"]),
                        controller.Request.Params["Deadline"]);
            }
        }

        static public string Delete(Library.Controllers.HomeController controller, string table, int id)
        {
            switch (table)
            {
                case "buy":
                    return DeleteHelper.FromBuyOrder(id);
                case "books":
                    return DeleteHelper.FromBook(id);
                case "reading":
                    return DeleteHelper.FromReadingOrder(id);
                case "employees":
                    return DeleteHelper.FromEmployee(id);
                default:
                    return DeleteHelper.FromIssued(id);
            }
        }

        static public List<Return_Book> GetListReturnBooks()
        {
            DBManager manager = new DBManager();
            var issued = manager.GetContext().IssuedBooks;
            //var lib = manager.GetContext().Lib;
            //var employee = manager.GetContext().Employees;
            List<Return_Book> set = new List<Return_Book>();
            foreach(var issBook in issued)
            {
                DBManager man2 = new DBManager();
                Return_Book book = new Return_Book
                {
                    Id = issBook.Id,
                    Book_title = man2.GetContext().Lib.Find(issBook.Id_book).Book_title,
                    FIO_employee = man2.GetContext().Employees.Find(issBook.Id_employee).FIO,
                    Deadline = issBook.Deadline
                };

                set.Add(book);
                
            }
            return set;
        }

        static public List<Orders> GetListOrders(string type)
        {
            DBManager manager = new DBManager();
            List<Orders> orders = new List<Orders>();
            if (type == "buy")
            {
                foreach(var order in manager.GetContext().BuyOrders)
                {
                    DBManager man2 = new DBManager();
                    orders.Add(new Orders
                    {
                        Id = order.Id,
                        Book_title = man2.GetContext().Lib.Find(order.Id_book).Book_title,
                        Employee = man2.GetContext().Employees.Find(order.Id_employee).FIO
                    });
                }
            }
            else if (type == "reading")
            {
                foreach (var order in manager.GetContext().ReadingOrders)
                {
                    DBManager man2 = new DBManager();
                    orders.Add(new Orders
                    {
                        Id = order.Id,
                        Book_title = man2.GetContext().Lib.Find(order.Id_book).Book_title,
                        Employee = man2.GetContext().Employees.Find(order.Id_employee).FIO
                    });
                }
            }

            return orders;
        }

        static public bool DelInOrders(string mode, string id, string type)
        {
            try
            {
                DBManager manager = new DBManager();

                if (type == "buy")
                {
                    var context = manager.GetContext().BuyOrders;
                    var order = context.Find(id);
                    if (mode == "update")
                    {
                        //UpdateHelper.FromBuyOrder(order.Id)
                    }
                }
                else if (type == "read")
                {

                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

    }

}
