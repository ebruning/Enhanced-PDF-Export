using System;
using System.Windows.Forms;
using Kofax.Eclipse.Base;

namespace EnhancedPdfExport
{
    public partial class EnhancedPdfExportSetup : Form
    {
        private Settings _settings = new Settings();

        public EnhancedPdfExportSetup(Settings settings, IIndexField[] indexFields)
        {
            _settings = settings;

            InitializeComponent();

            PopulateIndexField(indexFields);

            UpdateUI();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtExportDirectory.Text))
            {
                MessageBox.Show("Export folder cannot be empty.");
                return;
            }

            _settings.ExportDirectory = txtExportDirectory.Text;
            _settings.Author = txtAuthor.Text;
            _settings.Creator = txtCreator.Text;
            _settings.Keywords = txtKeywords.Text;
            _settings.Subject = txtSubject.Text;
            _settings.Title = txtTitle.Text;

            _settings.EnableChapters = cbEnableBookmarks.Checked;
            _settings.ChapterIndex = cbIndexField.SelectedItem.ToString();
            _settings.BatchCreation = GetPdfType();

            DialogResult = DialogResult.OK;
        }

        private void UpdateUI()
        {
            txtExportDirectory.Text = _settings.ExportDirectory;
            txtAuthor.Text = _settings.Author;
            txtCreator.Text = _settings.Creator;
            txtKeywords.Text = _settings.Keywords;
            txtSubject.Text = _settings.Subject;
            txtTitle.Text = _settings.Title;

            cbEnableBookmarks.Checked = _settings.EnableChapters;
            cbIndexField.SelectedItem =_settings.ChapterIndex;

            SetPdfType();
            
        }

        private void PopulateIndexField(IIndexField [] indexFields)
        {        
            cbIndexField.Items.Clear();

            foreach (IIndexField indexField in indexFields)
                cbIndexField.Items.Add(indexField.Label);
        }

        private void SetPdfType()
        {
            if (_settings.BatchCreation == 0)
                rbOnePerBatch.Checked = true;
            else
                rbOnePerDoc.Checked = true;
        }

        private int GetPdfType()
        {
            return rbOnePerBatch.Checked ? 0 : 1;
        }

        private void cbEnableBookmarks_CheckedChanged(object sender, EventArgs e)
        {
            cbIndexField.Enabled = cbEnableBookmarks.Checked;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();

            browserDialog.SelectedPath = _settings.ExportDirectory;

            if (browserDialog.ShowDialog() == DialogResult.OK)
                txtExportDirectory.Text = browserDialog.SelectedPath;
        }
    }
}
