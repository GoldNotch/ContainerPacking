using System;
using System.Collections.Generic;

namespace ContainerPacking
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Константы для тестирования
            const int maxN = 10; //максимальная размерность тестов (максимальное количество предметов)
            const int tests_count = 1000; //Количество тестов на одну размерность задачи
            const int accuracy = 1; //точность - количество выполнений одного и того же теста
            const int v = 25; //вместимость конейнера

            //Основной цикл меню
            uint selectedMenuItem = 1;
            while (selectedMenuItem != 0)
            {
                selectedMenuItem = InterfaceManager.ShowConsoleMenu(
                    ">>>ЗАДАЧА ОДНОМЕРНОЙ УПАКОВКИ<<<\nМеню:", new MenuItem[] 
                    { 
                        new MenuItem(0, "Выход"), 
                        new MenuItem(1, "Решение задачи"), 
                        new MenuItem(2, "Тестирование всех алгоритмов на сгенерированных данных"),
                        new MenuItem(3, "Тестирование отдельного алгоритма на сгенерированных данных")
                    });
                //Выход
                if (selectedMenuItem == 0)
                    break;
                //Решение задачи
                else if (selectedMenuItem == 1)
                {
                    //Ввод объёма контейнера
                    Console.Write("Введите объём одного контейнера: ");
                    uint V;
                    while (true)
                        if (uint.TryParse(Console.ReadLine(), out V) && V != 0)
                            break;
                    Console.Clear();
                    //Выбор способа ввода весов предметов
                    selectedMenuItem = InterfaceManager.ShowConsoleMenu(
                        "Выберите способ ввода:", new MenuItem[]
                        {
                            new MenuItem(1, "Файл"),
                            new MenuItem(2, "Ручной ввод"),
                        });
                    //Ввод весов предметов из файла
                    List<Item> items = null;
                    if (selectedMenuItem == 1)
                        items = FileManager.GetItemsFromFile("ItemsWeight.txt", V);
                    //Если нужно ввести с консоли, или же из файла веса не были считаны - ввод с консоли. Читает только верный ввод
                    if (selectedMenuItem == 2 || items == null)
                    {
                        Console.WriteLine("Вводите веса каждого предмета, по одному в одну строку. Окончание ввода - пустая строка:");
                        items = new List<Item>();
                        uint itemWeight = 0;
                        uint itemIndex = 1;
                        while (true)
                        {
                            string enteredStr = Console.ReadLine();
                            if (enteredStr == "")
                                break;
                            if (!uint.TryParse(enteredStr, out itemWeight) || itemWeight > V)
                                continue;
                            items.Add(new Item(itemIndex, itemWeight));
                            itemIndex++;
                        }
                    }
                    //Создание структуры входных данных
                    InputData inputData = new InputData { V = V, items = items };
                    //Выбор алгоритма
                    PackingAlgorithm algorithm;
                    selectedMenuItem = InterfaceManager.ShowConsoleMenu(
                        "Выберите алгоритм решения задачи:", new MenuItem[]
                        {
                            new MenuItem(1, "Точный (переборный)"),
                            new MenuItem(2, "Приближённый с фиксированной погрешностью (FFS)"),
                            new MenuItem(3, "Приближённый (BF)")
                        });
                    if (selectedMenuItem == 1)
                        algorithm = Packer.FullSearch;
                    else if (selectedMenuItem == 2)
                        algorithm = Packer.FFS;
                    else
                        algorithm = Packer.BF;
                    //Решение
                    ContainerListShell containers = new ContainerListShell();
                    containers.Bins = algorithm(inputData.items, inputData.V);
                    //Вывод решения
                    Console.Clear();
                    inputData.PrintItemList();
                    containers.WriteInfo();
                    Console.ReadKey();
                }
                //Комплексное тестирование
                else if (selectedMenuItem == 2)
                {
                    string FS_solutions_file = "FullSearch_Solutions.txt";
                    string FS_table_file = "FullSearch_SolutionsTable.txt";
                    string FFS_solutions_file = "FFS_Solutions.txt";
                    string FFS_table_file = "FFS_SolutionsTable.txt";
                    string BF_solutions_file = "BF_Solutions.txt";
                    string BF_table_file = "BF_SolutionsTable.txt";
                    //Чистим файлы
                    FileManager.ClearFile(FS_solutions_file);
                    FileManager.ClearFile(FS_table_file);
                    FileManager.ClearFile(FFS_solutions_file);
                    FileManager.ClearFile(FFS_table_file);
                    FileManager.ClearFile(BF_solutions_file);
                    FileManager.ClearFile(BF_table_file);
                    int exactlySlnsForFFSCount = 0;
                    int exactlySlnsForBFCount = 0;
                    Tester.WarmPackingMethods(); //"Прогреваем" методы упаковки перед тестированием
                    //Цикл по размерностям
                    for (int n = 1; n <= maxN; n++)
                    {
                        Console.WriteLine("V = " + n);
                        double FS_middle_time = 0;
                        double FFS_middle_time = 0;
                        double BF_middle_time = 0;
                        float BFpercent = 0;      //процент точных решений
                        float FFSpercent = 0;     // процент точных решений
                        double BFdeviation = 0.0; //отклонение от точного решения
                        double FFSdeviation = 0.0;//отклонение от точного решения

                        FileManager.WriteStringIntoFile(FFS_solutions_file, $"\t\t\t------N = {n}------\n");
                        FileManager.WriteStringIntoFile(BF_solutions_file, $"\t\t\t------N = {n}------\n");
                        FileManager.WriteStringIntoFile(FS_solutions_file, $"\t\t\t------N = {n}------\n");

                        //Цикл по тестам размерности n
                        for (int i = 1; i <= tests_count; i++)
                        {
                            Console.Write($"\rКоличество тестов: {i}");
                            double fullTestTime = 0,
                                curTestTime;
                            //Генерируем новый тест размерности n
                            InputData data = Generator.GenerateRandomZhenya(v, n); //Generate
                            //Создаём объект решения
                            var containers = new ContainerListShell();
                            long exactlyContainersForTestCount;
                            
                            #region FullSearch_Testing
                            //Тестируем точное решение
                            for (int j = 1; j <= accuracy; j++)
                            {
                                //Данное копирование нужно чтобы убрать оптимизацию кэша процессора.
                                //При частом обращении к одному и тому же списку процессор знает где и что искать
                                InputData temp_data = (InputData)data.Clone();
                                containers.Bins = Tester.TestPackingAlgorithm(Packer.FullSearch, temp_data.items, v, out curTestTime);
                                fullTestTime += curTestTime;
                            }
                            //Пишем результаты в файл
                            FileManager.WriteResultIntoFile(FS_solutions_file, data, containers);
                            FS_middle_time += fullTestTime;
                            //Запоминаем количество конейнеров в точном решении
                            exactlyContainersForTestCount = containers.Bins.Count;
                            #endregion

                            #region BF_Testing
                            //Тестируем BF
                            fullTestTime = 0;
                            for (int j = 1; j <= accuracy; j++)
                            {
                                //Данное копирование нужно чтобы убрать оптимизацию кэша процессора.
                                //При частом обращении к одному и тому же списку процессор знает где и что искать
                                InputData temp_data = (InputData)data.Clone();
                                containers.Bins = Tester.TestPackingAlgorithm(Packer.BF, temp_data.items, v, out curTestTime);
                                fullTestTime += curTestTime;
                            }
                            //Пишем результаты в файл
                            FileManager.WriteResultIntoFile(BF_solutions_file, data, containers);
                            BF_middle_time += fullTestTime / accuracy;
                            //Если решение совпало с точным
                            if (containers.Bins.Count == exactlyContainersForTestCount)
                            {
                                BFpercent++;
                                exactlySlnsForBFCount++;
                            }
                            BFdeviation += Math.Abs((double)containers.Bins.Count - (double)exactlyContainersForTestCount) / exactlyContainersForTestCount;
                            #endregion

                            #region FFS_Testing
                            //Тестируем FFS
                            fullTestTime = 0;
                            for (int j = 1; j <= accuracy; j++)
                            {
                                //Данное копирование нужно чтобы убрать оптимизацию кэша процессора.
                                //При частом обращении к одному и тому же списку процессор знает где и что искать
                                InputData temp_data = (InputData)data.Clone();
                                containers.Bins = Tester.TestPackingAlgorithm(Packer.FFS, data.items, v, out curTestTime);
                                fullTestTime += curTestTime;
                            }
                            //Пишем результаты в файл
                            FileManager.WriteResultIntoFile(FFS_solutions_file, data, containers);
                            
                            FFS_middle_time += fullTestTime / accuracy;
                            //Если решение совпало с точным
                            if (containers.Bins.Count == exactlyContainersForTestCount)
                            {
                                exactlySlnsForFFSCount++;
                                FFSpercent++;
                            }
                            FFSdeviation += Math.Abs((double)containers.Bins.Count - (double)exactlyContainersForTestCount) / exactlyContainersForTestCount;
                            #endregion
                        }
                        Console.WriteLine();
                        BFdeviation /= tests_count;
                        FFSdeviation /= tests_count;
                        BFpercent *= 100.0f / tests_count;
                        FFSpercent *= 100.0f / tests_count;

                        FileManager.WriteTableRowIntoFile(FS_table_file, n, FS_middle_time / tests_count, 100, 0.0);
                        FileManager.WriteTableRowIntoFile(BF_table_file, n, BF_middle_time / tests_count, (int)BFpercent, BFdeviation);
                        FileManager.WriteTableRowIntoFile(FFS_table_file, n, FFS_middle_time / tests_count, (int)FFSpercent, FFSdeviation);
                    }
                    FileManager.WriteStringIntoFile(FFS_solutions_file, $"Получено точных решений {exactlySlnsForFFSCount} из {maxN * tests_count}");
                    FileManager.WriteStringIntoFile(BF_solutions_file, $"Получено точных решений {exactlySlnsForBFCount} из {maxN * tests_count}");
                    Console.WriteLine("Тестирование завершено");
                    Console.ReadKey();
                }
                //Тестирование одного алгоритма
                else if (selectedMenuItem == 3) {
                    string info = "";
                    //Выбор тестируемого алгоритма
                    selectedMenuItem = InterfaceManager.ShowConsoleMenu(
                    "Выберите алгоритм:", new MenuItem[]
                    {
                        new MenuItem(1, "Переборный"),
                        new MenuItem(2, "FFS"),
                        new MenuItem(3, "BF")
                    });
                    PackingAlgorithm algorithm;
                    string testfile, timefile;
                    if (selectedMenuItem == 1)
                    {
                        info += "FullSearch";
                        algorithm = Packer.FullSearch;
                        testfile = "FS_SingleTestSolutions.txt";
                        timefile = "FS_SingleTestTime.txt";
                    }
                    else if (selectedMenuItem == 2)
                    {
                        info += "FFS";
                        algorithm = Packer.FFS;
                        testfile = "FFS_SingleTestSolutions.txt";
                        timefile = "FFS_SingleTestTime.txt";
                    }
                    else
                    {
                        info += "BF";
                        algorithm = Packer.BF;
                        testfile = "BF_SingleTestSolutions.txt";
                        timefile = "BF_SingleTestTime.txt";
                    }
                    //Чистим файлы
                    FileManager.ClearFile(testfile);
                    FileManager.ClearFile(timefile);
                    //Выбор содержания тестов
                    selectedMenuItem = InterfaceManager.ShowConsoleMenu(
                    "Выберите характер тестов:", new MenuItem[]
                    {
                        new MenuItem(1, "Лучший случай"),
                        new MenuItem(2, "Случайные тесты"),
                        new MenuItem(3, "Худший случай")
                    });
                    GeneratorMethod GenerateTest;
                    if (selectedMenuItem == 1)
                    {
                        info += "_Best case"; 
                        GenerateTest = Generator.GenerateBestCase;
                    }
                    else if (selectedMenuItem == 2)
                    {
                        info += " random case";
                        GenerateTest = Generator.GenerateRandomZhenya;
                    }
                    else
                    {
                        info += " worst case";
                        GenerateTest = Generator.GenerateWorstCase;
                    }
                    //Выбор начального объёма тестов
                    Console.Write("Введите начальный объём тестов: ");
                    uint beginN;
                    while (true)
                        if (uint.TryParse(Console.ReadLine(), out beginN) && beginN != 0)
                            break;
                    Console.Clear();
                    //Выбор шага увеличения объёмов
                    Console.Write("Введите шаг увеличения объёма тестов: ");
                    uint stepN;
                    while (true)
                        if (uint.TryParse(Console.ReadLine(), out stepN) && stepN != 0)
                            break;
                    Console.Clear();
                    //Выбор количества тестов
                    Console.Write("Введите количество тестов: ");
                    uint countN;
                    while (true)
                        if (uint.TryParse(Console.ReadLine(), out countN) && countN != 0)
                            break;
                    Console.Clear();
                    Console.WriteLine(info);
                    //Тестирование
                    Tester.WarmPackingMethods(); //"Прогреваем" методы упаковки перед тестированием
                    //Цикл по размерностям
                    int endN = (int)(beginN + stepN * (countN - 1));
                    for (int n = (int)beginN; n <= endN; n += (int)stepN) {
                        Console.WriteLine("V = " + n);
                        double test_middle_time = 0;

                        FileManager.WriteStringIntoFile(testfile, $"\t\t\t------N = {n}------\n");
                        int local_tests_count = 100;
                        //Цикл по тестам размерности n
                        for (int i = 1; i <= local_tests_count; i++) {
                            Console.Write($"\rКоличество тестов: {i}");
                            double fullTestTime = 0,
                                curTestTime;
                            //Генерируем новый тест размерности n
                            InputData data = GenerateTest(v, n);
                            //Создаём объект решения
                            var containers = new ContainerListShell();
                            //Тестируем
                            fullTestTime = 0;
                            for (int j = 1; j <= accuracy; j++) {
                                InputData temp_data = (InputData)data.Clone();
                                containers.Bins = Tester.TestPackingAlgorithm(algorithm, temp_data.items, v, out curTestTime);
                                fullTestTime += curTestTime;
                            }
                            //Пишем результаты в файл
                            FileManager.WriteResultIntoFile(testfile, data, containers);
                            test_middle_time += fullTestTime / accuracy;
                        }
                        Console.WriteLine();
                        FileManager.WriteTimeIntoFile(timefile, n, test_middle_time / local_tests_count);
                    }
                    Console.WriteLine("Тестирование завершено");
                    Console.ReadKey();
                }
            }
        }
    }
}
