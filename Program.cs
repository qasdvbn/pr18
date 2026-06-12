using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicWeaponShop
{
    public class Item
    {
        public string Name;
        public int BasePrice;
        public int MinPrice;
        public int Quantity;

        public Item(string name, int basePrice, int minPrice, int quantity)
        {
            Name = name;
            BasePrice = basePrice;
            MinPrice = minPrice;
            Quantity = quantity;
        }
    }

    public class Shop
    {
        private List<Item> items = new List<Item>();
        private int playerGold = 500;
        private bool isOpen = true;

        public Shop()
        {
            items.Add(new Item("меч", 120, 90, 5));
            items.Add(new Item("доспех", 80, 60, 3));
            items.Add(new Item("зелье", 30, 20, 10));
        }

        public void Run()
        {
            Console.WriteLine("Магическая лавка. Покупайте пока есть деньги.");

            while (isOpen)
            {
                ShowItems();
                Console.WriteLine("У вас " + playerGold + " золота");
                Console.Write("> ");
                string input = Console.ReadLine().ToLower();

                if (input == "уйти") break;

                string[] parts = input.Split(' ');
                if (parts.Length < 2)
                {
                    Console.WriteLine("Формат: товар количество [цена]");
                    continue;
                }

                string itemName = parts[0];
                if (!int.TryParse(parts[1], out int amount) || amount <= 0)
                {
                    Console.WriteLine("Неверное количество");
                    continue;
                }

                Item item = items.FirstOrDefault(i => i.Name == itemName && i.Quantity >= amount);
                if (item == null)
                {
                    Console.WriteLine("Нет такого товара");
                    continue;
                }

                int finalPrice = item.BasePrice;

                if (parts.Length >= 3 && int.TryParse(parts[2], out int offer))
                {
                    finalPrice = TryHaggle(item, offer);
                    if (finalPrice == -1) continue;
                }

                Buy(item, amount, finalPrice);
            }

            Console.WriteLine("Лавка закрылась. Вы потратили " + (500 - playerGold) + " золота");
            Console.ReadKey();
        }

        private void ShowItems()
        {
            Console.WriteLine("\nТовары:");
            bool hasItems = false;
            foreach (Item i in items.Where(i => i.Quantity > 0))
            {
                Console.WriteLine(i.Name + " " + i.BasePrice + " зол. (" + i.Quantity + " шт.)");
                hasItems = true;
            }
            if (!hasItems)
            {
                Console.WriteLine("Товаров нет");
                isOpen = false;
            }
        }

        private int TryHaggle(Item item, int offerPerUnit)
        {
            Console.WriteLine("Продавец просит " + item.BasePrice + " золота за штуку");
            Console.WriteLine("Вы предлагаете " + offerPerUnit + " золота за штуку");

            if (offerPerUnit >= item.BasePrice)
            {
                Console.WriteLine("Продавец согласен");
                return offerPerUnit;
            }

            if (offerPerUnit >= item.MinPrice)
            {
                Console.WriteLine("Продавец согласен");
                return offerPerUnit;
            }

            Console.WriteLine("Продавец не согласен");
            Console.Write("Купить за " + item.MinPrice + "? (да/нет): ");
            if (Console.ReadLine().ToLower() == "да")
                return item.MinPrice;

            return -1;
        }

        private void Buy(Item item, int amount, int pricePerUnit)
        {
            int total = pricePerUnit * amount;

            if (playerGold < total)
            {
                Console.WriteLine("Не хватает золота. У вас " + playerGold + ", нужно " + total);
                return;
            }

            Console.WriteLine("Цена: " + total + " золота");
            Console.Write("Купить? (да/нет): ");

            if (Console.ReadLine().ToLower() != "да")
            {
                Console.WriteLine("Отмена");
                return;
            }

            playerGold -= total;
            item.Quantity -= amount;
            Console.WriteLine("Куплено " + amount + "x " + item.Name + ". Осталось " + playerGold + " золота");

            if (playerGold == 0)
            {
                Console.WriteLine("У вас кончились деньги. Лавка закрывается.");
                isOpen = false;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            new Shop().Run();
        }
    }
}