using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Known.Web.Admin
{
    public partial class UpFileList : AdminPage1
    {
        protected string CurrentPath;
        protected DirectoryInfo CurrentDirectory;
        protected string AllowFileExtension = "jpg,jpeg,gif,png,bmp,ico,rar,zip,7z,txt,html,js,css,chm,doc,docx,xls,xlsx,csv,ppt,pptx,psd,pdf,swf,mp3,wma";
        protected string FileName;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetPageTitle("附件管理");

            FileName = Path.GetFileName(Request.Url.AbsolutePath);
            CurrentPath = Request.Get<string>("path");
            CheckPath();

            var operate = Request.Get<string>("operate");
            if (operate == "delete")
            {
                Delete();
            }

            var result = Request.Get<int>("result");
            if (result == 1)
            {
                ShowError("同名文件已存在，上传取消！");
            }
            else if (result == 2)
            {
                ShowMessage("上传成功！");
            }
            else if (result == 3)
            {
                ShowMessage("删除成功！");
            }
            else if (result == 444)
            {
                ShowMessage("权限不够！");
            }

            
        }

        protected void CheckPath()
        {
            if (string.IsNullOrEmpty(CurrentPath) || CurrentPath.IndexOf("upfiles") == -1)
            {
                CurrentPath = SitePath + "upfiles/";
            }

            var path = Server.MapPath(CurrentPath);
            if (!Directory.Exists(path))
            {
                //Response.Redirect(FileName);
            }

            CurrentDirectory = new DirectoryInfo(path);
        }

        protected void Delete()
        {
            string deletepath = Request.Get<string>("deletepath");
            string category = Request.Get<string>("category");

            string return_deletepath = deletepath.Substring(0, deletepath.LastIndexOf('/') + 1);

            //if (PageUtils.CurrentUser.Type != (int)UserType.Administrator)
            //{
            //    Response.Redirect(FileName + "?path=" + return_deletepath + "&result=444");
            //}

            if (deletepath.IndexOf("upfiles") != -1)
            {
                if (category.IndexOf("directory") != -1)
                {
                    Directory.Delete(Server.MapPath(deletepath), true);
                }
                else if (category.IndexOf("archive") != -1)
                {
                    File.Delete(Server.MapPath(deletepath));
                }
            }

            Response.Redirect(FileName + "?path=" + return_deletepath + "&result=3");
        }

        protected string GetPathUrl()
        {
            string path2 = CurrentPath.Substring(1, CurrentPath.Length - 2);
            string[] tempPath = path2.Split('/');
            string temp = "/";
            string pathLink = SitePath.TrimEnd('/');

            for (int i = 0; i < tempPath.Length; i++)
            {
                temp += tempPath[i] + "/";
                if (i == 0 && SitePath.Length > 1) //有虚拟目录
                {
                    continue;
                }
                pathLink += string.Format("/<a href='{2}?path={0}'>{1}</a>", temp, tempPath[i], FileName);
            }
            return pathLink;
        }

        protected string GetFileImage(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".gif":
                case ".jpg":
                case ".png":
                case ".bmp":
                case ".tif":
                    return "jpg.gif";
                case ".doc":
                case ".docx":
                case ".rtf":
                    return "doc.gif";
                case ".ppt":
                case ".pptx":
                    return "ppt.gif";
                case ".xls":
                case ".xlsx":
                case ".csv":
                    return "xls.gif";
                case ".pdf":
                    return "pdf.gif";
                case ".rar":
                case ".zip":
                case ".cab":
                case ".7z":
                    return "rar.gif";
                case ".wav":
                case ".wmv":
                case ".wma":
                case ".mpeg":
                case ".avi":
                case ".mp3":
                    return "wma.gif";
                case ".ini":
                case ".txt":
                case ".css":
                case ".js":
                case ".htm":
                case ".html":
                case ".xml":
                case ".h":
                case ".c":
                case ".php":
                case ".vb":
                case ".cpp":
                case ".cs":
                case ".aspx":
                case ".asm":
                case ".sln":
                case ".vs":
                    return "txt.gif";
                case ".fla":
                case ".flv":
                case ".swf":
                    return "swf.gif";
                case ".psd":
                    return "psd.gif";
                case ".chm":
                    return "chm.gif";
                case ".dll":
                case ".exe":
                case ".msi":
                case ".db":
                    return "exe.gif";
                default: return "default.gif";
            }
        }

        protected bool IsImage(string ext)
        {
            if (!string.IsNullOrEmpty(ext))
            {
                ext = ext.ToLower();
            }
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp" || ext == ".png")
            {
                return true;
            }
            return false;
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            HttpPostedFile postedFile = Request.Files[0];
            string uploadPath = SitePath + "upfiles/" + DateTime.Now.ToString("yyyyMM") + "/"; //文件保存相对路径
            string saveDirectory = Server.MapPath(uploadPath); //文件保存绝对文件夹
            //string waterPath = Server.MapPath("../common/images/watermark.gif");//待改

            string fileName = Path.GetFileName(postedFile.FileName); //文件名
            fileName = fileName.Replace(" ", "");
            fileName = fileName.Replace("%", "");
            fileName = fileName.Replace("&", "");
            fileName = fileName.Replace("#", "");
            fileName = fileName.Replace("'", "");
            fileName = fileName.Replace("+", "");

            string fileExtension = Path.GetExtension(postedFile.FileName); //文件后缀
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName); //没有后缀的文件名

            if (postedFile.ContentLength == 0)
            {
                ShowError("请选择要上传的文件!");
                return;
            }

            string[] fileExts = AllowFileExtension.Split(',');
            bool allow = false;
            foreach (string str in fileExts)
            {
                if (("." + str) == fileExtension)
                {
                    allow = true;
                    break;
                }
            }
            if (allow == false)
            {
                ShowError("您上传的文件格式不被允许!");
                return;
            }

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            int type = Request.Get<int>("rblistType");
            int iCounter = 0;
            int result = 1;
            while (true)
            {
                string fileSavePath = saveDirectory + fileName;
                //string fileSavePath2 = saveDirectory + "da563457-1c3c-4b28-bf73-92f87f930896" + fileName;
                if (File.Exists(fileSavePath))
                {
                    if (type == 1)//跳过
                    {
                        result = 1;
                        break;
                    }
                    else if (type == 2)//重命名
                    {
                        iCounter++;
                        fileName = fileNameWithoutExtension + "(" + iCounter + ")" + fileExtension;
                    }
                    else if (type == 3)//覆盖 
                    {
                        File.Delete(fileSavePath);
                    }
                }
                else
                {
                    //if ((fileExtension == ".gif" || fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".bmp" || fileExtension == ".png") && chkWatermark.Checked)
                    //{
                    //    postedFile.SaveAs(fileSavePath2);

                    //    string newFileName = Path.GetFileNameWithoutExtension(postedFile.FileName) + "w(" + iCounter + ")" + Path.GetExtension(postedFile.FileName);
                    //    string newImagePath = Server.MapPath(uploadPath + newFileName);
                    //    string waterImagePath = Server.MapPath(ConfigHelper.SitePath + "common/images/watermark/" + SettingManager.GetSetting().WatermarkImage);

                    //    if (SettingManager.GetSetting().WatermarkType == 2 && File.Exists(waterImagePath))
                    //    {
                    //        Watermark.CreateWaterImage(fileSavePath2, fileSavePath, SettingManager.GetSetting().WatermarkPosition, waterImagePath, SettingManager.GetSetting().WatermarkTransparency, SettingManager.GetSetting().WatermarkQuality);
                    //    }
                    //    else
                    //    {
                    //        Watermark.CreateWaterText(fileSavePath2, fileSavePath, SettingManager.GetSetting().WatermarkPosition, SettingManager.GetSetting().WatermarkText, SettingManager.GetSetting().WatermarkQuality, SettingManager.GetSetting().WatermarkFontName, SettingManager.GetSetting().WatermarkFontSize);
                    //    }
                    //    File.Delete(fileSavePath2);
                    //}
                    //else
                    //{
                        postedFile.SaveAs(fileSavePath);
                    //}

                    result = 2;
                    break;
                }
            }

            Response.Redirect(FileName + "?path=" + uploadPath + "&result=" + result);
        }
    }
}