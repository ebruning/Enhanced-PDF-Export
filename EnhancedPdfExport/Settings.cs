using System;

namespace EnhancedPdfExport
{
    public class Settings
    {
        public string   ExportDirectory { get; set; }
        public Int32    BatchCreation   { get; set; }
        public bool     EnableChapters  { get; set; }
        public string   ChapterIndex    { get; set; }

        public string Author        { get; set; }
        public string Creator       { get; set; }
        public string Keywords      { get; set; }
        public string Subject       { get; set; }
        public string Title         { get; set; }
    }
}
