/** ********************************************************************************
* Texture Viewer
* @ 2019 RNGTM
***********************************************************************************/
namespace TextureTool
{
    /** ********************************************************************************
    * @summary 便利系メソッド定義
    ***********************************************************************************/
    internal static class Utils
    {
        private static readonly string[] sizeUnits = { "KB", "MB", "GB", "TB" };

        /** ********************************************************************************
        * @summary バイト数を読みやすいものに変換
        ***********************************************************************************/
        public static string ConvertToHumanReadableSize(ulong sizeInBytes)
        {
            var len = sizeInBytes / 1024.0;
            var order = 0;
            while (len >= 1024 && order < sizeUnits.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizeUnits[order]}";
        }
    }
}