﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvHelper.Tests.DataTableTests
{
	[TestClass]
    public class CsvDataReaderTests
    {
		[TestMethod]
        public void GetValuesTest()
		{
			var s = new StringBuilder();
			s.AppendLine("Boolean,Byte,Bytes,Char,Chars,DateTime,Decimal,Double,Float,Guid,Short,Int,Long,Null");
			s.AppendLine("true,1,0x0102,a,ab,1/1/2019,1.23,4.56,7.89,eca0c8c6-9a2a-4e6c-8599-3561abda13f1,1,2,3,");
			using (var reader = new StringReader(s.ToString()))
			using (var csv = new CsvReader(reader))
			{
				csv.Configuration.Delimiter = ",";
				var dataReader = new CsvDataReader(csv);
				dataReader.Read();

				Assert.AreEqual(true, dataReader.GetBoolean(0));
				Assert.AreEqual(1, dataReader.GetByte(1));

				byte[] byteBuffer = new byte[2];
				dataReader.GetBytes(2, 0, byteBuffer, 0, byteBuffer.Length);
				Assert.AreEqual(0x1, byteBuffer[0]);
				Assert.AreEqual(0x2, byteBuffer[1]);

				Assert.AreEqual('a', dataReader.GetChar(3));

				char[] charBuffer = new char[2];
				dataReader.GetChars(4, 0, charBuffer, 0, charBuffer.Length);
				Assert.AreEqual('a', charBuffer[0]);
				Assert.AreEqual('b', charBuffer[1]);

				Assert.IsNull(dataReader.GetData(0));
				Assert.AreEqual(DateTime.Parse("1/1/2019"), dataReader.GetDateTime(5));
				Assert.AreEqual(typeof(string).Name, dataReader.GetDataTypeName(0));
				Assert.AreEqual(1.23m, dataReader.GetDecimal(6));
				Assert.AreEqual(4.56d, dataReader.GetDouble(7));
				Assert.AreEqual(typeof(string), dataReader.GetFieldType(0));
				Assert.AreEqual(7.89f, dataReader.GetFloat(8));
				Assert.AreEqual(Guid.Parse("eca0c8c6-9a2a-4e6c-8599-3561abda13f1"), dataReader.GetGuid(9));
				Assert.AreEqual(1, dataReader.GetInt16(10));
				Assert.AreEqual(2, dataReader.GetInt32(11));
				Assert.AreEqual(3, dataReader.GetInt64(12));
				Assert.AreEqual("Boolean", dataReader.GetName(0));
				Assert.AreEqual(0, dataReader.GetOrdinal("Boolean"));

				Assert.AreEqual("true", dataReader.GetString(0));
				Assert.AreEqual("true", dataReader.GetValue(0));

				var objectBuffer = new object[14];
				dataReader.GetValues(objectBuffer);
				Assert.AreEqual("true", objectBuffer[0]);
				Assert.AreEqual("", objectBuffer[13]);
				Assert.IsTrue(dataReader.IsDBNull(13));
			}
		}

		[TestMethod]
		public void GetSchemaTableTest()
		{
			var s = new StringBuilder();
			s.AppendLine("Id,Name");
			s.AppendLine("1,one");
			s.AppendLine("2,two");
			using (var reader = new StringReader(s.ToString()))
			using (var csv = new CsvReader(reader))
			{
				csv.Configuration.Delimiter = ",";
				var dataReader = new CsvDataReader(csv);

				var schemaTable = dataReader.GetSchemaTable();
				Assert.AreEqual(25, schemaTable.Columns.Count);
				Assert.AreEqual(2, schemaTable.Rows.Count);
			}
		}

		[TestMethod]
		public void DataTableLoadTest()
		{
			var s = new StringBuilder();
			s.AppendLine("Id,Name");
			s.AppendLine("1,one");
			s.AppendLine("2,two");
			using (var reader = new StringReader(s.ToString()))
			using (var csv = new CsvReader(reader))
			{
				csv.Configuration.Delimiter = ",";
				var dataReader = new CsvDataReader(csv);

				var dataTable = new DataTable();
				dataTable.Columns.Add("Id", typeof(int));
				dataTable.Columns.Add("Name", typeof(string));

				dataTable.Load(dataReader);

				Assert.AreEqual(2, dataTable.Rows.Count);
				Assert.AreEqual(1, dataTable.Rows[0]["Id"]);
				Assert.AreEqual("one", dataTable.Rows[0]["Name"]);
				Assert.AreEqual(2, dataTable.Rows[1]["Id"]);
				Assert.AreEqual("two", dataTable.Rows[1]["Name"]);
			}
		}

		[TestMethod]
		public void DataTableLoadNoHeaderTest()
		{
			var s = new StringBuilder();
			s.AppendLine("1,one");
			s.AppendLine("2,two");
			using (var reader = new StringReader(s.ToString()))
			using (var csv = new CsvReader(reader))
			{
				csv.Configuration.HasHeaderRecord = false;
				csv.Configuration.Delimiter = ",";
				var dataReader = new CsvDataReader(csv);

				var dataTable = new DataTable();

				dataTable.Load(dataReader);

				Assert.AreEqual(0, dataTable.Rows.Count);
			}
		}
	}
}
