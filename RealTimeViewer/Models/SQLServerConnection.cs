using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RealTimeViewer.Models
{
    public class SQLServerConnection : BindableBase
    {
        #region プロパティ
        public ObservableCollection<string> NameList { get; set; }
        public ObservableCollection<string> TableNameList { get; set; }
        public DataTable Table { get; set; }
        #endregion

        #region フィールド
        private SqlConnectionStringBuilder _connection;
        #endregion

        #region コンストラクタ

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="integratedSecurity">True：WIndows認証  False:SQL Server認証</param>
        public SQLServerConnection(string serverName, string userID, string password, bool integratedSecurity)
        {
            NameList = new ObservableCollection<string>();
            Table = new DataTable();

            _connection = new SqlConnectionStringBuilder
            {
                DataSource = serverName,
                UserID = userID,
                Password = password,
                IntegratedSecurity = integratedSecurity,
            };
        }
        #endregion

        #region メソッド

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task GetDBNameListAsync()
        {
            var tempList = new ObservableCollection<string>();
            using (var cn = new SqlConnection(_connection.ConnectionString))
            using (var cm = new SqlCommand("SELECT [name] FROM sys.databases", cn))
            {
                await cn.OpenAsync();
                using (IDataReader reader = await cm.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        tempList.Add(reader["name"].ToString());
                    }
                }
            }
            NameList.Clear();
            NameList.AddRange(tempList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="catalogName"></param>
        /// <returns></returns>
        public async Task GetTableNameListAsync(string catalogName)
        {
            var tempList = new ObservableCollection<string>();
            _connection.InitialCatalog = catalogName;
            using (var cn = new SqlConnection(_connection.ConnectionString))
            using (var cm = new SqlCommand("SELECT [name] FROM sysobjects WHERE xtype = 'u'", cn))
            {
                await cn.OpenAsync();
                using (IDataReader reader = await cm.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        tempList.Add(reader["name"].ToString());
                    }
                }
            }

            NameList.Clear();
            NameList.AddRange(tempList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="catalogName"></param>
        /// <returns></returns>
        public async Task GetDataTableAsync(string tableName)
        {
            using (var cn = new SqlConnection(_connection.ConnectionString))
            using (var cm = new SqlCommand("SELECT * FROM " + tableName, cn))
            {
                await cn.OpenAsync();
                using (IDataReader reader = await cm.ExecuteReaderAsync())
                {
                    Table.BeginLoadData();
                    Table.Clear();
                    Table.Load(reader);
                    Table.EndLoadData();
                }
            }
        }
        #endregion
    }
}