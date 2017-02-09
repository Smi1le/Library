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
                var idBook = Convert.ToInt32(Request.Params["Id_book"]);
                string message = "";
                Employee employee = manager.GetContext().Employees.Where(s => s.FIO == name).FirstOrDefault();
                Book book = manager.GetContext().Lib.Find(idBook);
                var issBook = manager.GetContext().IssuedBooks.Where(s => s.Id_book == idBook);
                
                    if (type == "read,")
                    {
                        if (book.Number_copies - issBook.Count() > 0)
                        {
                            message += UpdateHelper.FromEmployee(employee.Id, employee.Department, employee.FIO, employee.Phone, employee.Read_books + 1);
                            message += InsertHelper.FromIssued(idBook, employee.Id, DateTime.Now.AddDays(15).ToString());
                            var d = manager.GetContext().ReadingOrders.Where(s => s.Id_employee == employee.Id);
                            if (d != null)
                            {
                                foreach (var b in d)
                                {
                                    if (b.Id_book == idBook)
                                        DeleteHelper.FromReadingOrder(b.Id);
                                }
                            }

                        }
                        else
                            message = "В данный момент нет свободных книг. Вы можете оформить заказ на чтение";
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
        public ActionResult Buy(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public void Buy(Buy_Order order)
        {
            var orderedName = string.IsNullOrEmpty(Request.Params["ordered_name"]) ? "" : Request.Params["ordered_name"];
            var bookName = string.IsNullOrEmpty(Request.Params["book_name"]) ? "" : Request.Params["book_name"];
            manager.Insert<Book>(new Book { Book_title = bookName, Short_description = "", Number_copies = 0 });
            order.Id_book = manager.GetIdElement(TablesNames.Books, bookName);
            order.Id_employee = manager.GetIdElement(TablesNames.Employees, orderedName);
            
            manager.Insert<Buy_Order>(order);
            string message = "Заказ на покупку был оформлен";
            Response.Redirect("/Home/Buy?mes=" + message);
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
        public ActionResult WorkWithBD(string table, string message, string mode)
        {
            ViewBag.NamesList = LibApp.GetListNamesTables(table);
            ViewBag.Message = message;
            ViewBag.Table = LibApp.GetTable(table);
            ViewBag.TableName = table;
            ViewBag.Context = manager.GetContext();
            ViewBag.Mode = mode;
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

        [HttpGet]
        public ActionResult Add(string message)
        {
            ViewBag.Table = manager.GetContext().Lib;
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public void Add()
        {
            string new_name = Request.Params["name_new_book"];
            string desc = Request.Params["desc_new_book"];
            Response.Redirect("/Home/AddBook?name=" + new_name + "&desc=" + desc + "&type=new");
        }

        [HttpGet]
        public void AddBook(string id, string type, string name, string desc)
        {
            string message = "";
            if (type == "add")
            {
                try
                {
                    DBManager manager = new DBManager();
                    var book = manager.GetContext().Lib.Find(Convert.ToInt32(id));
                    UpdateHelper.FromBook(Convert.ToInt32(id), book.Book_title, book.Short_description, Convert.ToString(book.Number_copies + 1), book.How_many_times.ToString());
                    message = "Запись успешно обновлена";
                }
                catch (Exception ex)
                {
                    message = "Не удалось обновить запись. Попробуйте еще раз";
                }
            }
            else if (type == "new")
            {
                try
                {
                    DBManager manager = new DBManager();
                    Book book = new Book
                    {
                        Book_title = name,
                        Short_description = desc,
                        Number_copies = 1,
                        How_many_times = 0
                    }; 
                    InsertHelper.FromBook(name, desc, Convert.ToString(1), Convert.ToString(0));
                    int idBook = manager.GetContext().Lib.First(s => s.Book_title == name).Id;
                    DBManager man2 = new DBManager();

                    var order = man2.GetContext().BuyOrders.Where(s => s.Id_book == idBook);
                    if (order.Count() > 0)
                    {
                        foreach (var el in order)
                        {
                            DeleteHelper.FromBuyOrder(el.Id);
                        }
                    }
                    message = "Запись успешно обновлена";
                }
                catch (Exception ex)
                {
                    message = "Не удалось обновить запись. Попробуйте еще раз";
                }
            }
            else
            {
                message = "Не удалось обновить запись. Попробуйте еще раз";
            }
            Response.Redirect("/Home/Add?message=" + message);
        }

        [HttpGet]
        public ActionResult Orders(string type, string message)
        {


            ViewBag.TableName = type == "buy" ? "buy" : "reading";
            ViewBag.NamesList = new List<string>{ "Название книги", "Имя заказавшего работника"};
            ViewBag.Type = type;
            ViewBag.Table = LibApp.GetListOrders(type);
            return View();
        }

        [HttpPost]
        public void Orders()
        {

        }

        [HttpGet]
        public ActionResult UpdateToOrders(string type, string id, string mode)
        {
            try
            {
                ViewBag.Type = type;
                ViewBag.Mode = mode;

                if (type == "buy")
                    ViewBag.Element = manager.GetContext().BuyOrders.Find(Convert.ToInt32(id));
                else
                    ViewBag.Element = manager.GetContext().ReadingOrders.Find(Convert.ToInt32(id));

                Book book = manager.GetContext().Lib.Find(ViewBag.Element.Id_book);
                ViewBag.Id_empl = manager.GetContext().Employees.Find(ViewBag.Element.Id_employee).Id;
                ViewBag.Employee = manager.GetContext().Employees.Find(ViewBag.Element.Id_employee).FIO;
                ViewBag.Books = manager.GetContext().Lib;
                List<string> names = new List<string>();
                foreach (var em in manager.GetContext().Employees)
                {
                    names.Add(em.FIO);
                }
                ViewBag.ListEmployees = names;
                ViewBag.BookName = book.Book_title;
                ViewBag.Short_desc = book.Short_description;
                if (mode == "delete")
                {
                    if (type == "buy")
                    {
                        var order = manager.GetContext().BuyOrders.Find(Convert.ToInt32(Request.Params["id"]));
                        var innerCount = manager.GetContext().ReadingOrders.Where(s => s.Id_book == order.Id_book);
                        if (innerCount.Count() > 0)
                        {
                            foreach(var el in innerCount)
                            {
                                DeleteHelper.FromReadingOrder(el.Id);
                            }
                        }
                        DeleteHelper.FromBuyOrder(order.Id);
                        DeleteHelper.FromBook(manager.GetContext().Lib.Find(order.Id_book).Id);
                    }
                    else
                    {
                        DeleteHelper.FromReadingOrder(Convert.ToInt32(id));
                    }
                    string message = "Запись удалена";
                    Response.Redirect("/Home/Orders?message=" + message + "&type=" + type);
                }
                return View();
            }
            catch(Exception ex)
            {
                string message = "Не удалось обновить запись";
                Response.Redirect("/Home/Orders?message=" + message + "&type=" + type);
                return View();
            }
        }

        [HttpPost]
        public void UpdateToOrders()
        {
            try
            {
                string bookName = Request.Params["new_name"];
                string empl = Request.Params["selection"];
                string desc = Request.Params["desc"];

                if (Request.Params["type"] == "buy")
                {
                    var order = manager.GetContext().BuyOrders.Find(Convert.ToInt32(Request.Params["id"]));
                    var book = manager.GetContext().Lib.Find(order.Id_book);
                    UpdateHelper.FromBook(book.Id, bookName, desc, book.Number_copies.ToString(), book.How_many_times.ToString());
                    UpdateHelper.FromBuyOrder(order.Id, manager.GetContext().Employees.First(s => s.FIO == empl).Id, order.Id_book);
                }
                else
                {
                    Book book = manager.GetContext().Lib.Where(s => s.Book_title == bookName).FirstOrDefault();
                    var order = manager.GetContext().ReadingOrders.Find(Convert.ToInt32(Request.Params["id"]));
                    UpdateHelper.FromReadingOrder(order.Id, manager.GetContext().Employees.First(s => s.FIO == empl).Id, book.Id);
                }
                string message = "Запись обновлена";
                Response.Redirect("/Home/Orders?message=" + message + "&type=" + Request.Params["type"]);
            }
            catch(Exception ex)
            {
                string message = "Не удалось обновить запись";
                Response.Redirect("/Home/Orders?message=" + message + "&type=" + Request.Params["type"]);
            }
        }
    }
}