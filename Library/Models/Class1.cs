using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace Library.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FIO { get; set; }

        public string Phone { get; set; }

        public string Department { get; set; }

    }

    public class BookForPage
    {
        public int Id { get; set; }

        public string Book_title { get; set; }

        public string Description { get; set; }

        public int Number_copies_on_hands { get; set; }
    }

    public class BookOnHands
    {
        public string Book_title {get; set; }

        public int Number_copies_on_hands { get; set; }

    }

    public class Issued_book
    {
        public int Id { get; set; }

        public int Id_employee { get; set; }

        public int Id_book { get; set; }

        public DateTime Deadline { get; set; }
    }
    
    public class Reading_Order
    {
        public int Id { get; set; }

        public int Id_book { get; set; }

        public int Id_employee { get; set; }
    }

    public class Buy_Order
    {
        public int Id { get; set; }

        public int Id_employee { get; set; }

        public int Id_book { get; set; }
    }

    public class Book
    {
        public int Id { get; set; }

        public string Book_title { get; set; }

        public string Short_description { get; set; }

        public int Number_copies { get; set; }
    }

    public class LibraryContext: DbContext
    {

        public LibraryContext() : base("DefaultConnection")
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Reading_Order> ReadingOrders { get; set; }
        public DbSet<Book> Lib { get; set; }
        public DbSet<Buy_Order> BuyOrders { get; set; }
        public DbSet<Issued_book> IssuedBooks { get; set; }

    }

    public class BookDbInitializer : DropCreateDatabaseAlways<LibraryContext>
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected override void Seed(LibraryContext context)
        {
            var students = new List<Employee>
            {
            new Employee{Id=1,FIO="Alexander",Phone="45125",Department="sdsd"},
            new Employee{Id=2,FIO="Alexander",Phone="45125",Department="sdsd"},
            new Employee{Id=3,FIO="Alexander",Phone="45125",Department="sdsd"},
            new Employee{Id=4,FIO="Alexander",Phone="45125",Department="sdsd"},
            new Employee{Id=5,FIO="Alexander",Phone="45125",Department="sdsd"},
            new Employee{Id=6,FIO="Alexander",Phone="45125",Department="sdsd"}
            };
            students.ForEach(s => context.Employees.Add(s));

            context.SaveChanges();
            logger.Info("you debil");
        }
    }

    
}
