using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ContainerPacking
{
    class Tester
    {
        public static void WarmPackingMethods() {
            //"прогреваем" методы на простых данных
            Packer.FullSearch(new List<Item> { new Item(0, 3) }, 3); 
            Packer.FFS(new List<Item> { new Item(0, 3) }, 3);
            Packer.BF(new List<Item> { new Item(0, 3) }, 3);
        }

        public static List<Container> TestPackingAlgorithm(PackingAlgorithm algorithm, List<Item> items, uint V, out double Time)
        {
            List<Container> res;
            var stopwatch = new Stopwatch(); //секундомер
            GC.Collect();  //принудительно запускаем сборщик мусора, чтоб он не выехал во время теста

            stopwatch.Start();
            res = algorithm(items, V);
            stopwatch.Stop();
            Time = stopwatch.Elapsed.TotalMilliseconds;
            return res;
        }

    }

    delegate List<Container> PackingAlgorithm(List<Item> items, uint V);
}
