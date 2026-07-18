using System;
using System.IO;

class Program
{
    const int MaxItems = 5;
    const double GstRate = 0.05;

    static string[] itemDescriptions = new string[MaxItems];
    static double[] itemPrices = new double[MaxItems];
    static int itemCount = 0;

    static int tipMethod = 3;
    static double tipValue = 0.0;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        bool keepRunning = true;

        while (keepRunning)
        {
            DisplayMenu();
            int choice = ReadInt("Enter your choice: ", 0, 7);
            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    AddItem();
                    break;
                case 2:
                    RemoveItem();
                    break;
                case 3:
                    AddTip();
                    break;
                case 4:
                    DisplayBill();
                    break;
                case 5:
                    Console.WriteLine("Option 5 selected (Not implemented yet).");
                    break;
                case 6:
                    Console.WriteLine("Option 6 selected (Not implemented yet).");
                    break;
                case 7:
                    Console.WriteLine("Option 7 selected (Not implemented yet).");
                    break;
                case 0:
                    Console.WriteLine("Good-bye and thanks for using this program.");
                    keepRunning = false;
                    break;
            }

            if (keepRunning)
            {
                Console.WriteLine("\n// Menu omitted to save screen space");
            }
        }
    }

    static void DisplayMenu()
    {
        Console.WriteLine("┌─────────────────────────────┐");
        Console.WriteLine("│                             │");
        Console.WriteLine("│  Polina's Cafe              │");
        Console.WriteLine("│  -------------------------  │");
        Console.WriteLine("│  1. Add Item                │");
        Console.WriteLine("│  2. Remove Item             │");
        Console.WriteLine("│  3. Add Tip                 │");
        Console.WriteLine("│  4. Display Bill            │");
        Console.WriteLine("│  5. Clear All               │");
        Console.WriteLine("│  6. Save to file            │");
        Console.WriteLine("│  7. Load from file          │");
        Console.WriteLine("│  0. Exit                    │");
        Console.WriteLine("│                             │");
        Console.WriteLine("└─────────────────────────────┘");
    }

    static void AddItem()
    {
        if (itemCount >= MaxItems)
        {
            Console.WriteLine("Cannot add more items. The bill limit is 5 items.");
            return;
        }

        string description = ReadString("Enter description: ", 3, 20);
        double price = ReadDouble("Enter price: ", 0.01);

        itemDescriptions[itemCount] = description;
        itemPrices[itemCount] = price;
        itemCount++;

        Console.WriteLine("Add item was successful.");
    }

    static void RemoveItem()
    {
        if (itemCount == 0)
        {
            Console.WriteLine("There are no items in the bill to remove.");
            return;
        }

        Console.WriteLine("{0,-8} {1,-20} {2,10}", "ItemNo", "Description", "Price");
        Console.WriteLine("---------------------------------------------");
        for (int i = 0; i < itemCount; i++)
        {
            Console.WriteLine("{0,6}   {1,-20} {2,10:C2}", i + 1, itemDescriptions[i], itemPrices[i]);
        }

        int itemNo = ReadInt("Enter the item number to remove or 0 to cancel: ", 0, itemCount);
        if (itemNo == 0)
        {
            Console.WriteLine("Removal cancelled.");
            return;
        }

        int indexToRemove = itemNo - 1;

        for (int i = indexToRemove; i < itemCount - 1; i++)
        {
            itemDescriptions[i] = itemDescriptions[i + 1];
            itemPrices[i] = itemPrices[i + 1];
        }

        itemDescriptions[itemCount - 1] = null;
        itemPrices[itemCount - 1] = 0;
        itemCount--;

        Console.WriteLine("Remove item was successful.");
    }

    static void AddTip()
    {
        if (itemCount == 0)
        {
            Console.WriteLine("There are no items in the bill to add tip for.");
            return;
        }

        double netTotal = CalculateNetTotal();
        Console.WriteLine($"Net Total: {netTotal:C2}");
        Console.WriteLine("1 - Tip Percentage");
        Console.WriteLine("2 - Tip Amount");
        Console.WriteLine("3 - No Tip");

        int method = ReadInt("Enter Tip Method: ", 1, 3);
        tipMethod = method;

        if (method == 1)
        {
            tipValue = ReadDouble("Enter tip percentage: ", 0);
        }
        else if (method == 2)
        {
            tipValue = ReadDouble("Enter tip amount: ", 0);
        }
        else
        {
            tipValue = 0;
            Console.WriteLine("Tip removed successfully.");
        }
    }

    static void DisplayBill()
    {
        if (itemCount == 0)
        {
            Console.WriteLine("There are no items in the bill to display.");
            return;
        }

        Console.WriteLine("{0,-30} {1,10}", "Description", "Price");
        Console.WriteLine("-----------------------------------------");
        for (int i = 0; i < itemCount; i++)
        {
            Console.WriteLine("{0,-30} {1,10:C2}", itemDescriptions[i], itemPrices[i]);
        }
        Console.WriteLine("-----------------------------------------");

        double netTotal = CalculateNetTotal();
        double tipAmount = CalculateTipAmount(netTotal);
        double gstAmount = netTotal * GstRate;
        double totalAmount = netTotal + tipAmount + gstAmount;

        Console.WriteLine("{0,30} {1,10:C2}", "Net Total", netTotal);
        Console.WriteLine("{0,30} {1,10:C2}", "Tip Amount", tipAmount);
        Console.WriteLine("{0,30} {1,10:C2}", "GST Amount", gstAmount);
        Console.WriteLine("{0,30} {1,10:C2}", "Total Amount", totalAmount);
    }

    static double CalculateNetTotal()
    {
        double sum = 0;
        for (int i = 0; i < itemCount; i++)
        {
            sum += itemPrices[i];
        }
        return sum;
    }

    static double CalculateTipAmount(double netTotal)
    {
        if (tipMethod == 1)
        {
            return Math.Round((netTotal * (tipValue / 100.0)), 2);
        }
        if (tipMethod == 2)
        {
            return tipValue;
        }
        return 0.0;
    }

    static int ReadInt(string prompt, int min, int max)
    {
        int result;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (int.TryParse(input, out result) && result >= min && result <= max)
            {
                return result;
            }
            Console.WriteLine($"Invalid input. Please enter an integer between {min} and {max}.");
        }
    }

    static double ReadDouble(string prompt, double min)
    {
        double result;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (double.TryParse(input, out result) && result >= min)
            {
                return result;
            }
            Console.WriteLine($"Invalid input. Please enter a positive number (minimum {min:0.00}).");
        }
    }

    static string ReadString(string prompt, int minLength, int maxLength)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input) && input.Length >= minLength && input.Length <= maxLength)
            {
                return input;
            }
            Console.WriteLine($"Invalid input. String must be between {minLength} and {maxLength} characters.");
        }
    }
}