using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PianNotes.UnitTests
{
    [TestClass]
    public class DatabaserTests
    {
        [TestMethod]
        public void CheckConnection_AttemptConnection_IsOpen()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.CheckConnection();

            //Assert
            Assert.IsTrue(result);
        }


        //Depends on database with TestSheet: Id = 1, Title = TestSheet, Path = C//TestPath
        [TestMethod]
        public void SearchId_SearchId1_IsTestSheet1()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.SearchId(1);

            //Assert
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("TestSheet", result.Title);
            Assert.AreEqual("C//TestPath", result.Path);
        }

        //Depends on database with TestSheet: Id = 1, Title = TestSheet, Path = C//TestPath
        [TestMethod]
        public void SearchId_SearchIdThatDoesNotExist_IsNull()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.SearchId(9999999);

            //Assert
            Assert.AreEqual(null, result);
        }

        //Depends on database with named TestSheet
        [TestMethod]
        public void SearchTitle_SearchTitleTestSheet_FoundTestSheets()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.SearchTitle("TestSheet");

            //Assert
            Assert.AreNotEqual(null, result);
        }

        //Depends on database with named TestSheet
        [TestMethod]
        public void SearchTitle_SearchTitleThatDoesNotExist_IsNull()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.SearchTitle("TestShewegwggdset");

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        //Depends on database
        [TestMethod]
        public void SearchNumber_Skip1Pick1_1ResultId2()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.SearchNumber(1,1);

            //Assert
            Assert.AreEqual(1, result.Count);
        }
    }
}
