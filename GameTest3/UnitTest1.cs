using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

[TestClass]
public class GameTests
{
    private Player player1;
    private Player player2;
    private List<CreatureCard> deck1;
    private List<Spell> spells1;

    [TestInitialize]
    public void Setup()
    {
        // Инициализация игроков и колод
        player1 = new Player();
        player2 = new Player();

        deck1 = new List<CreatureCard>
        {
            new CreatureCard("Гоблин", "Маленький, но хитрый.", 2, 3),
            new CreatureCard("Огур", "Большой и сильный.", 5, 7)
        };

        spells1 = new List<Spell>
        {
            new HealingSpell("Исцеление", "Восстанавливает 5 здоровья цели.", 5),
            new DamageSpell("Огненный шар", "Наносит 7 урона врагу.", 7)
        };

        // Разыгрываем карты
        player1.Creatures.Add(deck1[0]);
        player1.Creatures.Add(deck1[1]);
    }

    [TestMethod]
    public void TestHealingSpellOnPlayer()
    {
        // Установим здоровье игрока на 10
        player1.Health = 10;

        // Применяем заклинание исцеления
        HealingSpell healingSpell = (HealingSpell)spells1[0];
        healingSpell.Cast(player1);

        // Проверяем, что здоровье увеличилось
        Assert.AreEqual(15, player1.Health, "Здоровье игрока должно быть 15 после исцеления.");
    }

    [TestMethod]
    public void TestHealingSpellOnCreature()
    {
        // Установим здоровье существа на 2
        player1.Creatures[0].Health = 2;

        // Применяем заклинание исцеления на существо
        HealingSpell healingSpell = (HealingSpell)spells1[0];
        healingSpell.Cast(player1, player1.Creatures[0]);

        // Проверяем, что здоровье существа увеличилось
        Assert.AreEqual(7, player1.Creatures[0].Health, "Здоровье существа должно быть 7 после исцеления.");
    }

    [TestMethod]
    public void TestDamageSpellOnOpponent()
    {
        // Установим здоровье противника на 15
        player2.Health = 15;

        // Применяем заклинание урона
        DamageSpell damageSpell = (DamageSpell)spells1[1];
        damageSpell.Cast(player2);

        // Проверяем, что здоровье противника уменьшилось
        Assert.AreEqual(8, player2.Health, "Здоровье противника должно быть 8 после применения заклинания урона.");
    }

    

    [TestMethod]
    public void TestCreatureCardPlay()
    {
        // Проверяем, что у игрока 2 существа
        Assert.AreEqual(2, player1.Creatures.Count, "У игрока должно быть 2 существа на поле.");
    }

    [TestMethod]
    public void TestCreatureHealthAfterDamage()
    {
        // Установим здоровье существа на 5
        player1.Creatures[0].Health = 5;

        // Наносим урон существу
        DamageSpell damageSpell = new DamageSpell("Удар", "Наносит 5 урона.", 5);
        damageSpell.Cast(player1, player1.Creatures[0]);

        // Проверяем, что здоровье существа стало 0
        Assert.AreEqual(0, player1.Creatures[0].Health, "Здоровье существа должно быть 0 после получения урона.");
    }

    [TestMethod]
    public void TestGameOverCondition()
    {
        // Установим здоровье обоих игроков на 0
        player1.Health = 0;
        player2.Health = 0;

        // Проверяем, что игра завершена
        Assert.IsTrue(IsGameOver(player1, player2), "Игра должна быть завершена, если здоровье обоих игроков равно 0.");
    }

    [TestMethod]
    public void TestGameOverWhenPlayer1Loses()
    {
        // Установим здоровье игрока 1 на 0
        player1.Health = 0;
        player2.Health = 10;

        // Проверяем, что игра завершена
        Assert.IsTrue(IsGameOver(player1, player2), "Игра должна быть завершена, если здоровье игрока 1 равно 0.");
    }

    [TestMethod]
    public void TestGameOverWhenPlayer2Loses()
    {
        // Установим здоровье игрока 2 на 0
        player1.Health = 10;
        player2.Health = 0;

        // Проверяем, что игра завершена
        Assert.IsTrue(IsGameOver(player1, player2), "Игра должна быть завершена, если здоровье игрока 2 равно 0.");
    }

    private bool IsGameOver(Player player1, Player player2)
    {
        return player1.Health <= 0 || player2.Health <= 0;
    }
}