namespace Known.Drawing
{
    /// <summary>
    /// 图片参数选项。
    /// </summary>
    public class ImageOption
    {
        /// <summary>
        /// 取得或设置X位置。
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// 取得或设置Y位置。
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
        /// 取得或设置图片水印文本。
        /// </summary>
        public string WatermarkText { get; set; }
    }
}
