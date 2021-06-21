using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Xml.Linq;

namespace Example_01
{
    public class Program
    {
        private static List<List<int>> groups;              // Список групп.
        private static string sourceFile = "num.txt";       // Имя файла с числом.
        private static string fileDB = "MyDB.csv";          // Имя файла для сохранения данных.
        private static string fileZipDB = "MyDB.csv.zip";       // Имя файла для сохранения данных.

        /// <summary>
        /// Точка входа в программу.
        /// </summary>
        /// <param name="args">Параметры запуска.</param>
        public static void Main(string[] args)
        {
            Start();        

            Console.ReadLine();
        }

        /// <summary>
        /// Запустить программу.
        /// </summary>
        private static void Start()
        {
            uint count;         // Количество чисел

            if (File.Exists(sourceFile))       // Если файл существует.        
            {
                count = GetNumberForFile(sourceFile);           // Считываем число из файла.
                Console.WriteLine($"Количество чисел: {count}.");
            }
            else      // Иначе
            {
                Console.WriteLine("Файла с числом нет.");
                return;             // Выход из программы.
            }

            DateTime start = DateTime.Now;                      // Создать начальную точку времени.
            groups = GetGroupsNumbers(count);                   // Получить списко групп чисел.

            Console.WriteLine("Время на группировку чисел.");
            ShowElapsedTime(start);                             // Показать время пройденное на группировку.

            GetMode();                                          // Выбрать режим.

            Console.WriteLine("Заархивировать данные? (д/н)");
            if (Console.ReadKey(true).Key == ConsoleKey.L &&
                File.Exists(fileDB))
                ArchiveData();
        }

        /// <summary>
        /// Получить число из файла.
        /// </summary>
        /// <returns>Число.</returns>
        private static uint GetNumberForFile(string path)
        {
            var success = uint.TryParse(File.ReadAllText(path), out uint count);
            if (success)
            {
                Console.WriteLine($"Считано число: {count}");
                return count;
            }
            else
            {
                Console.WriteLine("Должно быть числовое значение, которое должно быть больще либо равно 0.");
                return 0;
            }
        }

        /// <summary>
        /// Получить список групп чисел.
        /// </summary>
        /// <param name="count">Количество чисел.</param>
        /// <returns>Списко групп.</returns>
        public static List<List<int>> GetGroupsNumbers(uint count)
        {
            List<List<int>> groups = new List<List<int>>();     // Инициализируем список групп.
            int degree = 2;                                     // Инициализируем степень.

            // Если количество равно 0.
            if (count == 0)
                return groups;      // Вернуть пустой список групп.

            groups.Add(new List<int> { 1 });    // Дабавляеем первую группу.

            // Если количество равно 1.
            if (count == 1)
                return groups;      // Вернуть список с 1 группой.

            // Объявляем и инициализируем список чисел.
            List<int> group = new List<int>();

            // Проходимся по порядковым числам, начиная с 2.
            for (int i = 2; i <= count; i++)
            {
                group.Add(i);                   // Добавить число в список.

                // Если число i меньше 2 в степени degree и i не равно количеству чисел.
                if ((int) Math.Pow(2, degree) - 1 ==  i || i == count)
                {
                    groups.Add(group); // Добавляем группу в списко.
                    degree++; // Увеличиваем степень на единицу.
                    group = new List<int>(); // Обнуляем список чисел.
                }
            }

            // Вернуть список групп чисел.
            return groups;
        }

        /// <summary>
        /// Показать прошедшее время со с момента начальной временной точки.
        /// </summary>
        /// <param name="start">Начальная временная точка.</param>
        private static void ShowElapsedTime(DateTime start)
        {
            TimeSpan ts = DateTime.Now.Subtract(start);
            Console.WriteLine($"Затраченное время: {ts.Minutes} мин. " +
                              $"{ts.Seconds} сек. {ts.Milliseconds} мс.");
        }

        /// <summary>
        /// Получить номер режима программы.
        /// </summary>
        private static void GetMode()
        {
            Console.WriteLine("Выберите один из режимов:");
            Console.WriteLine("1) Показать в консоли количество групп.");
            Console.WriteLine("2) Сохранить список групп чисел.");
            Console.Write("Ваш выбор: ");

            int.TryParse(Console.ReadLine(), 0, null, out int numMod);
            switch (numMod)
            {
                case 1:
                    PrintCountGroups();
                    break;
                case 2:
                    SaveToFile();
                    break;
                default:
                    GetMode();
                    break;
            }
        }

        /// <summary>
        /// Сохранить данные в файл.
        /// </summary>
        private static void SaveToFile()
        {
            using (StreamWriter sw = new StreamWriter(fileDB))
            {
                groups.ForEach(group => sw.WriteLine(string.Join(";", group)));
            }

            Console.WriteLine("Файл с данными сохранен в MyDB.csv.");
        }

        /// <summary>
        /// Вывести на консоль количество списков групп чисел.
        /// </summary>
        private static void PrintCountGroups()
        {
            Console.WriteLine($"Количество групп: {groups.Count}.");
        }

        /// <summary>
        /// Заархивировать данные.
        /// </summary>
        private static void ArchiveData()
        {
            using (var input = File.Open(fileDB, FileMode.Open))
            {
                using (var output = File.Open(fileZipDB, FileMode.OpenOrCreate))
                {
                    using (var gzip = new GZipStream(output, CompressionMode.Compress))
                    {
                        input.CopyTo(gzip);
                        Console.WriteLine($"Файл {fileZipDB} создан.\n" +
                                          $"Длина входящего потока: {input.Length}\n" +
                                          $"Длина исходящего потока: {output.Length}");
                    }
                }
            }
        }
    }
}
