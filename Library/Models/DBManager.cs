using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Models;

namespace Library.Models
{
    enum TablesNames { Employees, BuyOrders, ReadOrders, Books, IssuedBooks}

    class DBManager
    {
        private LibraryContext context;
        public DBManager()
        {
            context = new LibraryContext();
        }

        public string Insert<Employee>(Library.Models.Employee employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
            return "Данные изменены";
        }

        public string Insert<Buy_Order>(Library.Models.Buy_Order order)
        {
            context.BuyOrders.Add(order);
            context.SaveChanges();
            return "Данные изменены";
        }

        public string Insert<Reading_Order>(Library.Models.Reading_Order order)
        {
            context.ReadingOrders.Add(order);
            context.SaveChanges();
            return "Данные изменены";
        }

        public string Insert<Book>(Library.Models.Book book)
        {
            context.Lib.Add(book);
            context.SaveChanges();
            return "Данные изменены";
        }

        public string Update<Employee>(int id, Library.Models.Employee empl)
        {

            Library.Models.Employee employee = context.Employees.Where(s => s.Id == id).FirstOrDefault();

            if (employee == null)
            {
                return "Работника с таким именем нет в базе данных";
            }
            using (var dbCtx = new LibraryContext())
            {
                employee.FIO = empl.FIO;
                employee.Phone = empl.Phone;
                employee.Department = empl.Department;
                dbCtx.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                dbCtx.SaveChanges();
            }

            context = new LibraryContext();
            return "Данные изменены";
        }

        public string Update<Buy_Order>(int id, Library.Models.Buy_Order order)
        {
            Library.Models.Buy_Order newOrder = context.BuyOrders.Where(s => s.Id == id).FirstOrDefault();

            if (newOrder == null)
            {
                return "Такого заказа нет";
            }
            using (var dbCtx = new LibraryContext())
            {
                newOrder.Id_employee = order.Id_employee;
                newOrder.Id_book = order.Id_book;
                dbCtx.Entry(newOrder).State = System.Data.Entity.EntityState.Modified;
                dbCtx.SaveChanges();
            }

            context = new LibraryContext();
            return "Данные изменены";
        }

        public string Update<Reading_Order>(int id, Library.Models.Reading_Order order)
        {
            Library.Models.Reading_Order newOrder = context.ReadingOrders.Where(s => s.Id == id).FirstOrDefault();

            if (newOrder == null)
            {
                return "Такого заказа нет";
            }
            using (var dbCtx = new LibraryContext())
            {
                newOrder.Id_book = order.Id_book;
                newOrder.Id_employee = order.Id_employee;
                dbCtx.Entry(newOrder).State = System.Data.Entity.EntityState.Modified;
                dbCtx.SaveChanges();
            }

            context = new LibraryContext();
            return "Данные изменены";
        }

        public string Update<Book>(int id, Library.Models.Book book)
        {
            Library.Models.Book newBook = context.Lib.Where(s => s.Id == id).FirstOrDefault();

            if (newBook == null)
            {
                return "Такой книги нет";
            }
            using (var dbCtx = new LibraryContext())
            {
                newBook.Number_copies = book.Number_copies;
                newBook.Short_description = book.Short_description;
                newBook.Book_title = book.Book_title;
                dbCtx.Entry(newBook).State = System.Data.Entity.EntityState.Modified;
                dbCtx.SaveChanges();
            }

            context = new LibraryContext();
            return "Данные изменены";
        }


        public string Delete<Employee>(Library.Models.Employee employee)
        {
            var delBook = context.Employees.First(s => s.FIO == employee.FIO);
            context.Employees.Remove(delBook);
            context.SaveChanges();
            return "Данные изменены";
        }

        public string Delete<Buy_Order>(Library.Models.Buy_Order order)
        {
            var delBook = context.BuyOrders.First(s => s.Id_book == order.Id_book);
            context.BuyOrders.Remove(delBook);
            context.SaveChanges();
            return "Данные изменены";
        }

        public string Delete<Reading_Order>(Library.Models.Reading_Order order)
        {
            var delBook = context.ReadingOrders.First(s => s.Id_book == order.Id_book);
            context.ReadingOrders.Remove(delBook);
            context.SaveChanges();
            return "Данные изменены";
        }

        public string Delete<Book>(Library.Models.Book book)
        {
            var delBook = context.Lib.First(s => s.Book_title == book.Book_title);
            context.Lib.Remove(delBook);
            context.SaveChanges();
            return "Данные изменены";
        }

        public int GetIdElement(TablesNames tb, string con)
        {
            switch (tb)
            {
                case TablesNames.Books:
                    return context.Lib.First(s => s.Book_title == con).Id;
                case TablesNames.BuyOrders:
                    return context.BuyOrders.First(s => s.Id_book == GetIdElement(TablesNames.Books, con)).Id;
                case TablesNames.Employees:
                    return context.Employees.First(s => s.FIO == con).Id;
                case TablesNames.ReadOrders:
                    return context.ReadingOrders.First(s => s.Id_book == GetIdElement(TablesNames.Books, con)).Id;
                default:
                    return -1;
            }
        }

        public LibraryContext GetContext()
        {
            return context;
        }

    }
}
