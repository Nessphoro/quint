using Quintessence.Logic;
using Quintessence.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Quintessence
{
    class FileToSourceConverter:IValueConverter
    {
        Dictionary<string, bool> supportedExtensions = new Dictionary<string, bool>(156);
        public FileToSourceConverter()
        {
            #region Generated
            supportedExtensions.Add("7z", true);
            supportedExtensions.Add("ace", true);
            supportedExtensions.Add("aicf", true);
            supportedExtensions.Add("aiff", true);
            supportedExtensions.Add("aif", true);
            supportedExtensions.Add("ai", true);
            supportedExtensions.Add("amr", true);
            supportedExtensions.Add("asf", true);
            supportedExtensions.Add("asp", true);
            supportedExtensions.Add("asx", true);
            supportedExtensions.Add("avi", true);
            supportedExtensions.Add("bat", true);
            supportedExtensions.Add("bin", true);
            supportedExtensions.Add("blank", true);
            supportedExtensions.Add("blend", true);
            supportedExtensions.Add("bmp", true);
            supportedExtensions.Add("bup", true);
            supportedExtensions.Add("cab", true);
            supportedExtensions.Add("cad", true);
            supportedExtensions.Add("cbr", true);
            supportedExtensions.Add("cda", true);
            supportedExtensions.Add("cdl", true);
            supportedExtensions.Add("cdr", true);
            supportedExtensions.Add("chm", true);
            supportedExtensions.Add("com", true);
            supportedExtensions.Add("Cpp", true);
            supportedExtensions.Add("css", true);
            supportedExtensions.Add("csv", true);
            supportedExtensions.Add("cs", true);
            supportedExtensions.Add("c", true);
            supportedExtensions.Add("dat", true);
            supportedExtensions.Add("divx", true);
            supportedExtensions.Add("dll", true);
            supportedExtensions.Add("dmg", true);
            supportedExtensions.Add("docx", true);
            supportedExtensions.Add("doc", true);
            supportedExtensions.Add("Dss", true);
            supportedExtensions.Add("Dvf", true);
            supportedExtensions.Add("dwg", true);
            supportedExtensions.Add("dxf", true);
            supportedExtensions.Add("eml", true);
            supportedExtensions.Add("eps", true);
            supportedExtensions.Add("epub", true);
            supportedExtensions.Add("exe", true);
            supportedExtensions.Add("flac", true);
            supportedExtensions.Add("fla", true);
            supportedExtensions.Add("flv", true);
            supportedExtensions.Add("gif", true);
            supportedExtensions.Add("gp", true);
            supportedExtensions.Add("gz", true);
            supportedExtensions.Add("hqx", true);
            supportedExtensions.Add("html", true);
            supportedExtensions.Add("htm", true);
            supportedExtensions.Add("h", true);
            supportedExtensions.Add("ico", true);
            supportedExtensions.Add("ifo", true);
            supportedExtensions.Add("indd", true);
            supportedExtensions.Add("ink", true);
            supportedExtensions.Add("iso", true);
            supportedExtensions.Add("jar", true);
            supportedExtensions.Add("jpeg", true);
            supportedExtensions.Add("jpg", true);
            supportedExtensions.Add("js", true);
            supportedExtensions.Add("key", true);
            supportedExtensions.Add("lnk", true);
            supportedExtensions.Add("log", true);
            supportedExtensions.Add("m4b", true);
            supportedExtensions.Add("m4p", true);
            supportedExtensions.Add("m4v", true);
            supportedExtensions.Add("mcd", true);
            supportedExtensions.Add("mdb", true);
            supportedExtensions.Add("mid", true);
            supportedExtensions.Add("mkv", true);
            supportedExtensions.Add("mov", true);
            supportedExtensions.Add("mp2", true);
            supportedExtensions.Add("mp3", true);
            supportedExtensions.Add("mp4", true);
            supportedExtensions.Add("mpeg", true);
            supportedExtensions.Add("mpg", true);
            supportedExtensions.Add("msi", true);
            supportedExtensions.Add("net", true);
            supportedExtensions.Add("odb", true);
            supportedExtensions.Add("odc", true);
            supportedExtensions.Add("odf", true);
            supportedExtensions.Add("odg", true);
            supportedExtensions.Add("odi", true);
            supportedExtensions.Add("odm", true);
            supportedExtensions.Add("ods", true);
            supportedExtensions.Add("odt", true);
            supportedExtensions.Add("ogg", true);
            supportedExtensions.Add("otc", true);
            supportedExtensions.Add("otf", true);
            supportedExtensions.Add("otg", true);
            supportedExtensions.Add("oth", true);
            supportedExtensions.Add("oti", true);
            supportedExtensions.Add("otp", true);
            supportedExtensions.Add("ott", true);
            supportedExtensions.Add("pcx", true);
            supportedExtensions.Add("pdb", true);
            supportedExtensions.Add("pdd", true);
            supportedExtensions.Add("pdf", true);
            supportedExtensions.Add("php", true);
            supportedExtensions.Add("png", true);
            supportedExtensions.Add("pps", true);
            supportedExtensions.Add("pptx", true);
            supportedExtensions.Add("ppt", true);
            supportedExtensions.Add("psd", true);
            supportedExtensions.Add("ps", true);
            supportedExtensions.Add("ptb", true);
            supportedExtensions.Add("pub", true);
            supportedExtensions.Add("qbb", true);
            supportedExtensions.Add("qbw", true);
            supportedExtensions.Add("qxd", true);
            supportedExtensions.Add("qxp", true);
            supportedExtensions.Add("ram", true);
            supportedExtensions.Add("rar", true);
            supportedExtensions.Add("raw", true);
            supportedExtensions.Add("resx", true);
            supportedExtensions.Add("rmvb", true);
            supportedExtensions.Add("rm", true);
            supportedExtensions.Add("rtf", true);
            supportedExtensions.Add("rw", true);
            supportedExtensions.Add("sea", true);
            supportedExtensions.Add("ses", true);
            supportedExtensions.Add("sitx", true);
            supportedExtensions.Add("sit", true);
            supportedExtensions.Add("sln", true);
            supportedExtensions.Add("sql", true);
            supportedExtensions.Add("ss", true);
            supportedExtensions.Add("svg", true);
            supportedExtensions.Add("swf", true);
            supportedExtensions.Add("tga", true);
            supportedExtensions.Add("tgz", true);
            supportedExtensions.Add("tiff", true);
            supportedExtensions.Add("tif", true);
            supportedExtensions.Add("trt", true);
            supportedExtensions.Add("ttf", true);
            supportedExtensions.Add("txt", true);
            supportedExtensions.Add("vcd", true);
            supportedExtensions.Add("vob", true);
            supportedExtensions.Add("wav", true);
            supportedExtensions.Add("webm", true);
            supportedExtensions.Add("webp", true);
            supportedExtensions.Add("wma", true);
            supportedExtensions.Add("wmv", true);
            supportedExtensions.Add("wpd", true);
            supportedExtensions.Add("wps", true);
            supportedExtensions.Add("xaml", true);
            supportedExtensions.Add("xar", true);
            supportedExtensions.Add("xcf", true);
            supportedExtensions.Add("xhtml", true);
            supportedExtensions.Add("xlsx", true);
            supportedExtensions.Add("xls", true);
            supportedExtensions.Add("xml", true);
            supportedExtensions.Add("xtm", true);
            supportedExtensions.Add("zip", true);
            #endregion
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SharedFile file = value as SharedFile;
            if(file.IsFolder)
            {
                return new BitmapImage(new Uri("/Resources/folder.png", UriKind.Relative));
            }
            else
            {
                string[] substring = file.Name.Split('.');
                if (substring.Length >= 2)
                {
                    string extension = substring[substring.Length - 1].ToLower();

                    if(supportedExtensions.ContainsKey(extension))
                    {
                        return new BitmapImage(new Uri(string.Format("/Resources/Icons/{0}_icon.png",extension), UriKind.Relative));
                    }
                    else
                    {
                        return new BitmapImage(new Uri("/Resources/Icons/blank_icon.png", UriKind.Relative));
                    }
                       
                }
                else
                {
                    return new BitmapImage(new Uri("/Resources/Icons/blank_icon.png", UriKind.Relative));
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    class StatusToSourceConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SharedFile file = value as SharedFile;
            switch(file.State)
            {
                case SharedFileState.Error:
                    return new BitmapImage(new Uri("/Resources/Status/error.png", UriKind.Relative));
                case SharedFileState.Syncing:
                    return new BitmapImage(new Uri("/Resources/Status/syncing.png", UriKind.Relative));
                case SharedFileState.Synced:
                    return new BitmapImage(new Uri("/Resources/Status/ok.png", UriKind.Relative));
                 
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
