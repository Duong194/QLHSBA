namespace DACS_web.Models
{
    public static class HocViNhanVien
    {
        public static List<string> GetAllHocVi()
        {
            return new List<string>
            {
                "Bác sĩ (BS)",
                "Bác sĩ Chuyên khoa I (BS.CKI)",
                "Bác sĩ Chuyên khoa II (BS.CKII)",
                "Thạc sĩ (ThS)",
                "Tiến sĩ (TS)",
                "Phó Giáo sư - Tiến sĩ (PGS.TS)",
                "Giáo sư - Tiến sĩ (GS.TS)"
            };
        }
    }
}