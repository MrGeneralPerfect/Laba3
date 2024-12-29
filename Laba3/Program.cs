using Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

public class Program
{
    private static int player1DeckIndex;
    private static int player2DeckIndex;
    private static List<CreatureCard>[] creatureDecks = new List<CreatureCard>[4];
    private static List<Spell>[] spellDecks = new List<Spell>[4];
    private static Player player1;
    private static Player player2;

    // Списки для хранения разыгранных и умерших существ
    private static List<CreatureCard> player1PlayedCreatures = new List<CreatureCard>();
    private static List<CreatureCard> player2PlayedCreatures = new List<CreatureCard>();
    private static List<CreatureCard> player1DeadCreatures = new List<CreatureCard>();
    private static List<CreatureCard> player2DeadCreatures = new List<CreatureCard>();
    // Переменные для колод и заклинаний
    private static List<CreatureCard> player1Deck;
    private static List<CreatureCard> player2Deck;
    private static List<Spell> player1Spells;
    private static List<Spell> player2Spells;

    // Список всех доступных заклинаний
    private static List<Spell> allSpells = new List<Spell>
    {
        new HealingSpell("Исцеление", "Восстанавливает 5 здоровья цели.", 5),
        new DamageSpell("Огненный шар", "Наносит 7 урона врагу.", 7),
        new HealingSpell("Лечение", "Восстанавливает 4 здоровья союзнику.", 4),
        new DamageSpell("Молния", "Наносит 6 урона врагу.", 6),
        new HealingSpell("Светлое исцеление", "Восстанавливает 8 здоровья цели.", 8),
        new DamageSpell("Ледяной шторм", "Наносит 5 урона врагу и замедляет его.", 5),
        new HealingSpell("Божественное исцеление", "Восстанавливает 4 здоровья цели.", 4),
        new DamageSpell("Теневой удар", "Наносит 7 урона врагу.", 7)
    };

    public static void Main(string[] args)
    {
        // Вывод правил игры
        DisplayGameRules();

        // Инициализация колод
        InitializeDecks();

        // Проверка наличия сохраненной игры
        if (File.Exists("SaveStats.txt"))
        {
            Console.WriteLine("Найдена сохраненная игра. Хотите продолжить? (y/n)");
            string choice = Console.ReadLine();
            if (choice.ToLower() == "y")
            {
                player1 = new Player();
                player2 = new Player();
                LoadGame();
                Console.WriteLine("Игра загружена. Начинаем игровой цикл...");
                StartGameLoop();
                return; // Завершаем выполнение Main, так как игра уже загружена
            }
        }

        // Выбор колод игроками
        player1DeckIndex = ChooseDeck(1);
        player2DeckIndex = ChooseDeck(2);

        // Проверка на одинаковые колоды
        while (player1DeckIndex == player2DeckIndex)
        {
            Console.WriteLine("Игроки не могут выбрать одинаковые колоды. Пожалуйста, выберите другую колоду для игрока 2.");
            player2DeckIndex = ChooseDeck(2);
        }

        // Получение колод для игроков
        player1Deck = creatureDecks[player1DeckIndex];
        player2Deck = creatureDecks[player2DeckIndex];
        player1Spells = spellDecks[player1DeckIndex];
        player2Spells = spellDecks[player2DeckIndex];

        // Создание игроков
        player1 = new Player();
        player2 = new Player();

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

            // Сохранение состояния игры после каждого хода
            SaveGame();

            // Проверка условий окончания игры
            if (IsGameOver(player1, player2))
            {
                gameRunning = false; // Остановить игру, если условия выполнены
            }
        }

