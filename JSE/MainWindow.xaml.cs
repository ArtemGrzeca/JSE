using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JSE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string jalopyFolderPath = "";
        private readonly byte[] magicHeader = { 0x7E, 0x00, 0x0A };
        private readonly byte[] magicFooter = { 0x7B };
        private readonly byte magicOffset = 0x0B;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void jseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Cheaking save
            jalopyFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\LocalLow\MinskWorks\Jalopy";
            if (!Directory.Exists(jalopyFolderPath)) throw new DirectoryNotFoundException("Jalopy save folder not found");

            int moneyAmount = BitConverter.ToInt32(GetData("moneyAmount"), 0); moneyTextBox.Text = moneyAmount.ToString();
        }

        private void saveButtonToolBar_Click(object sender, RoutedEventArgs e)
        {
            //Saving money amount
            int moneyAmount = Convert.ToInt32(moneyTextBox.Text); SetData("moneyAmount", BitConverter.GetBytes(moneyAmount));
        }

        private void aboutButtonToolBar_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private byte[] GetData(string filename, int filesize = 0x10, int datasize = 0x4)
        {
            if (filesize < 0 || datasize < 0 || datasize > filesize) throw new ArgumentException(string.Format("Invalid arguments: {0}", filename));
            if (!File.Exists(jalopyFolderPath + @"\" + filename)) throw new FileNotFoundException(string.Format("{0} not found", filename)); byte[] ret = new byte[datasize];
            FileStream fs = File.Open(jalopyFolderPath + @"\" + filename, FileMode.Open); using (BinaryReader br = new BinaryReader(fs))
            {
                if ((int)br.BaseStream.Length != filesize) throw new InvalidDataException(string.Format("Invalid size of {0}", filename));
                byte[] file = br.ReadBytes((int)br.BaseStream.Length);
                for (int i = 0; i < magicHeader.Length; i++) if (magicHeader[i] != file[i]) throw new InvalidDataException(string.Format("Bad header magic file: {0}", filename));
                for (int i = 0; i < magicFooter.Length; i++) if (magicFooter[i] != file[(int)br.BaseStream.Length - 1 - i]) throw new InvalidDataException(string.Format("Bad footer magic file: {0}", filename));
                Array.Copy(file, magicOffset, ret, 0, datasize);
            }
            fs.Close(); return ret;
        }

        private void SetData(string filename, byte[] data, int filesize = 0x10)
        {
            if (filesize < 0 || data.Length < 0 || data.Length > filesize) throw new ArgumentException(string.Format("Invalid arguments: {0}", filename));
            if (!File.Exists(jalopyFolderPath + @"\" + filename)) throw new FileNotFoundException(string.Format("{0} not found", filename)); byte[] file;
            FileStream fs = File.Open(jalopyFolderPath + @"\" + filename, FileMode.Open); using (BinaryReader br = new BinaryReader(fs))
            {
                if ((int)br.BaseStream.Length != filesize) throw new InvalidDataException(string.Format("Invalid size of {0}", filename));
                file = br.ReadBytes((int)br.BaseStream.Length);
                for (int i = 0; i < magicHeader.Length; i++) if (magicHeader[i] != file[i]) throw new InvalidDataException(string.Format("Bad header magic file: {0}", filename));
                for (int i = 0; i < magicFooter.Length; i++) if (magicFooter[i] != file[(int)br.BaseStream.Length - 1 - i]) throw new InvalidDataException(string.Format("Bad footer magic file: {0}", filename));
                Array.Copy(data, 0, file, magicOffset, data.Length);
            }
            fs.Close(); fs = File.Open(jalopyFolderPath + @"\" + filename, FileMode.Truncate);
            using (BinaryWriter bw = new BinaryWriter(fs)) { bw.Write(file); bw.Flush(); } fs.Close();
        }
    }
}
