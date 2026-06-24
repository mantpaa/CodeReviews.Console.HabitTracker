
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitLoggerProgram;

class HabitLoggerProgram
{
    static string connectionString = "data source=habitLogger.db";
    public static void Main()
    {
        InitializeDatabase();


        string? readInput = "";

        while (readInput != "exit")
        {
            DisplayMenu();
            readInput = Console.ReadLine();
            switch (readInput)
            {
                case "1":
                    AddNewHabit();
                    break;
                case "2":
                    DisplayHabits();
                    break;
                case "3":
                    UpdateHabit();
                    break;
                case "4":
                    DeleteHabit();
                    break;
                case "5":
                    Console.WriteLine("Exiting the application. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    readInput = Console.ReadLine();

                    break;
            }
        }
    }





    public static void DisplayMenu()
    {
        Console.WriteLine("Welcome to the Habit Logger!");
        Console.WriteLine(new string('-', Console.BufferWidth));
        Console.WriteLine("1. Add a new habit type.");
        Console.WriteLine("2. View all habit entries.");
        Console.WriteLine("3. Update a habit.");
        Console.WriteLine("4. Delete a habit entry");
        Console.WriteLine("5. Exit");
    }

    public static void DisplayInstructions()
    {
        Console.WriteLine("A habit is a behavior that you've done some x number of times, such as: " +
            "\n- Drinking water 3 times" +
            "\n- Going to the gym 5 times" +
            "\n- Meditating 7 times" +
            "\n- Solving 10 coding problems");
        Console.WriteLine("\nA habit is not about how long you did something (hours of sleep), but about quantity (how many fruits you ate).");

    }


    public static void AddNewHabit()
    {
        Console.WriteLine("Enter the habit name:");
        string name = Console.ReadLine();
        string date = GetDate();
        Console.WriteLine("Enter the quantity:");
        int quantity = GetQuantity();

        AddRecord(name, date, quantity);
    }

    public static void DisplayHabits()
    {
        List<string> records = GetAllRecords();
        if (records.Count == 0)
        {
            Console.WriteLine("There are no records yet.");
        }
        else
        {
            Console.WriteLine("Habit Records:");
            foreach (var record in records)
            {
                Console.WriteLine(record);
            }
        }
    }

    public static void UpdateHabit()
    {
        if (GetAllRecords().Count == 0)
        {
            Console.WriteLine("No records to update. Please add a record first.");
            return;
        }
        int id;


        id = GetId();

        if (id == -1)
        {
            Console.WriteLine("Update cancelled.");
            return;
        }

        string name = GetName();
        string date = GetDate();

        int quantity = GetQuantity();


        bool isValid = false;
        string proceed = "";
        while (isValid == false)
        {
            Console.WriteLine("Update ready, to proceed enter 'y', to cancel enter 'n':");
            proceed = Console.ReadLine();

            if (proceed != "y" && proceed != "n")
            {
                Console.WriteLine("Invalid option.");
            }
            else
                isValid = true;
        }

        if (proceed == "y")
            UpdateRecord(id, name, date, quantity);
        else
            Console.WriteLine("Update cancelled.");
    }


    // Very similar to update, but not sure how to "DRY" this. Including optional param "Action" to determine update or delete seems "dirty".
    public static void DeleteHabit()
    {
        if (GetAllRecords().Count == 0)
        {
            Console.WriteLine("No records to delete.");
            return;
        }
        int id;
        Console.WriteLine("Enter the Id of the record you want to update:");
        id = GetId();

        if (id == -1)
        {
            Console.WriteLine("Delete cancelled.");
            return;
        }

        DeleteRecord(id);
    }

    #region Get and validate fields
    public static int GetId()
    {
        bool isValid = false;
        int id = -1;
        Console.WriteLine("Enter id to continue or 'b' cancel:");
        while (isValid == false)
        {
            string readInput = Console.ReadLine();

            if (readInput == "b")
                return -1;
            if (!int.TryParse(readInput, out id))
            {
                Console.WriteLine("Invalid input: " + readInput + "try again.");
            }
            else if (GetRecordById(id) == string.Empty)
            {
                Console.WriteLine("No record with the given Id exists. Please try again.");
            }
            else
                isValid = true;
        }

        return id;
    }
    public static string GetName()
    {
        bool isValid = false;
        string name = string.Empty;
        while (isValid == false)
        {
            Console.WriteLine("Enter the habit name");
            name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Habit name cannot be empty. Please try again.");
            }
            else
                isValid = true;
        }
        return name;
    }

