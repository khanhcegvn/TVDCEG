namespace TVDCEG
{
    public interface TVDSettting
    {
        string GetFolderPath();
        string GetFileName();
        string GetFullFileName();
        void SaveSetting();
    }
}
