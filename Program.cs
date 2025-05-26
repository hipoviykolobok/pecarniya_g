using System;
using System.Collections.Generic;
using System.Linq;

namespace BakerySimulation
{
    // Базовый класс Человек
    public class Human
    {
        public string Post { get; set; }
        public int Money { get; set; }

        public Human(string post, int money)
        {
            Post = post;
            Money = money;
        }
    }

    // Класс Начальник
    public class Boss : Human
    {
        public Boss(string post = "Начальник", int money = 50000) : base(post, money)
        {
        }

        // Нанять пекаря
        public Baker HireBaker()
        {
            return new Baker();
        }

        // Нанять кассира
        public Cashier HireCashier()
        {
            return new Cashier();
        }

        // Выплатить зарплату сотруднику
        public void Payday(int amount, Human employee)
        {
            if (Money >= amount)
            {
                Money -= amount;
                employee.Money += amount;
            }
            else
            {
                Console.WriteLine("Недостаточно средств у начальника для выплаты зарплаты.");
            }
        }
    }

    // Класс Пекарь
    public class Baker : Human
    {
        public Baker(string post = "Пекарь", int money = 100) : base(post, money)
        {
        }

        // Приготовить продукт, возвращает объект продукта
        public Production CookProduct(Type productType, int quantity, int price)
        {
            // Пекарь зарабатывает 10% от цены за единицу
            Money += (price * quantity) / 10;

            Production product = null;
            if (productType == typeof(Bun))
                product = new Bun(price, quantity);
            else if (productType == typeof(Bread))
                product = new Bread(price, quantity);
            else if (productType == typeof(Meal))
                product = new Meal(price, quantity);
            else if (productType == typeof(Drink))
                product = new Drink(price, quantity);

            return product;
        }
    }

    // Класс Кассир
    public class Cashier : Human
    {
        public Cashier(string post = "Кассир", int money = 300) : base(post, money)
        {
        }

        // Рассчитать общую стоимость и добавить 10% комиссионных кассиру
        public int CalculateBill(List<(Production product, int quantity)> products)
        {
            int totalCost = products.Sum(p => p.product.Price * p.quantity);
            Money += totalCost / 10;
            return totalCost;
        }
    }

    // Класс Клиент
    public class Client : Human
    {
        // Список товаров в корзине (продукт + количество)
        public List<(Production product, int quantity)> ShoppingBag { get; private set; }

        public Client(string post = "Клиент", int money = 1000) : base(post, money)
        {
            ShoppingBag = new List<(Production, int)>();
        }

        // Добавить товар в корзину
        public void AddToShoppingBag(Production product, int quantity)
        {
            ShoppingBag.Add((product, quantity));
        }

        // Оплатить товары через кассира
        public void Checkout(Cashier cashier)
        {
            int totalCost = cashier.CalculateBill(ShoppingBag);
            if (totalCost > Money)
            {
                Console.WriteLine("Недостаточно средств");
            }
            else
            {
                Money -= totalCost;
                Console.WriteLine($"Общая стоимость покупок {totalCost}, денег осталось - {Money}");
                ShoppingBag.Clear(); // Очистить корзину после оплаты
            }
        }
    }

