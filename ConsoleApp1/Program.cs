using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;

using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ConsoleApp1
{
    // From https://www.pinvoke.net/default.aspx/Structures/OPENFILENAME.html
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int flagsEx;
    }
    internal class Program
    {
        [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetOpenFileName(ref OpenFileName ofn);

        private static string ShowDialog()
        {
            var ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(ofn);
            // Only allow JPG files
            ofn.lpstrFilter = "JPEG Files (*.jpg;*.jpeg)\0*.jpg;*.jpeg\0All Files (*.*)\0*.*\0";
            ofn.lpstrFile = new string(new char[256]);
            ofn.nMaxFile = ofn.lpstrFile.Length;
            ofn.lpstrFileTitle = new string(new char[64]);
            ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
            ofn.lpstrTitle = "Select a JPG File";
            if (GetOpenFileName(ref ofn))
                return ofn.lpstrFile;
            return string.Empty;
        }
        [STAThread]
        static async Task Main(string[] args)
        {
           
            var url = "https://freeimage.host/api/1/upload?key=6d207e02198a847aa98d0a2a901485a5";
            var filePath = ShowDialog();  // Yüklemek istediğiniz dosyanın yolu
            var responseFilePath = "response.txt";
           


            using (var client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {
                    byte[] fileBytes = File.ReadAllBytes(path);
                    var fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    form.Add(fileContent, "source", Path.GetFileName(path));

                    var response = await client.PostAsync(url, form);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Yanıtı metin dosyasına kaydet
                    File.WriteAllText(responseFilePath, responseContent);

                    Console.WriteLine("Yanıt başarıyla metin dosyasına kaydedildi: " + responseFilePath);
                }
            }
        }

    





    }

}
