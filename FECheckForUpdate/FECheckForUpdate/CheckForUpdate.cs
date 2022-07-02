namespace FreeEcho.FECheckForUpdate;

/// <summary>
/// 更新確認
/// </summary>
public class CheckForUpdate
{
    /// <summary>
    /// バージョン比較
    /// </summary>
    /// <param name="applicationVersionInformation">アプリケーションのバージョン情報</param>
    /// <param name="checkVersionInformation">確認するバージョン情報</param>
    /// <returns>更新確認の結果</returns>
    public static CheckForUpdateResult VersionCompare(
        VersionInformation applicationVersionInformation,
        VersionInformation checkVersionInformation
        )
    {
        CheckForUpdateResult result;

        if ((applicationVersionInformation.Major == checkVersionInformation.Major) && (applicationVersionInformation.Minor == checkVersionInformation.Minor) && (applicationVersionInformation.Build == checkVersionInformation.Build) && (applicationVersionInformation.Revision == checkVersionInformation.Revision))
        {
            result = CheckForUpdateResult.LatestVersion;
        }
        else if (applicationVersionInformation.Major < checkVersionInformation.Major)
        {
            result = CheckForUpdateResult.NotLatestVersion;
        }
        else if ((applicationVersionInformation.Major == checkVersionInformation.Major) && (applicationVersionInformation.Minor < checkVersionInformation.Minor))
        {
            result = CheckForUpdateResult.NotLatestVersion;
        }
        else if ((applicationVersionInformation.Major == checkVersionInformation.Major) && (applicationVersionInformation.Minor == checkVersionInformation.Minor) && (applicationVersionInformation.Build < checkVersionInformation.Build))
        {
            result = CheckForUpdateResult.NotLatestVersion;
        }
        else if ((applicationVersionInformation.Major == checkVersionInformation.Major) && (applicationVersionInformation.Minor == checkVersionInformation.Minor) && (applicationVersionInformation.Build == checkVersionInformation.Build) && (applicationVersionInformation.Revision < checkVersionInformation.Revision))
        {
            result = CheckForUpdateResult.NotLatestVersion;
        }
        else
        {
            result = CheckForUpdateResult.LatestVersion;
        }

        return (result);
    }

    /// <summary>
    /// アプリケーションファイルとURLから更新確認
    /// </summary>
    /// <param name="filePath">確認するアプリケーションのファイルパス</param>
    /// <param name="url">確認するバージョン情報が書かれたファイルがあるURL</param>
    /// <param name="checkBeta">ベータバージョンも確認するかの値 (確認しない「false」/確認する「true」)</param>
    /// <returns>更新確認の結果</returns>
    public static CheckForUpdateResult CheckForUpdateFileURL(
        string filePath,
        string url,
        bool checkBeta = false
        )
    {
        return (CheckForUpdateFileURL(filePath, url, out _, checkBeta));
    }

    /// <summary>
    /// アプリケーションファイルとURLから更新確認、URLのバージョン番号取得
    /// </summary>
    /// <param name="filePath">確認するアプリケーションのファイルパス</param>
    /// <param name="url">確認するバージョン情報が書かれたファイルがあるURL</param>
    /// <param name="urlVersionInformation">URLから取得したバージョン情報を格納</param>
    /// <param name="checkBeta">ベータバージョンも確認するかの値 (確認しない「false」/確認する「true」)</param>
    /// <returns>更新確認の結果</returns>
    public static CheckForUpdateResult CheckForUpdateFileURL(
        string filePath,
        string url,
        out VersionInformation? urlVersionInformation,
        bool checkBeta = false
        )
    {
        urlVersionInformation = null;

        CheckForUpdateResult result = CheckForUpdateResult.Fail;        // 更新確認の結果

        try
        {
            // URLから文字列取得
            System.Threading.Tasks.Task<string> task = GetWebPageString(url);      // ウェブページの文字列取得のタスク
            System.IO.StringReader sr = new(task.Result);     // 取得した文字列
            int count = 0;      // カウント用
            string[] versionString = new string[2] { "", "" };        // バージョン番号の文字列 (「0」は標準バージョン、「1」はベータバージョン)
            while (-1 < sr.Peek())
            {
                versionString[count++] = sr.ReadLine() ?? "";
                if (versionString.Length <= count)
                {
                    break;
                }
            }
            VersionInformation[] versionInformation = new VersionInformation[2];       // バージョン情報 (「0」は標準バージョン、「1」はベータバージョン)
            if (string.IsNullOrEmpty(versionString[0]) == false)
            {
                versionInformation[0] = StringFromNumber(versionString[0]);
            }
            if (checkBeta && (string.IsNullOrEmpty(versionString[1]) == false))
            {
                versionInformation[1] = StringFromNumber(versionString[1]);
            }

            // ファイルからバージョン情報取得
            System.Diagnostics.FileVersionInfo fileVersionInformation = System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath);     // ファイルバージョン情報

            // 更新確認
            if (versionInformation[0] != null)
            {
                result = VersionCompare(new(fileVersionInformation.FileMajorPart, fileVersionInformation.FileMinorPart, fileVersionInformation.FileBuildPart, fileVersionInformation.FilePrivatePart), versionInformation[0]);
            }
            // ベータバージョンの更新確認
            bool beta = false;      // 最新のベータバージョンがあるかの値
            if (versionInformation[1] != null)
            {
                // ベータバージョンを確認して、通常バージョンとベータバージョンを確認
                CheckForUpdateResult tempResult = VersionCompare(new(fileVersionInformation.FileMajorPart, fileVersionInformation.FileMinorPart, fileVersionInformation.FileBuildPart, fileVersionInformation.FilePrivatePart), versionInformation[1]);
                if (tempResult == CheckForUpdateResult.NotLatestVersion)
                {
                    result = CheckForUpdateResult.NotLatestVersion;
                    tempResult = VersionCompare(versionInformation[0], versionInformation[1]);
                    if (tempResult == CheckForUpdateResult.NotLatestVersion)
                    {
                        beta = true;
                        versionInformation[1].Beta = true;
                    }
                }
            }
            if (beta)
            {
                urlVersionInformation = new(versionInformation[1]);
            }
            else
            {
                urlVersionInformation = new(versionInformation[0]);
            }
        }
        catch
        {
        }

