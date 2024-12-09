using System;
using System.Collections.Generic;

public abstract class Card
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Card(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public abstract void Play();
}

public class CreatureCard : Card
{
    public int Attack { get; set; }
    public int Health { get; set; }

    public CreatureCard(string name, string description, int attack, int health)
        : base(name, description)
    {
        Attack = attack;
        Health = health;
    }

    public override void Play()
    {
        Console.WriteLine($"{Name} с атакой {Attack} и здоровьем {Health} разыграна.");
    }
}

public class Deck
{
    public List<Card> Cards { get; private set; }

    public Deck()
    {
        Cards = new List<Card>();
    }

    public void AddCard(Card card)
    {
        Cards.Add(card);
    }

    public void Shuffle()
    {
        Random rng = new Random();
        int n = Cards.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            Card value = Cards[n];
            Cards[n] = Cards[k];
            Cards[k] = value;
        }
    }

    public Card DrawCard()
    {
        if (Cards.Count == 0) return null;
        Card card = Cards[0];
        Cards.RemoveAt(0);
        return card;
    }
}

public abstract class Spell
{
    public string Name { get; set; }
    public string Description { get; set; }

    public Spell(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public abstract void Cast(Player target, CreatureCard targetCreature = null);
}

public class HealingSpell : Spell
{
    public int HealAmount { get; set; }

    public HealingSpell(string name, string description, int healAmount)
        : base(name, description)
    {
        HealAmount = healAmount;
    }

    public override void Cast(Player target, CreatureCard targetCreature = null)
    {
        if (targetCreature != null)
        {
            targetCreature.Health += HealAmount;
            Console.WriteLine($"{Name} использовано на {targetCreature.Name}! Восстановлено {HealAmount} здоровья. Текущее здоровье: {targetCreature.Health}");
        }
        else
        {
            target.Health += HealAmount;
            Console.WriteLine($"{Name} использовано на игрока! Восстановлено {HealAmount} здоровья. Текущее здоровье: {target.Health}");
        }
    }
}

public class DamageSpell : Spell
{
    public int DamageAmount { get; set; }

    public DamageSpell(string name, string description, int damageAmount)
        : base(name, description)
    {
        DamageAmount = damageAmount;
    }

    public override void Cast(Player target, CreatureCard targetCreature = null)
    {
        if (targetCreature != null)
        {
            targetCreature.Health -= DamageAmount;
            Console.WriteLine($"{Name} использовано на {targetCreature.Name}! Нанесено {DamageAmount} урона. Текущее здоровье: {targetCreature.Health}");
        }
        else
        {
            target.Health -= DamageAmount;
            Console.WriteLine($"{Name} использовано на игрока! Нанесено {DamageAmount} урона. Текущее здоровье: {target.Health}");
        }
    }
}

public class Player
{
    public int Health { get; set; }
    public List<CreatureCard> Creatures { get; set; }

    public Player()
    {
        Health = 15; // Начальное здоровье игрока
        Creatures = new List<CreatureCard>();
    }
}

public class Program
{
    // Массивы с колодами
    private static List<CreatureCard>[] creatureDecks = new List<CreatureCard>[4];
    private static List<Spell>[] spellDecks = new List<Spell>[4];

