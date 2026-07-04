using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineLibraryMiniProject.ConsoleApp.Helpers
{
    public static class ConsoleManagement
    {
        // Xəta mesajlarını qırmızı göstərmək üçün
        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[XƏTA] {message}");
            Console.ResetColor();
        }

        // Uğurlu mesajları yaşıl göstərmək üçün
        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[UĞURLU] {message}");
            Console.ResetColor();
        }

        // Təhlükəsiz şəkildə integer (rəqəm) oxumaq üçün dövr
        public static int ReadInteger(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.Parse(Console.ReadLine(), out int result))
                {
                    return result;
                }
                WriteError("Zəhmət olmasa düzgün bir rəqəm daxil edin!");
            }
        }

        // Boş olmayan mətn oxumaq üçün
        public static string ReadString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                WriteError("Bu sahə boş buraxıla bilməz!");
            }
        }
    }
}
