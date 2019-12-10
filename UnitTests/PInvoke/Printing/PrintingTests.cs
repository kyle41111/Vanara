﻿using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Linq;
using Vanara.Extensions;
using static Vanara.PInvoke.WinSpool;

namespace Vanara.PInvoke.Tests
{
	[TestFixture]
	public class PrintingTests
	{
		private const string connPtrName = "Foobar";
		private const string defKey = "PrinterDriverData";
		private static readonly string defaultPrinterName = new System.Drawing.Printing.PrinterSettings().PrinterName;
		private SafeHPRINTER hprnt;

		[OneTimeSetUp]
		public void _Setup() => Assert.That(OpenPrinter(defaultPrinterName, out hprnt), ResultIs.Successful);

		[OneTimeTearDown]
		public void _TearDown() => hprnt?.Dispose();

		[Test]
		public void AddPrinterTest()
		{
			const string key = "TestOnly";
			const string name = "TestOnlyPrinter";
			var pi = GetPrinter<PRINTER_INFO_2>(hprnt);
			var pi2 = new PRINTER_INFO_2
			{
				pPrinterName = name,
				pPortName = "LPT1:",
				pDriverName = pi.pDriverName,
				pPrintProcessor = pi.pPrintProcessor,
				Attributes = PRINTER_ATTRIBUTE.PRINTER_ATTRIBUTE_LOCAL
			};
			var p2 = new SafeHPRINTER(default, false);
			try
			{
				Assert.That(p2 = AddPrinter(null, 2, pi2), ResultIs.ValidHandle);
				GetSet("Test", 123, 123U);
				GetSet("Test", 123L, 123UL);
				GetSet("Test", "123");
				GetSet("Test", new byte[] { 1, 2, 3 });
				GetSet("Test", new[] { "1", "2", "3" });
				GetSet("Test", 123, 123U, REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN);
				// Test serializable
				var sz = new System.Drawing.Size(4, 4);
				GetSet("Test", sz, new byte[] { 4, 0, 0, 0, 4, 0, 0, 0 });

				Assert.That(() => SetPrinterData(p2, "Test8", 1, REG_VALUE_TYPE.REG_LINK), Throws.Exception);
				Assert.That(() => SetPrinterData(p2, "Test8", 1, REG_VALUE_TYPE.REG_RESOURCE_LIST), Throws.Exception);
				Assert.That(ResetPrinter(p2, new PRINTER_DEFAULTS { pDatatype = pi.pDatatype }), ResultIs.Successful);
			}
			finally
			{
				Assert.That(DeletePrinter(p2), ResultIs.Successful);
			}

			void GetSet(string vn, object v, object r = null, REG_VALUE_TYPE t = REG_VALUE_TYPE.REG_NONE)
			{
				if (r is null) r = v;
				Assert.That(SetPrinterData(p2, vn, v, t), ResultIs.Successful);
				Assert.That(GetPrinterData(p2, vn), v.GetType().IsArray ? (IResolveConstraint)Is.EquivalentTo((IEnumerable)r) : Is.EqualTo(r));
				Assert.That(DeletePrinterData(p2, vn), ResultIs.Successful);
				Assert.That(SetPrinterDataEx(p2, key, vn, v, t), ResultIs.Successful);
				Assert.That(GetPrinterDataEx(p2, key, vn), v.GetType().IsArray ? (IResolveConstraint)Is.EquivalentTo((IEnumerable)r) : Is.EqualTo(r));
				Assert.That(DeletePrinterDataEx(p2, key, vn), ResultIs.Successful);
				Assert.That(DeletePrinterKey(p2, key), ResultIs.Successful);
			}
		}

		[Test]
		public void AddPrinterConnectionTest()
		{
			Assert.That(AddPrinterConnection(connPtrName), ResultIs.Successful);
			Assert.That(DeletePrinterConnection(connPtrName), ResultIs.Successful);
		}

