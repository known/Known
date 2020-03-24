namespace Known.Drawing
{
    /// <summary>
    /// 图片选项。
    /// </summary>
    public class ImageOption
    {
        /// <summary>
        /// 取得或设置 X 轴位置。
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 取得或设置 Y 轴位置。
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 取得或设置宽度。
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 取得或设置高度。
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 取得或设置水印文字。
        /// </summary>
        public string WatermarkText { get; set; }
    }
}
