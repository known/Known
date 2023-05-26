/**
* This file is part of the MarkdownSharp package
* For the full copyright and license information,
* view the LICENSE file that was distributed with this source code.
*/
namespace WebSite.Core
{
    public interface IMarkdownExtension
    {
        /// <summary>
        /// Replace inline element
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string Transform(string text);
    }
}