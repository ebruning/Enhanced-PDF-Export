using System;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Image = iTextSharp.text.Image;
using Kofax.Eclipse.Base;
using System.Collections.Generic;

namespace EnhancedPdfExport.Pdf
{
    class PdfGenerator
    {
        Settings _settings = new Settings();

        public PdfGenerator(Settings settings)
        {
            _settings = settings;
        }

        private void CreatePdf(string[] tiffImages, string outputFile)
        {
            Document    document    = new Document();
            PdfWriter   writer      = PdfWriter.GetInstance(document, new System.IO.FileStream(outputFile, System.IO.FileMode.Create));

            document.Open();

            for (int count = 0; count < tiffImages.Length; count++)
            {
                Bitmap  bitmap  = new Bitmap(tiffImages[count]);
                int     total   = bitmap.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);

                PdfContentByte cb = writer.DirectContent;
                
                for (int k = 0; k < total; ++k)
                {
                    bitmap.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, k);
                    Image image = Image.GetInstance(bitmap, System.Drawing.Imaging.ImageFormat.Bmp);

                    image.ScalePercent(72f / image.DpiX * 100);
                    image.SetAbsolutePosition(0, 0);
                    cb.AddImage(image);
                    document.NewPage();
                }
            }
            UpdateProperties(document);
            document.Close();
        }

        private void CreatePdfBookMarks(List<PdfDef> pdfDocuments, string outputFile)
        {
            Document    document    = new Document();
            PdfWriter   writer      = PdfWriter.GetInstance(document, new System.IO.FileStream(outputFile, System.IO.FileMode.Create));

            document.Open();

            foreach (PdfDef pdfDocument in pdfDocuments)
            {
                String [] tiffImages = new string[pdfDocument.TiffImages.Length];

                for (int i = 0; i < pdfDocument.TiffImages.Length; i++)
                    tiffImages[i] = pdfDocument.TiffImages[i];

                Chapter chapter = new Chapter(pdfDocument.ChapterTitle, pdfDocument.ChapterNumber);

                document.Add(chapter);

                for (int count = 0; count < tiffImages.Length; count++)
                {
                    Bitmap bitmap = new Bitmap(tiffImages[count]);
                    int total = bitmap.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);

                    PdfContentByte cb = writer.DirectContent;

                    for (int k = 0; k < total; ++k)
                    {
                        bitmap.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, k);
                        Image image = Image.GetInstance(bitmap, System.Drawing.Imaging.ImageFormat.Bmp);

                        image.ScalePercent(72f / image.DpiX * 100);
                        image.SetAbsolutePosition(0, 0);
                        cb.AddImage(image);
                        document.NewPage();
                    }
                }
            }
            UpdateProperties(document);
            document.Close(); 
        }

        public void Convert(IDocument doc, string outputFilename)
        {
            String[] inputFileNames = new String[doc.PageCount];

            for (int i = 1; i <= doc.PageCount; i++)
                inputFileNames[i - 1] = doc.GetPage(i).OutputFileName;
          
            CreatePdf(inputFileNames, outputFilename);
        }

        public void Convert(List<PdfDef> pdfDocuments, string outputFilename, bool enableBookmarks)
        {
            if (!enableBookmarks)
                CreatePdf(GetAllTiffImages(pdfDocuments), outputFilename);
            else
                CreatePdfBookMarks(pdfDocuments, outputFilename);
        }

        private string[] GetAllTiffImages(List<PdfDef> pdfDocuments)
        {
            List<string> tmpList = new List<string>();

            foreach (PdfDef document in pdfDocuments)
            {
                for (int i = 0; i < document.TiffImages.Length; i++)
                    tmpList.Add(document.TiffImages[i]);
            }

            String [] inputFilenames = new String[tmpList.Count];

            for (int i = 0; i < tmpList.Count; i++)
            {
                inputFilenames[i] = tmpList[i];
            }

            return inputFilenames;
        }

        private void UpdateProperties(Document document)
        {
            document.AddAuthor(_settings.Author);
            document.AddCreationDate();
            document.AddCreator(_settings.Creator);
            document.AddKeywords(_settings.Keywords);
            document.AddSubject(_settings.Subject);
            document.AddTitle(_settings.Title);
        }
    }
}
