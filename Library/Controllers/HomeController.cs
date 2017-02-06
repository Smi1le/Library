using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Library.Models;
using MySql.Data.MySqlClient;


namespace Library.Controllers
{
    public class HomeController : Controller
    {
        DBManager manager = new DBManager();

        [HttpGet]
        public ActionResult BooksOnHands()
        {
            IEnumerable<Book> lib = manager.GetContext().Lib;
            List<BookOnHands> books = new List<BookOnHands>();
            int number = 0;
            List<Book> oldBooks = lib.ToList();
            foreach (var b in lib)
            {
                DBManager man2 = new DBManager();
                BookOnHands newBook = new BookOnHands
                {
                    Book_title = oldBooks[number].Book_title,
                    Number_copies_on_hands = LibApp.GetNumberBookOnHands(oldBooks[number], man2.GetContext())
                };
                books.Add(newBook);
                ++number;
            }
            ViewBag.Lib = books;

            return View();
        }

        [HttpGet]
        public ActionResult ListBooks()
        {
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
            
            ViewBag.Lib = books;

            return View();
        }

        [HttpGet]
        public ActionResult ReadBook(int id, string type)
        {
            ViewBag.BookId = id;
            ViewBag.Employees = manager.GetContext().Employees;
            return View();
        }

        [HttpPost]
        public void ReadBook()
        {
            try
            {
                var type = Request.Params["Type"];
                var name = Request.Params["selection"];
                var idBook = Request.Params["Id_book"];
                string message = "";
                Employee employee = manager.GetContext().Employees.Where(s => s.FIO == name).FirstOrDefault();

                if (type == "read,")
                {
                    message += UpdateHelper.FromEmployee(employee.Id, employee.Department, employee.FIO, employee.Phone, employee.Read_books + 1);
                    message += InsertHelper.FromIssued(Convert.ToInt32(idBook), employee.Id, DateTime.Now.AddDays(15).ToString());
                    var d = manager.GetContext().ReadingOrders.Where(s => s.Id_employee == employee.Id);
                    if (d != null)
                    {
                        foreach(var b in d)
                        {
                            if (b.Id_book == Convert.ToInt32(idBook))
                                DeleteHelper.FromReadingOrder(b.Id);
                        }
                    }
                }
                else if (type == "order,")
                    message = InsertHelper.FromReadingOrder(employee.Id, Convert.ToInt32(idBook));

                Response.Redirect("/Home/Read?message=" + message);
            }
            catch(Exception ex)
            {
                Response.Redirect("/Home/Read?message=" + ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Buy()
        {
            return View();
        }

        [HttpPost]
        public string Buy(Buy_Order order)
        {
            var orderedName = string.IsNullOrEmpty(Request.Params["ordered_name"]) ? "" : Request.Params["ordered_name"];
            var bookName = string.IsNullOrEmpty(Request.Params["book_name"]) ? "" : Request.Params["book_name"];
            manager.Insert<Book>(new Book { Book_title = bookName, Short_description = "", Number_copies = 0 });
            order.Id_book = manager.GetIdElement(TablesNames.Books, bookName);
            order.Id_employee = manager.GetIdElement(TablesNames.Employees, orderedName);
            
            manager.Insert<Buy_Order>(order);
            return "Спасибо," + orderedName + ", за покупку!";
        }

        [HttpGet]
        public ActionResult Read(string message)
        {
            ViewBag.Message = message;
            ViewBag.NamesList = new List<string> { "Название книги", "Краткое описание", "Количество свободных книг" };
            ViewBag.Table = LibApp.GetListAvailableBooks();

            return View();
        }

        [HttpPost]
        public string Read(Reading_Order order)
        {
            var orderedName = string.IsNullOrEmpty(Request.Params["ordered_name"]) ? "" : Request.Params["ordered_name"];
            var nameBook = string.IsNullOrEmpty(Request.Params["name_book"]) ? "" : Request.Params["name_book"];
            Book book = manager.GetContext().Lib.First(s => s.Book_title == nameBook);
            Employee employee = manager.GetContext().Employees.FirstOrDefault(s => s.FIO == orderedName);
            order.Id_book = book.Id;
            order.Id_employee = employee.Id;

            manager.GetContext().ReadingOrders.Add(order);
            manager.GetContext().SaveChanges();
            return "Спасибо," + order.Id_book + ", за покупку!" + " Название книги: " + order.Id_employee;
        }

        public ActionResult Index()
        {
            List<string> l = new List<string> { "books", "reading", "buy", "employees", "issued" };
            ViewBag.List = l;
            ViewBag.IssuedBooks = manager.GetContext().IssuedBooks;


            return View();
        }
        [HttpGet]
        public ActionResult Update(string table, int Id)
        {
            ViewBag.TableName = table;
            ViewBag.Element = LibApp.GetTable(table).Find(Id);
            return View();
        }

        [HttpPost]
        public void Update()
        {
            var tableName = string.IsNullOrEmpty(Request.Params["table_name"]) ? "" : Request.Params["table_name"];
            string message = LibApp.Update(tableName, this);
            Response.Redirect("/Home/WorkWithBD?table=" + tableName + "&message=" + message);
        }

        [HttpGet]
        public ActionResult Insert()
        {
            return View();
        }

        [HttpPost]
        public void Insert(string ex = "")
        {
            var tableName = string.IsNullOrEmpty(Request.Params["table_name"]) ? "" : Request.Params["table_name"];
            Response.Redirect("/Home/Insert2?tableName=" + tableName);
        }

        [HttpGet]
        public ActionResult Delete(string table, int Id, string returnPage)
        {
            

            if (returnPage == "WorkWithDB")
            {
                string message = LibApp.Delete(this, table, Id);
                Response.Redirect("/Home/WorkWithBD?table=" + table + "&message=" + message);
            }
            else if(returnPage == "Return")
            {
                DBManager man2 = new DBManager();
                var t = man2.GetContext().IssuedBooks;
                var order = t.Find(Id);
                var book = manager.GetContext().Lib.Find(order.Id_book);
                UpdateHelper.FromBook(book.Id, book.Book_title, book.Short_description, book.Number_copies.ToString(), Convert.ToString(book.How_many_times + 1));
                string message = LibApp.Delete(this, table, Id);
                Response.Redirect("/Home/Return?message=" + message);
            }
            return View();
        }

        [HttpGet]
        public ActionResult WorkWithBD(string table, string message)
        {
            ViewBag.NamesList = LibApp.GetListNamesTables(table);
            ViewBag.Message = message;
            ViewBag.Table = LibApp.GetTable(table);
            ViewBag.TableName = table;
            ViewBag.Context = manager.GetContext();
            return View();
        }

        [HttpPost]
        public void WorkWithBD()
        {
            var str = Request.Form["add_record"];
            if (Request.Form["add_record"] == "Добавить")
            {
                var tableName = Request.Params["table"];
                string message = LibApp.Insert(tableName, this);
                Response.Redirect("/Home/WorkWithBD?table=" + tableName + "&message=" + message);
            }

        }
        [HttpGet]
        public ActionResult Return(string message)
        {
            ViewBag.Table = LibApp.GetListReturnBooks();//manager.GetContext().IssuedBooks;
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public void Return()
        {
            int id = Convert.ToInt32(Request.Params["id"]);
            string message = DeleteHelper.FromIssued(id);
            Response.Redirect("/Home/Return?message=" + message);
        }

        [HttpGet]
        public ActionResult Rating(string type)
        {
            ViewBag.Type = type;
            if (type == "Работники")
                ViewBag.Table = manager.GetContext().Employees.OrderByDescending(s => s.Read_books);
            else if (type == "Книги")
                ViewBag.Table = manager.GetContext().Lib.OrderByDescending(s => s.How_many_times);


            return View();
        }

        [HttpPost]
        public void Rating()
        {
            string type = Request.Params["selection"];
            Response.Redirect("/Home/Rating?type=" + type);
        }

    }
}