        return (result);
    }

    /// <summary>
    /// ウェブページの文字列を取得
    /// </summary>
    /// <param name="url">URL</param>
    /// <returns>Task</returns>
    private static async System.Threading.Tasks.Task<string> GetWebPageString(
        string url
        )
    {
        System.Net.Http.HttpClient client = new();
        return (await client.GetStringAsync(url).ConfigureAwait(false));
    }

    /// <summary>
    /// バージョン番号の文字列を数値に変換して取得
    /// </summary>
    /// <param name="versionString">バージョン番号の文字列</param>
    /// <returns>バージョン情報</returns>
    public static VersionInformation StringFromNumber(
        string versionString
        )
    {
        VersionInformation versionNumber = new();     // バージョン情報
        int count = 0;      // カウント用
        int startIndex = 0;        // 開始インデックス
        int getInt = 0;        // int型の値

        // メジャーバージョンを取り出す
        for (; count < versionString.Length; count++)
        {
            if ((versionString[count] < '0') || ('9' < versionString[count]))
            {
                break;
            }
        }
        if (startIndex != count)
        {
            if (int.TryParse(versionString.Substring(startIndex, count - startIndex), out getInt))
            {
                versionNumber.Major = getInt;
            }
            else
            {
                return (versionNumber);
            }
        }
        // マイナーバージョンを取り出す
        if (count < versionString.Length)
        {
            count++;
            for (startIndex = count; count < versionString.Length; count++)
            {
                if ((versionString[count] < '0') || ('9' < versionString[count]))
                {
                    break;
                }
            }
            if (startIndex != count)
            {
                if (int.TryParse(versionString.Substring(startIndex, count - startIndex), out getInt))
                {
                    versionNumber.Minor = getInt;
                }
                else
                {
                    return (versionNumber);
                }
            }
        }
        // ビルドバージョンを取り出す
        if (count < versionString.Length)
        {
            count++;
            for (startIndex = count; count < versionString.Length; count++)
            {
                if ((versionString[count] < '0') || ('9' < versionString[count]))
                {
                    count++;
                    break;
                }
            }
            if (startIndex != count)
            {
                if (int.TryParse(versionString.Substring(startIndex, count - startIndex), out getInt))
                {
                    versionNumber.Build = getInt;
                }
                else
                {
                    return (versionNumber);
                }
            }
        }
        // リビジョンバージョンを取り出す
        if (count < versionString.Length)
        {
            count++;
            for (startIndex = count; count < versionString.Length; count++)
            {
                if ((versionString[count] < '0') || ('9' < versionString[count]))
                {
                    count++;
                    break;
                }
            }
            if (startIndex != count)
            {
                if (int.TryParse(versionString.Substring(startIndex, count - startIndex), out getInt))
                {
                    versionNumber.Revision = getInt;
                }
                else
                {
                    return (versionNumber);
                }
            }
        }

        return (versionNumber);
    }
}
