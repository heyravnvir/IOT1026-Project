﻿using System.Reflection;
using MinotaurLabyrinth;
using Moq;
using Moq.Protected;

namespace MinotaurLabyrinthTest
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void DummyTest()
        {
            Assert.AreNotSame(1, 2);
        }


[TestMethod]
        public void Test_Activate_LostGame()
        {
            var mock = new Mock<Toxic>() { CallBase = true }; //need to set Callbase = true, then can call the override/virtual method
            mock.Protected().Setup<int>("Dice3Dices").Returns(11);
            mock.Protected().Setup<bool>("GetUserSelectOption").Returns(true);
            var toxic = mock.Object;

            (Map map, Hero hero) = LabyrinthCreator.InitializeMap(Size.Small);
            toxic.Activate(hero, map);

            mock.Protected().Verify("HandleLostGame", Times.Once(), new object[] { hero });
            Assert.AreEqual(hero.IsPoisoned, true);
        }


    }
}
