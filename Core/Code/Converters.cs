using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SourceCode.Workflow.Client;
using SourceCode.SmartObjects;
using K2Field.Helpers.Core.Code.SmartObjectCustomTypes;

namespace K2Field.Helpers.Core.Code
{
    public class Converters
    {
        /// <summary>
        /// Converts a DataTable to a list of a specified class
        /// </summary>
        /// <param name="t">typeof(class)</param>
        /// <param name="data">datatable to convert</param>
        /// <returns>list of boxed class objects</returns>
        public static List<object> ConvertDataTableToClass(Type t, DataTable data)
        {
            List<object> res = new List<object>();

            foreach (DataRow row in data.Rows)
            {
                object obj = t.Assembly.CreateInstance(t.FullName);

                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    try
                    {
                        if (data.Columns.IndexOf(prop.Name) >= 0)
                        {

                            switch (prop.PropertyType.Name)
                            {
                                case "FileObject":
                                    //Need to convert from string to fileobject
                                    FileObject fileobj = (row[data.Columns[prop.Name]] == null) ? new FileObject(string.Empty) : new FileObject(row[data.Columns[prop.Name]].ToString());
                                    prop.SetValue(obj, fileobj, null);
                                    break;

                                case "HyperLinkObject":
                                    //convert from string to hyperlink property.
                                    HyperLinkObject hyperObject = (row[data.Columns[prop.Name]] == null) ? new HyperLinkObject(string.Empty) : new HyperLinkObject(row[data.Columns[prop.Name]].ToString());
                                    prop.SetValue(obj, hyperObject, null);
                                    break;

                                default:

                                    if (row[data.Columns[prop.Name]] != System.DBNull.Value)
                                    {
                                        if (prop.PropertyType.IsGenericType)
                                        {
                                            switch (Nullable.GetUnderlyingType(prop.PropertyType).Name)
                                            {
                                                case "Int32":
                                                    int? propValueINT = IsNullOrEmpty(row[data.Columns[prop.Name]]) ? (int?)null : int.Parse(row[data.Columns[prop.Name]].ToString());
                                                    prop.SetValue(obj, propValueINT, null);
                                                    break;

                                                case "Int64":
                                                    long? propValueINT64 = IsNullOrEmpty(row[data.Columns[prop.Name]]) ? (long?)null : long.Parse(row[data.Columns[prop.Name]].ToString());
                                                    prop.SetValue(obj, propValueINT64, null);
                                                    break;
                                                
                                                case "Decimal":
                                                    decimal? propValueDECIMAL = IsNullOrEmpty(row[data.Columns[prop.Name]]) ? (decimal?)null : decimal.Parse(row[data.Columns[prop.Name]].ToString());
                                                    prop.SetValue(obj, propValueDECIMAL, null);
                                                    break;
                                                case "Boolean":
                                                    bool? propValueBOOL = IsNullOrEmpty(row[data.Columns[prop.Name]]) ? (bool?)null : bool.Parse(row[data.Columns[prop.Name]].ToString());
                                                    prop.SetValue(obj, propValueBOOL, null);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            prop.SetValue(obj, (object)row[data.Columns[prop.Name]], null);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error converting '{0}' to {1}. Extra details {2}", prop.Name, prop.PropertyType.Name, ex.ToString()));
                    }
                }

                res.Add(obj);
            }
            return res;
        }


        /// <summary>
        /// Converts a Dictionary<string, object> to a list of a specified class
        /// </summary>
        /// <param name="t">typeof(class)</param>
        /// <param name="data">Dictionary<string,object> to convert</param>
        /// <returns>boxed class object</returns>
        public static object ConvertDictionaryToClass(Type t, Dictionary<string, object> data)
        {
            object obj = t.Assembly.CreateInstance(t.FullName);

            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                try
                {
                    if (data.ContainsKey(prop.Name))
                    {
                        switch (prop.PropertyType.Name)
                        {
                            case "FileObject":
                                //Need to convert from string to fileobject
                                FileObject fileobj = (data[prop.Name] == null) ? new FileObject(string.Empty) : new FileObject(data[prop.Name].ToString());
                                prop.SetValue(obj, fileobj, null);
                                break;

                            case "HyperLinkObject":
                                //convert from string to hyperlink property.
                                HyperLinkObject hyperObject = (data[prop.Name] == null) ? new HyperLinkObject(string.Empty) : new HyperLinkObject(data[prop.Name].ToString());
                                prop.SetValue(obj, hyperObject, null);
                                break;



                            default:

                                if (prop.PropertyType.IsGenericType)
                                {

                                    switch (Nullable.GetUnderlyingType(prop.PropertyType).Name)
                                    {


                                        case "Int32":
                                            int? propValueINT = IsNullOrEmpty(data[prop.Name]) ? (int?)null : int.Parse(data[prop.Name].ToString());
                                            prop.SetValue(obj, propValueINT, null);
                                            break;

                                        case "Int64":
                                            long? propValueINT64 = IsNullOrEmpty(data[prop.Name]) ? (long?)null : long.Parse(data[prop.Name].ToString());
                                            prop.SetValue(obj, propValueINT64, null);
                                            break;

                                        case "Decimal":
                                            decimal? propValueDECIMAL = IsNullOrEmpty(data[prop.Name]) ? (decimal?)null : decimal.Parse(data[prop.Name].ToString());
                                            prop.SetValue(obj, propValueDECIMAL, null);
                                            break;
                                        case "Boolean":
                                            bool? propValueBOOL = IsNullOrEmpty(data[prop.Name]) ? (bool?)null : bool.Parse(data[prop.Name].ToString());
                                            prop.SetValue(obj, propValueBOOL, null);
                                            break;
                                    }

                                }
                                else
                                {
                                    if (data[prop.Name] != null) prop.SetValue(obj, (object)data[prop.Name], null);
                                }




                                //object value = data[prop.Name];
                                //if (prop.PropertyType.IsGenericType && Nullable.GetUnderlyingType(prop.PropertyType) != null && Nullable.GetUnderlyingType(prop.PropertyType).IsEnum)
                                //{
                                //}
                                //else
                                //{
                                //}

                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error converting '{0}' to {1}. Extra details {2}", prop.Name, prop.PropertyType.Name, ex.ToString()));
                }
            }

            return obj;
        }

        private static bool IsNullOrEmpty(object item)
        {
            if (item == null) return true;

            if (item.ToString().Length == 0) return true;

            //else
            return false;
        }

        public static Dictionary<string, object> ConvertClassToDictionary(object dataClass)
        {
            Dictionary<string, object> results = new Dictionary<string, object>();

            foreach (PropertyInfo prop in dataClass.GetType().GetProperties())
            {
                try
                {
                    results.Add(prop.Name, prop.GetValue(dataClass, null));
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error Adding items to Dictionary from class. Item '{0}'. Extra details {1}", prop.Name, ex.ToString()));
                }
            }

            return results;
        }

        public static DataTable InsertUnderScoreIntoColumnNameSpaces(DataTable data)
        {
            foreach (DataColumn col in data.Columns)
            {
                col.ColumnName = col.ColumnName.Replace(" ", "_");
            }
            return data;
        }

        public static DataTable RemoveSpaceInColumnNameSpaces(DataTable data)
        {
            foreach (DataColumn col in data.Columns)
            {
                col.ColumnName = col.ColumnName.Replace(" ", "");
            }
            return data;
        }

        public static object ConvertToSmartObjectType(SourceCode.SmartObjects.Client.SmartProperty prop)
        {
            return ConvertToSmartObjectType(prop.Value, prop.Type);
        }

        public static object ConvertToSmartObjectType(string value, SourceCode.SmartObjects.Client.PropertyType type)
        {
            if (string.IsNullOrEmpty(value)) return null;

            switch (type)
            {
                case SourceCode.SmartObjects.Client.PropertyType.AutoGuid:
                    return new Guid(value);

                case SourceCode.SmartObjects.Client.PropertyType.Autonumber:
                    return int.Parse(value);
                case SourceCode.SmartObjects.Client.PropertyType.DateTime:
                    return DateTime.Parse(value);
                case SourceCode.SmartObjects.Client.PropertyType.Decimal:
                    return decimal.Parse(value);
                case SourceCode.SmartObjects.Client.PropertyType.File:
                    break;
                case SourceCode.SmartObjects.Client.PropertyType.Guid:
                    return new Guid(value);
                case SourceCode.SmartObjects.Client.PropertyType.HyperLink:
                    break;
                case SourceCode.SmartObjects.Client.PropertyType.Image:
                    break;
                case SourceCode.SmartObjects.Client.PropertyType.Memo:
                    return value;
                case SourceCode.SmartObjects.Client.PropertyType.MultiValue:
                    break;
                case SourceCode.SmartObjects.Client.PropertyType.Number:
                    return int.Parse(value);
                case SourceCode.SmartObjects.Client.PropertyType.Text:
                    return value;
                case SourceCode.SmartObjects.Client.PropertyType.YesNo:
                    return bool.Parse(value);
            }
            return value;
        }

        public static CoreDataField ConvertWFDataFieldToCoreDataField(DataField df)
        {
            CoreDataField retVal = new CoreDataField();
            retVal.Name = df.Name;
            retVal.Value = df.Value;
            return retVal;
        }
    }
}