		[Test]
		public void AddPrinterConnection2Test()
		{
			var drv = GetPrinter<PRINTER_INFO_2>(hprnt).pDriverName;
			Assert.That(AddPrinterConnection2(default, connPtrName, PRINTER_CONNECTION_FLAGS.PRINTER_CONNECTION_MISMATCH, drv), ResultIs.Successful);
			Assert.That(DeletePrinterConnection(connPtrName), ResultIs.Successful);
		}

		[Test]
		public void AdvancedDocumentPropertiesTest()
		{
			var devmodeOut = DEVMODE.Default;
			Assert.That(AdvancedDocumentProperties(HWND.NULL, hprnt, defaultPrinterName, ref devmodeOut, DEVMODE.Default), ResultIs.Successful);
			Assert.That(AdvancedDocumentProperties(HWND.NULL, hprnt, defaultPrinterName), ResultIs.Successful);
		}

		[Test]
		public void ConnectToPrinterDlgTest()
		{
			SafeHPRINTER p;
			Assert.That(p = ConnectToPrinterDlg(HWND.NULL), ResultIs.ValidHandle);
			p.Dispose();
		}

		[Test]
		public void EnumFormsTest()
		{
			FORM_INFO_1[] res1;
			Assert.That(res1 = EnumForms<FORM_INFO_1>(hprnt).ToArray(), Is.Not.Empty);
			TestContext.WriteLine(string.Join(",", res1.Select(v => v.pName)));
			FORM_INFO_2[] res2;
			Assert.That(res2 = EnumForms<FORM_INFO_2>(hprnt).ToArray(), Is.Not.Empty);
			TestContext.WriteLine(string.Join(",", res2.Select(v => v.Flags)));
		}

		[Test]
		public void EnumJobsTest()
		{
			Assert.That(EnumJobs<JOB_INFO_1>(hprnt), Is.Empty);
			Assert.That(EnumJobs<JOB_INFO_2>(hprnt), Is.Empty);
			Assert.That(EnumJobs<JOB_INFO_3>(hprnt), Is.Empty);
			Assert.That(EnumJobs<JOB_INFO_4>(hprnt), Is.Empty);
		}

		[Test]
		public void EnumPrinterDataExTest()
		{
			var res1 = EnumPrinterDataEx(hprnt, defKey);
			Assert.That(res1, Is.Not.Empty);
			TestContext.WriteLine(string.Join(",", res1.Select(v => $"{v.valueName}={v.value} ({v.valueType})")));
		}

		[Test]
		public void EnumPrinterDataTest()
		{
			var res1 = EnumPrinterData(hprnt);
			Assert.That(res1, Is.Not.Empty);
			TestContext.WriteLine(string.Join(",", res1.Select(v => $"{v.valueName}={v.value} ({v.valueType})")));
		}

		[Test]
		public void EnumPrinterKeyTest()
		{
			string[] res1;
			Assert.That(res1 = EnumPrinterKey(hprnt, "").ToArray(), Is.Not.Empty);
			TestContext.WriteLine(string.Join(",", res1));
		}

		[Test]
		public void EnumPrintersTest()
		{
			PRINTER_INFO_1[] res1;
			Assert.That(res1 = EnumPrinters<PRINTER_INFO_1>().ToArray(), Is.Not.Empty);
			TestContext.WriteLine(string.Join(",", res1.Select(v => v.pName)));
			PRINTER_INFO_2[] res2;
			Assert.That(res2 = EnumPrinters<PRINTER_INFO_2>().ToArray(), Is.Not.Empty);
			TestContext.WriteLine(string.Join(",", res2.Select(v => v.Status)));
			//PRINTER_INFO_3[] res3;
			//Assert.That(res3 = EnumPrinters<PRINTER_INFO_3>().ToArray(), Is.Not.Empty);
			PRINTER_INFO_4[] res4;
			Assert.That(res4 = EnumPrinters<PRINTER_INFO_4>().ToArray(), Is.Not.Empty);
			TestContext.WriteLine(string.Join(",", res4.Select(v => v.Attributes)));
			PRINTER_INFO_5[] res5;
			Assert.That(res5 = EnumPrinters<PRINTER_INFO_5>().ToArray(), Is.Not.Empty);
			TestContext.WriteLine(string.Join(",", res5.Select(v => v.pPortName)));
			//PRINTER_INFO_6[] res6;
			//Assert.That(res6 = EnumPrinters<PRINTER_INFO_6>().ToArray(), Is.Not.Empty);
			//PRINTER_INFO_7[] res7;
			//Assert.That(res7 = EnumPrinters<PRINTER_INFO_7>().ToArray(), Is.Not.Empty);
			//PRINTER_INFO_8[] res8;
			//Assert.That(res8 = EnumPrinters<PRINTER_INFO_8>().ToArray(), Is.Not.Empty);
			//PRINTER_INFO_9[] res9;
			//Assert.That(res9 = EnumPrinters<PRINTER_INFO_9>().ToArray(), Is.Not.Empty);
		}

