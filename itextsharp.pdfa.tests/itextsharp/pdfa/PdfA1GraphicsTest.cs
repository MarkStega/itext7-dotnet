using System;
using System.IO;
using NUnit.Framework;
using iTextSharp.Kernel.Color;
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Canvas;
using iTextSharp.Kernel.Pdf.Extgstate;
using iTextSharp.Kernel.Pdf.Xobject;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;

namespace iTextSharp.Pdfa
{
	public class PdfA1GraphicsTest : ExtendedITextTest
	{
		public static readonly String sourceFolder = TestContext.CurrentContext.TestDirectory
			 + "/../../resources/itextsharp/pdfa/";

		public static readonly String cmpFolder = sourceFolder + "cmp/PdfA1GraphicsTest/";

		public static readonly String destinationFolder = TestContext.CurrentContext.TestDirectory
			 + "/test/itextsharp/pdfa/PdfA1GraphicsTest/";

		[TestFixtureSetUp]
		public static void BeforeClass()
		{
			CreateOrClearDestinationFolder(destinationFolder);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void ColorCheckTest1()
		{
			Assert.That(() => 
			{
				PdfWriter writer = new PdfWriter(new MemoryStream());
				Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
					.Open);
				PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
					, "sRGB IEC61966-2.1", @is);
				PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent
					);
				doc.AddNewPage();
				PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
				canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
				canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom
					());
				canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom
					());
				canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop
					());
				canvas.Fill();
				canvas.SetFillColor(iTextSharp.Kernel.Color.Color.RED);
				canvas.MoveTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop
					());
				canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom
					());
				canvas.LineTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom
					());
				canvas.Fill();
				doc.Close();
			}
			, Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.DevicergbAndDevicecmykColorspacesCannotBeUsedBothInOneFile));
;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void ColorCheckTest2()
		{
			Assert.That(() => 
			{
				PdfWriter writer = new PdfWriter(new MemoryStream());
				Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
					.Open);
				PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
					, "sRGB IEC61966-2.1", @is);
				PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent
					);
				doc.AddNewPage();
				PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
				canvas.SetFillColor(new DeviceCmyk(0.1f, 0.1f, 0.1f, 0.1f));
				canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom
					());
				canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom
					());
				canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop
					());
				canvas.Fill();
				doc.Close();
			}
			, Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.DevicecmykMayBeUsedOnlyIfTheFileHasACmykPdfAOutputIntent));
;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void ColorCheckTest3()
		{
			Assert.That(() => 
			{
				PdfWriter writer = new PdfWriter(new MemoryStream());
				PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, null);
				doc.AddNewPage();
				PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
				canvas.SetFillColor(iTextSharp.Kernel.Color.Color.GREEN);
				canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom
					());
				canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom
					());
				canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop
					());
				canvas.Fill();
				doc.Close();
			}
			, Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.IfDeviceRgbCmykGrayUsedInFileThatFileShallContainPdfaOutputIntent));
;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void ColorCheckTest4()
		{
			String outPdf = destinationFolder + "pdfA1b_colorCheckTest4.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA1b_colorCheckTest4.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
				.Open);
			PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
				, "sRGB IEC61966-2.1", @is);
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent
				);
			doc.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
			canvas.SetFillColor(iTextSharp.Kernel.Color.Color.GREEN);
			canvas.MoveTo(doc.GetDefaultPageSize().GetLeft(), doc.GetDefaultPageSize().GetBottom
				());
			canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetBottom
				());
			canvas.LineTo(doc.GetDefaultPageSize().GetRight(), doc.GetDefaultPageSize().GetTop
				());
			canvas.Fill();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void EgsCheckTest1()
		{
			Assert.That(() => 
			{
				PdfWriter writer = new PdfWriter(new MemoryStream());
				Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
					.Open);
				PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
					, "sRGB IEC61966-2.1", @is);
				PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent
					);
				doc.AddNewPage();
				PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
				canvas.SetExtGState(new PdfExtGState().SetTransferFunction(new PdfName("Test")));
				canvas.Rectangle(30, 30, 100, 100).Fill();
				doc.Close();
			}
			, Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.AnExtgstateDictionaryShallNotContainTheTrKey));
