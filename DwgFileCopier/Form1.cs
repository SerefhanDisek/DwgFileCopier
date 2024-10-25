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
        }

        private void btnCopyFiles_Click(object sender, EventArgs e)
        {
            // Kullan�c�dan gerekli bilgileri al
            string excelFilePath = txtExcelFilePath.Text;
            string sourceDirectory = txtSourceDirectory.Text;
            string destinationDirectory = txtDestinationDirectory.Text;

            // Excel dosyas�ndan dosya isimlerini oku
            List<string> fileNames = GetFileNamesFromExcel(excelFilePath);

            // Dosyalar� kopyala
            CopyDwgFiles(sourceDirectory, destinationDirectory, fileNames);

            MessageBox.Show("Dosya kopyalama i�lemi tamamland�.");
        }

        private List<string> GetFileNamesFromExcel(string filePath)
        {
            List<string> fileNames = new List<string>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // EPPlus i�in lisans ayar�

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.First(); // �lk sayfay� al
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 1; row <= rowCount; row++)
                {
                    string fileName = worksheet.Cells[row, 1].Text; // �lk s�tundaki dosya isimlerini al
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
            // Hedef klas�r yoksa olu�tur
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
                    File.Copy(sourceFile, destFile, true); // E�er dosya varsa �zerine yazar
                    Console.WriteLine($"Kopyalanan dosya: {fileName}");
                }
                else
                {
                    Console.WriteLine($"Dosya bulunamad�: {fileName}");
                }
            }
        }
    }
}