    public static string GetDate()
    {
        string pattern = "dd-MM-yy";
        bool isValid = false;
        DateTime date = DateTime.MinValue;

        Console.WriteLine($"Enter a date ({pattern}) or enter 'today', to get todays date:");
        string userInput = Console.ReadLine();

        string returnDate = "";
        while (!isValid)
        {
            if (userInput.ToLower().Trim() == "today")
            {
                date = DateTime.Today;
                isValid = true;
            }
            else
            {
                isValid = DateTime.TryParseExact(userInput, pattern, null, DateTimeStyles.None, out date);
                if (!isValid)
                {
                    Console.WriteLine($"Invalid date format. Please enter a date in the format {pattern} or 'today':");
                    userInput = Console.ReadLine();
                }
            }
        }

        returnDate += date.Month + "-" + date.Day + "-" + date.Year;
        return returnDate;
    }
    public static int GetQuantity()
    {
        bool isValid = false;
        int quantity = 0;
        Console.WriteLine("Enter the quantity (unit of measurement for the habit, such as: " +
                          "Drink water 3 times, Go to the gym 5 times, Meditate 7 times, Solve 10 coding problems), etc...");

        while (!isValid)
        {
            Console.WriteLine("Your input (must be a positive integer):");
            string userInput = Console.ReadLine();

            if (!int.TryParse(userInput, out quantity))
                Console.WriteLine("Invalid input. Please enter a valid integer.");
            else if (quantity < 0)
            {
                Console.WriteLine("Quantity cannot be negative. Please enter a positive integer.");
            }
            else isValid = true;
        }

        return quantity;
    }
    #endregion Get and validate fields

    #region Initialize Db and Table
    private static void InitializeDatabase()
    {
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand cmd = new SqliteCommand()
            {
                Connection = connection,
                CommandText = @"CREATE TABLE IF NOT EXISTS habits(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Date TEXT,
                    Quantity INTEGER);"


            };

            var res = cmd.ExecuteNonQuery();
        }
    }

    #endregion Initialize Db and Table

    #region CRUD operations
    private static void AddRecord(string name, string date, int quantity)
    {
        using (SqliteConnection con = new SqliteConnection(connectionString))
        {
            con.Open();
            SqliteCommand cmd = new SqliteCommand()
            {
                Connection = con,
                CommandText = $"INSERT INTO habits(Name,Date,Quantity) VALUES ($name, $date, $quantity);"
            };

            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$date", date);
            cmd.Parameters.AddWithValue("$quantity", quantity);

            var res = cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    private static List<string> GetAllRecords()
    {
        List<string> records = new List<string>();

        using (SqliteConnection con = new SqliteConnection(connectionString))
        {
            con.Open();

            using (SqliteCommand cmd = new SqliteCommand("SELECT * FROM habits;", con))
            using (SqliteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    records.Add(
                        $"Id: {reader["Id"]}, Name: {reader["Name"]}, Date: {reader["Date"]}, Quantity: {reader["Quantity"]}"
                    );
                }
            }
        }

        return records;
    }

    private static string GetRecordById(int id)
    {
        string record = string.Empty;
        using (SqliteConnection con = new SqliteConnection(connectionString))
        {
            con.Open();
            SqliteCommand cmd = new SqliteCommand()
            {
                Connection = con,
                CommandText = $"SELECT * FROM habits WHERE Id = $id"
            };
            cmd.Parameters.AddWithValue("$id", id);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    record = $"Id: {reader["Id"]}, Name: {reader["Name"]}, Date: {reader["Date"]}, Quantity: {reader["Quantity"]}";
                }
            }
        }

        return record;
    }

    private static void UpdateRecord(int id, string name, string date, int quantity)
    {
        using (SqliteConnection con = new SqliteConnection(connectionString))
        {
            con.Open();
            SqliteCommand cmd = new SqliteCommand()
            {
                Connection = con,
                CommandText = @$"UPDATE habits 
                                 SET name = $name, 
                                     date = $date, 
                                     quantity = $quantity WHERE Id = $id"
            };
            cmd.Parameters.AddWithValue("$name", name);
            cmd.Parameters.AddWithValue("$date", date);
            cmd.Parameters.AddWithValue("$quantity", quantity);
            cmd.Parameters.AddWithValue("$id", id);

            var res = cmd.ExecuteNonQuery();
            con.Close();
        }
    }

    private static void DeleteRecord(int id)
    {
        using (SqliteConnection con = new SqliteConnection(connectionString))
        {
            con.Open();
            SqliteCommand cmd = new SqliteCommand()
            {
                Connection = con,
                CommandText = $"DELETE FROM habits WHERE Id = $id"
            };
            cmd.Parameters.AddWithValue("$id", id);
            var res = cmd.ExecuteNonQuery();
            con.Close();
        }
    }
    #endregion CRUD operations
}
