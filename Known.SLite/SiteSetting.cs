using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Known.SLite
{
    public class SiteSetting
    {
        private static SiteSetting instance;

        private SiteSetting()
        {
            Load();
        }

        public static SiteSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SiteSetting();
                }
                return instance;
            }
        }

        public string SiteName { get; set; }
        public string SiteDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public bool Enabled { get; set; }
        public string FooterHtml { get; set; }

        public void Save()
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "delete from kweb_Settings";
            command.ExecuteOnSubmit();

            command.Text = "insert into kweb_Settings values('SiteName',?SettingValue)";
            command.Parameters.Add("SettingValue", SiteName);
            command.ExecuteOnSubmit();

            command.Text = "insert into kweb_Settings values('SiteDescription',?SettingValue)";
            command.Parameters.Add("SettingValue", SiteDescription);
            command.ExecuteOnSubmit();

            command.Text = "insert into kweb_Settings values('MetaKeywords',?MetaKeywords)";
            command.Parameters.Add("SettingValue", MetaKeywords);
            command.ExecuteOnSubmit();

            command.Text = "insert into kweb_Settings values('MetaDescription',?SettingValue)";
            command.Parameters.Add("SettingValue", MetaDescription);
            command.ExecuteOnSubmit();

            command.Text = "insert into kweb_Settings values('Enabled',?SettingValue)";
            command.Parameters.Add("SettingValue", Enabled ? "Y" : "N");
            command.ExecuteOnSubmit();

            command.Text = "insert into kweb_Settings values('FooterHtml',?SettingValue)";
            command.Parameters.Add("SettingValue", FooterHtml);
            command.ExecuteOnSubmit();

            DbHelper.Default.SubmitChanges();
        }

        private void Load()
        {
            var command = DbHelper.Default.CreateCommand();
            command.Text = "select * from kweb_Settings";
            var data = command.ToTable();
            var dictionary = data.ToDictionary<string, string>("SettingName", "SettingValue");
            SiteName = dictionary.GetValue("SiteName");
            SiteDescription = dictionary.GetValue("SiteDescription");
            MetaKeywords = dictionary.GetValue("MetaKeywords");
            MetaDescription = dictionary.GetValue("MetaDescription");
            Enabled = dictionary.GetValue("Enabled", "Y") == "Y";
            FooterHtml = dictionary.GetValue("FooterHtml");
        }
    }
}
