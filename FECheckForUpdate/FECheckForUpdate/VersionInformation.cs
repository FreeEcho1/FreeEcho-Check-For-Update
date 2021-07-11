namespace FreeEcho
{
    namespace FECheckForUpdate
    {
        /// <summary>
        /// バージョン情報
        /// </summary>
        public class VersionInformation
        {
            /// <summary>
            /// メジャーバージョン
            /// </summary>
            public int Major
            {
                get;
                set;
            } = 0;
            /// <summary>
            /// マイナーバージョン
            /// </summary>
            public int Minor
            {
                get;
                set;
            } = 0;
            /// <summary>
            /// ビルドバージョン
            /// </summary>
            public int Build
            {
                get;
                set;
            } = 0;
            /// <summary>
            /// リビジョンバージョン
            /// </summary>
            public int Revision
            {
                get;
                set;
            } = 0;
            /// <summary>
            /// ベータバージョンかの値
            /// </summary>
            public bool Beta
            {
                get;
                set;
            } = false;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public VersionInformation()
            {
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="majorVersion">メジャーバージョン</param>
            /// <param name="minorVersion">マイナーバージョン</param>
            /// <param name="buildVersion">ビルドバージョン</param>
            /// <param name="revisionVersion">リビジョンバージョン</param>
            /// <param name="betaVersion">ベータバージョンかの値</param>
            public VersionInformation(
                int majorVersion,
                int minorVersion,
                int buildVersion,
                int revisionVersion,
                bool betaVersion = false
                )
            {
                Major = majorVersion;
                Minor = minorVersion;
                Build = buildVersion;
                Revision = revisionVersion;
                Beta = betaVersion;
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="versionInformation">バージョン情報</param>
            public VersionInformation(
                VersionInformation versionInformation
                )
            {
                Major = versionInformation.Major;
                Minor = versionInformation.Minor;
                Build = versionInformation.Build;
                Revision = versionInformation.Revision;
                Beta = versionInformation.Beta;
            }
        }
    }
}
