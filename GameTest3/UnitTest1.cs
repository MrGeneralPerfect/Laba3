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
        // ������������� ������� � �����
        player1 = new Player();
        player2 = new Player();

        deck1 = new List<CreatureCard>
        {
            new CreatureCard("������", "���������, �� ������.", 2, 3),
            new CreatureCard("����", "������� � �������.", 5, 7)
        };

        spells1 = new List<Spell>
        {
            new HealingSpell("���������", "��������������� 5 �������� ����.", 5),
            new DamageSpell("�������� ���", "������� 7 ����� �����.", 7)
        };

        // ����������� �����
        player1.Creatures.Add(deck1[0]);
        player1.Creatures.Add(deck1[1]);
    }

    [TestMethod]
    public void TestHealingSpellOnPlayer()
    {
        // ��������� �������� ������ �� 10
        player1.Health = 10;

        // ��������� ���������� ���������
        HealingSpell healingSpell = (HealingSpell)spells1[0];
        healingSpell.Cast(player1);

        // ���������, ��� �������� �����������
        Assert.AreEqual(15, player1.Health, "�������� ������ ������ ���� 15 ����� ���������.");
    }

    [TestMethod]
    public void TestHealingSpellOnCreature()
    {
        // ��������� �������� �������� �� 2
        player1.Creatures[0].Health = 2;

        // ��������� ���������� ��������� �� ��������
        HealingSpell healingSpell = (HealingSpell)spells1[0];
        healingSpell.Cast(player1, player1.Creatures[0]);

        // ���������, ��� �������� �������� �����������
        Assert.AreEqual(7, player1.Creatures[0].Health, "�������� �������� ������ ���� 7 ����� ���������.");
    }

    [TestMethod]
    public void TestDamageSpellOnOpponent()
    {
        // ��������� �������� ���������� �� 15
        player2.Health = 15;

        // ��������� ���������� �����
        DamageSpell damageSpell = (DamageSpell)spells1[1];
        damageSpell.Cast(player2);

        // ���������, ��� �������� ���������� �����������
        Assert.AreEqual(8, player2.Health, "�������� ���������� ������ ���� 8 ����� ���������� ���������� �����.");
    }

    

    [TestMethod]
    public void TestCreatureCardPlay()
    {
        // ���������, ��� � ������ 2 ��������
        Assert.AreEqual(2, player1.Creatures.Count, "� ������ ������ ���� 2 �������� �� ����.");
    }

    [TestMethod]
    public void TestCreatureHealthAfterDamage()
    {
        // ��������� �������� �������� �� 5
        player1.Creatures[0].Health = 5;

        // ������� ���� ��������
        DamageSpell damageSpell = new DamageSpell("����", "������� 5 �����.", 5);
        damageSpell.Cast(player1, player1.Creatures[0]);

        // ���������, ��� �������� �������� ����� 0
        Assert.AreEqual(0, player1.Creatures[0].Health, "�������� �������� ������ ���� 0 ����� ��������� �����.");
    }

    [TestMethod]
    public void TestGameOverCondition()
    {
        // ��������� �������� ����� ������� �� 0
        player1.Health = 0;
        player2.Health = 0;

        // ���������, ��� ���� ���������
        Assert.IsTrue(IsGameOver(player1, player2), "���� ������ ���� ���������, ���� �������� ����� ������� ����� 0.");
    }

    [TestMethod]
    public void TestGameOverWhenPlayer1Loses()
    {
        // ��������� �������� ������ 1 �� 0
        player1.Health = 0;
        player2.Health = 10;

        // ���������, ��� ���� ���������
        Assert.IsTrue(IsGameOver(player1, player2), "���� ������ ���� ���������, ���� �������� ������ 1 ����� 0.");
    }

    [TestMethod]
    public void TestGameOverWhenPlayer2Loses()
    {
        // ��������� �������� ������ 2 �� 0
        player1.Health = 10;
        player2.Health = 0;

        // ���������, ��� ���� ���������
        Assert.IsTrue(IsGameOver(player1, player2), "���� ������ ���� ���������, ���� �������� ������ 2 ����� 0.");
    }

    private bool IsGameOver(Player player1, Player player2)
    {
        return player1.Health <= 0 || player2.Health <= 0;
    }
}