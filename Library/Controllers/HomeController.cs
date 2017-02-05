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
       /* [HttpPost]
        public string BooksOnHands(string ex)
        {
            return "";
        }*/

        [HttpGet]
        public ActionResult ListBooks()
        {
            /*ssued_book issuedBook = new Issued_book { Id_book = 2, Id_employee = 4, Deadline = DateTime.Now.Date.AddDays(15) };
            manager.GetContext().IssuedBooks.Add(issuedBook);
            manager.GetContext().SaveChanges();*/

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
        public ActionResult ReadBook(int id)
        {
            ViewBag.BookId = id;
            return View();
        }

        [HttpPost]
        public string ReadBook(Reading_Order order)
        {
            var name = string.IsNullOrEmpty(Request.Params["Person"]) ? "" : Request.Params["Person"];
            Employee employee = manager.GetContext().Employees.Where(s => s.FIO == name).FirstOrDefault<Employee>();
            //order 

            return name;
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
            manager.Insert<Book>(new Book { Book_title = bookName, Short_description = "", Number_copies = 1 });
            order.Id_book = manager.GetIdElement(TablesNames.Books, bookName);
            order.Id_employee = manager.GetIdElement(TablesNames.Employees, orderedName);
            
            manager.Insert<Buy_Order>(order);
            return "Спасибо," + orderedName + ", за покупку!";
        }

        [HttpGet]
        public ActionResult Read()
        {
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
            string message = LibApp.Update(this);
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

        /*[HttpGet]
        public ActionResult Insert2(string tableName)
        {
            ViewBag.tableName = tableName;
            return View();
        }

        [HttpPost]
        public string Insert2()
        {
            string tableName = Request.QueryString["tableName"];
            string answer = "";
            if (tableName == "employee")
            {
                var department = string.IsNullOrEmpty(Request.Params["dep_name"]) ? "" : Request.Params["dep_name"];
                var FIO = string.IsNullOrEmpty(Request.Params["FIO_name"]) ? "" : Request.Params["FIO_name"];
                var phone = string.IsNullOrEmpty(Request.Params["phone_number"]) ? "" : Request.Params["phone_number"];
                answer = InsertHelper.FromEmployee(department, FIO, phone);
            }
            else if (tableName == "buy orders")
            {
                var ordered_name = string.IsNullOrEmpty(Request.Params["ordered_name"]) ? "" : Request.Params["ordered_name"];
                var book_title = string.IsNullOrEmpty(Request.Params["book_title"]) ? "" : Request.Params["book_title"];
                answer = InsertHelper.FromBuyOrder(ordered_name, book_title);
            }
            else if (tableName == "reading orders")
            {
                var orderedName = string.IsNullOrEmpty(Request.Params["ordered_name"]) ? "" : Request.Params["ordered_name"];
                var bookTitle = string.IsNullOrEmpty(Request.Params["book_title"]) ? "" : Request.Params["book_title"];
                answer = InsertHelper.FromReadingOrder(orderedName, bookTitle);
            }
            else if (tableName == "books")
            {
                var title = string.IsNullOrEmpty(Request.Params["title"]) ? "" : Request.Params["title"];
                var desc = string.IsNullOrEmpty(Request.Params["desc"]) ? "" : Request.Params["desc"];
                var numCop = string.IsNullOrEmpty(Request.Params["num_cop"]) ? "" : Request.Params["num_cop"];
                answer = InsertHelper.FromBook(title, desc, numCop);
            }
            return answer;
        }*/


       /* [HttpGet]
        public ActionResult UpdateTwo(string table, string update)
        {
            ViewBag.tableName = table;
            ViewBag.updateName = update;
            return View();
        }

        [HttpPost]
        public string UpdateTwo(string ex)
        {
            string tableName = Request.QueryString["table"];
            string updateName = Request.QueryString["update"];
            string answer = "";
            if (tableName == "employee")
            {
                var department = string.IsNullOrEmpty(Request.Params["dep_name"]) ? "" : Request.Params["dep_name"];
                var FIO = string.IsNullOrEmpty(Request.Params["FIO_name"]) ? "" : Request.Params["FIO_name"];
                var phone = string.IsNullOrEmpty(Request.Params["phone_number"]) ? "" : Request.Params["phone_number"];
                answer = UpdateHelper.FromEmployee(department, FIO, phone, updateName);
            }
            else if (tableName == "buy orders")
            {
                var ordered_name = string.IsNullOrEmpty(Request.Params["ordered_name"]) ? "" : Request.Params["ordered_name"];
                var book_title = string.IsNullOrEmpty(Request.Params["book_title"]) ? "" : Request.Params["book_title"];
                answer = UpdateHelper.FromBuyOrder(ordered_name, book_title, updateName);
            }
            else if (tableName == "reading orders")
            {
                var orderedName = string.IsNullOrEmpty(Request.Params["ordered_name"]) ? "" : Request.Params["ordered_name"];
                var bookTitle = string.IsNullOrEmpty(Request.Params["book_title"]) ? "" : Request.Params["book_title"];
                answer = UpdateHelper.FromReadingOrder(orderedName, bookTitle, updateName);
            }
            else if (tableName == "books")
            {
                var title = string.IsNullOrEmpty(Request.Params["title"]) ? "" : Request.Params["title"];
                var desc = string.IsNullOrEmpty(Request.Params["desc"]) ? "" : Request.Params["desc"];
                var numCop = string.IsNullOrEmpty(Request.Params["num_cop"]) ? "" : Request.Params["num_cop"];
                answer = UpdateHelper.FromBook(title, desc, numCop, updateName);
            }
            return answer;
        }*/

        [HttpGet]
        public ActionResult Delete(string table, int Id)
        {
            string message = LibApp.Delete(this, table, Id);
            Response.Redirect("/Home/WorkWithBD?table=" + table + "&message=" + message);
            return View();
        }

        [HttpPost]
        public void Delete(string ex = "")
        {
            /*var tableName = string.IsNullOrEmpty(Request.Params["table_name"]) ? "" : Request.Params["table_name"];
            var firstSection = string.IsNullOrEmpty(Request.Params["first_section"]) ? "" : Request.Params["first_section"];
            var secondSection = string.IsNullOrEmpty(Request.Params["second_section"]) ? "" : Request.Params["second_section"];
            string answer = "";
            if (tableName == "employee")
            {
                answer = DeleteHelper.FromEmployee(firstSection == "" ? secondSection : firstSection);
            }
            else if (tableName == "buy orders")
            {
                answer = DeleteHelper.FromBuyOrder(firstSection == "" ? secondSection : firstSection);
            }
            else if (tableName == "reading orders")
            {
                answer = DeleteHelper.FromReadingOrder(firstSection, secondSection);
            }
            else if (tableName == "books")
            {
                answer = DeleteHelper.FromBook(firstSection == "" ? secondSection : firstSection);
            }*/

        }

        [HttpGet]
        public ActionResult WorkWithBD(string table, string message)
        {
            ViewBag.NamesList = LibApp.GetListNamesTables(table);
            ViewBag.Message = message;
            ViewBag.Table = LibApp.GetTable(table);
            ViewBag.TableName = table;
            return View();
        }

        [HttpPost]
        public void WorkWithBD()
        {
            var str = Request.Form["add_record"];
            if (Request.Form["add_record"] == "Добавить")
            {
                var tableName = Request.Params["table"];
                string message = LibApp.Insert(this);
                Response.Redirect("/Home/WorkWithBD?table=" + tableName + "&message=" + message);
            }
            else if (Request.Form["delete"] == "Удалить")
            {
                var Id = ViewBag.ID;
                string mes = "";
            }
            /*else if (Request.Form["update"] == "Изменить")
            {

            }*/

        }

    }
}