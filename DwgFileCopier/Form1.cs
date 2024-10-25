using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;

namespace DwgFileCopier
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            txtExcelFilePath.AllowDrop = true; 
            txtExcelFilePath.DragEnter += TxtExcelFilePath_DragEnter;
            txtExcelFilePath.DragDrop += TxtExcelFilePath_DragDrop;

            txtSourceDirectory.AllowDrop = true; 
            txtSourceDirectory.DragEnter += TxtSourceDirectory_DragEnter;
            txtSourceDirectory.DragDrop += TxtSourceDirectory_DragDrop;

            txtDestinationDirectory.AllowDrop = true; 
            txtDestinationDirectory.DragEnter += TxtDestinationDirectory_DragEnter;
            txtDestinationDirectory.DragDrop += TxtDestinationDirectory_DragDrop;
        }

        private void TxtExcelFilePath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void TxtExcelFilePath_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                if (Path.GetExtension(files[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(files[0]).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    txtExcelFilePath.Text = files[0];
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir Excel dosyasý sürükleyin (.xlsx veya .xls).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TxtSourceDirectory_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void TxtSourceDirectory_DragDrop(object sender, DragEventArgs e)
        {
            string[] directories = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (directories.Length > 0 && Directory.Exists(directories[0]))
            {
                txtSourceDirectory.Text = directories[0];
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir kaynak klasör sürükleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtDestinationDirectory_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void TxtDestinationDirectory_DragDrop(object sender, DragEventArgs e)
        {
            string[] directories = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (directories.Length > 0 && Directory.Exists(directories[0]))
            {
                txtDestinationDirectory.Text = directories[0];
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir hedef klasör sürükleyin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCopyFiles_Click(object sender, EventArgs e)
        {
            try
            {
                string excelFilePath = txtExcelFilePath.Text;
                string sourceDirectory = txtSourceDirectory.Text;
                string destinationDirectory = txtDestinationDirectory.Text;

                if (string.IsNullOrWhiteSpace(excelFilePath) || string.IsNullOrWhiteSpace(sourceDirectory) || string.IsNullOrWhiteSpace(destinationDirectory))
                {
                    MessageBox.Show("Lütfen tüm dosya yollarýný ve klasörleri belirtin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists(excelFilePath))
                {
                    MessageBox.Show("Belirtilen Excel dosyasý bulunamadý.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!Directory.Exists(sourceDirectory))
                {
                    MessageBox.Show("Belirtilen kaynak klasör bulunamadý.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                List<string> fileNames = GetFileNamesFromExcel(excelFilePath);
                if (fileNames.Count == 0)
                {
                    MessageBox.Show("Excel dosyasýnda kopyalanacak dosya adý bulunamadý.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CopyDwgFiles(sourceDirectory, destinationDirectory, fileNames);
                MessageBox.Show("Dosya kopyalama iþlemi tamamlandý.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluþtu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> GetFileNamesFromExcel(string filePath)
        {
            List<string> fileNames = new List<string>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    MessageBox.Show("Excel dosyasýnda geçerli bir sayfa bulunamadý.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return fileNames;
                }

                int rowCount = worksheet.Dimension.Rows;
                for (int row = 1; row <= rowCount; row++)
                {
                    string fileName = worksheet.Cells[row, 1].Text;
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        fileNames.Add(fileName.Trim());
                    }
                }
            }

            return fileNames;
        }

        private void CopyDwgFiles(string sourceDir, string destDir, List<string> fileNames)
        {
            try
            {
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                foreach (var fileName in fileNames)
                {
                    string sourceFile = Path.Combine(sourceDir, fileName);
                    if (File.Exists(sourceFile))
                    {
                        string destFile = Path.Combine(destDir, fileName);
                        File.Copy(sourceFile, destFile, true);
                        Console.WriteLine($"Kopyalanan dosya: {fileName}");
                    }
                    else
                    {
                        Console.WriteLine($"Dosya bulunamadý: {fileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya kopyalama sýrasýnda bir hata oluþtu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