    // Базовый класс для продукции
    public class Production
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }

        public Production(string name, int price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }

    public class Bun : Production
    {
        public Bun(int price, int quantity) : base("Булочка", price, quantity)
        {
        }
    }

    public class Bread : Production
    {
        public Bread(int price, int quantity) : base("Хлеб", price, quantity)
        {
        }
    }

    public class Meal : Production
    {
        public Meal(int price, int quantity) : base("Еда с собой", price, quantity)
        {
        }
    }

    public class Drink : Production
    {
        public Drink(int price, int quantity) : base("Напитки", price, quantity)
        {
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Boss boss = new Boss();
            Baker baker = boss.HireBaker();
            Cashier cashier = boss.HireCashier();

            Client client1 = new Client();
            Client client2 = new Client();

            Dictionary<string, Production> storage = new Dictionary<string, Production>();

            while (true)
            {
                Console.WriteLine("\n1. Приготовить продукцию");
                Console.WriteLine("2. Добавить товар в корзину");
                Console.WriteLine("3. Перейти к оплате");
                Console.WriteLine("4. Выйти");
                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.WriteLine("\nВыберите продукт, который хотите приготовить:");
                    Console.WriteLine("1 - Булочка, 2 - Хлеб, 3 - Еда с собой, 4 - Напиток");
                    Console.Write("Выберите продукцию: ");
                    string productChoice = Console.ReadLine();

                    Console.Write("Введите количество: ");
                    if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Неверное количество");
                        continue;
                    }

                    Console.Write("Введите цену за единицу: ");
                    if (!int.TryParse(Console.ReadLine(), out int price) || price <= 0)
                    {
                        Console.WriteLine("Неверная цена");
                        continue;
                    }

                    Dictionary<string, string> productNames = new Dictionary<string, string>
                    {
                        {"1", "Булочка"},
                        {"2", "Хлеб"},
                        {"3", "Еда с собой"},
                        {"4", "Напиток"}
                    };

                    Dictionary<string, Type> productTypes = new Dictionary<string, Type>
                    {
                        {"1", typeof(Bun)},
                        {"2", typeof(Bread)},
                        {"3", typeof(Meal)},
                        {"4", typeof(Drink)}
                    };

                    if (productTypes.ContainsKey(productChoice))
                    {
                        Production product = baker.CookProduct(productTypes[productChoice], quantity, price);
                        if (storage.ContainsKey(productChoice))
                        {
                            // Увеличить количество, если продукт уже есть на складе
                            storage[productChoice].Quantity += quantity;
                        }
                        else
                        {
                            storage[productChoice] = product;
                        }
                        Console.WriteLine($"Приготовлено {quantity} шт. {productNames[productChoice]}, по цене {price} руб/шт.");
                    }
                    else
                    {
                        Console.WriteLine("Неверный ввод");
                    }
                }
                else if (choice == "2")
                {
                    if (storage.Count == 0)
                    {
                        Console.WriteLine("На складе нет продукции.");
                        continue;
                    }

                    Console.WriteLine("\nВыберите продукт для покупки:");
                    Console.WriteLine("1 - Булочка, 2 - Хлеб, 3 - Еда с собой, 4 - Напиток");
                    Console.Write("Номер продукции: ");
                    string productChoice = Console.ReadLine();

                    if (!storage.ContainsKey(productChoice))
                    {
                        Console.WriteLine("Такого продукта нет на складе.");
                        continue;
                    }

                    Console.Write("Количество продукции: ");
                    if (!int.TryParse(Console.ReadLine(), out int quantityClient) || quantityClient <= 0)
                    {
                        Console.WriteLine("Неверное количество");
                        continue;
                    }

                    Production productInStorage = storage[productChoice];
                    if (productInStorage.Quantity < quantityClient)
                    {
                        Console.WriteLine($"Недостаточно товара на складе. Доступно: {productInStorage.Quantity}");
                        continue;
                    }

                    // Уменьшить количество товара на складе
                    productInStorage.Quantity -= quantityClient;

                    // Добавить товар в корзину клиента
                    client1.AddToShoppingBag(new Production(productInStorage.Name, productInStorage.Price, quantityClient), quantityClient);

                    Console.WriteLine($"Добавлено в корзину {quantityClient} шт. {productInStorage.Name}, по цене {productInStorage.Price} руб/шт.");

                    // Удалить продукт со склада, если количество стало 0
                    if (productInStorage.Quantity == 0)
                    {
                        storage.Remove(productChoice);
                    }
                }
                else if (choice == "3")
                {
                    if (client1.ShoppingBag.Count == 0)
                    {
                        Console.WriteLine("Корзина пуста.");
                        continue;
                    }
                    client1.Checkout(cashier);
                }
                else if (choice == "4")
                {
                    Console.WriteLine("Выход из программы");
                    break;
                }
                else
                {
                    Console.WriteLine("Неправильный ввод");
                }
            }
        }
    }
}
