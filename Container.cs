using System;
using System.Collections.Generic;
using System.Text;

namespace ContainerPacking
{
    public class Container
    {
        private List<Item> items;               // Список предметов
        public uint V { get; private set; }     // Вместимость контейнера
        public uint CurrentLoad { get; set; }   // Текущая загрузка контейнера

        public Container(uint v)
        {
            V = v;
            CurrentLoad = 0;
            items = new List<Item>();
        }

        public Container(uint v, Item firstItem)
        {
            V = v;
            CurrentLoad = firstItem.M;
            items = new List<Item>
            {
                firstItem
            };
        }

        public void AddItem(Item item)
        {
            items.Add(item);
            CurrentLoad += item.M;
        }

        public override string ToString()
        {
            var sb = new StringBuilder("[");

            foreach (var item in items)
                sb.Append(item.ToString() + ", ");

            sb.Remove(sb.Length - 2, 2);
            sb.Append("]");

            sb.Append($" - объём: {CurrentLoad}/{V}");

            return sb.ToString();
        }
    }

    public class ContainerListShell
    {
        public List<Container> Bins { get; set; }

        public void WriteInfo()
        {
            Console.WriteLine("Раскладка по контейнерам:");
            foreach (var bin in Bins)
                Console.WriteLine(bin.ToString());
            Console.WriteLine($"Ушло контейнеров: {Bins.Count}");
        }
    }
}
