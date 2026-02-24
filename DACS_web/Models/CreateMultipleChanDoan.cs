namespace DACS_web.Models
{
    public class CreateMultipleChanDoan
    {
        public string MaHoSo { get; set; }
        public string[] LoaiChanDoans { get; set; }
        public string[] MaICDs { get; set; }
        public string[] ViTris { get; set; }
        public string[] MoTas { get; set; }
    }
}
