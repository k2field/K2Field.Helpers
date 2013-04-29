using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using SourceCode.Hosting.Client.BaseAPI;
using SourceCode.Workflow.Client;
using System.Configuration;

namespace K2Field.Helpers.Core.Code
{

    /// <summary>
    /// Provides methods to access K2 blackpearl functionality through the K2 API
    /// </summary>
    public class K2WorkflowClient : IDisposable
    {
        public Connection connection;
        public string currentUser { get; set; }
        private bool disposed = false;

        public K2WorkflowClient(string _K2Server, uint _Port, string _SecurityLabelName)
        {
            try
            {
                connection = new Connection();
                SCConnectionStringBuilder scBuilder = new SCConnectionStringBuilder();

                scBuilder.Authenticate = true;
                scBuilder.Host = _K2Server;
                scBuilder.Integrated = true;
                scBuilder.IsPrimaryLogin = true;
                scBuilder.Port = _Port;
                scBuilder.SecurityLabelName = _SecurityLabelName;

                connection.Open(scBuilder.Host, scBuilder.ConnectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create connection : " + ex.ToString());
            }
        }

        public K2WorkflowClient(string K2ServerHost, string connectionString)
        {
            try
            {
                connection = new Connection();
                connection.Open(K2ServerHost, connectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load connection string ConnectionString :" + ex.ToString());
            }
        }

        public int StartK2Process(string processName, string folio)
        {
            return StartK2Process(processName, folio, new Dictionary<string, object>());
        }

        public void CloseConnection()
        {
            connection.Close();
            connection.Dispose();
        }

        /// <summary>
        /// Start a workflow process
        /// </summary>
        /// <param name="processName">Full name of the process to start</param>
        /// <param name="folio">Folio of the process to start</param>
        /// <param name="dataFields">NameValueCollection of datafields Name,Value</param>
        /// <returns></returns>
        public int StartK2Process(string processName, string folio, Dictionary<string, object> dataFields)
        {
            int processId;

            ProcessInstance processInstance = connection.CreateProcessInstance(processName);
            processInstance.Folio = folio;

            if (dataFields != null)
            {
                foreach (KeyValuePair<string, object> field in dataFields)
                {
                    processInstance.DataFields[field.Key].Value = field.Value;
                }
            }
            connection.StartProcessInstance(processInstance);
            processId = processInstance.ID;

            return processId;
        }
        /// <summary>
        /// Start a workflow process
        /// </summary>
        /// <param name="processName">Full name of the process to start</param>
        /// <param name="folio">Folio of the process to start</param>
        /// <param name="dataFields">NameValueCollection of datafields Name,Value</param>
        /// <returns></returns>
        public int StartK2Process(string processName, string folio, Dictionary<string, CoreDataField> dataFields)
        {
            int processId;

            ProcessInstance processInstance = connection.CreateProcessInstance(processName);
            processInstance.Folio = folio;

            if (dataFields != null)
            {
                foreach (KeyValuePair<string, CoreDataField> field in dataFields)
                {
                    processInstance.DataFields[field.Key].Value = field.Value.Value;
                }
            }
            connection.StartProcessInstance(processInstance);
            processId = processInstance.ID;

            return processId;
        }

        /// <summary>
        /// Get a worklist item using the worklist Serial Number (by default will allocate the item)
        /// </summary>
        /// <param name="serialNumber">Worklist Serial Number</param>
        /// <returns>A Worklist</returns>
        public WorklistItem GetWorkListItem(string serialNumber)
        {
            return GetWorkListItem(serialNumber, true);
        }


        /// <summary>
        /// Get a worklist item using the worklist Serial Number
        /// </summary>
        /// <param name="serialNumber">Worklist Serial Number</param>
        /// <returns>A Worklist</returns>
        public WorklistItem GetWorkListItem(string serialNumber, bool allocate)
        {
            var worklistItem = connection.OpenWorklistItem(serialNumber, "ASP", allocate);
            return worklistItem;
        }

        /// <summary>
        /// Get a worklist item using the worklist Serial Number
        /// </summary>
        /// <param name="serialNumber">Worklist Serial Number</param>
        /// <param name="ImpersonateUser">Username to impersonate.</param>
        /// <returns>K2 worklist</returns>
        public WorklistItem GetWorkListItem(string serialNumber, string ImpersonateUser)
        {
            return GetWorkListItem(serialNumber, ImpersonateUser, true);
        }

        /// <summary>
        /// Get a worklist item using the worklist Serial Number
        /// </summary>
        /// <param name="serialNumber">Worklist Serial Number</param>
        /// <param name="ImpersonateUser">Username to impersonate.</param>
        /// <param name="allocate">should we allocate the worklist item</param>
        /// <returns>K2 worklist item</returns>
        public WorklistItem GetWorkListItem(string serialNumber, string ImpersonateUser, bool allocate)
        {
            ImpersonateUser = ImpersonateUser.StartsWith("K2:") ? ImpersonateUser : "K2:" + ImpersonateUser;

            connection.ImpersonateUser(ImpersonateUser);
            var worklistItem = connection.OpenWorklistItem(serialNumber, "ASP", allocate);
            connection.RevertUser();
            return worklistItem;
        }


        /// <summary>
        /// Get a worklist item for a certain Process Instance Id. (NOTE: This might be a resource intensive method if used in large loops, etc so don't over do this one.
        /// </summary>
        /// <param name="ProcessInstaceId">Process Instance Id</param>
        /// <returns>K2 work list item</returns>
        public WorklistItem GetWorkListItem(int ProcessInstaceId)
        {
            WorklistCriteria wlc = new WorklistCriteria();
            wlc.AddFilterField(WCField.ProcessID, WCCompare.Equal, ProcessInstaceId);

            //var item = connection.OpenServerItem("");
            //item.ProcessInstance.

            Worklist wl = connection.OpenWorklist(wlc);

            if (wl.Count > 0)
            {
                return wl[0];
            }
            else return null;
        }

        /// <summary>
        /// Not used, just for testing at the moment
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="synch"></param>
        public void FinishServerItem(string sn, bool synch)
        {
            ServerItem serverItem = connection.OpenServerItem(sn);
            serverItem.Finish(synch);

        }

        /// <summary>
        /// Get all worklist items for a specific process
        /// </summary>
        /// <param name="ProcessInstaceId">ProcessInstaceId</param>
        /// <returns>Worklist</returns>
        public Worklist GetAllWorkListItem(int ProcessInstaceId)
        {
            WorklistCriteria wlc = new WorklistCriteria();
            wlc.AddFilterField(WCField.ProcessID, WCCompare.Equal, ProcessInstaceId);
            return connection.OpenWorklist(wlc);
        }

        /// <summary>
        /// Gets a process instance based from a given ProcessInstaceId
        /// </summary>
        /// <param name="processInstaceId">ProcessInstaceId</param>
        /// <returns>ProcessInstance</returns>
        public ProcessInstance GetProcessInstance(int processInstaceId)
        {
            return connection.OpenProcessInstance(processInstaceId);
        }

        /// <summary>
        /// Gets datafields for a given process instance id  (Can only be used when the user has admin access on the process)
        /// </summary>
        /// <param name="processInstaceId">Process Instance ID</param>
        /// <returns>Dictionary<string, object> of datafields</returns>
        /// 
        public Dictionary<string, object> GetProcessDataFields(int processInstaceId)
        {
            Dictionary<string, object> returnCollection = new Dictionary<string, object>();

                var proc = GetProcessInstance(processInstaceId);
                foreach (DataField field in proc.DataFields)
                {
                    returnCollection.Add(field.Name, field.Value);
                }
            return returnCollection;
        }

        public T GetProcessDataFields<T>(int processInstaceId)
        {
            Dictionary<string, object> df = GetProcessDataFields(processInstaceId);
            return (T)Converters.ConvertDictionaryToClass(typeof(T), df);
        }

        /// <summary>
        /// Sets datafields for a given process instance id (Can only be used when the user has admin access on the process)
        /// </summary>
        /// <param name="processInstaceId">Process Instance ID</param>
        /// <param name="dataFields">Datafields to update</param>
        /// <returns>true/false if the update worked</returns>
        public bool SetProcessDataFields(int processInstaceId, Dictionary<string, object> dataFields)
        {

            WindowsPrincipalHelper.RunWithServiceAccount(delegate()
            {
                var proc = GetProcessInstance(processInstaceId);

                foreach (KeyValuePair<string, object> dataFieldName in dataFields)
                {
                    try
                    {
                        proc.DataFields[dataFieldName.Key].Value = dataFieldName.Value;
                    }
                    catch (Exception ex)
                    {
                        string eMessage = string.Format("Problem settings datafield :'{0}', Error : {1} ", dataFieldName, ex.ToString());
                        throw new Exception(eMessage);
                    }
                }
                proc.Update();
            });
            return true;
        }

        public bool SetProcessDataFields(int processInstaceId, object myclass)
        {
            Dictionary<string, object> df = Converters.ConvertClassToDictionary(myclass);
            return SetProcessDataFields(processInstaceId,df);
        }

        /// <summary>
        /// Get a worklist for the currently logged on user
        /// </summary>
        /// <returns>K2 Work List</returns>
        public Worklist GetWorkList()
        {
            return connection.OpenWorklist();
        }

        /// <summary>
        /// Get a worklist for the currently logged on user using worklist criteria to filter.
        /// </summary>
        /// <param name="worklistCriteria">Work list criteria object that allows for worklist filtering</param>
        /// <returns>K2 Work list</returns>
        public Worklist GetWorkList(WorklistCriteria worklistCriteria)
        {
            return connection.OpenWorklist(worklistCriteria);
        }

        /// <summary>
        /// Check if a datafield exists in a given datafield collections
        /// </summary>
        /// <param name="dataFields">Data Field collection</param>
        /// <param name="fieldName">Name of the datafield to look for</param>
        /// <returns></returns>
        private bool PropertyDataFieldExists(DataFields dataFields, string fieldName)
        {
            bool fieldExists = false;
            foreach (DataField dataField in dataFields)
            {
                if (dataField.Name == fieldName)
                {
                    fieldExists = true;
                }
            }
            return fieldExists;
        }

        /// <summary>
        /// Get a list of available actions for a worklist item
        /// </summary>
        /// <param name="serialNumber">Worklist Serial Number</param>
        /// <param name="alloc">allocate this worklist item or not</param>
        /// <returns>List of available actions</returns>
        public List<string> GetWorklistActions(string serialNumber, bool alloc)
        {
            //TODO: Test this method
            List<string> actions = new List<string>();
            WorklistItem worklistItem = null;

            try
            {
                worklistItem = connection.OpenWorklistItem(serialNumber, "ASP", alloc);

                foreach (SourceCode.Workflow.Client.Action action in worklistItem.Actions)
                {
                    actions.Add(action.Name);
                }
            }
            catch
            {
                throw;
            }
            return actions;
        }

        /// <summary>
        /// Action a worklist item
        /// </summary>
        /// <param name="serialNumber">Worklist Serial Number</param>
        /// <param name="action">Action to apply to the workflow</param>
        public void ActionWorklistItem(string serialNumber, string action)
        {
            WorklistItem worklistItem = connection.OpenWorklistItem(serialNumber, "ASP", true);
            worklistItem.Actions[action].Execute();
        }

        /// <summary>
        /// Check if a datafield exits in the process instance
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="dataFieldName"></param>
        /// <returns></returns>
        private bool DataFieldExists(ref ProcessInstance processInstance, string dataFieldName)
        {
            bool exists = false;
            foreach (DataField dataField in processInstance.DataFields)
            {
                if (dataField.Name == dataFieldName)
                {
                    exists = true;
                    break;
                }
            }
            return exists;
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
                if (connection != null)
                {
                    connection.Dispose();
                }
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