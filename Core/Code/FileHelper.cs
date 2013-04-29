using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SourceCode.SmartObjects.Client;
using SourceCode.SmartObjects.Client.Filters;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;

namespace K2Field.Helpers.Core.Code
{

    /// <summary>
    /// Summary description for FileHelper
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Gets the file name from the full path of the item
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetFileNameFromFullPath(string filepath)
        {
            return filepath.Substring(filepath.LastIndexOf(@"\") + 1);
        }

        /// <summary>
        /// gets the file name without the full path
        /// </summary>
        /// <param name="file">http posted file</param>
        /// <returns></returns>
        public static string GetFileNameFromPostedFile(HttpPostedFile file)
        {
            return file.FileName.Substring(file.FileName.LastIndexOf(@"\") + 1);
        }

        /// <summary>
        /// Converts a uploaded file to a BYTE array
        /// </summary>
        /// <param name="file">HttpPostedFile file</param>
        /// <returns>byte[]</returns>
        public static byte[] ConvertPostedFileToByteArray(HttpPostedFile file)
        {
            int filelen = file.ContentLength;
            byte[] mydata = new byte[filelen];
            file.InputStream.Read(mydata, 0, filelen);

            return mydata;
        }

        /// <summary>
        /// Converts a uploaded file to a base 64 string
        /// </summary>
        /// <param name="file">HttpPostedFile file</param>
        /// <returns>base 6 string</returns>
        public static string ConvertPostedFileToBase64String(HttpPostedFile file)
        {
            return Convert.ToBase64String(FileHelper.ConvertPostedFileToByteArray(file));
        }

        /// <summary>
        /// Use this to convert a post http file to an k2 file object used by smartobjects
        /// </summary>
        /// <param name="file">posted http file</param>
        /// <returns>K2 File Object</returns>
        public static Helpers.Core.Code.SmartObjectCustomTypes.FileObject ConvertPostedFileToK2FileObject(HttpPostedFile file)
        {
            return new SmartObjectCustomTypes.FileObject(GetFileNameFromPostedFile(file), ConvertPostedFileToBase64String(file));
        }

        /// <summary>
        /// Create K2 File object 
        /// </summary>
        /// <param name="filename">filename</param>
        /// <param name="filebytes">file bytes[]</param>
        /// <returns></returns>
        public static Helpers.Core.Code.SmartObjectCustomTypes.FileObject ConvertPostedFileToK2FileObject(string filename, byte[] filebytes)
        {
            return new SmartObjectCustomTypes.FileObject(filename, System.Convert.ToBase64String(filebytes));
        }


        /// <summary>
        /// Convert file from system to byte array
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static byte[] ConvertFileToByteArray(string filename)
        {
            FileInfo f = new FileInfo(filename);

            if (!f.Exists)
            {
                throw new Exception(string.Format("file cannot be found : {0}",filename));
            }

            var fs = f.OpenRead();
            byte[] oData = new byte[f.Length];   
            fs.Read(oData,0,System.Convert.ToInt32(fs.Length));   
            fs.Close();
            fs = null;
            return oData; 
        }

        private static string ConvertFileToBase64String(string filename)
        {
            return System.Convert.ToBase64String(ConvertFileToByteArray(filename));
        }

        /// <summary>
        /// Converts a file from a given path to a K2 FileObject, ready to be used by a smartobject
        /// </summary>
        /// <param name="filename">full path to the file</param>
        /// <returns>K2 fileobject</returns>
        public static Helpers.Core.Code.SmartObjectCustomTypes.FileObject ConvertFileToK2FileObject(string filename)
        {
            return new SmartObjectCustomTypes.FileObject(GetFileNameFromFullPath(filename), ConvertFileToBase64String(filename));
        }


        /// <summary>
        /// Creates a file from a byte array
        /// </summary>
        /// <param name="_FilePathAndName">full path and filename</param>
        /// <param name="_ByteArray">byte array</param>
        public static void ByteArrayToFile(string _FilePathAndName, byte[] _ByteArray)
        {
            System.IO.FileStream FileStream = new FileStream(_FilePathAndName, FileMode.OpenOrCreate, FileAccess.Write);
            FileStream.Write(_ByteArray, 0, _ByteArray.Length);
            FileStream.Close();
        }

        ////<summary>
        ////Converts a uploaded file to a smartobject File Property
        ////</summary>
        ////<param name="file">HttpPostedFile file</param>
        ////<returns>byte[]</returns>
        //public static FileProperty ConvertFileToSmartFileProperty(HttpPostedFile file)
        //{
        //    SourceCode.SmartObjects.Services.ServiceSDK.Objects.FileProperty fileprop = new FileProperty();
        //    fileprop.Content = System.Convert.ToBase64String(ConvertFileToByteArray(file), Base64FormattingOptions.None);
        //    fileprop.FileName = GetActualFileNameFromPostedFile(file);
        //    return fileprop;
        //}





    }
}