		[Test]
		public void FormTest()
		{
			const string name = "TestOnlyForm";
			var form1 = EnumForms<FORM_INFO_1>(hprnt).First();
			form1.pName = name;
			form1.Flags = FormFlags.FORM_USER;
			Assert.That(AddForm(hprnt, form1), ResultIs.Successful);
			try
			{
				FORM_INFO_2 fi2 = default;
				Assert.That(() => fi2 = GetForm<FORM_INFO_2>(hprnt, name), Throws.Nothing);
				Assert.That(fi2.Flags, Is.EqualTo(FormFlags.FORM_USER));
				TestHelper.WriteValues(fi2);

				form1.Size = new SIZE(form1.Size.cx / 2, form1.Size.cy / 2);
				Assert.That(SetForm(hprnt, name, form1), ResultIs.Successful);
			}
			finally
			{
				Assert.That(DeleteForm(hprnt, name), ResultIs.Successful);
			}
		}

		[Test]
		public void JobTest()
		{
			Assert.That(AddJob(hprnt, out var path, out var id), ResultIs.Successful);
			try
			{
				System.IO.File.WriteAllText(path, "Test page.");

				JOB_INFO_2 ji2 = default;
				Assert.That(() => ji2 = GetJob<JOB_INFO_2>(hprnt, id), Throws.Nothing);
				Assert.That(ji2.JobId, Is.EqualTo(id));
				TestHelper.WriteValues(ji2);

				var jobInfo = new JOB_INFO_1 { JobId = id, Priority = JOB_PRIORITY.MAX_PRIORITY, Status = ji2.Status, pDatatype = ji2.pDatatype };
				Assert.That(SetJob(hprnt, id, jobInfo), ResultIs.Successful);

				Assert.That(ScheduleJob(hprnt, id), ResultIs.Successful);
			}
			finally
			{
				Assert.That(SetJob(hprnt, id, JOB_CONTROL.JOB_CONTROL_DELETE), ResultIs.Successful);
			}
		}

		[Test]
		public void PortTest()
		{
			var port = GetPrinter<PRINTER_INFO_2>(hprnt).pPortName;

			Assert.That(ConfigurePort(null, HWND.NULL, port), ResultIs.Successful);

			Assert.That(SetPort(null, port, PORT_STATUS.PORT_STATUS_OFFLINE, PORT_STATUS_TYPE.PORT_STATUS_TYPE_ERROR), ResultIs.Successful);
			Assert.That(SetPort(null, port, "Off-line", PORT_STATUS_TYPE.PORT_STATUS_TYPE_ERROR), ResultIs.Successful);
			Assert.That(SetPort(null, port, 0, 0), ResultIs.Successful);
		}

		[Test]
		public void SpoolFileTest()
		{
			var hspf = GetSpoolFileHandle(hprnt);
			Assert.That(hspf, ResultIs.ValidHandle);
			var bytes = new byte[] { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 };
			Kernel32.WriteFile(hspf, bytes, (uint)bytes.Length, out _);
			Assert.That(CommitSpoolData(hprnt, hspf, (uint)bytes.Length), ResultIs.Successful);
			Assert.That(() => hspf.Dispose(), Throws.Nothing);
		}
	}
}