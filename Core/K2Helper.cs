using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using SourceCode.Workflow.Management;
using SourceCode.Workflow.Client;
using K2Field.Helpers.Core.Code;
using System.Security;
using System.Security.Principal;

namespace K2Field.Helpers.Core
{
    public class K2Helper : IDisposable
    {
        private K2SmartObjectClient _SmartObjectClient;
        private K2WorkflowClient _WorkflowClient;
        private WorkflowManagementServer _WorkflowServer;

        private string _K2Server;

        /// <summary>
        /// By default 5252
        /// </summary>
        private uint _WorkflowClientPort;

        /// <summary>
        /// By default 5555
        /// </summary>
        private uint _WorkflowManagementServerPort;

        /// <summary>
        /// By Default K2:
        /// </summary>
        private string _SecurityLabelName { get; set; }

        /// <summary>
        /// Creates an instance of this class, will use the standard ports for Management and Workflow client connections
        /// </summary>
        /// <param name="K2ServerName">K2 server name</param>
        public K2Helper(string K2ServerName)
        {
            _K2Server = K2ServerName;
            _WorkflowClientPort = 5252;
            _WorkflowManagementServerPort = 5555;
            _SecurityLabelName = "K2";
        }

        /// <summary>
        /// Creates an instance of the class with overrides for the workflow client and management server ports
        /// </summary>
        /// <param name="K2ServerName">k2 server name</param>
        /// <param name="WorkflowClientPort">Workflow client port</param>
        /// <param name="WorkflowManagementServerPort">Management serverport</param>
        public K2Helper(string K2ServerName, uint WorkflowClientPort, uint WorkflowManagementServerPort)
        {
            _K2Server = K2ServerName;
            _WorkflowClientPort = WorkflowClientPort;
            _WorkflowManagementServerPort = WorkflowManagementServerPort;
            _SecurityLabelName = "K2";
        }

        /// <summary>
        /// Creates an instance of the class with overrides for the workflow client and management server ports and security label
        /// </summary>
        /// <param name="K2ServerName">k2 server name</param>
        /// <param name="WorkflowClientPort">Workflow client port</param>
        /// <param name="WorkflowManagementServerPort">Management serverport</param>
        /// <param name="SecurityLabelName">security label</param>
        public K2Helper(string K2ServerName, uint WorkflowClientPort, uint WorkflowManagementServerPort, string SecurityLabelName)
        {
            _K2Server = K2ServerName;
            _WorkflowClientPort = WorkflowClientPort;
            _WorkflowManagementServerPort = WorkflowManagementServerPort;
            _SecurityLabelName = SecurityLabelName;
        }

        /// <summary>
        /// Workflow Management Server
        /// </summary>
        /// <returns>Workflow Management Server</returns>
        public WorkflowManagementServer WorkflowServer()
        {
            if (_WorkflowServer == null)
            {
                _WorkflowServer = new WorkflowManagementServer(_K2Server, _WorkflowManagementServerPort);
                _WorkflowServer.Open();
                return _WorkflowServer;
            }
            else
            {
                return _WorkflowServer;
            }
        }

        /// <summary>
        /// Workflow Client
        /// </summary>
        /// <returns>Workflow Client</returns>
        public K2WorkflowClient WorkflowClient()
        {
            if (_WorkflowClient == null)
            {
                _WorkflowClient = new K2WorkflowClient(_K2Server, _WorkflowClientPort, _SecurityLabelName);
                return _WorkflowClient;
            }
            else
            {
                return _WorkflowClient;
            }
        }

        /// <summary>
        /// SmartObject Client
        /// </summary>
        /// <returns>SmartObject Client</returns>
        public K2SmartObjectClient SmartObjectClient()
        {
            if (_SmartObjectClient == null)
            {
                _SmartObjectClient = new K2SmartObjectClient(_K2Server,_WorkflowManagementServerPort);
                return _SmartObjectClient;
            }
            else
            {
                return _SmartObjectClient;
            }
        }

        /// <summary>
        /// Actions a worklist item
        /// </summary>
        /// <param name="serialNumber">Serial number of the worklist item</param>
        /// <param name="action">Action to execute</param>
        /// <returns></returns>
        public void ActionWorklistItem(string serialNumber, string action)
        {
                WorkflowClient().ActionWorklistItem(serialNumber, action);
        }

        /// <summary>
        /// Starts  a K2 process with process data fields.
        /// </summary>
        /// <param name="processName">Process Name</param>
        /// <param name="folio">Folio</param>
        /// <returns>the process instance ID if it work else -1 if there was an error.</returns>
        public virtual int StartK2Process(string processName, string folio)
        {
            return StartK2Process(processName,folio,new Dictionary<string,object>());
        }

        /// <summary>
        /// Starts  a K2 process with process data fields.
        /// </summary>
        /// <param name="processName">Process Name</param>
        /// <param name="folio">Folio</param>
        /// <param name="inputs">Datafield to start the process with</param>
        /// <returns>the process instance ID if it work else -1 if there was an error.</returns>
        public virtual int StartK2Process(string processName, string folio, Dictionary<string,object> inputs)
        {
            return WorkflowClient().StartK2Process(processName, folio, inputs);
        }

        /// <summary>
        /// Starts  a K2 process with process data fields.
        /// </summary>
        /// <param name="processName">Process Name</param>
        /// <param name="folio">Folio</param>
        /// <param name="inputs">Datafield to start the process with</param>
        /// <returns>the process instance ID if it work else -1 if there was an error.</returns>
        public virtual int StartK2Process(string processName, string folio, Dictionary<string, CoreDataField> inputs)
        {
            return WorkflowClient().StartK2Process(processName, folio, inputs);
        }

        public void Dispose()
        {
            //Check the server connections and close if needed.
            if (_SmartObjectClient != null)
            {
                _SmartObjectClient.CloseConnection();
                
                _SmartObjectClient = null;
            }

            //Check the server connections and close if needed.
            if (_WorkflowServer != null)
            {
                if (_WorkflowServer.Connection.IsConnected) _WorkflowServer.Connection.Close();
                _WorkflowServer = null;
            }

            //Check the server connections and close if needed.
            if (_WorkflowClient != null)
            {
                if (_WorkflowClient.connection != null) _WorkflowClient.CloseConnection();
                _WorkflowClient.Dispose();
                _WorkflowClient = null;
            }
        }
    }
}
