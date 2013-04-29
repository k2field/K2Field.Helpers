using System;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace K2Field.Helpers.Core.Code
{
    [Serializable]
    public class K2DBActions
    {
        public K2DBAction DBAction { get; set; }

        public enum K2DBAction
        {
            None,
            Create,
            Update,
            Delete
        }
    }

    public class K2SmartObjectClient
    {
        public K2SmartObjectDAL SMOClient;

        #region ctor

        /// <summary>
        /// when creating an instance of this class, it automatically creates the connection.
        /// </summary>
        public K2SmartObjectClient(string k2Server)
        {
            try
            {
                SMOClient = new K2SmartObjectDAL(k2Server);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public K2SmartObjectClient(string k2Server, uint port)
        {
            try
            {
                SMOClient = new K2SmartObjectDAL(k2Server, (int)port);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Closes the current server connection if its open
        /// </summary>
        public void CloseConnection()
        {
            SMOClient.CloseServerConnection();
            SMOClient = null;
        }

        #endregion Other Methods

        #region Create Methods

        /// <summary>
        /// Creates a SmartObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">class of smartobject object </param>
        /// <returns>stringly typed object </returns>
        public T SmartObjectCreate<T>(object data)
        {
            return (T)SmartObjectCreate(data);
        }

        /// <summary>
        /// Saves the data object. 
        /// </summary>
        /// <param name="data">This objects class must be called the same as the smartobject</param>
        /// <returns></returns>
        public object SmartObjectCreate(object data)
        {
            return SmartObjectCreate(data, data.GetType().Name);
        }

        public object SmartObjectCreate(object data, string smartObject)
        {
            return SmartObjectCreate(data, smartObject, Enums.SmartObjectMethods.Create.ToString());
        }

        public object SmartObjectCreate(object data, string smartObject, string smartObjectMethod)
        {
            Dictionary<string, object> InputData = Converters.ConvertClassToDictionary(data);

            Dictionary<string, object> returnCollection = SMOClient.CallSmartObjectExecuteMethod(smartObject, smartObjectMethod, InputData);

            return Converters.ConvertDictionaryToClass(data.GetType(), returnCollection);
        }

        //public string SmartObjectCreate(object data,string smartObject)
        //{
        //    Dictionary<string, object> InputData = Converters.ConvertClassToDictionary(data);

        //    Dictionary<string, object> returnCollection = smartObjectClient.CallSmartObjectExecuteMethod(smartObject, Enums.SmartObjectMethods.Create.ToString(), InputData);

        //    if (returnCollection.Count > 0)
        //    {
        //        return returnCollection.Values.First().ToString();
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}

        //public string SmartObjectCreate(object data, string primaryKeyField, string smartObject)
        //{
        //    return SmartObjectCreate(data, primaryKeyField, smartObject, Enums.SmartObjectMethods.Create.ToString());
        //}

        //public string SmartObjectCreate(object data, string primaryKeyField, string smartObject, string smartObjectMethod)
        //{
        //    Dictionary<string, object> InputData = Converters.ConvertClassToDictionary(data);

        //    Dictionary<string, object> returnCollection = smartObjectClient.CallSmartObjectExecuteMethod(smartObject, smartObjectMethod, InputData);

        //    if (!returnCollection.ContainsKey(primaryKeyField))
        //    {
        //        throw new Exception("Unable to locate '" + primaryKeyField + "' in the return collection ");
        //    }
        //    return returnCollection[primaryKeyField].ToString();
        //}

        /// <summary>
        /// Call a smartobject Create Method
        /// </summary>
        /// <param name="inputData">Data to insert</param>
        /// <param name="primaryKeyField">Primary key of the Smartoject</param>
        /// <param name="smartObject">Name of the smartobject</param>
        /// <returns>The ID of the record that this creates</returns>
        public string SmartObjectCreate(Dictionary<string, object> inputData, string primaryKeyField, string smartObject)
        {
            return SmartObjectCreate(inputData, primaryKeyField, smartObject, Enums.SmartObjectMethods.Create.ToString());
        }

        /// <summary>
        /// Call a smartobject Create Method
        /// </summary>
        /// <param name="inputData">Data to insert</param>
        /// <param name="primaryKeyField">Primary key of the Smartoject</param>
        /// <param name="smartObject">Name of the smartobject</param>
        /// <param name="smartObjectMethod">Name of the custom Create Method</param>
        /// <returns>The ID of the record that this creates</returns>
        public string SmartObjectCreate(Dictionary<string, object> inputData, string primaryKeyField, string smartObject, string smartObjectMethod)
        {
            Dictionary<string, object> returnCollection = SMOClient.CallSmartObjectExecuteMethod(smartObject, smartObjectMethod, inputData);

            if (!returnCollection.ContainsKey(primaryKeyField))
            {
                throw new Exception("Unable to locate '" + primaryKeyField + "' in the return collection ");
            }
            return returnCollection[primaryKeyField].ToString();
        }

        #endregion Create Methods

        #region Save Methods

        public object SmartObjectSave(object data)
        {
            return SmartObjectSave(data, data.GetType().Name, Enums.SmartObjectMethods.Save.ToString());
        }

        public object SmartObjectSave(object data, string smartObject)
        {
            return SmartObjectSave(data, smartObject, Enums.SmartObjectMethods.Save.ToString());
        }

        public object SmartObjectSave(object data, string smartObject, string smartObjectMethod)
        {
            Dictionary<string, object> obj = Code.Converters.ConvertClassToDictionary(data);
            var returnitems = SMOClient.CallSmartObjectExecuteMethod(smartObject, smartObjectMethod, obj);
            return Converters.ConvertDictionaryToClass(data.GetType(), returnitems);
        }

        /// <summary>
        /// Call a smartobject Save Method
        /// </summary>
        /// <param name="inputData">Data to insert</param>
        /// <param name="smartObject">Name of the smartobject</param>
        public void SmartObjectSave(Dictionary<string, object> inputData, string smartObject)
        {
            SmartObjectSave(inputData, smartObject, Enums.SmartObjectMethods.Save.ToString());
        }
        /// <summary>
        /// Call a smartobject Save Method
        /// </summary>
        /// <param name="inputData">Data to insert</param>
        /// <param name="smartObject">Name of the smartobject</param>
        /// <param name="smartObjectMethod">Name of the custom Save Method</param>
        public void SmartObjectSave(Dictionary<string, object> inputData, string smartObject, string smartObjectMethod)
        {
            SMOClient.CallSmartObjectExecuteMethod(smartObject.ToString(), smartObjectMethod.ToString(), inputData);
        }

        #endregion Save Methods

        #region Load Methods

        public T SmartObjectLoad<T>(object data)
        {
            return (T)SmartObjectLoad(data);
        }

        public T SmartObjectLoad<T>(object data, string smartObject)
        {
            return (T)SmartObjectLoad(data,smartObject);
        }

        public T SmartObjectLoad<T>(object data, string smartObject, string smartObjectMethod)
        {
            return (T)SmartObjectLoad(data, smartObject, smartObjectMethod);
        }

        public object SmartObjectLoad(object data)
        {
            return SmartObjectLoad(data, data.GetType().Name);
        }

        public object SmartObjectLoad(object data, string smartObject)
        {
            return SmartObjectLoad(data, smartObject, Enums.SmartObjectMethods.Load.ToString());
        }

        public object SmartObjectLoad(object data, string smartObject, string smartObjectMethod)
        {
            Dictionary<string, object> obj = SMOClient.CallSmartObjectExecuteMethod(smartObject, smartObjectMethod, Code.Converters.ConvertClassToDictionary(data));
            return Code.Converters.ConvertDictionaryToClass(data.GetType(), obj);
        }

        /// <summary>
        /// Call a smartobject Load Method
        /// </summary>
        /// <param name="criteria">criteria</param>
        /// <param name="smartObject">Name of the smartobject</param>
        /// <returns>DataRow of results</returns>
        public Dictionary<string, object> SmartObjectLoad(Dictionary<string, object> criteria, string smartObject)
        {
            return SmartObjectLoad(criteria, smartObject, Enums.SmartObjectMethods.Load.ToString());
        }
        /// <summary>
        /// Call a smartobject Load Method
        /// </summary>
        /// <param name="criteria">criteria</param>
        /// <param name="smartObject">Name of the smartobject</param>
        /// <param name="smartObjectMethod">Name of the custom Save Method</param>
        /// <returns>Datarow of results</returns>
        public Dictionary<string, object> SmartObjectLoad(Dictionary<string, object> criteria, string smartObject, string smartObjectMethod)
        {
            return SMOClient.CallSmartObjectExecuteMethod(smartObject, smartObjectMethod, criteria);
        }

        #endregion Load Methods

        #region Delete Methods

        public void SmartObjectDelete(object item)
        {
            this.SmartObjectDelete(item, item.GetType().Name);
        }

        public void SmartObjectDelete(object item, string smartObject)
        {
            this.SmartObjectDelete(item, smartObject, Enums.SmartObjectMethods.Delete.ToString());
        }

        public void SmartObjectDelete(object item, string smartObject, string smartObjectMethod)
        {
            this.SMOClient.CallSmartObjectExecuteMethod(smartObject.ToString(), smartObjectMethod, Converters.ConvertClassToDictionary(item));
        }

        /// <summary>
        /// Call a smartobject Delete Method
        /// </summary>
        /// <param name="criteria">Criteria</param>
        /// <param name="smartObject">Name of the smartobject</param>
        public void SmartObjectDelete(Dictionary<string, object> criteria, string smartObject)
        {
            SmartObjectDelete(criteria, smartObject, Enums.SmartObjectMethods.Delete.ToString());
        }

        /// <summary>
        /// Call a smartobject Delete Method
        /// </summary>
        /// <param name="criteria">Criteria</param>
        /// <param name="smartObject">Name of the smartobject</param>
        /// <param name="smartObjectMethod">Name of the custom Delete Method</param>
        public void SmartObjectDelete(Dictionary<string, object> criteria, string smartObject, string smartObjectMethod)
        {
            SMOClient.CallSmartObjectExecuteMethod(smartObject.ToString(), smartObjectMethod, criteria);
        }

        #endregion Delete Methods

        #region GetList Methods

        /// <summary>
        /// Call a smartobject GetList Method
        /// </summary>
        /// <param name="criteria">criteria</param>
        /// <param name="smartObject">Name of the smartobject</param>
        /// <returns>Datatable of results</returns>
        public DataTable SmartObjectGetList(Dictionary<string, object> criteria, string smartObject)
        {
            return SmartObjectGetList(criteria, smartObject, Enums.SmartObjectMethods.GetList.ToString());
        }

        /// <summary>
        /// Call a smartobject GetList Method
        /// </summary>
        /// <param name="criteria">criteria</param>
        /// <param name="smartObject">Name of the smartobject</param>
        /// <param name="smartObjectMethod">Name of the custom Getlist Method</param>
        /// <returns>Datatable of results</returns>
        public DataTable SmartObjectGetList(Dictionary<string, object> criteria, string smartObject, string smartObjectMethod)
        {
            return SMOClient.CallSmartObjectListMethod(smartObject, smartObjectMethod, criteria);
        }

        /// <summary>
        /// Call a smartobject GetList Method
        /// </summary>
        /// <param name="criteria">criteria</param>
        /// <param name="smartObject">Name of the smartobject</param>
        /// <param name="smartObjectMethod">Name of the custom Getlist Method</param>
        /// <returns>Datatable of results</returns>
        public DataTable SmartObjectGetList(Dictionary<string, object> criteria, string smartObject, string smartObjectMethod, bool useFilter)
        {
            return SMOClient.CallSmartObjectListMethod(smartObject, smartObjectMethod, criteria, useFilter);
        }

        public List<T> SmartObjectGetList<T>(object criteriaClass)
        {
            return SmartObjectGetList(criteriaClass).Cast<T>().ToList();
        }

        public List<T> SmartObjectGetList<T>(object criteriaClass, string smartObject)
        {
            return SmartObjectGetList(criteriaClass,smartObject).Cast<T>().ToList();
        }

        public List<T> SmartObjectGetList<T>(object criteriaClass, string smartObject,string smartObjectMethod)
        {
            return SmartObjectGetList(criteriaClass, smartObject, smartObjectMethod).Cast<T>().ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteriaClass"></param>
        /// <returns></returns>
        public List<object> SmartObjectGetList(object criteriaClass)
        {
            return this.SmartObjectGetList(criteriaClass, criteriaClass.GetType().Name);
        }

        public List<object> SmartObjectGetList(object criteriaClass, string smartObject)
        {
            return SmartObjectGetList(criteriaClass, smartObject, Enums.SmartObjectMethods.GetList.ToString());
        }

        public List<object> SmartObjectGetList(object criteriaClass, string smartObject, string smartObjectMethod)
        {
            var dt = SMOClient.CallSmartObjectListMethod(smartObject, smartObjectMethod, Converters.ConvertClassToDictionary(criteriaClass));
            return Converters.ConvertDataTableToClass(criteriaClass.GetType(), dt);
        }

        #endregion GetList Methods

        #region ADO Query Methods

        /// <summary>
        /// Smartobject ADO query
        /// </summary>
        /// <param name="sql">SQL statment to execute </param>
        /// <param name="parameters">query paramters, if used then command type of StoredProcedure is used</param>
        /// <returns>A Datatable of results </returns>
        public DataTable SmartObjectADOQuery(string sql)
        {
            return SmartObjectADOQuery(sql, new Dictionary<string, object>());
        }

        /// <summary>
        /// Smartobject ADO query
        /// </summary>
        /// <param name="sql">SQL statment to execute </param>
        /// <param name="parameters">query paramters, if used then command type of StoredProcedure is used</param>
        /// <returns>A Datatable of results </returns>
        public DataTable SmartObjectADOQuery(string sql, Dictionary<string, object> parameters)
        {
            return SmartObjectADOQuery(sql, parameters, false);
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
            return SMOClient.SmartObjectADOQuery(sql, parameters, directExecution);
        }

        #endregion ADO Query Methods

        #region Process List Methods

        public List<T> SmartObjectProcessList<T>(object data, string createMethod, string updateMethod, string deleteMethod)
        {
            var list = data as List<T>;
            var returnlist = data as List<T>; // return list of classes

            for (int i = 0; i < list.Count; i++)
            {
                var action = list[i] as K2Field.Helpers.Core.Code.K2DBActions;

                if (action == null)
                {
                    throw new Exception("SMO class does not inherit 'K2Field.Helpers.Core.Code.K2DBActions'");
                }

                switch (action.DBAction)
                {
                    case K2DBActions.K2DBAction.Create:
                        list[i] = (T)SmartObjectCreate(list[i], list[i].GetType().Name, createMethod);
                        break;

                    case K2DBActions.K2DBAction.Delete:
                        SmartObjectDelete(list[i], list[i].GetType().Name, deleteMethod);
                        list.RemoveAt(i);
                        i--;
                        break;

                    case K2DBActions.K2DBAction.Update:
                        list[i] = (T)SmartObjectSave(list[i], list[i].GetType().Name, updateMethod);
                        break;
                }
            }

            return list;
        }

        public List<T> SmartObjectProcessList<T>(object data)
        {
            return SmartObjectProcessList<T>(data, Enums.SmartObjectMethods.Create.ToString(), Enums.SmartObjectMethods.Save.ToString(), Enums.SmartObjectMethods.Delete.ToString());
        }

        /// <summary>
        /// This will set the action on each item
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        /// <param name="data">List of Classes</param>
        /// <param name="forceDBActionToAll">force an action for all records</param>
        /// <returns>list of T</returns>
        public List<T> SmartObjectProcessList<T>(object data, Code.K2DBActions.K2DBAction forceDBActionToAll)
        {
            var list = data as List<T>;

            for (int i = 0; i < list.Count; i++)
            {
                var record = list[i];
                var action = record as K2Field.Helpers.Core.Code.K2DBActions;

                if (action == null)
                {
                    throw new Exception("SMO class does not inherit 'K2Field.Helpers.Core.Code.K2DBActions'");
                }
                else
                {
                    action.DBAction = forceDBActionToAll;
                }

            }

            return SmartObjectProcessList<T>(list);
        }

        #endregion Process List Methods

    }
}