        Console.WriteLine("Игра окончена!");
    }

    private static void SaveGame()
    {
        using (StreamWriter writer = new StreamWriter("SaveStats.txt"))
        {
            writer.WriteLine("/Игрок 1");
            writer.WriteLine(player1.Health);
            writer.WriteLine(string.Join(" ", player1.Creatures.ConvertAll(c => c.Name)));
            writer.WriteLine(string.Join(" ", player1PlayedCreatures.ConvertAll(c => c.Name)));
            writer.WriteLine(string.Join(" ", player1DeadCreatures.ConvertAll(c => c.Name)));

            writer.WriteLine("/Игрок 2");
            writer.WriteLine(player2.Health);
            writer.WriteLine(string.Join(" ", player2.Creatures.ConvertAll(c => c.Name)));
            writer.WriteLine(string.Join(" ", player2PlayedCreatures.ConvertAll(c => c.Name)));
            writer.WriteLine(string.Join(" ", player2DeadCreatures.ConvertAll(c => c.Name)));
        }

        // Сохранение индексов колод в SaveTurn.txt
        using (StreamWriter writer = new StreamWriter("SaveTurn.txt"))
        {
            writer.WriteLine(player1DeckIndex);
            writer.WriteLine(player2DeckIndex);
        }
    }


    private static void LoadGame()
    {
        using (StreamReader reader = new StreamReader("SaveStats.txt"))
        {
            string line;
            if ((line = reader.ReadLine()) != null && line == "/Игрок 1")
            {
                player1.Health = int.Parse(reader.ReadLine());
                string creaturesLine = reader.ReadLine();
                if (!string.IsNullOrEmpty(creaturesLine))
                {
                    string[] creatures = creaturesLine.Split(' ');
                    foreach (var creatureName in creatures)
                    {
                        CreatureCard creature = FindCreatureByName(creatureName);
                        if (creature != null)
                        {
                            player1.Creatures.Add(creature);
                        }
                    }
                }
                Console.WriteLine("Игрок 1 загружен: Здоровье = " + player1.Health);

                // Загружаем разыгранные карты
                string playedCreaturesLine = reader.ReadLine();
                if (!string.IsNullOrEmpty(playedCreaturesLine))
                {
                    string[] playedCreatures = playedCreaturesLine.Split(' ');
                    foreach (var creatureName in playedCreatures)
                    {
                        CreatureCard creature = FindCreatureByName(creatureName);
                        if (creature != null)
                        {
                            player1PlayedCreatures.Add(creature);
                        }
                    }
                }

                // Загружаем умершие карты
                string deadCreaturesLine = reader.ReadLine();
                if (!string.IsNullOrEmpty(deadCreaturesLine))
                {
                    string[] deadCreatures = deadCreaturesLine.Split(' ');
                    foreach (var creatureName in deadCreatures)
                    {
                        CreatureCard creature = FindCreatureByName(creatureName);
                        if (creature != null)
                        {
                            player1DeadCreatures.Add(creature);
                        }
                    }
                }
            }

            if ((line = reader.ReadLine()) != null && line == "/Игрок 2")
            {
                player2.Health = int.Parse(reader.ReadLine());
                string creaturesLine = reader.ReadLine();
                if (!string.IsNullOrEmpty(creaturesLine))
                {
                    string[] creatures = creaturesLine.Split(' ');
                    foreach (var creatureName in creatures)
                    {
                        CreatureCard creature = FindCreatureByName(creatureName);
                        if (creature != null)
                        {
                            player2.Creatures.Add(creature);
                        }
                    }
                }
                Console.WriteLine("Игрок 2 загружен: Здоровье = " + player2.Health);

                // Загружаем разыгранные карты
                string playedCreaturesLine = reader.ReadLine();
                if (!string.IsNullOrEmpty(playedCreaturesLine))
                {
                    string[] playedCreatures = playedCreaturesLine.Split(' ');
                    foreach (var creatureName in playedCreatures)
                    {
                        CreatureCard creature = FindCreatureByName(creatureName);
                        if (creature != null)
                        {
                            player2PlayedCreatures.Add(creature);
                        }
                    }
                }

                // Загружаем умершие карты
                string deadCreaturesLine = reader.ReadLine();
                if (!string.IsNullOrEmpty(deadCreaturesLine))
                {
                    string[] deadCreatures = deadCreaturesLine.Split(' ');
                    foreach (var creatureName in deadCreatures)
                    {
                        CreatureCard creature = FindCreatureByName(creatureName);
                        if (creature != null)
                        {
                            player2DeadCreatures.Add(creature);
                        }
                    }
                }
            }
        }

        // Загрузка индексов колод из SaveTurn.txt
        using (StreamReader turnReader = new StreamReader("SaveTurn.txt"))
        {
            player1DeckIndex = int.Parse(turnReader.ReadLine());
            player2DeckIndex = int.Parse(turnReader.ReadLine());
            player1Deck = creatureDecks[player1DeckIndex];
            player1Spells = spellDecks[player1DeckIndex];
            player2Deck = creatureDecks[player2DeckIndex];
            player2Spells = spellDecks[player2DeckIndex];

            Console.WriteLine("Колода игрока 1 загружена: " + string.Join(", ", player1Deck.Select(c => c.Name)));
            Console.WriteLine("Заклинания игрока 1 загружены: " + string.Join(", ", player1Spells.Select(s => s.Name)));
            Console.WriteLine("Колода игрока 2 загружена: " + string.Join(", ", player2Deck.Select(c => c.Name)));
            Console.WriteLine("Заклинания игрока 2 загружены: " + string.Join(", ", player2Spells.Select(s => s.Name)));
        }
    }

    private static List<Spell> LoadSpells(StreamReader reader)
    {
        List<Spell> spells = new List<Spell>();

        // Считываем заклинания из файла
        string spellsLine = reader.ReadLine();
        if (!string.IsNullOrEmpty(spellsLine))
        {
            string[] spellNames = spellsLine.Split(' ');
            foreach (var spellName in spellNames)
            {
                Spell spell = FindSpellByName(spellName);
                if (spell != null)
                {
                    spells.Add(spell);
                }
            }
        }

        return spells;
    }

    private static List<CreatureCard> LoadDeck(StreamReader reader)
    {
        List<CreatureCard> deck = new List<CreatureCard>();

        // Считываем колоду из файла
        string deckLine = reader.ReadLine();
        if (!string.IsNullOrEmpty(deckLine))
        {
            string[] creatureNames = deckLine.Split(' ');
            foreach (var creatureName in creatureNames)
            {
                CreatureCard creature = FindCreatureByName(creatureName);
                if (creature != null)
                {
                    deck.Add(creature);
                }
            }
        }

        return deck;
    }

    private static CreatureCard FindCreatureByName(string name)
    {
        foreach (var deck in creatureDecks)
        {
            foreach (var creature in deck)
            {
                if (creature.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                { // Возвращаем копию существа, чтобы не изменять оригинал в колоде
                  return new CreatureCard(creature.Name, creature.Description, creature.Attack, creature.Health);
                } 
            } 
        } return null; // Если существо не найдено
    }

    private static Spell FindSpellByName(string name)
    {

        return allSpells.FirstOrDefault(spell => spell.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    }

    private static void StartGameLoop()

    {

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


            // Сохранение состояния игры после каждого хода

            SaveGame();


            // Проверка условий окончания игры

            if (IsGameOver(player1, player2))

            {

                gameRunning = false; // Остановить игру, если условия выполнены

            }

        }


        Console.WriteLine("Игра окончена!");

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



    private static void DisplayGameRules()

    {

        Console.WriteLine("Правила игры:");

        Console.WriteLine("1. Главная задача игры уничтожить своего опонента.");

        Console.WriteLine("2. Вы можете в 1 ход выполнять одно из указанных действий: разыграть существо, применить заклинание, атаковать существом.");

        Console.WriteLine("   Если у вас закончились существа, либо на столе нет существ для атаки, ход переходит к оппоненту за невнимательность.");

        Console.WriteLine("3. Существа находятся в ограниченном количестве, и разыгрывать их нужно с умом.");

        Console.WriteLine("4. Игра заканчивается, когда у одного из игроков заканчивается здоровье.");

        Console.WriteLine();

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

    private static void PlayerTurn(Player currentPlayer, List<CreatureCard> playerDeck, List<Spell> playerSpells, Player opponent)
    {
        // Проверка на null
        if (playerDeck == null || playerSpells == null)
        {
            Console.WriteLine("Ошибка: колода или заклинания не инициализированы.");
            return;
        }

        Console.WriteLine($"Текущее здоровье игрока: {currentPlayer.Health}");
        Console.WriteLine("Выберите действие: 1 - Разыграть существо, 2 - Применить заклинание, 3 - Атаковать существом");

        int action;
        while (!int.TryParse(Console.ReadLine(), out action) || action < 1 || action > 3)
        {
            Console.WriteLine("Неверный выбор. Пожалуйста, выберите действие: 1 - Разыграть существо, 2 - Применить заклинание, 3 - Атаковать существом");
        }

        if (action == 1) // Разыграть существо
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
                Console.WriteLine($"{cardToPlay.Name} с атакой {cardToPlay.Attack} и здоровьем {cardToPlay.Health} разыграна.");
            }
            else
            {
                Console.WriteLine("Ваша колода пуста! Вы не можете разыграть существо.");
            }
        }
        else if (action == 2) // Применить заклинание
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
        else if (action == 3) // Атаковать существом
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
                Console.WriteLine("У вас нет существ для атаки!");
            }
        }
        else
        {
            Console.WriteLine("Неверный выбор действия.");
        }
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
        new HealingSpell("Божественное исцеление", "Восстанавливает 4 здоровья цели.", 4),
        new DamageSpell("Теневой удар", "Наносит 7 урона врагу.", 7)
    };
    }
}
