﻿using DatabaseSchemaReader.Data;
using DatabaseSchemaReaderTest.IntegrationTests;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestContext = System.Object;
#endif

namespace DatabaseSchemaReaderTest.SqlGen.InsertWriterTests
{
    [TestClass]
    public class ScriptWriterTest
    {
        const string Providername = "System.Data.SqlClient";
        const string ConnectionString = ConnectionStrings.Northwind;


        [TestMethod]
        public void TestInsertIntegration()
        {
            //arrange
            var dbReader = TestHelper.GetNorthwindReader();
            var table = dbReader.Table("Customers");

            var rdr = new ScriptWriter();

            //act
            var txt = rdr.ReadTable(table, ConnectionString, Providername);

            //assert
            Assert.IsTrue(txt.Contains("INSERT INTO [Customers]"), "Insert statments created");
            Assert.IsTrue(txt.Contains("[CustomerID],  [CompanyName]"), "Insert names the columns");
            Assert.IsTrue(txt.Contains("'ALFKI'"), "Data includes Alfreds Futterkiste");
        }

        [TestMethod]
        public void TestInsertNoDirectDatabaseReaderIntegration()
        {
            //arrange
            ProviderChecker.Check(Providername, ConnectionString);

            var rdr = new ScriptWriter();

            //act
            var txt = rdr.ReadTable("Customers", ConnectionString, Providername);

            //assert
            Assert.IsTrue(txt.Contains("INSERT INTO [Customers]"), "Insert statments created");
            Assert.IsTrue(txt.Contains("[CustomerID],  [CompanyName]"), "Insert names the columns");
            Assert.IsTrue(txt.Contains("'ALFKI'"), "Data includes Alfreds Futterkiste");
        }

        [TestMethod]
        public void TestInsertWithDataReader()
        {
            //arrange
            var dbReader = TestHelper.GetNorthwindReader();
            dbReader.DataTypes(); //need the datatypes here, so this must be called before or after
            var table = dbReader.Table("Categories");

            var rdr = new ScriptWriter();

            string result = null;

            //act
            rdr.ReadTable(table, ConnectionString, Providername, insertString =>
            {
                result = insertString;
                return false; //only need one record, return
            });

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains("INSERT INTO [Categories]"), "Insert statments created");
            Assert.IsTrue(result.Contains("[CategoryName],"), "Insert names the columns");
        }
    }
}
