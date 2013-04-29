using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.SmartObjects.Client;
using SourceCode.Hosting.Client.BaseAPI;
using System.Collections.Specialized;
using SourceCode.SmartObjects.Client.Filters;
using System.Configuration;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.Hosting.Server;
using SourceCode.Data.SmartObjectsClient;
using System.Xml;

namespace K2Field.Helpers.Core.Code
{

    /// <summary>
    /// Class to manage interaction with the K2 SmartObject Server using the Client API
    /// </summary>
    public class K2SmartObjectDAL : IDisposable
    {
        //        string connectionString = string.Empty;
        public SmartObjectClientServer SMOServer;
        private bool disposed = false;

        /// <summary>
        /// Overrride the default constructor to force taking of connection string
        /// Also creates a connection, allowing us to perform multi gets, etc, without having to recreate multiple conns
        /// </summary>
        /// <param name="clientConnectionString">k2 server name</param>
        public K2SmartObjectDAL(string servername):this(servername, 5555)
        {
            Console.WriteLine("K2SmartObjectDAL({0})", servername);
        }

        public K2SmartObjectDAL(string servername,int port)
        {
            Console.WriteLine("K2SmartObjectDAL({0},{1})", servername, port);
            SMOServer = new SmartObjectClientServer();
            string connStr = string.Format("Integrated=True;IsPrimaryLogin=True;Authenticate=True;EncryptedPassword=False;Host={0};Port={1}", servername, port);
            SMOServer.CreateConnection();
            SMOServer.Connection.Open(connStr);
        }


        public void CloseServerConnection()
        {
            if (SMOServer != null)
            {
                if (SMOServer.Connection != null)
                {
                    SMOServer.Connection.Dispose();
                }
                SMOServer = null;
            }
        }

        /// <summary>
        /// Call a SmartObject Execeute Type Method
        /// </summary>
        /// <param name="objectName">SmartObject Name</param>
        /// <param name="methodName">Method Name</param>
        /// <param name="properties">Collection of Properties for the input parameters of the method</param>
        /// <returns>Collection of Properties from the SmartObject return property collection</returns>
        public Dictionary<string, object> CallSmartObjectExecuteMethod(string objectName, string methodName, Dictionary<string, object> properties)
        {

            Dictionary<string, object> returnvalues = new Dictionary<string, object>();
            try
            {
                if (!SMOServer.Connection.IsConnected) return new Dictionary<string, object>();

                SmartObject smartObject = SMOServer.GetSmartObject(objectName);
                smartObject.MethodToExecute = methodName;

                //Set Parameter
                foreach (SmartParameter para in smartObject.Methods[methodName].Parameters)
                {
                    if (properties.ContainsKey(para.Name)) SetPropertyValue(para, properties[para.Name]);
                }

                foreach (SmartProperty prop in smartObject.Methods[methodName].InputProperties)
                {
                    //if property is the primary key and method is create don't add this item.
                    if ((prop.Type == PropertyType.AutoGuid || prop.Type == PropertyType.Autonumber) && methodName == "Create") continue;

                    //See if this key exists in the data properteries
                    if (properties.ContainsKey(prop.Name)) SetPropertyValue(prop, properties[prop.Name]);
                }

                SmartObject returnitem = SMOServer.ExecuteScalar(smartObject);

                for (int i = 0; i < returnitem.Properties.Count; i++)
                {
                    returnvalues.Add(returnitem.Properties[i].Name, Converters.ConvertToSmartObjectType(returnitem.Properties[i]));
                }

            }
            catch (SmartObjectException ex)
            {
                ParseSmOException(ex);
            }
            return returnvalues;
        }

        private void ParseSmOException(SmartObjectException ex)
        {
            StringBuilder sb = new StringBuilder();
            foreach (SmartObjectExceptionData m in ex.BrokerData)
            {
                sb.AppendFormat("{0} - {1} - service:{2}",m.Message, m.InnerExceptionMessage, m.ServiceName);
            }
            throw new Exception(sb.ToString(), ex);        
        }


        /// <summary>
        /// Call a SmartObject get list and return a data table
        /// </summary>
        /// <param name="objectName">SmartObjec Name</param>
        /// <param name="methodName">Method Name</param>
        /// <param name="properties">Collection of Properties for the input parameters of the method</param>
        /// <returns>Generic list of Collection of Properties from the SmartObject return property collection</returns>
        public DataTable CallSmartObjectListMethod(string objectName, string methodName, Dictionary<string, object> properties)
        {
            return CallSmartObjectListMethod(objectName, methodName, properties, false);
        }



        private Equals CreateEqualsFilter(string name, object value)
        {
            PropertyType pt;
            if (value is int)
            {
                pt = PropertyType.Number;
            }
            else if (value is string)
            {
                pt = PropertyType.Text;
            }
            else
            {
                throw new ArgumentException(string.Format("Can only cope with objects of int and string for now. Value of {0} was a {1}", name ,value.GetType()));
            }
            var filter = new Equals();
            filter.Left = new PropertyExpression(name, PropertyType.Text);
            filter.Right = new ValueExpression(value, pt);
            return filter;
        }

