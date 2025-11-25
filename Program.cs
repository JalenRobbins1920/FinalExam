using System;
using System.Collections.Generic;
using System.IO;

namespace LibraryCheckoutSystem
{
    internal class Program
    {
        // =============================================================
        // LISTS FOR STORING DATA
        // =============================================================

        // Stores all library items loaded from catalog.txt
        static List<LibraryItem> catalog = new List<LibraryItem>();

        // Stores all items the student has checked out
        static List<CheckoutItem> checkoutList = new List<CheckoutItem>();

        // File names for saving and loading
        const string catalogFile = "catalog.txt";
        const string checkoutFile = "myCheckouts.txt";

        // =============================================================
        // MAIN METHOD - Entry point of the entire program
        // =============================================================
        static void Main(string[] args)
        {
            Console.Title = "Library Checkout System";

            // Load catalog.txt at program startup
            LoadCatalog();

            bool running = true; // controls menu loop

            // =============================================================
            // MAIN MENU LOOP (runs until user chooses Exit)
            // =============================================================
            while (running)
            {
                Console.Clear();
                Console.WriteLine("===== LIBRARY CHECKOUT SYSTEM =====");
                Console.WriteLine("1. Add a library item");
                Console.WriteLine("2. View available items");
                Console.WriteLine("3. Check out an item");
                Console.WriteLine("4. Return an item");
                Console.WriteLine("5. View my checkout receipt");
                Console.WriteLine("6. Save my checkout list");
                Console.WriteLine("7. Load my previous checkout list");
                Console.WriteLine("8. Exit");
                Console.Write("\nChoose an option: ");

                string choice = Console.ReadLine(); // user input

                // Use switch statement for menu navigation
                switch (choice)
                {
                    case "1": AddLibraryItem(); break;
                    case "2": ViewCatalog(); break;
                    case "3": CheckoutItemMenu(); break;
                    case "4": ReturnItem(); break;
                    case "5": PrintReceipt(); break;
                    case "6": SaveCheckoutList(); break;
                    case "7": LoadCheckoutList(); break;
                    case "8": running = false; break;

                    default:
                        Console.WriteLine("Invalid choice. Press Enter.");
                        Console.ReadLine();
                        break;
                }
            }

            Console.WriteLine("Goodbye!");
        }

        // =============================================================
        // METHOD: LOAD CATALOG FROM FILE
        // Reads catalog.txt and builds the catalog list
        // =============================================================
        static void LoadCatalog()
        {
            // If file doesn’t exist, create empty catalog
            if (!File.Exists(catalogFile))
            {
                Console.WriteLine("Catalog file not found. Creating empty catalog.");
                return;
            }

            // Read all lines from catalog.txt
            string[] lines = File.ReadAllLines(catalogFile);

            // Loop through every line and parse it
            foreach (string line in lines)
            {
                // Format: id,title,type,latefee
                string[] parts = line.Split(',');

                // Make sure the line is valid
                if (parts.Length == 4)
                {
                    int id = int.Parse(parts[0]);
                    string title = parts[1];
                    string type = parts[2];
                    double fee = double.Parse(parts[3]);

                    // Create a new LibraryItem object
                    catalog.Add(new LibraryItem(id, title, type, fee));
                }
            }
        }

        // =============================================================
        // METHOD: ADD A NEW ITEM TO THE CATALOG
        // User enters data, program appends to file + adds to list
        // =============================================================
        static void AddLibraryItem()
        {
            Console.Clear();
            Console.WriteLine("=== Add New Library Item ===");

            // Ask user for all needed info
            Console.Write("Enter ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter Title: ");
            string title = Console.ReadLine();

            Console.Write("Enter Type (Book/DVD): ");
            string type = Console.ReadLine();

            Console.Write("Enter Daily Late Fee: ");
            double fee = double.Parse(Console.ReadLine());

            // Create new item
            LibraryItem newItem = new LibraryItem(id, title, type, fee);
            catalog.Add(newItem);

            // Save it permanently to catalog.txt
            File.AppendAllText(catalogFile, $"{id},{title},{type},{fee}\n");

            Console.WriteLine("Item added and saved!");
            Console.ReadLine();
        }

