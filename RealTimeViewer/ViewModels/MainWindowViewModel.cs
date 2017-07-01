using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Data;
using RealTimeViewer.Models;
using System.Threading.Tasks;
using System;
using System.Data;

namespace RealTimeViewer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region プロパティ
        public ObservableCollection<string> Collection { get; set; }
        public ObservableCollection<string> NameList { get; set; }
        public DataTable Table { get; set; }
        public string SelectedItem { get; set; }
        #endregion

        #region コマンド
        public DelegateCommand DBNameListCommand { get; set; }
        public DelegateCommand SelectedCommand { get; set; }
        #endregion

        #region フィールド
        private DisplayType _displayType = DisplayType.None;
        private SQLServerConnection _connection;
        #endregion

        #region コンストラクタ
        public MainWindowViewModel()
        {
            DBNameListCommand = new DelegateCommand(() => GetDBNameListAsync());
            SelectedCommand = new DelegateCommand(
                () => GetTableNameListBySelectedDBAsync(SelectedItem),
                () => SelectedItem != null);

            //接続文字列の設定 (後で外出ししたい)
            _connection = new SQLServerConnection("MYCOMPUTER", "sa", "P@ssw0rd", true);
            //_connection = new SQLServerConnection("tkyvr04613", "sa", "P@ssw0rd", false);

            //Modelの値を受け取る
            Collection = _connection.NameList;
            Table = _connection.Table;

            Table.Columns.Add("ID");
            Table.Columns.Add("FullName");

            //DB名を取得
            Task.Run(async () => await _connection.GetDBNameListAsync());

            //UIへ非同期的にバインドさせる
            BindingOperations.EnableCollectionSynchronization(Collection, new object());
        }
        #endregion

        #region メソッド

        /// <summary>
        /// 繋いでいるサーバの持っているDB名を列挙
        /// </summary>
        private async void GetDBNameListAsync()
        {
            await _connection.GetDBNameListAsync();
            _displayType = DisplayType.DBNameList;
        }

        /// <summary>
        /// 繋いでいるDBの持っているテーブル名を列挙
        /// </summary>
        private async void GetTableNameListBySelectedDBAsync(string selectedName)
        {
            if (_displayType == DisplayType.DBNameList
                || _displayType == DisplayType.None)
            {
                await _connection.GetTableNameListAsync(selectedName);
            }
            else if (_displayType == DisplayType.TableNameList)
            {
                await _connection.GetDataTableAsync(selectedName);
            }
            var hoge = Table;


            _displayType = DisplayType.TableNameList;
        }
        #endregion

        #region Enum
        private enum DisplayType
        {
            None,
            DBNameList,
            TableNameList,
        }
        #endregion
    }
}
