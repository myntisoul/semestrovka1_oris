using semestrovka1_oris.framework.core;
using System.Net;
using System.Text;
using System.Text.Json;


namespace semestrovka1_oris
{
    public class Settings
    {
        public string[] Prefixes { get; set; }
    }

    class Program
    {
        static async Task Main()
        {
            //string pathSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
            //string jsonString = File.ReadAllText(pathSettings);
            //var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            //Settings settings = JsonSerializer.Deserialize<Settings>(jsonString, options);

            using (var db = new GameContext())
            {
                db.Database.EnsureCreated();
                if (!db.Roles.Any())
                {
                    db.Roles.Add(new Role { Id = 1, Name = "Убийца", Description = "Выводит любого персонажа из игры на один раунд", ImageURL = "/images/roles/assassin.png" });
                    db.Roles.Add(new Role { Id = 2, Name = "Вор", Description = "Cпособен украсть золото у другого персонажа (кроме Ассасина и его жертвы)", ImageURL = "/images/roles/thief.png" });
                    db.Roles.Add(new Role { Id = 3, Name = "Маг", Description = "Даёт возможность получить новые карты из колоды в обмен на свои, либо обменять все имеющиеся у себя в руке карты на все карты, имеющиеся в руке у другого игрока.", ImageURL = "/images/roles/magician.png" });
                    db.Roles.Add(new Role { Id = 4, Name = "Король", Description = "Получает дополнительный доход со своих дворянских кварталов (жёлтого цвета) и позволяет владельцу получить корону и первым совершать выбор персонажа в следующем раунде.", ImageURL = "/images/roles/king.png" });
                    db.Roles.Add(new Role { Id = 5, Name = "Епископ", Description = "Получает защиту зданий от разрушения в этот раунд и получает монету за каждый церковный (синий) квартал в своем городе.", ImageURL = "/images/roles/bishop.png" });
                    db.Roles.Add(new Role { Id = 6, Name = "Торговец", Description = "Автоматически получает дополнительный денежный бонус в 1 монету и доход со своих кварталов зелёного цвета.", ImageURL = "/images/roles/trader.png" });
                    db.Roles.Add(new Role { Id = 7, Name = "Архитектор", Description = "Автоматически получает дополнительные два квартала из колоды после совершения действия (то есть после взятия денег или карт кварталов перед постройкой) и может построить вплоть до трёх кварталов (остальные персонажи строят только один квартал в ход).", ImageURL = "/images/roles/architect.png" });
                    db.Roles.Add(new Role { Id = 8, Name = "Военачальник", Description = "Получает 1 монету за каждый военный (красный) квартал. В конце своего хода может разрушить здание любого игрока (кроме Епископа) за плату, на 1 монету меньшую, чем стоимость постройки квартала. Кварталы стоимостью 1 можно разрушать бесплатно.", ImageURL = "/images/roles/condotier.png" });

                    db.SaveChanges();
                }
                if (!db.Districts.Any())
                {
                    if (!db.Districts.Any())
                    {
                        db.Districts.AddRange(
                            // Желтые (Аристократия)
                            new District { Id = 1, Name = "Поместье", Description = "", Cost = 3, Quantity = 5, Color = "Желтый", ImageURL = "/images/districts/manor.png" },
                            new District { Id = 2, Name = "Замок", Description = "", Cost = 4, Quantity = 4, Color = "Желтый", ImageURL = "/images/districts/castle.png" },
                            new District { Id = 3, Name = "Дворец", Description = "", Cost = 5, Quantity = 3, Color = "Желтый", ImageURL = "/images/districts/palace.png" },

                            // Синие (Религия)
                            new District { Id = 4, Name = "Храм", Description = "", Cost = 1, Quantity = 3, Color = "Синий", ImageURL = "/images/districts/temple.png" },
                            new District { Id = 5, Name = "Церковь", Description = "", Cost = 2, Quantity = 3, Color = "Синий", ImageURL = "/images/districts/church.png" },
                            new District { Id = 6, Name = "Монастырь", Description = "", Cost = 3, Quantity = 2, Color = "Синий", ImageURL = "/images/districts/monastery.png" },
                            new District { Id = 7, Name = "Собор", Description = "", Cost = 5, Quantity = 2, Color = "Синий", ImageURL = "/images/districts/cathedral.png" },

                            // Зеленые (Торговля)
                            new District { Id = 8, Name = "Таверна", Description = "", Cost = 1, Quantity = 5, Color = "Зеленый", ImageURL = "/images/districts/tavern.png" },
                            new District { Id = 9, Name = "Рынок", Description = "", Cost = 2, Quantity = 3, Color = "Зеленый", ImageURL = "/images/districts/market.png" },
                            new District { Id = 10, Name = "Торговая точка", Description = "", Cost = 2, Quantity = 1, Color = "Зеленый", ImageURL = "/images/districts/trading_post.png" },
                            new District { Id = 11, Name = "Доки", Description = "", Cost = 3, Quantity = 3, Color = "Зеленый", ImageURL = "/images/districts/docks.png" },
                            new District { Id = 12, Name = "Порт", Description = "", Cost = 4, Quantity = 3, Color = "Зеленый", ImageURL = "/images/districts/port.png" },
                            new District { Id = 13, Name = "Муниципалитет", Description = "", Cost = 5, Quantity = 1, Color = "Зеленый", ImageURL = "/images/districts/town_hall.png" },

                            // Красные (Военные)
                            new District { Id = 14, Name = "Сторожевая вышка", Description = "", Cost = 1, Quantity = 3, Color = "Красный", ImageURL = "/images/districts/watchtower.png" },
                            new District { Id = 15, Name = "Тюрьма", Description = "", Cost = 3, Quantity = 3, Color = "Красный", ImageURL = "/images/districts/prison.png" },
                            new District { Id = 16, Name = "Марсово поле", Description = "", Cost = 3, Quantity = 2, Color = "Красный", ImageURL = "/images/districts/battlefield.png" },
                            new District { Id = 17, Name = "Крепость", Description = "", Cost = 4, Quantity = 2, Color = "Красный", ImageURL = "/images/districts/fortress.png" },

                            // Фиолетовые (Уникальные - с описанием)
                            new District { Id = 18, Name = "Квартал призраков", Description = "При подсчете победных очков этот квартал становится того цвета, который вы сами выберете. Свойство не действует, если квартал призраков построен на последнем ходу игры.", Cost = 2, Quantity = 1, Color = "Фиолетовый", ImageURL = "/images/districts/haunted_quarter.png" },
                            new District { Id = 19, Name = "Башня", Description = "Башня не может быть уничтожена Военачальником", Cost = 3, Quantity = 2, Color = "Фиолетовый", ImageURL = "/images/districts/keep.png" },
                            new District { Id = 20, Name = "Великая стена", Description = "Разрушение кварталов вашего города будет стоить Военачальнику на одно золото дороже", Cost = 6, Quantity = 1, Color = "Фиолетовый", ImageURL = "/images/districts/great_wall.png" },
                            new District { Id = 21, Name = "Университет", Description = "Постройка этого квартала стоит 6 золота, но он приносит 8 очков в конце игры", Cost = 6, Quantity = 1, Color = "Фиолетовый", ImageURL = "/images/districts/university.png" },
                            new District { Id = 22, Name = "Обсерватория", Description = "Если при совершении действия вы тянете карты, то тяните три карты, одну оставьте в руке, а остальные две положите вниз колоды", Cost = 5, Quantity = 1, Color = "Фиолетовый", ImageURL = "/images/districts/observatory.png" },
                            new District { Id = 23, Name = "Кладбище", Description = "Когда Военачальник разрушает ваш квартал, можете заплатить одно золото и вернуть разрушенный квартал в руку. Свойство не действует, если вы сами Военачальник.", Cost = 5, Quantity = 1, Color = "Фиолетовый", ImageURL = "/images/districts/graveyard.png" },
                            new District { Id = 24, Name = "Кузница", Description = "Один раз за ход вы можете за два золота взять в руку три карты из колоды районов", Cost = 5, Quantity = 1, Color = "Фиолетовый", ImageURL = "/images/districts/smithy.png" },
                            new District { Id = 25, Name = "Школа магии", Description = "Для получения прибыли, школа может стать кварталом того цвета, какой вы сами выберете.", Cost = 6, Quantity = 1, Color = "Фиолетовый", ImageURL = "/images/districts/school_of_magic.png" },
                            new District { Id = 26, Name = "Библиотека", Description = "Если вы на своем ходу при совершении действия тянете карты, оставьте в руке обе вытянутые карты", Cost = 6, Quantity = 1, Color = "Фиолетовый", ImageURL = "/images/districts/library.png" },
                            new District { Id = 27, Name = "Лаборатория", Description = "Один раз за ход вы можете сбросить одну карту кварталов с руки и получить за это одно золото из банка", Cost = 5, Quantity = 1, Color = "Фиолетовый", ImageURL = "/images/districts/laboratory.png" },
                            new District { Id = 28, Name = "Ворота дракона", Description = "Постройка этого квартала стоит 6 золота, но он приносит 8 очков в конце игры", Cost = 6, Quantity = 1, Color = "Фиолетовый", ImageURL = "/images/districts/dragon_gate.png" }
                        );
                    }
                    db.SaveChanges();
                }
            }   

            HttpServer server = new HttpServer();
            server.Start();
            Console.WriteLine("write 'stop' to stop the server");
            Console.WriteLine("http://127.0.0.1:8888/");
            while (true)
            {
                string command = Console.ReadLine();
                if (command == "stop") { server.Stop(); break; }
            }
        }
    }
}