    public static void Main(string[] args)
    {
        // Инициализация колод
        InitializeDecks();

        // Выбор колод игроками
        int player1DeckIndex = ChooseDeck(1);
        int player2DeckIndex = ChooseDeck(2);

        // Проверка на одинаковые колоды
        while (player1DeckIndex == player2DeckIndex)
        {
            Console.WriteLine("Игроки не могут выбрать одинаковые колоды. Пожалуйста, выберите другую колоду для игрока 2.");
            player2DeckIndex = ChooseDeck(2);
        }

        // Получение колод для игроков
        List<CreatureCard> player1Deck = creatureDecks[player1DeckIndex];
        List<CreatureCard> player2Deck = creatureDecks[player2DeckIndex];
        List<Spell> player1Spells = spellDecks[player1DeckIndex];
        List<Spell> player2Spells = spellDecks[player2DeckIndex];

        // Создание игроков
        Player player1 = new Player();
        Player player2 = new Player();

        // Вывод доступных существ и заклинаний
        DisplayAvailableCards(player1Deck, player1Spells, 1);
        DisplayAvailableCards(player2Deck, player2Spells, 2);

        // Определение, кто начинает первым
        int firstPlayer = DetermineFirstPlayer();

        // Основной игровой цикл
        bool gameRunning = true;
        while (gameRunning)
        {
            if (firstPlayer == 1)
            {
                Console.WriteLine("\nХод игрока 1:");
                PlayerTurn(player1, player1Deck, player1Spells, player2);
                firstPlayer = 2; // Передаем ход игроку 2
            }
            else
            {
                Console.WriteLine("\nХод игрока 2:");
                PlayerTurn(player2, player2Deck, player2Spells, player1);
                firstPlayer = 1; // Передаем ход игроку 1
            }

            // Проверка условий окончания игры
            if (IsGameOver(player1, player2))
            {
                gameRunning = false; // Остановить игру, если условия выполнены
            }
            else if (player1.Creatures.Count == 0 && player1Spells.Count == 0 && player2.Creatures.Count == 0 && player2Spells.Count == 0)
            {
                gameRunning = false; // Остановить игру, если у обоих игроков нет существ и заклинаний
                if (player1.Health > player2.Health)
                {
                    Console.WriteLine("Игрок 1 победил за счет большего здоровья!");
                }
                else if (player2.Health > player1.Health)
                {
                    Console.WriteLine("Игрок 2 победил за счет большего здоровья!");
                }
                else
                {
                    Console.WriteLine("Ничья! У обоих игроков одинаковое здоровье.");
                }
            }
        }

        Console.WriteLine("Игра окончена!");
    }

    private static void DisplayAvailableCards(List<CreatureCard> playerDeck, List<Spell> playerSpells, int playerNumber)
    {
        Console.WriteLine($"\nДоступные существа игрока {playerNumber}:");
        for (int i = 0; i < playerDeck.Count; i++)
        {
            Console.WriteLine($"{i}: {playerDeck[i].Name} - {playerDeck[i].Description} (Атака: {playerDeck[i].Attack}, Здоровье: {playerDeck[i].Health})");
        }

        Console.WriteLine($"Доступные заклинания игрока {playerNumber}:");
        for (int i = 0; i < playerSpells.Count; i++)
        {
            Console.WriteLine($"{i}: {playerSpells[i].Name} - {playerSpells[i].Description}");
        }
    }

