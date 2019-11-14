using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerPacking
{
    class InterfaceManager
    {
        //Отображение меню. Возвращает ID выбранного пункта меню
        public static uint ShowConsoleMenu(string header, MenuItem[] items)
        {
            bool isSelectingEnd = false;
            uint selectedID = 0;
            while (!isSelectingEnd)
            {
                Console.Clear();
                //Печатаем заголовок
                Console.WriteLine(header);
                //Печатаем пункты меню
                foreach (var item in items)
                    Console.WriteLine(item);
                Console.WriteLine("Ваш выбор: ");
                //Ввод и проверка ввода на число
                if (!uint.TryParse(Console.ReadLine(), out selectedID))
                    continue;
                //Проверка ввода на соответствие пункту меню
                foreach (var item in items)
                    if (item.ID == selectedID)
                        isSelectingEnd = true;
            }
            Console.Clear();
            return selectedID;
        }
    }

    //Класс пункта меню: номер пункта и его текст
    class MenuItem
    {
        public uint ID { get; set; }
        public string Text { get; set; }

        //Конструктор
        public MenuItem(uint iD, string text)
        {
            ID = iD;
            Text = text;
        }

        public override string ToString()
        {
            return $"{ID}. {Text}";
        }
    }
}
