namespace DACS_web.Models
{
    public static class VaiTroNhanVien
    {
        public const string BacSi = "Bác sĩ";
        public const string YTa = "Y tá";

        public static List<string> GetAllRoles()
        {
            return new List<string> { BacSi, YTa };
        }
    }
}
