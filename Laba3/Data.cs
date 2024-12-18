using System;
using System.Collections.Generic;
using Classes;

public class Data
{
    public static List<CreatureCard>[] CreateCreatureDecks()
    {
        return new List<CreatureCard>[]
        {
            CreateDeck1(),
            CreateDeck2(),
            CreateDeck3(),
            CreateDeck4()
        };
    }

    public static List<Spell>[] CreateSpellDecks()
    {
        return new List<Spell>[]
        {
            CreateSpellsDeck1(),
            CreateSpellsDeck2(),
            CreateSpellsDeck3(),
            CreateSpellsDeck4()
        };
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