    private static void PlayerTurn(Player currentPlayer, List<CreatureCard> playerDeck, List<Spell> playerSpells, Player opponent)
    {
        Console.WriteLine($"Текущее здоровье игрока: {currentPlayer.Health}");
        Console.WriteLine("Выберите действие: 1 - Разыграть существо, 2 - Применить заклинание, 3 - Атаковать существом");

        int action;
        while (!int.TryParse(Console.ReadLine(), out action) || action < 1 || action > 3)
        {
            Console.WriteLine("Неверный выбор. Пожалуйста, выберите действие: 1 - Разыграть существо, 2 - Применить заклинание, 3 - Атаковать существом");
        }

        if (action == 1)
        {
            if (playerDeck.Count > 0)
            {
                Console.WriteLine("Выберите существо для разыгрывания:");
                for (int i = 0; i < playerDeck.Count; i++)
                {
                    Console.WriteLine($"{i}: {playerDeck[i].Name}");
                }

                int creatureIndex;
                while (!int.TryParse(Console.ReadLine(), out creatureIndex) || creatureIndex < 0 || creatureIndex >= playerDeck.Count)
                {
                    Console.WriteLine("Неверный выбор существа. Пожалуйста, выберите существ из списка.");
                }

                CreatureCard cardToPlay = playerDeck[creatureIndex];
                cardToPlay.Play();
                currentPlayer.Creatures.Add(cardToPlay); // Добавляем существо к игроку
                playerDeck.RemoveAt(creatureIndex); // Удаляем карту из колоды после разыгрывания
            }
            else
            {
                Console.WriteLine("Ваша колода пуста!");
            }
        }
        else if (action == 2)
        {
            if (playerSpells.Count > 0)
            {
                Console.WriteLine("Выберите заклинание:");
                for (int i = 0; i < playerSpells.Count; i++)
                {
                    Console.WriteLine($"{i}: {playerSpells[i].Name} - {playerSpells[i].Description}");
                }

                int spellIndex;
                while (!int.TryParse(Console.ReadLine(), out spellIndex) || spellIndex < 0 || spellIndex >= playerSpells.Count)
                {
                    Console.WriteLine("Неверный выбор заклинания. Пожалуйста, выберите заклинание из списка.");
                }

                Console.WriteLine("Выберите цель заклинания: 1 - Игрока противника, 2 - Существо противника, 3 - Себя, 4 - Свое существо");
                int targetChoice;
                while (!int.TryParse(Console.ReadLine(), out targetChoice) || (targetChoice < 1 || targetChoice > 4))
                {
                    Console.WriteLine("Неверный выбор цели. Пожалуйста, выберите 1, 2, 3 или 4.");
                }

                if (targetChoice == 1)
                {
                    playerSpells[spellIndex].Cast(opponent);
                }
                else if (targetChoice == 2)
                {
                    if (opponent.Creatures.Count > 0)
                    {
                        Console.WriteLine("Выберите существо противника для атаки:");
                        for (int j = 0; j < opponent.Creatures.Count; j++)
                        {
                            Console.WriteLine($"{j}: {opponent.Creatures[j].Name} (Здоровье: {opponent.Creatures[j].Health})");
                        }

                        int targetCreatureIndex;
                        while (!int.TryParse(Console.ReadLine(), out targetCreatureIndex) || targetCreatureIndex < 0 || targetCreatureIndex >= opponent.Creatures.Count)
                        {
                            Console.WriteLine("Неверный выбор существа. Пожалуйста, выберите существ из списка.");
                        }

                        playerSpells[spellIndex].Cast(opponent, opponent.Creatures[targetCreatureIndex]);
                        if (opponent.Creatures[targetCreatureIndex].Health <= 0)
                        {
                            Console.WriteLine($"{opponent.Creatures[targetCreatureIndex].Name} уничтожено!");
                            opponent.Creatures.RemoveAt(targetCreatureIndex);
                        }
                    }
                    else
                    {
                        Console.WriteLine("У противника нет существ для применения заклинания!");
                    }
                }
                else if (targetChoice == 3)
                {
                    playerSpells[spellIndex].Cast(currentPlayer);
                }
                else if (targetChoice == 4)
                {
                    if (currentPlayer.Creatures.Count > 0)
                    {
                        Console.WriteLine("Выберите свое существо для исцеления:");
                        for (int j = 0; j < currentPlayer.Creatures.Count; j++)
                        {
                            Console.WriteLine($"{j}: {currentPlayer.Creatures[j].Name} (Здоровье: {currentPlayer.Creatures[j].Health})");
                        }

                        int targetCreatureIndex;
                        while (!int.TryParse(Console.ReadLine(), out targetCreatureIndex) || targetCreatureIndex < 0 || targetCreatureIndex >= currentPlayer.Creatures.Count)
                        {
                            Console.WriteLine("Неверный выбор существа. Пожалуйста, выберите существ из списка.");
                        }

                        playerSpells[spellIndex].Cast(currentPlayer, currentPlayer.Creatures[targetCreatureIndex]);
                    }
                    else
                    {
                        Console.WriteLine("У вас нет существ для исцеления!");
                    }
                }
            }
            else
            {
                Console.WriteLine("У вас нет заклинаний!");
            }
        }
        else if (action == 3)
        {
            if (currentPlayer.Creatures.Count > 0)
            {
                Console.WriteLine("Выберите существо для атаки:");
                for (int i = 0; i < currentPlayer.Creatures.Count; i++)
                {
                    Console.WriteLine($"{i}: {currentPlayer.Creatures[i].Name} (Атака: {currentPlayer.Creatures[i].Attack}, Здоровье: {currentPlayer.Creatures[i].Health})");
                }

                int creatureIndex;
                while (!int.TryParse(Console.ReadLine(), out creatureIndex) || creatureIndex < 0 || creatureIndex >= currentPlayer.Creatures.Count)
                {
                    Console.WriteLine("Неверный выбор существа. Пожалуйста, выберите существ из списка.");
                }

                Console.WriteLine("Выберите цель атаки: 1 - Игрока противника, 2 - Существо противника");
                int targetChoice;
                while (!int.TryParse(Console.ReadLine(), out targetChoice) || (targetChoice != 1 && targetChoice != 2))
                {
                    Console.WriteLine("Неверный выбор цели. Пожалуйста, выберите 1 или 2.");
                }

                if (targetChoice == 1)
                {
                    opponent.Health -= currentPlayer.Creatures[creatureIndex].Attack;
                    Console.WriteLine($"{currentPlayer.Creatures[creatureIndex].Name} атакует игрока противника! Текущее здоровье противника: {opponent.Health}");
                }
                else if (targetChoice == 2)
                {
                    if (opponent.Creatures.Count > 0)
                    {
                        Console.WriteLine("Выберите существо противника для атаки:");
                        for (int j = 0; j < opponent.Creatures.Count; j++)
                        {
                            Console.WriteLine($"{j}: {opponent.Creatures[j].Name} (Здоровье: {opponent.Creatures[j].Health})");
                        }

                        int targetCreatureIndex;
                        while (!int.TryParse(Console.ReadLine(), out targetCreatureIndex) || targetCreatureIndex < 0 || targetCreatureIndex >= opponent.Creatures.Count)
                        {
                            Console.WriteLine("Неверный выбор существа. Пожалуйста, выберите существ из списка.");
                        }

                        opponent.Creatures[targetCreatureIndex].Health -= currentPlayer.Creatures[creatureIndex].Attack;
                        Console.WriteLine($"{currentPlayer.Creatures[creatureIndex].Name} атакует {opponent.Creatures[targetCreatureIndex].Name}! Текущее здоровье: {opponent.Creatures[targetCreatureIndex].Health}");
                        if (opponent.Creatures[targetCreatureIndex].Health <= 0)
                        {
                            Console.WriteLine($"{opponent.Creatures[targetCreatureIndex].Name} уничтожено!");
                            opponent.Creatures.RemoveAt(targetCreatureIndex);
                        }
                    }
                    else
                    {
                        Console.WriteLine("У противника нет существ для атаки!");
                    }
                }
            }
            else
            {
                Console.WriteLine("У вас нет существ для атаки! Вы можете разыграть существо или использовать заклинание.");
            }
        }
        else
        {
            Console.WriteLine("Неверный выбор действия.");
        }
    }

