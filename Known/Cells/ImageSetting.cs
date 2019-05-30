namespace Known.Cells
{
    /// <summary>
    /// 图片设置类。
    /// </summary>
    public class ImageSetting
    {
        /// <summary>
        /// 创建一个图片设置类实例。
        /// </summary>
        /// <param name="left">图片左边距。</param>
        /// <param name="top">图片上边距。</param>
        /// <param name="width">图片宽度。</param>
        /// <param name="height">图片高度。</param>
        public ImageSetting(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// 取得图片左边距。
        /// </summary>
        public int Left { get; }

        /// <summary>
        /// 取得图片上边距。
        /// </summary>
        public int Top { get; }

        /// <summary>
        /// 取得图片宽度。
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// 取得图片高度。
        /// </summary>
        public int Height { get; }
    }
}