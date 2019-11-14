using System;
using System.Collections.Generic;

namespace ContainerPacking
{
    public class Item : ICloneable
    {
        public uint Id { get; private set; }    // Идентификатор предмета
        public uint M { get; private set; }     // Масса предмета

        public Item(uint id, uint m)
        {
            Id = id;
            M = m;
        }

        public override string ToString()
        {
            return Id.ToString() + "(" + M.ToString() + ")";
        }

        public object Clone()
        {
            return new Item(Id, M);
        }
    }

    //Класс-сортировщик двух предметов по убыванию веса
    public class ItemOnWeightDescComparer : IComparer<Item>
    {
        public int Compare(Item x, Item y)
        {
            return y.M.CompareTo(x.M);
        }
    }
}
