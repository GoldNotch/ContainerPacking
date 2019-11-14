using System;
using System.Collections.Generic;

namespace ContainerPacking
{
    public struct InputData : ICloneable
    {
        public uint V;
        public List<Item> items;

        public void PrintItemList()
        {
            if (items == null)
                return;
            Console.WriteLine("Список вещей:");
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                Console.Write(items[i]);
                if (i < itemsCount - 1)
                    Console.Write(", ");
            }
            Console.WriteLine();
        }

        public object Clone()
        {

            InputData clone = new InputData();
            clone.V = V;
            clone.items = new List<Item>(items.Count);
            //копирование предметов
            for (int i = 0; i < items.Count; ++i)
                clone.items.Add((Item)items[i].Clone());
            return clone;
        }
    }

    //генератор тестов
    public static class Generator
    {
        static Random randomer = new Random();
        public static InputData Generate(int maxV, int items_count)
        {
            InputData data;
            data.V = (uint)maxV;//(uint)randomer.Next(1, maxV);
            data.items = new List<Item>(items_count);

            for (uint i = 0; i < items_count; ++i)
                data.items.Add(new Item(i, (uint)randomer.Next(1, (int)data.V * 4 / 7)));

            return data;
        }

        public static InputData GenerateRandomZhenya(int maxV, int items_count) {
            InputData data;
            //Random randomer = new Random();
            data.V = (uint)maxV;
            data.items = new List<Item>(items_count);

            //Соотношение весов предметов
            int percentOfBig = 10; //Вещи, вес которых больше V / 2
            int percentOfMiddle = 80; //Вещи, вес которых от 1/4*V до V/2
            //Вещи, вес которых от 1 до 1/4V = остаток

            //Количество предметов каждого типа, исходя из соотношения
            int countOfBig = (int)(items_count / 100d * percentOfBig);
            int countOfMiddle = (int)(items_count / 100d * percentOfMiddle);
            int countOfSmall = items_count - (countOfBig + countOfMiddle);

            for (uint i = 0; i < items_count; ++i) {
                //Выбираем, вещь какой категории будем класть
                int itemCategory;
                bool isCategoryChoiced;
                do {
                    itemCategory = randomer.Next(3); //0, 1, 2
                    isCategoryChoiced = 
                        (itemCategory == 0 && countOfBig > 0 || itemCategory == 1 && countOfMiddle > 0 || itemCategory == 2 && countOfSmall > 0 ? true : false);
                } while (!isCategoryChoiced);
                //Большой предмет
                if (itemCategory == 0 && countOfBig > 0) {
                    //Вес от V/2 + 1 до V
                    data.items.Add(new Item(i, (uint)randomer.Next((int)data.V /2 + 1, (int)data.V + 1)));
                    countOfBig--;
                }
                //Средний предмет
                else if (itemCategory == 1 && countOfMiddle > 0){
                    //Вес от V/4 до V/2
                    data.items.Add(new Item(i, (uint)randomer.Next((int)data.V / 4, (int)data.V /2 + 1)));
                    countOfMiddle--;
                }
                //Маленький предмет
                else if (itemCategory == 2 && countOfSmall > 0) {
                    //Вес от 1 до V/4 - 1
                    data.items.Add(new Item(i, (uint)randomer.Next(1, (int)data.V / 4)));
                    countOfSmall--;
                }
            }

            return data;
        }

        public static InputData GenerateRandom(int maxV, int items_count)
        {
            InputData data;
            //Random randomer = new Random();
            data.V = (uint)randomer.Next(1, maxV);
            data.items = new List<Item>(items_count);

            for (uint i = 0; i < items_count; ++i)
                data.items.Add(new Item(i, (uint)randomer.Next(1, (int)data.V * 4 / 7)));

            return data;
        }

        public static InputData GenerateWorstCase(int maxV, int items_count) {
            InputData data;
            //Random randomer = new Random();
            data.V = (uint)maxV;//(uint)randomer.Next(1, maxV);
            data.items = new List<Item>(items_count);

            for (uint i = 0; i < items_count; ++i)
                data.items.Add(new Item(i, (uint)maxV));

            return data;
        }

        public static InputData GenerateBestCase(int maxV, int items_count) {
            InputData data;
            //Random randomer = new Random();
            data.V = (uint)maxV;//(uint)randomer.Next(1, maxV);
            data.items = new List<Item>(items_count);
            uint i = 0;
            for (; i < maxV && i < items_count; ++i)
                data.items.Add(new Item(i, 1U));
            for (; i < items_count; ++i)
                data.items.Add(new Item(i, 0u));

            return data;
        }
    }

    //Делегат для метода-генератора лучших и худших тестов
    public delegate InputData GeneratorMethod(int maxV, int items_count);
}