    private static bool IsGameOver(Player player1, Player player2)
    {
        if (player1.Health <= 0)
        {
            Console.WriteLine("Игрок 2 победил!");
            return true;
        }
        else if (player2.Health <= 0)
        {
            Console.WriteLine("Игрок 1 победил!");
            return true;
        }
        return false;
    }

    private static int DetermineFirstPlayer()
    {
        Random random = new Random();
        int player1Roll = random.Next(0, 101);
        int player2Roll = random.Next(0, 101);

        Console.WriteLine($"\nИгрок 1 бросает: {player1Roll}");
        Console.WriteLine($"Игрок 2 бросает: {player2Roll}\n");

        while (player1Roll == player2Roll)
        {
            Console.WriteLine("Ничья! Бросаем снова.");
            player1Roll = random.Next(0, 101);
            player2Roll = random.Next(0, 101);
            Console.WriteLine($"Игрок 1 бросает: {player1Roll}");
            Console.WriteLine($"Игрок 2 бросает: {player2Roll}");
        }

        return player1Roll > player2Roll ? 1 : 2; // Возвращаем номер игрока, который начинает первым
    }

    private static void InitializeDecks()
    {
        creatureDecks[0] = CreateDeck1();
        creatureDecks[1] = CreateDeck2();
        creatureDecks[2] = CreateDeck3();
        creatureDecks[3] = CreateDeck4();

        spellDecks[0] = CreateSpellsDeck1();
        spellDecks[1] = CreateSpellsDeck2();
        spellDecks[2] = CreateSpellsDeck3();
        spellDecks[3] = CreateSpellsDeck4();
    }

