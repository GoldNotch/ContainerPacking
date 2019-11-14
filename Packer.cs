using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ContainerPacking
{
    //упаковщик контейнеров
    public static class Packer
    {
        //стандартный метод FF
        public static List<Container> FF(List<Item> items, uint V)
        {
            //ulong c = 2;//int i = 0; создание списка
            var containers = new List<Container>();

            for(int i = 0; i < items.Count; ++i)
            {
                //c += 3;//проверка условия цикла, int j = 0; присваивание
                bool isInserted = false;
                for(int j = 0; j < containers.Count; ++j)
                {
                   // ++c;//проверка условия цикла
                    if (containers[j].CurrentLoad + items[i].M <= V)
                    {
                        containers[j].AddItem(items[i]);
                        isInserted = true;
                        //c += 5;//тело условия
                        break;
                    }
                    //c += 3;//проверка условия + ++j
                }
                //c += 1;//последнее условие цикла
                if (!isInserted)
                {
                    //c += 4;//вставка занимает 4 операции
                    containers.Add(new Container(V, items[i]));
                }
                //c += 2;//проверка условия + ++i
            }
            //c++;
            //Debug.WriteLine($"FFS({items.Count}) = {c + items.Count * Math.Log(items.Count) / Math.Log(2.0) + 1}");
            return containers;
        }

        //рекурсивный полный перебор
        static void FullSearch_rec(uint V, ref List<Container> result, List<Item> free_items,  List<Item> taken_items)
        {
            if (free_items.Count == 0)
            {
                var list = FF(taken_items, V);
                if (result == null || list.Count < result.Count) result = list;
            }
            else
                for(int i = 0; i < free_items.Count; ++i)
                {
                    var item = free_items[i];
                    //удаляем из свободных предметов и вставляем во взятые
                    free_items.RemoveAt(i);
                    taken_items.Add(item);
                    FullSearch_rec(V, ref result, free_items, taken_items);
                    //удаляем из взятых и вставляем в свободные
                    taken_items.Remove(item);
                    free_items.Insert(i, item);
                }
        }

        public static List<Container> FullSearch(List<Item> items, uint V)
        {
            List<Container> list = null;
            var taken_items = new List<Item>();
            FullSearch_rec(V, ref list, items, taken_items);
            return list;
        }

        public static List<Container> FFS(List<Item> items, uint V)
        {
            items.Sort(new ItemOnWeightDescComparer()); //Сортировка предметов по убыванию веса
            return FF(items, V);
        }

        #region BestFit: implementation by Lashov Nickolay

        public static List<Container> BF(List<Item> items, uint V)
        {
            //ulong c = 2;//int i = 0; создание списка
            var bins = new List<Container>();

            foreach(var item in items)
            {
                //c += 4;//int i = 0, два присваивания и условие цикла
                int bestIdx = -1;
                int maxLoad = -1;

                // ищем такой контейнер, чтобы загрузка не превышала объем и была максимальной
                for (int i = 0; i < bins.Count; ++i)
                {
                    //c += 3;//условие цикла, присвание, +
                    var load = bins[i].CurrentLoad + item.M;
                    if (load <= V && load > maxLoad)
                    {
                        bestIdx = i;
                        maxLoad = (int)load;
                        //c += 2;//тело условия
                    }
                    //c += 4;//3 условия цикла, ++i
                }
                //c++;//последнее условие цикла
                if (bestIdx == -1)
                {
                    //c += 5;//создание контейнера
                    bins.Add(new Container(V, item));
                }
                else
                {
                    //c += 3;//вставка в список
                    bins[bestIdx].AddItem(item);
                }
                //c += 2;//условие цикла и  условие выше
            }
            //c++;
            //Debug.WriteLine($"BF({items.Count}) = {c + 1}");
            return bins;
        }

        #endregion
    }
}