        /// <summary>
        /// Call a SmartObject get list and return a data table
        /// </summary>
        /// <param name="objectName">SmartObjec Name</param>
        /// <param name="methodName">Method Name</param>
        /// <param name="properties">Collection of Properties for the input parameters of the method</param>
        /// <returns>Generic list of Collection of Properties from the SmartObject return property collection</returns>
        public DataTable CallSmartObjectListMethod(string objectName, string methodName, Dictionary<string, object> properties, bool useFilter)
        {
            List<Dictionary<string, object>> returnCollection = new List<Dictionary<string, object>>();
            if (!SMOServer.Connection.IsConnected) return new DataTable();

            SmartObject smartObject = SMOServer.GetSmartObject(objectName);
            smartObject.MethodToExecute = methodName;
            if (useFilter)
            {
                SmartListMethod getList = smartObject.ListMethods[methodName];
                LogicalFilter currentFilter = null;
                LogicalFilter lastFilter = null;

                foreach (var prop in properties)
                {
                    lastFilter = currentFilter;
                    currentFilter = CreateEqualsFilter(prop.Key, prop.Value);
                    if (lastFilter != null)
                    {
                        //keeps creating 'and' filters for all properties 
                        //e.g. PIID=1243 AND DataName="Data.Field" AND ProcessName = "myProcess"
                        currentFilter = new And(lastFilter, currentFilter);
                    }
                }

                ////////var leftFilter = CreateEqualsFilter(properties[0],int.Parse(properties["ProcessInstanceID"].ToString())) ;
                ////////var rightFilter = CreateEqualsFilter("DataName", properties["DataName"].ToString());
                if (currentFilter != null)
                {// if there is more than one filter
                    //If only one property, then this is an Equals filter
                    //for more than one property it is an AND filter
                    getList.Filter = currentFilter;
                }
            }
            else
            {
                //Set Parameter
                foreach (SmartParameter para in smartObject.ListMethods[methodName].Parameters)
                {
                    if (properties.ContainsKey(para.Name)) SetPropertyValue(para, properties[para.Name]);
                }

                //Set propery
                foreach (SmartProperty prop in smartObject.ListMethods[methodName].InputProperties)
                {
                    //See if this key exists in the data properteries
                    if (properties.ContainsKey(prop.Name)) SetPropertyValue(prop, properties[prop.Name]);
                }
            }
            //maybe temp, remove space from displayname
            return K2Field.Helpers.Core.Code.Converters.RemoveSpaceInColumnNameSpaces(SMOServer.ExecuteListDataTable(smartObject));
        }

        private static void SetPropertyValue(SmartProperty smartProperty, object value)
        {
            if (value == null)
            {
                smartProperty.ValueBehaviour = ValueBehaviour.Unchanged;
                smartProperty.Value = null;
            }
            else if (value == DBNull.Value)
            {
                smartProperty.ValueBehaviour = ValueBehaviour.Clear;
                smartProperty.Value = null;
            }
            else if (value.ToString() == string.Empty)
            {
                smartProperty.ValueBehaviour = ValueBehaviour.Empty;
                smartProperty.Value = string.Empty;
            }
            else
            {
                smartProperty.ValueBehaviour = ValueBehaviour.None;
                smartProperty.Value = value.ToString();
            }
        }

        /// <summary>
        /// Uploads a file to a smartobject
        /// </summary>
        /// <param name="objectName">name of objectName</param>
        /// <param name="methodName">name of methodName</param>
        /// <param name="properties">properties as Data Dictionary</param>
        public void SmartObjectUploadFile(string objectName, string methodName, Dictionary<string, object> properties)
        {
            if (!SMOServer.Connection.IsConnected) return;
            SmartObject smartObject = SMOServer.GetSmartObject(objectName);
            smartObject.MethodToExecute = methodName;

            foreach (KeyValuePair<string, object> prop in properties)
            {
                try
                {
                    //If type if byte[] then its a file, otherwise just add it.
                    if (prop.Value.GetType() == typeof(FileProperty))
                    {
                        smartObject.Properties[prop.Key].Value = ((FileProperty)prop.Value).Value.ToString();
                    }
                    else
                    {
                        smartObject.Properties[prop.Key].Value = prop.Value.ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Problem setting Property {0}, value : {1}  in SmartObject {2}", prop.Key, prop.Value.ToString(), objectName), ex);
                }

            }
            SMOServer.ExecuteScalar(smartObject);
        }

        /// <summary>
        /// Smartobject ADO query
        /// </summary>
        /// <param name="sql">SQL statment to execute </param>
        /// <param name="parameters">query paramters, if used then command type of StoredProcedure is used</param>
        /// <param name="directExecution">Will this query use direct execution</param>
        /// <returns>A Datatable of results </returns>
        public DataTable SmartObjectADOQuery(string sql, Dictionary<string, object> parameters, bool directExecution)
        {
            SOCommand command = null;
            DataTable dataTable = null;
            try
            {
                command = new SOCommand();
                command.Connection = new SOConnection(this.SMOServer.Connection.Host, int.Parse(this.SMOServer.Connection.Port.ToString()));
                command.Connection.DirectExecution = directExecution;
                command.Connection.Port = 5555;
                command.CommandText = sql;
                if (parameters.Count > 0)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (KeyValuePair<string, object> item in parameters)
                    {
                        command.Parameters.Add(new SOParameter(item.Key, item.Value.ToString()));
                    }
                }
                SODataAdapter adapter = new SODataAdapter(command);
                dataTable = new DataTable();
                adapter.Fill(dataTable);
            }
            finally
            {
                command.Connection.Close();
                command.Connection.Dispose();
            }
            return dataTable;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                CloseServerConnection();
                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.

            }
            disposed = true;
        }
        #endregion
    }

}