;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void EgsCheckTest2()
		{
			String outPdf = destinationFolder + "pdfA1b_egsCheckTest2.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA1b_egsCheckTest2.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
				.Open);
			PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
				, "sRGB IEC61966-2.1", @is);
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent
				);
			doc.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
			canvas.SetExtGState(new PdfExtGState().SetTransferFunction2(PdfName.Default));
			canvas.Rectangle(30, 30, 100, 100).Fill();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void EgsCheckTest3()
		{
			Assert.That(() => 
			{
				PdfWriter writer = new PdfWriter(new MemoryStream());
				Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
					.Open);
				PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
					, "sRGB IEC61966-2.1", @is);
				PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent
					);
				doc.AddNewPage();
				PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
				canvas.SetExtGState(new PdfExtGState().SetTransferFunction2(new PdfName("Test")));
				canvas.Rectangle(30, 30, 100, 100).Fill();
				doc.Close();
			}
			, Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.AnExtgstateDictionaryShallNotContainTheTR2KeyWithAValueOtherThanDefault));
;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void EgsCheckTest4()
		{
			Assert.That(() => 
			{
				PdfWriter writer = new PdfWriter(new MemoryStream());
				Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
					.Open);
				PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
					, "sRGB IEC61966-2.1", @is);
				PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent
					);
				doc.AddNewPage();
				PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
				canvas.SetExtGState(new PdfExtGState().SetRenderingIntent(new PdfName("Test")));
				canvas.Rectangle(30, 30, 100, 100).Fill();
				doc.Close();
			}
			, Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.IfSpecifiedRenderingShallBeOneOfTheFollowingRelativecolorimetricAbsolutecolorimetricPerceptualOrSaturation));
;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void TransparencyCheckTest1()
		{
			Assert.That(() => 
			{
				PdfWriter writer = new PdfWriter(new MemoryStream());
				Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
					.Open);
				PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
					, "sRGB IEC61966-2.1", @is);
				PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent
					);
				doc.AddNewPage();
				PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
				PdfFormXObject xObject = new PdfFormXObject(new Rectangle(100, 100));
				PdfCanvas xObjCanvas = new PdfCanvas(xObject, doc);
				xObjCanvas.Rectangle(30, 30, 10, 10).Fill();
				//imitating transparency group
				//todo replace with real transparency group logic when implemented
				PdfDictionary group = new PdfDictionary();
				group.Put(PdfName.S, PdfName.Transparency);
				xObject.Put(PdfName.Group, group);
				canvas.AddXObject(xObject, new Rectangle(300, 300));
				doc.Close();
			}
			, Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.AGroupObjectWithAnSKeyWithAValueOfTransparencyShallNotBeIncludedInAFormXobject));
;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		[Test]
		public virtual void TransparencyCheckTest2()
		{
			Assert.That(() => 
			{
				PdfWriter writer = new PdfWriter(new MemoryStream());
				Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
					.Open);
				PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
					, "sRGB IEC61966-2.1", @is);
				PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent
					);
				doc.AddNewPage();
				PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
				canvas.SetExtGState(new PdfExtGState().SetSoftMask(new PdfName("Test")));
				canvas.Rectangle(30, 30, 100, 100).Fill();
				doc.Close();
			}
			, Throws.TypeOf<PdfAConformanceException>().With.Message.EqualTo(PdfAConformanceException.TheSmaskKeyIsNotAllowedInExtgstate));
;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XmpException"/>
		/// <exception cref="System.Exception"/>
		[Test]
		public virtual void TransparencyCheckTest3()
		{
			String outPdf = destinationFolder + "pdfA1b_transparencyCheckTest3.pdf";
			String cmpPdf = cmpFolder + "cmp_pdfA1b_transparencyCheckTest3.pdf";
			PdfWriter writer = new PdfWriter(outPdf);
			Stream @is = new FileStream(sourceFolder + "sRGB Color Space Profile.icm", FileMode
				.Open);
			PdfOutputIntent outputIntent = new PdfOutputIntent("Custom", "", "http://www.color.org"
				, "sRGB IEC61966-2.1", @is);
			PdfADocument doc = new PdfADocument(writer, PdfAConformanceLevel.PDF_A_1B, outputIntent
				);
			doc.AddNewPage();
			PdfCanvas canvas = new PdfCanvas(doc.GetLastPage());
			canvas.SetExtGState(new PdfExtGState().SetSoftMask(PdfName.None));
			canvas.Rectangle(30, 30, 100, 100).Fill();
			doc.Close();
			CompareResult(outPdf, cmpPdf);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.Exception"/>
		private void CompareResult(String outPdf, String cmpPdf)
		{
			String result = new CompareTool().CompareByContent(outPdf, cmpPdf, destinationFolder
				, "diff_");
			if (result != null)
			{
				NUnit.Framework.Assert.Fail(result);
			}
		}
	}
}
