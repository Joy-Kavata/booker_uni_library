using System;
using Microsoft.Data.Sqlite;

namespace BookerLibrary
{
    public class Book
    {
        public string BookNumber { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Copies { get; set; }

        private const string ConnectionString = "Data Source=library.db";

        public Book(string bookNumber = "", string title = "", string author = "", decimal price = 0, int copies = 0)
        {
            BookNumber = bookNumber;
            Title = title;
            Author = author;
            Price = price;
            Copies = copies;
        }

        public static void CreateDatabase()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Books (
                        BookNumber TEXT PRIMARY KEY,
                        Title TEXT NOT NULL,
                        Author TEXT NOT NULL,
                        Price REAL NOT NULL,
                        Copies INTEGER NOT NULL
                    );";

                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void InsertBook()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = @"
            INSERT OR IGNORE INTO Books (BookNumber, Title, Author, Price, Copies)
            VALUES (@BookNumber, @Title, @Author, @Price, @Copies);";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@BookNumber", this.BookNumber);
                    command.Parameters.AddWithValue("@Title", this.Title);
                    command.Parameters.AddWithValue("@Author", this.Author);
                    command.Parameters.AddWithValue("@Price", this.Price);
                    command.Parameters.AddWithValue("@Copies", this.Copies);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Book '{Title}' inserted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Book '{Title}' ({BookNumber}) already exists. Skipping insert.");
                    }
                }
            }
        }

        public static void DisplayAllBooks()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT BookNumber, Title, Author, Price, Copies FROM Books;";

                using (var command = new SqliteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    Console.WriteLine("=========================================================================================");
                    Console.WriteLine(string.Format("{0,-15} | {1,-30} | {2,-20} | {3,-10} | {4,-6}",
                        "Book Number", "Title", "Author", "Price ($)", "Copies"));
                    Console.WriteLine("=========================================================================================");

                    bool hasRecords = false;
                    while (reader.Read())
                    {
                        hasRecords = true;
                        string bookNo = reader.GetString(0);
                        string title = reader.GetString(1);
                        string author = reader.GetString(2);
                        decimal price = reader.GetDecimal(3);
                        int copies = reader.GetInt32(4);

                        Console.WriteLine(string.Format("{0,-15} | {1,-30} | {2,-20} | {3,-10:F2} | {4,-6}",
                            bookNo, title, author, price, copies));
                    }

                    if (!hasRecords)
                    {
                        Console.WriteLine("No records found in the database.");
                    }

                    Console.WriteLine("=========================================================================================\n");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Booker University Library Management System ---\n");

            Book.CreateDatabase();

            Book book1 = new Book("BK-101", "Database Systems", "Elmasri & Navathe", 85.50m, 12);
            Book book2 = new Book("BK-102", "C# Programming", "Erich Gamma", 62.00m, 8);
            Book book3 = new Book("BK-103", "Software Engineering", "Ian Sommerville", 95.00m, 5);
            Book book4 = new Book("BK-104", "Software Development", "Ian Sommerville", 76.00m, 90);

            Console.WriteLine("Inserting new books...");
            book1.InsertBook();
            book2.InsertBook();
            book3.InsertBook();
            book4.InsertBook();

            Console.WriteLine("Displaying all inventory records for Chief Librarian:");
            Book.DisplayAllBooks();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}