using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using EnhancedPdfExport.Pdf;
using Kofax.Eclipse.Base;

namespace EnhancedPdfExport
{
    public class EnhancedPdfExport :IReleaseScript
    {
        private             string          _batchName      = string.Empty;
        private readonly    Settings        _settings        = new Settings();
        private             string          _batchFolder     = string.Empty;
        private             PdfGenerator    _docConverter;
        private readonly    List<PdfDef>    _documentList    = new List<PdfDef>();

        #region IReleaseScript2 Properties

        public string Description
        {
            get { return "Enhanced PDF Export"; }
        }

        public Guid Id
        {
            get { return new Guid("{4CC075B4-C27B-478a-B3C1-A5101AFB4CC0}"); }
        }

        public bool IsSupported(ReleaseMode mode)
        {
            if (mode == ReleaseMode.MultiPage)
                return true;

            throw new Exception("Unsupported export mode.");
        }

        public string Name
        {
            get { return "Enhanced PDF Export"; }
        }

        public ReleaseMode WorkingMode
        {
            get { return ReleaseMode.MultiPage; }
        }              

        #endregion

        #region Methods

        public void CustomizeSettings(IList<IExporter> exporters, IJob job, IApplication licenseData)
        {
            
        }

        public void EndBatch(IBatch batch, object handle, ReleaseResult result)
        {
            if (_settings.BatchCreation == 1) return;

            string pdfname = Path.Combine(_batchFolder, Path.ChangeExtension(_batchName, "pdf"));

            _docConverter.Convert(_documentList, pdfname, _settings.EnableChapters);
        }

        public void EndRelease(object handle, ReleaseResult result)
        {
            
        }

        public void Release(IDocument doc)
        {
            if (_settings.BatchCreation == 0)
            {
                PdfDef _pdf = new PdfDef();

                String[] inputFileNames = new String[doc.PageCount];

                for (int i = 1; i <= doc.PageCount; i++)
                    inputFileNames[i - 1] = doc.GetPage(i).OutputFileName;

                _pdf.ChapterNumber = doc.Number;
                _pdf.TiffImages    = inputFileNames;
                _pdf.ChapterTitle  = doc.GetIndexDataValue(_settings.ChapterIndex);

                _documentList.Add(_pdf);
            }
            else
            {
                _docConverter.Convert(doc, Path.Combine(_batchFolder, Path.ChangeExtension(doc.Number.ToString(), "pdf")));
            }
        }

        public object StartBatch(IBatch batch)
        {
            _batchFolder = Path.Combine(_settings.ExportDirectory, batch.Name);
            _batchName = batch.Name;

            if (!Directory.Exists(_batchFolder))
                Directory.CreateDirectory(_batchFolder);

            return null;
        }

        public object StartRelease(IList<IExporter> exporters, IIndexField[] indexFields, IDictionary<string, string> releaseData, IApplication licenseData)
        {
            _docConverter = new PdfGenerator(_settings);
            return null;
        }

        #endregion

        #region Settings and Setup
        
        public void DeserializeSettings(Stream input)
        {
            using (BinaryReader reader = new BinaryReader(input))
            {
                try
                {
                    _settings.ExportDirectory = reader.ReadString();
                    _settings.BatchCreation   = reader.ReadInt32();
                    _settings.EnableChapters  = reader.ReadBoolean();
                    _settings.ChapterIndex    = reader.ReadString();

                    _settings.Author          = reader.ReadString();
                    _settings.Creator         = reader.ReadString();
                    _settings.Keywords        = reader.ReadString();
                    _settings.Subject         = reader.ReadString();
                    _settings.Title           = reader.ReadString();
                }
                catch
                {
                    _settings.ExportDirectory = string.Empty;
                    _settings.BatchCreation   = 0;
                    _settings.EnableChapters  = false;
                    _settings.ChapterIndex    = string.Empty;

                    _settings.Author          = string.Empty;
                    _settings.Creator         = string.Empty;
                    _settings.Keywords        = string.Empty;
                    _settings.Subject         = string.Empty;
                    _settings.Title           = string.Empty;
                }
            } 
        }

        public void SerializeSettings(Stream output)
        {
            using (BinaryWriter writer = new BinaryWriter(output))
            {
                writer.Write(_settings.ExportDirectory);
                writer.Write(_settings.BatchCreation);
                writer.Write(_settings.EnableChapters);
                writer.Write(_settings.ChapterIndex);

                writer.Write(_settings.Author);
                writer.Write(_settings.Creator);
                writer.Write(_settings.Keywords);
                writer.Write(_settings.Subject);
                writer.Write(_settings.Title);
            } 
        }

        public void Setup(IList<IExporter> exporters, IJob job, IDictionary<string, string> releaseData, IApplication licenseData)
        {
            //EnhancedPdfExportSetup setupDialog = new EnhancedPdfExportSetup(_settings);
 
            //if (setupDialog.ShowDialog() != DialogResult.OK) return;
        }

        #endregion

        #region Unused Methods
        public object StartRelease(IList<IExporter> exporters, IIndexField[] indexFields, IDictionary<string, string> releaseData)
        {
            _docConverter = new PdfGenerator(_settings);
            return null; //Have to use this since IReleaseScript2 is missing the index fields for the setup.
        }

        public void Setup(IList<IExporter> exporters, IIndexField[] indexFields, IDictionary<string, string> releaseData)
        {
            EnhancedPdfExportSetup setupDialog = new EnhancedPdfExportSetup(_settings, indexFields);

            if (setupDialog.ShowDialog() != DialogResult.OK) return;
        }

        public void Release(IPage page)
        {
            throw new NotImplementedException();
        }

        public object StartDocument(IDocument doc)
        {
            return null;
        }

        public void EndDocument(IDocument doc, object handle, ReleaseResult result)
        {

        }
        #endregion        
    }
}
