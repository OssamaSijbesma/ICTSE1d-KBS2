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
        public void SearchTitle_SearchTitleTestSheet_FoundAllWithTitleAsTestSheet()
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

        //Depends on database
        [TestMethod]
        public void Search_SearchAll_AllFromTable()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.Search(null,null,null,0,0);

            //Assert
            Assert.IsTrue(result.Count > 0);
        }

        //Depends on database
        [TestMethod]
        public void Search_UseExtraSpecific_TestSheet1()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.Search("Id,Title,File", "Id", "1", 0, 0);

            //Assert
            Assert.IsTrue(result.Count == 1);
        }

        //Depends on database
        [TestMethod]
        public void Search_UseOffset_TestSheet2()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.Search(null, null, null, 1, 1);

            //Assert
            Assert.AreEqual(2, result[0].Id);
            Assert.AreEqual("TestSheet", result[0].Title);
            Assert.AreEqual("c//TestSheet2", result[0].Path);
        }

        //Depends on database
        [TestMethod]
        public void Search_WrongWhereBInput_NullResult()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.Search(null, "Title", "shfhw", 0, 0);

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        //Depends on database
        [TestMethod]
        public void Search_WrongWhereAInput_NullResult()
        {
            //Arange
            var db = new PiaNotes.Databaser();

            //Act
            var result = db.Search(null, "Tittttle", "Testsheet", 0, 0);

            //Assert
            Assert.AreEqual(null, result);
        }
    }
}