using System;
using System.Collections.Generic;

namespace Classes
{
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
}