    private static int ChooseDeck(int playerNumber)
    {
        Console.WriteLine($"Игрок {playerNumber}, выберите колоду (0-3):");
        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine($"{i}: Колода {i + 1}");
        }

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > 3)
        {
            Console.WriteLine("Неверный выбор. Пожалуйста, выберите колоду (0-3):");
        }

        return choice;
    }

    private static List<CreatureCard> CreateDeck1()
    {
        return new List<CreatureCard>
        {
            new CreatureCard("Гоблин", "Маленький, но хитрый.", 2, 3),
            new CreatureCard("Огур", "Большой и сильный.", 5, 7),
            new CreatureCard("Эльф", "Ловкий стрелок.", 3, 2),
            new CreatureCard("Тролль", "Медленный, но мощный.", 6, 10)
        };
    }

    private static List<CreatureCard> CreateDeck2()
    {
        return new List<CreatureCard>
        {
            new CreatureCard("Дракон", "Могущественное существо.", 8, 8),
            new CreatureCard("Скелет", "Неумолимый воин.", 3, 4),
            new CreatureCard("Вампир", "Сосущий кровь.", 4, 5),
            new CreatureCard("Фея", "Магическое существо.", 2, 2)
        };
    }

    private static List<CreatureCard> CreateDeck3()
    {
        return new List<CreatureCard>
        {
            new CreatureCard("Медведь", "Сильный и свирепый.", 7, 9),
            new CreatureCard("Лев", "Царь зверей.", 6, 6),
            new CreatureCard("Змея", "Ядовитая и быстрая.", 4, 3),
            new CreatureCard("Слон", "Громадное и мощное существо.", 5, 12)
        };
    }

    private static List<CreatureCard> CreateDeck4()
    {
        return new List<CreatureCard>
        {
            new CreatureCard("Феникс", "Возрождающийся из пепла.", 9, 5),
            new CreatureCard("Грифон", "Смешанное существо с головой орла и телом льва.", 6, 7),
            new CreatureCard("Кентавр", "Человек с телом лошади.", 5, 6),
            new CreatureCard("Джунглевая ящерица", "Быстрая и ловкая.", 3, 4)
        };
    }

    private static List<Spell> CreateSpellsDeck1()
    {
        return new List<Spell>
        {
            new HealingSpell("Исцеление", "Восстанавливает 5 здоровья цели.", 5),
            new DamageSpell("Огненный шар", "Наносит 7 урона врагу.", 7)
        };
    }

    private static List<Spell> CreateSpellsDeck2()
    {
        return new List<Spell>
        {
            new HealingSpell("Лечение", "Восстанавливает 4 здоровья союзнику.", 4),
            new DamageSpell("Молния", "Наносит 6 урона врагу.", 6)
        };
    }

    private static List<Spell> CreateSpellsDeck3()
    {
        return new List<Spell>
        {
            new HealingSpell("Светлое исцеление", "Восстанавливает 8 здоровья цели.", 8),
            new DamageSpell("Ледяной шторм", "Наносит 5 урона врагу и замедляет его.", 5)
        };
    }

    private static List<Spell> CreateSpellsDeck4()
    {
        return new List<Spell>
        {
            new HealingSpell("Божественное исцеление", "Восстанавливает 10 здоровья цели.", 10),
            new DamageSpell("Теневой удар", "Наносит 9 урона врагу.", 9)
        };
    }
}