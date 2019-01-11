using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Storage;

namespace PianNotes.UnitTests
{
    [TestClass]
    public class DatabaserTests
    {
        [TestMethod]
        public void CheckConnection_AttemptConnection_IsOpen()
        {
            //Arange
            var db = new PiaNotes.ViewModels.Databaser();

            //Act
            var result = db.CheckConnection();

            //Assert
            Assert.IsTrue(result);
        }

        //Depends on database
        [TestMethod]
        public void Search_SearchAll_AllFromTable()
        {
            //Arange
            var db = new PiaNotes.ViewModels.Databaser();

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
            var db = new PiaNotes.ViewModels.Databaser();

            //Act
            var result = db.Search("Id,Title,FileName", "Id", "1", 0, 0);

            //Assert
            Assert.IsTrue(result.Count == 1);
        }

        //Depends on database
        [TestMethod]
        public void Search_UseOffset_TestSheet2()
        {
            //Arange
            var db = new PiaNotes.ViewModels.Databaser();

            //Act
            var result = db.Search(null, null, null, 1, 1);

            //Assert
            Assert.IsTrue(result.Count == 1);
        }

        //Depends on database
        [TestMethod]
        public void Search_WrongWhereBInput_NullResult()
        {
            //Arange
            var db = new PiaNotes.ViewModels.Databaser();

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
            var db = new PiaNotes.ViewModels.Databaser();

            //Act
            var result = db.Search(null, "Tittttle", "Testsheet", 0, 0);

            //Assert
            Assert.AreEqual(null, result);
        }

        //Depends on database
        [TestMethod]
        public void GetAFileAsync_SearchId1_IsStorageFile()
        {
            //Arange
            var db = new PiaNotes.ViewModels.Databaser();

            //Act
            var result = db.GetAFileAsync(1);

            //Assert
            Assert.IsInstanceOfType(result, typeof(Task<StorageFile>));
        }
    }
}