using System;
using System.Data;
using SourceCode.SmartObjects.Client;
using SourceCode.Hosting.Client.BaseAPI;
using System.Text;
using System.Data.Common;
using System.IO;
using System.Xml.XPath;

namespace K2Field.Helpers.Core.Code.SmartObjectCustomTypes
{
    public abstract class NamedObject
    {

        #region Fields (3)

        private string _name;
        private string _value;
        protected const string SCNULL = "scnull";

        #endregion Fields

        #region Constructors (2)

        public NamedObject(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public NamedObject(string inputValue)
        {
            this.FromValue(inputValue);
        }

        #endregion Constructors

        #region Properties (2)

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        #endregion Properties

        #region Methods (3)

        // Public Methods (3) 

        public abstract void FromValue(string inputValue);

        public override string ToString()
        {
            return ToValue();
        }

        public abstract string ToValue();

        #endregion Methods

    }

    public class ImageObject : NamedObject
    {

        #region Constructors (2)

        public ImageObject(string name, string value)
            : base(name, value)
        {
        }

        public ImageObject(string inputValue)
            : base(inputValue)
        {
        }

        #endregion Constructors

        #region Methods (2)

        // Public Methods (2) 
        public override void FromValue(string inputValue)
        {
            if (!string.IsNullOrEmpty(inputValue))
            {
                using (StringReader reader = new StringReader(inputValue))
                {
                    XPathDocument xDoc = new XPathDocument(reader);
                    XPathNavigator xNav = xDoc.CreateNavigator();
                    string imageNameValue = xNav.SelectSingleNode("image/name").InnerXml;
                    string contentValue = xNav.SelectSingleNode("image/content").InnerXml;
                    if (imageNameValue == SCNULL)
                        this.Name = string.Empty;
                    else
                        this.Name = imageNameValue;
                    if (contentValue == SCNULL)
                        this.Value = string.Empty;
                    else
                        this.Value = contentValue;
                }
            }
            else
            {
                this.Value = string.Empty;
                this.Name = string.Empty;
            }
        }

        public override string ToValue()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<image>");
            if (this.Name == null)
            {
                sb.Append("<name/>");
            }
            else
            {
                sb.Append("<name>");
                sb.Append(this.Name);
                sb.Append("</name>");
            }
            if (this.Value == null)
            {
                sb.Append("<content/>");
            }
            else
            {
                sb.Append("<content>");
                sb.Append(this.Value);
                sb.Append("</content>");
            }
            sb.Append("</image>");
            return sb.ToString();
        }

        #endregion Methods

    }

    public class FileObject : NamedObject
    {

        #region Constructors (2)

        public FileObject(string name, string value)
            : base(name, value)
        {
        }

        public FileObject(string inputValue)
            : base(inputValue)
        {
        }

        #endregion Constructors

        #region Methods (2)

        // Public Methods (2) 
        public override void FromValue(string inputValue)
        {
            if (!string.IsNullOrEmpty(inputValue))
            {
                using (StringReader reader = new StringReader(inputValue))
                {
                    XPathDocument xDoc = new XPathDocument(reader);
                    XPathNavigator xNav = xDoc.CreateNavigator();
                    string fileNameValue = xNav.SelectSingleNode("file/name").InnerXml;
                    string contentValue = xNav.SelectSingleNode("file/content").InnerXml;
                    if (fileNameValue == SCNULL)
                        this.Name = string.Empty;
                    else
                        this.Name = fileNameValue;
                    if (contentValue == SCNULL)
                        this.Value = string.Empty;
                    else
                        this.Value = contentValue;
                }
            }
            else
            {
                this.Value = string.Empty;
                this.Name = string.Empty;
            }
        }

        public override string ToValue()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<file>");
            if (this.Name == null)
            {
                sb.Append("<name/>");
            }
            else
            {
                sb.Append("<name>");
                sb.Append(this.Name);
                sb.Append("</name>");
            }
            if (this.Value == null)
            {
                sb.Append("<content/>");
            }
            else
            {
                sb.Append("<content>");
                sb.Append(this.Value);
                sb.Append("</content>");
            }
            sb.Append("</file>");
            return sb.ToString();
        }

        #endregion Methods

    }

    public class HyperLinkObject : NamedObject
    {

        #region Constructors (2)

        public HyperLinkObject(string name, string value)
            : base(name, value)
        {
        }

        public HyperLinkObject(string inputValue)
            : base(inputValue)
        {
        }

        #endregion Constructors

        #region Methods (2)

        // Public Methods (2) 

        public override void FromValue(string inputValue)
        {
            if (!string.IsNullOrEmpty(inputValue))
            {
                using (StringReader reader = new StringReader(inputValue))
                {
                    XPathDocument xDoc = new XPathDocument(reader);
                    XPathNavigator xNav = xDoc.CreateNavigator();
                    string hyperLinkNameValue = xNav.SelectSingleNode("hyperlink/display").InnerXml;
                    string contentValue = xNav.SelectSingleNode("hyperlink/link").InnerXml;
                    if (hyperLinkNameValue == SCNULL)
                        this.Name = string.Empty;
                    else
                        this.Name = hyperLinkNameValue;
                    if (contentValue == SCNULL)
                        this.Value = string.Empty;
                    else
                        this.Value = contentValue;
                }
            }
            else
            {
                this.Value = string.Empty;
                this.Name = string.Empty;
            }
        }

        public override string ToValue()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<hyperlink>");
            if (this.Name == null)
            {
                sb.Append("<link/>");
            }
            else
            {
                sb.Append("<link>");
                sb.Append(this.Name);
                sb.Append("</link>");
            }
            if (this.Value == null)
            {
                sb.Append("<display/>");
            }
            else
            {
                sb.Append("<display>");
                sb.Append(this.Value);
                sb.Append("</display>");
            }
            sb.Append("</hyperlink>");
            return sb.ToString();
        }

        #endregion Methods

    }

}
