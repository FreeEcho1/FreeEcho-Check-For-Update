namespace FreeEcho.FECheckForUpdate;

/// <summary>
/// 更新確認の結果
/// </summary>
public enum CheckForUpdateResult : int
{
    /// <summary>
    /// 更新確認失敗
    /// </summary>
    Fail = 0,
    /// <summary>
    /// 最新バージョン
    /// </summary>
    LatestVersion,
    /// <summary>
    /// 最新バージョンではない
    /// </summary>
    NotLatestVersion,
}
