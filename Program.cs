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

        for (int i = 0; i < MaxItems; i++)
        {
            itemDescriptions[i] = "";
        }

        bool keepRunning = true;

        while (keepRunning)
        {
            DisplayMenu();
            int choice = ReadInt("Enter your choice: ", 0, 7);
            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    HandleAddItem();
                    break;
                case 2:
                    HandleRemoveItem();
                    break;
                case 3:
                    HandleAddTip();
                    break;
                case 4:
                    HandleDisplayBill();
                    break;
                case 5:
                    HandleClearAll();
                    break;
                case 6:
                    HandleSaveToFile();
                    break;
                case 7:
                    HandleLoadFromFile();
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

    #region UI Handlers

    static void HandleAddItem()
    {
        if (itemCount >= MaxItems)
        {
            Console.WriteLine("Cannot add more items. The bill limit is 5 items.");
            return;
        }

        string description = ReadString("Enter description: ", 3, 20);
        double price = ReadDouble("Enter price: ", 0.01);

        AddItem(description, price);
        Console.WriteLine("Add item was successful.");
    }

    static void HandleRemoveItem()
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

        RemoveItem(itemNo - 1);
        Console.WriteLine("Remove item was successful.");
    }

    static void HandleAddTip()
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
        double value = 0;

        if (method == 1)
        {
            value = ReadDouble("Enter tip percentage: ", 0);
        }
        else if (method == 2)
        {
            value = ReadDouble("Enter tip amount: ", 0);
        }

        SetTip(method, value);

        if (method == 3)
        {
            Console.WriteLine("Tip removed successfully.");
        }
    }

    static void HandleDisplayBill()
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

    static void HandleClearAll()
    {
        ClearAll();
        Console.WriteLine("All items have been cleared.");
    }

    static void HandleSaveToFile()
    {
        if (itemCount == 0)
        {
            Console.WriteLine("Nothing to save. The bill is empty.");
            return;
        }

        string fileName = ReadString("Enter the file path to save items to: ", 1, 10);

        if (!fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".csv";
        }

        string projectPath = GetProjectPath();
        string fullPath = Path.Combine(projectPath, fileName);

        if (SaveData(fullPath))
        {
            Console.WriteLine($"Write to file {fileName} was successful.");
        }
        else
        {
            Console.WriteLine("Error writing to file.");
        }
    }

    static void HandleLoadFromFile()
    {
        string fileName = ReadString("Enter the file path to load items from: ", 1, 10);

        if (!fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".csv";
        }

        string projectPath = GetProjectPath();
        string fullPath = Path.Combine(projectPath, fileName);

        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"Error: File '{fileName}' does not exist in project folder.");
            return;
        }

        if (LoadData(fullPath))
        {
            Console.WriteLine($"Read from {fileName} was successful.");
        }
        else
        {
            Console.WriteLine("Error reading from file or data format is invalid.");
        }
    }

    #endregion

    #region Pure Business Logic

    static void AddItem(string description, double price)
    {
        itemDescriptions[itemCount] = description;
        itemPrices[itemCount] = price;
        itemCount++;
    }

    static void RemoveItem(int indexToRemove)
    {
        for (int i = indexToRemove; i < itemCount - 1; i++)
        {
            itemDescriptions[i] = itemDescriptions[i + 1];
            itemPrices[i] = itemPrices[i + 1];
        }
        itemDescriptions[itemCount - 1] = "";
        itemPrices[itemCount - 1] = 0;
        itemCount--;
    }

    static void SetTip(int method, double value)
    {
        tipMethod = method;
        tipValue = value;
    }

    static void ClearAll()
    {
        for (int i = 0; i < MaxItems; i++)
        {
            itemDescriptions[i] = "";
            itemPrices[i] = 0;
        }
        itemCount = 0;
        tipMethod = 3;
        tipValue = 0;
    }

    static bool SaveData(string fullPath)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                for (int i = 0; i < itemCount; i++)
                {
                    string safeDesc = itemDescriptions[i].Contains(",") ? $"\"{itemDescriptions[i]}\"" : itemDescriptions[i];
                    writer.WriteLine($"{safeDesc},{itemPrices[i]}");
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    static bool LoadData(string fullPath)
    {
        try
        {
            string[] lines = File.ReadAllLines(fullPath);
            int loadedCount = 0;
            string[] tempDescriptions = new string[MaxItems];
            double[] tempPrices = new double[MaxItems];

            for (int i = 0; i < lines.Length && loadedCount < MaxItems; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] parts = lines[i].Split(',');
                if (parts.Length >= 2)
                {
                    string desc = parts[0].Trim('"');
                    if (double.TryParse(parts[1], out double price) && desc.Length >= 3 && desc.Length <= 20 && price > 0)
                    {
                        tempDescriptions[loadedCount] = desc;
                        tempPrices[loadedCount] = price;
                        loadedCount++;
                    }
                }
            }

            ClearAll();
            for (int i = 0; i < loadedCount; i++)
            {
                itemDescriptions[i] = tempDescriptions[i];
                itemPrices[i] = tempPrices[i];
            }
            itemCount = loadedCount;
            return true;
        }
        catch
        {
            return false;
        }
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

    static string GetProjectPath()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        DirectoryInfo? d1 = Directory.GetParent(baseDir);
        DirectoryInfo? d2 = d1?.Parent;
        DirectoryInfo? d3 = d2?.Parent;
        return d3?.FullName ?? baseDir;
    }

    #endregion

    #region Input Parsers

    static int ReadInt(string prompt, int min, int max)
    {
        int result;
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
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
            string? input = Console.ReadLine();
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
            string? input = Console.ReadLine();
            string trimmed = input?.Trim() ?? "";
            if (trimmed.Length >= minLength && trimmed.Length <= maxLength)
            {
                return trimmed;
            }
            Console.WriteLine($"Invalid input. String must be between {minLength} and {maxLength} characters.");
        }
    }

    #endregion
}