namespace DACS_web.Models
{
    public static class CaTrucOptions
    {
        public static Dictionary<string, (TimeSpan Start, TimeSpan End)> GetCaTruc()
        {
            return new Dictionary<string, (TimeSpan, TimeSpan)>
            {
                { "Sáng", (new TimeSpan(7, 0, 0), new TimeSpan(12, 0, 0)) },
                { "Chiều", (new TimeSpan(13, 0, 0), new TimeSpan(18, 0, 0)) },
                { "Tối", (new TimeSpan(18, 0, 0), new TimeSpan(22, 0, 0)) },
                { "Đêm", (new TimeSpan(22, 0, 0), new TimeSpan(7, 0, 0)) }
            };
        }

        public static List<string> GetTenCaTruc()
        {
            return new List<string> { "Sáng", "Chiều", "Tối", "Đêm" };
        }
        public static (TimeSpan Start, TimeSpan End) GetGioCaTruc(string tenCa)
        {
            var caTruc = GetCaTruc();
            return caTruc.ContainsKey(tenCa) ? caTruc[tenCa] : (TimeSpan.Zero, TimeSpan.Zero);
        }
    }
}