        // =============================================================
        // METHOD: DISPLAY ENTIRE CATALOG
        // =============================================================
        static void ViewCatalog()
        {
            Console.Clear();
            Console.WriteLine("=== Library Catalog ===");

            // Loop through every item and print it
            foreach (var item in catalog)
            {
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        // =============================================================
        // METHOD: CHECK OUT AN ITEM
        // =============================================================
        static void CheckoutItemMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Check Out an Item ===");

            Console.Write("Enter the ID of the item: ");
            int id = int.Parse(Console.ReadLine());

            // Try to find item in the catalog
            LibraryItem item = catalog.Find(i => i.Id == id);

            if (item == null)
            {
                Console.WriteLine("Item does not exist!");
                Console.ReadLine();
                return;
            }

            // Check if it is already checked out
            if (checkoutList.Exists(c => c.Item.Id == id))
            {
                Console.WriteLine("Item is already checked out!");
                Console.ReadLine();
                return;
            }

            // Loan rules: Book = 7 days, DVD = 3 days
            int loan = (item.Type.ToLower() == "dvd") ? 3 : 7;

            // Create a checkout object
            CheckoutItem checkout = new CheckoutItem(item, loan);

            // Add to checkout list
            checkoutList.Add(checkout);

            Console.WriteLine("Item checked out successfully!");
            Console.ReadLine();
        }

        // =============================================================
        // METHOD: RETURN AN ITEM
        // Removes item from checkout list
        // =============================================================
        static void ReturnItem()
        {
            Console.Clear();
            Console.WriteLine("=== Return Item ===");

            Console.Write("Enter the ID of the item to return: ");
            int id = int.Parse(Console.ReadLine());

            CheckoutItem checkout = checkoutList.Find(c => c.Item.Id == id);

            if (checkout == null)
            {
                Console.WriteLine("That item is not checked out.");
            }
            else
            {
                checkoutList.Remove(checkout);
                Console.WriteLine("Item returned successfully.");
            }

            Console.ReadLine();
        }

        // =============================================================
        // METHOD: PRINT RECEIPT
        // Shows items + calculates late fees
        // =============================================================
        static void PrintReceipt()
        {
            Console.Clear();
            Console.WriteLine("=== My Receipt ===");

            if (checkoutList.Count == 0)
            {
                Console.WriteLine("You have no checked-out items.");
                Console.ReadLine();
                return;
            }

            // Ask user how late each item is
            Console.Write("Enter number of days late (apply to all items): ");
            int daysLate = int.Parse(Console.ReadLine());

            double total = 0; // total fee accumulator

            // Loop through every checked-out item
            foreach (var item in checkoutList)
            {
                item.DaysLate = daysLate; // set late days
                Console.WriteLine(item.ToString()); // print item
                total += item.CalculateLateFee(); // add fee
            }

            // Print the total
            Console.WriteLine($"\nTotal Estimated Fees: ${total:0.00}");
            Console.ReadLine();
        }

        // =============================================================
        // METHOD: SAVE CHECKOUT LIST TO FILE
        // Writes: item ID, loan period, late days
        // =============================================================
        static void SaveCheckoutList()
        {
            using (StreamWriter writer = new StreamWriter(checkoutFile))
            {
                foreach (var item in checkoutList)
                {
                    writer.WriteLine($"{item.Item.Id},{item.LoanPeriodDays},{item.DaysLate}");
                }
            }

            Console.WriteLine("Checkout list saved.");
            Console.ReadLine();
        }

        // =============================================================
        // METHOD: LOAD CHECKOUT LIST FROM FILE
        // Reconstructs checkout list using stored data
        // =============================================================
        static void LoadCheckoutList()
        {
            if (!File.Exists(checkoutFile))
            {
                Console.WriteLine("No checkout file found.");
                Console.ReadLine();
                return;
            }

            checkoutList.Clear(); // Reset old list

            string[] lines = File.ReadAllLines(checkoutFile);

            // Loop through every saved line
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                int id = int.Parse(parts[0]);
                int loan = int.Parse(parts[1]);
                int late = int.Parse(parts[2]);

                // Find the catalog item with matching ID
                LibraryItem item = catalog.Find(i => i.Id == id);

                if (item != null)
                {
                    CheckoutItem c = new CheckoutItem(item, loan);
                    c.DaysLate = late;
                    checkoutList.Add(c);
                }
            }

            Console.WriteLine("Checkout list loaded!");
            Console.ReadLine();
        }
    }
}

