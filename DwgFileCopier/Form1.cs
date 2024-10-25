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
            // Kullanýcýdan gerekli bilgileri al
            string excelFilePath = txtExcelFilePath.Text;
            string sourceDirectory = txtSourceDirectory.Text;
            string destinationDirectory = txtDestinationDirectory.Text;

            // Excel dosyasýndan dosya isimlerini oku
            List<string> fileNames = GetFileNamesFromExcel(excelFilePath);

            // Dosyalarý kopyala
            CopyDwgFiles(sourceDirectory, destinationDirectory, fileNames);

            MessageBox.Show("Dosya kopyalama iþlemi tamamlandý.");
        }

        private List<string> GetFileNamesFromExcel(string filePath)
        {
            List<string> fileNames = new List<string>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // EPPlus için lisans ayarý

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.First(); // Ýlk sayfayý al
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 1; row <= rowCount; row++)
                {
                    string fileName = worksheet.Cells[row, 1].Text; // Ýlk sütundaki dosya isimlerini al
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
            // Hedef klasör yoksa oluþtur
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
                    File.Copy(sourceFile, destFile, true); // Eðer dosya varsa üzerine yazar
                    Console.WriteLine($"Kopyalanan dosya: {fileName}");
                }
                else
                {
                    Console.WriteLine($"Dosya bulunamadý: {fileName}");
                }
            }
        }
    }
}
