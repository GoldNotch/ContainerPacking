using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ContainerPacking
{
    class FileManager
    {
        //Считывание весов предметов из файла. Считывает только правильные данные
        //V - чтоб проверять, не превышает ли вес вместимость контейнера
        public static List<Item> GetItemsFromFile(string fileName, uint V)
        {
            List<Item> res = new List<Item>();
            StreamReader reader;
            try
            {
                reader = new StreamReader(fileName);
            }
            catch
            {
                Console.WriteLine("Файл не найден");
                return null;
            }
            uint itemID = 1;
            uint itemWeight = 0;
            while (true)
            {
                string str = reader.ReadLine();
                if (string.IsNullOrEmpty(str))
                    break;
                if (!uint.TryParse(str, out itemWeight) || itemWeight > V)
                    continue;
                res.Add(new Item(itemID, uint.Parse(str)) );
                itemID++;
            }
            reader.Close();
            return res;
        }

        //Печать результатов раскладки в файл
        public static void WriteResultIntoFile(string fileName, InputData data, ContainerListShell containers)
        {
            StreamWriter writer = new StreamWriter(fileName, true); //открываем файл на дозапись
            //Печатаем исходные данные
            writer.WriteLine("Список вещей:");
            int itemsCount = data.items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                writer.Write(data.items[i]);
                if (i < itemsCount - 1)
                    writer.Write(", ");
            }
            writer.WriteLine();
            //Печатаем результат
            writer.WriteLine("Раскладка по контейнерам:");
            foreach (var bin in containers.Bins)
                writer.WriteLine(bin.ToString());
            writer.WriteLine($"Ушло контейнеров: {containers.Bins.Count}");
            writer.WriteLine("-------------------------------------------");
            writer.Close();
        }

        //Печать времени в файл в формате "объём входа: время (в мс)"
        public static void WriteTimeIntoFile(string fileName, long n, double Time)
        {
            StreamWriter writer = new StreamWriter(fileName, true); //открываем файл на дозапись
            writer.WriteLine($"{n} {Time}");
            writer.Close();
        }

        public static void WriteTableRowIntoFile(string fileName, long n, double Time, int percent, double deviation)
        {
            StreamWriter writer = new StreamWriter(fileName, true); //открываем файл на дозапись
            writer.WriteLine($"{n} {Time} {percent} {deviation}");
            writer.Close();
        }

        //Печать строки в файл
        public static void WriteStringIntoFile(string fileName, string str)
        {
            StreamWriter writer = new StreamWriter(fileName, true); //открываем файл на дозапись
            writer.WriteLine(str);
            writer.Close();
        }

        //Очистка файла
        public static void ClearFile(string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName); //открываем файл на дозапись
            writer.Close();
        }
    }
}
