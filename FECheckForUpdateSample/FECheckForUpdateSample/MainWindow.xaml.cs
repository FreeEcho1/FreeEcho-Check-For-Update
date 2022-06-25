namespace FECheckForUpdateSample;

public partial class MainWindow : System.Windows.Window
{
    public MainWindow()
    {
        InitializeComponent();

        try
        {
            MethodComboBox.SelectedIndex = 0;
            String1TextBox.Text = System.Environment.GetCommandLineArgs()[0];

            MethodComboBox.SelectionChanged += MethodComboBox_SelectionChanged;
            UpdateCheckButton.Click += UpdateCheckButton_Click;
        }
        catch
        {
        }
    }

    /// <summary>
    /// 「確認方法」ComboBoxの「SelectionChanged」イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MethodComboBox_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e
        )
    {
        try
        {
            switch ((string)MethodComboBox.SelectedItem)
            {
                case "数値でバージョン確認":
                    String1Label.Content = "アプリケーションバージョン";
                    String2Label.Content = "確認するバージョン";
                    break;
                case "ファイルとURLでバージョン確認":
                case "ファイルとURLでバージョン確認 (最新バージョン表示)":
                    String1Label.Content = "アプリケーションファイルのパス";
                    String2Label.Content = "URL";
                    break;
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// 「更新確認」Buttonの「Click」イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateCheckButton_Click(
        object sender,
        System.Windows.RoutedEventArgs e
        )
    {
        try
        {
            FreeEcho.FECheckForUpdate.CheckForUpdateResult result = FreeEcho.FECheckForUpdate.CheckForUpdateResult.Fail;
            FreeEcho.FECheckForUpdate.VersionInformation versionNumber = null;

            switch ((string)MethodComboBox.SelectedItem)
            {
                case "数値でバージョン確認":
                    {
                        FreeEcho.FECheckForUpdate.VersionInformation versionNumber1 = null;
                        FreeEcho.FECheckForUpdate.VersionInformation versionNumber2 = null;
                        versionNumber1 = FreeEcho.FECheckForUpdate.CheckForUpdate.StringFromNumber(String1TextBox.Text);
                        versionNumber2 = FreeEcho.FECheckForUpdate.CheckForUpdate.StringFromNumber(String2TextBox.Text);
                        result = FreeEcho.FECheckForUpdate.CheckForUpdate.VersionCompare(versionNumber1, versionNumber2);
                    }
                    break;
                case "ファイルとURLでバージョン確認":
                    result = FreeEcho.FECheckForUpdate.CheckForUpdate.CheckForUpdateFileURL(String1TextBox.Text, String2TextBox.Text, (bool)CheckBetaCheckBox.IsChecked);
                    break;
                case "ファイルとURLでバージョン確認 (最新バージョン表示)":
                    result = FreeEcho.FECheckForUpdate.CheckForUpdate.CheckForUpdateFileURL(String1TextBox.Text, String2TextBox.Text, out versionNumber, (bool)CheckBetaCheckBox.IsChecked);
                    break;
            }

            switch (result)
            {
                case FreeEcho.FECheckForUpdate.CheckForUpdateResult.Fail:
                    System.Windows.MessageBox.Show("失敗");
                    break;
                case FreeEcho.FECheckForUpdate.CheckForUpdateResult.LatestVersion:
                    System.Windows.MessageBox.Show("最新バージョン");
                    break;
                case FreeEcho.FECheckForUpdate.CheckForUpdateResult.NotLatestVersion:
                    System.Windows.MessageBox.Show("最新バージョンがある" + ((versionNumber == null) ? "" : " - " + versionNumber.Major + "." + versionNumber.Minor + "." + versionNumber.Build + "." + versionNumber.Revision + (versionNumber.Beta ? " Beta" : "")));
                    break;
            }
        }
        catch
        {
        }
    }
}
