CREATE TABLE "SysModule" (
    "Id"          character varying(50)  NOT NULL,
    "CreateBy"    character varying(50)  NOT NULL,
    "CreateTime"  date                   NOT NULL,
    "ModifyBy"    character varying(50),
    "ModifyTime"  date,
    "Version"     integer                NOT NULL,
    "Extension"   text,
    "AppId"       character varying(50),
    "CompNo"      character varying(50)  NOT NULL,
    "ParentId"    character varying(50),
    "Code"        character varying(50),
    "Name"        character varying(50)  NOT NULL,
    "Icon"        character varying(50),
    "Description" character varying(200),
    "Target"      character varying(250),
    "Url"         character varying(200),
    "Sort"        integer                NOT NULL,
    "Enabled"     character varying(50)  NOT NULL,
    "EntityData"  text,
    "FlowData"    text,
    "PageData"    text,
    "FormData"    text,
    "Note"        text,
    PRIMARY KEY ("Id")
);

CREATE TABLE "SysConfig" (
    "AppId"       character varying(50) NOT NULL,
    "ConfigKey"   character varying(50) NOT NULL,
    "ConfigValue" text                  NOT NULL,
    PRIMARY KEY("AppId","ConfigKey")
);

CREATE TABLE "SysSetting" (
    "Id"         character varying(50)  NOT NULL,
    "CreateBy"   character varying(50)  NOT NULL,
    "CreateTime" date                   NOT NULL,
    "ModifyBy"   character varying(50),
    "ModifyTime" date,
    "Version"    integer                NOT NULL,
    "Extension"  text,
    "AppId"      character varying(50)  NOT NULL,
    "CompNo"     character varying(50)  NOT NULL,
    "BizType"    character varying(50)  NOT NULL,
    "BizName"    character varying(250),
    "BizData"    text,
    PRIMARY KEY ("Id")
);

CREATE TABLE "SysLog" (
    "Id"         character varying(50) NOT NULL,
    "CreateBy"   character varying(50) NOT NULL,
    "CreateTime" date                  NOT NULL,
    "ModifyBy"   character varying(50),
    "ModifyTime" date,
    "Version"    integer               NOT NULL,
    "Extension"  text,
    "AppId"      character varying(50) NOT NULL,
    "CompNo"     character varying(50) NOT NULL,
    "Type"       character varying(50),
    "Target"     character varying(50),
    "Content"    text,
    PRIMARY KEY ("Id")
);

CREATE TABLE "SysFile" (
    "Id"         character varying(50)  NOT NULL,
    "CreateBy"   character varying(50)  NOT NULL,
    "CreateTime" date                   NOT NULL,
    "ModifyBy"   character varying(50),
    "ModifyTime" date,
    "Version"    integer                NOT NULL,
    "Extension"  text,
    "AppId"      character varying(50)  NOT NULL,
    "CompNo"     character varying(50)  NOT NULL,
    "Category1"  character varying(50)  NOT NULL,
    "Category2"  character varying(50),
    "Name"       character varying(250) NOT NULL,
    "Type"       character varying(50),
    "Path"       character varying(500) NOT NULL,
    "Size"       integer                NOT NULL,
    "SourceName" character varying(250) NOT NULL,
    "ExtName"    character varying(50)  NOT NULL,
    "Note"       character varying(500),
    "BizId"      character varying(50),
    "ThumbPath"  character varying(500),
    PRIMARY KEY("Id")
);

CREATE TABLE "SysTask" (
    "Id"         character varying(50)   NOT NULL,
    "CreateBy"   character varying(50)   NOT NULL,
    "CreateTime" date                    NOT NULL,
    "ModifyBy"   character varying(50),
    "ModifyTime" date,
    "Version"    integer                 NOT NULL,
    "Extension"  text,
    "AppId"      character varying(50)   NOT NULL,
    "CompNo"     character varying(50)   NOT NULL,
    "BizId"      character varying(50)   NOT NULL,
    "Type"       character varying(50)   NOT NULL,
    "Name"       character varying(50)   NOT NULL,
    "Target"     character varying(200)  NOT NULL,
    "Status"     character varying(50)   NOT NULL,
    "BeginTime"  date,
    "EndTime"    date,
    "Note"       text,
    PRIMARY KEY("Id")
);

CREATE TABLE "SysDictionary" (
    "Id"           character varying(50)   NOT NULL,
    "CreateBy"     character varying(50)   NOT NULL,
    "CreateTime"   date                    NOT NULL,
    "ModifyBy"     character varying(50),
    "ModifyTime"   date,
    "Version"      integer                 NOT NULL,
    "Extension"    text,
    "AppId"        character varying(50)   NOT NULL,
    "CompNo"       character varying(50)   NOT NULL,
    "Category"     character varying(50),
    "CategoryName" character varying(50),
    "Code"         character varying(100),
    "Name"         character varying(250),
    "Sort"         integer                 NOT NULL,
    "Enabled"      character varying(50)   NOT NULL,
    "Note"         text,
    "Child"        text,
    PRIMARY KEY ("Id")
);

CREATE TABLE "SysCompany" (
    "Id"          character varying(50)  NOT NULL,
    "CreateBy"    character varying(50)  NOT NULL,
    "CreateTime"  date                   NOT NULL,
    "ModifyBy"    character varying(50),
    "ModifyTime"  date,
    "Version"     integer                NOT NULL,
    "Extension"   text,
    "AppId"       character varying(50)  NOT NULL,
    "CompNo"      character varying(50)  NOT NULL,
    "Code"        character varying(50),
    "Name"        character varying(250),
    "NameEn"      character varying(250),
    "SccNo"       character varying(18),
    "Industry"    character varying(50),
    "Region"      character varying(50),
    "Address"     character varying(500),
    "AddressEn"   character varying(500),
    "Contact"     character varying(50),
    "Phone"       character varying(50),
    "Note"        text,
    "SystemData"  text,
    "CompanyData" text,
    PRIMARY KEY("Id")
);

CREATE TABLE "SysOrganization" (
    "Id"         character varying(50) NOT NULL,
    "CreateBy"   character varying(50) NOT NULL,
    "CreateTime" date                  NOT NULL,
    "ModifyBy"   character varying(50),
    "ModifyTime" date,
    "Version"    integer               NOT NULL,
    "Extension"  text,
    "AppId"      character varying(50) NOT NULL,
    "CompNo"     character varying(50) NOT NULL,
    "ParentId"   character varying(50),
    "Code"       character varying(50),
    "Name"       character varying(50) NOT NULL,
    "ManagerId"  character varying(50),
    "Note"       text,
    PRIMARY KEY ("Id")
);

CREATE TABLE "SysRole" (
    "Id"         character varying(50)  NOT NULL,
    "CreateBy"   character varying(50)  NOT NULL,
    "CreateTime" date                   NOT NULL,
    "ModifyBy"   character varying(50),
    "ModifyTime" date,
    "Version"    integer                NOT NULL,
    "Extension"  text,
    "AppId"      character varying(50)  NOT NULL,
    "CompNo"     character varying(50)  NOT NULL,
    "Name"       character varying(50)  NOT NULL,
    "Enabled"    character varying(50)  NOT NULL,
    "Note"       text,
    PRIMARY KEY ("Id")
);

CREATE TABLE "SysRoleModule" (
    "RoleId"   character varying(50) NOT NULL,
    "ModuleId" character varying(50) NOT NULL,
    PRIMARY KEY ("RoleId","ModuleId")
);

CREATE TABLE "SysUser" (
    "Id"             character varying(50)  NOT NULL,
    "CreateBy"       character varying(50)  NOT NULL,
    "CreateTime"     date                   NOT NULL,
    "ModifyBy"       character varying(50),
    "ModifyTime"     date,
    "Version"        integer                NOT NULL,
    "Extension"      text,
    "AppId"          character varying(50)  NOT NULL,
    "CompNo"         character varying(50)  NOT NULL,
    "OrgNo"          character varying(50)  NOT NULL,
    "UserName"       character varying(50)  NOT NULL,
    "Password"       character varying(50)  NOT NULL,
    "Name"           character varying(50)  NOT NULL,
    "EnglishName"    character varying(50),
    "Gender"         character varying(50),
    "Phone"          character varying(50),
    "Mobile"         character varying(50),
    "Email"          character varying(50),
    "Enabled"        character varying(50)  NOT NULL,
    "Note"           text,
    "FirstLoginTime" date,
    "FirstLoginIP"   character varying(50),
    "LastLoginTime"  date,
    "LastLoginIP"    character varying(50),
    "Type"           character varying(50),
    "Role"           character varying(500),
    "Data"           text,
    PRIMARY KEY ("Id")
);

CREATE TABLE "SysUserRole" (
    "UserId" character varying(50) NOT NULL,
    "RoleId" character varying(50) NOT NULL,
    PRIMARY KEY ("UserId","RoleId")
);

CREATE TABLE "SysNotice" (
    "Id"          character varying(50)   NOT NULL,
    "CreateBy"    character varying(50)   NOT NULL,
    "CreateTime"  date                    NOT NULL,
    "ModifyBy"    character varying(50),
    "ModifyTime"  date,
    "Version"     integer                 NOT NULL,
    "Extension"   text,
    "AppId"       character varying(50)   NOT NULL,
    "CompNo"      character varying(50)   NOT NULL,
    "Status"      character varying(50)   NOT NULL,
    "Title"       character varying(50)   NOT NULL,
    "Content"     text,
    "PublishBy"   character varying(50),
    "PublishTime" date,
    PRIMARY KEY("Id")
);

CREATE TABLE "SysMessage" (
    "Id"         character varying(50)   NOT NULL,
    "CreateBy"   character varying(50)   NOT NULL,
    "CreateTime" date                    NOT NULL,
    "ModifyBy"   character varying(50),
    "ModifyTime" date,
    "Version"    integer                 NOT NULL,
    "Extension"  text,
    "AppId"      character varying(50)   NOT NULL,
    "CompNo"     character varying(50)   NOT NULL,
    "UserId"     character varying(50)   NOT NULL,
    "Type"       character varying(50)   NOT NULL,
    "MsgBy"      character varying(50)   NOT NULL,
    "MsgLevel"   character varying(50)   NOT NULL,
    "Category"   character varying(50),
    "Subject"    character varying(250)  NOT NULL,
    "Content"    text                    NOT NULL,
    "FilePath"   character varying(500),
    "IsHtml"     character varying(50)   NOT NULL,
    "Status"     character varying(50)   NOT NULL,
    "BizId"      character varying(50),
    PRIMARY KEY("Id")
);

CREATE TABLE "SysFlow" (
    "Id"         character varying(50)  NOT NULL,
    "CreateBy"   character varying(50)  NOT NULL,
    "CreateTime" date                   NOT NULL,
    "ModifyBy"   character varying(50),
    "ModifyTime" date,
    "Version"    integer                NOT NULL,
    "Extension"  text,
    "AppId"      character varying(50)  NOT NULL,
    "CompNo"     character varying(50)  NOT NULL,
    "FlowCode"   character varying(50)  NOT NULL,
    "FlowName"   character varying(50)  NOT NULL,
    "FlowStatus" character varying(50)  NOT NULL,
    "BizId"      character varying(50)  NOT NULL,
    "BizName"    character varying(200) NOT NULL,
    "BizUrl"     character varying(200) NOT NULL,
    "BizStatus"  character varying(50)  NOT NULL,
    "CurrStep"   character varying(50)  NOT NULL,
    "CurrBy"     character varying(200) NOT NULL,
    "PrevStep"   character varying(50),
    "PrevBy"     character varying(200),
    "NextStep"   character varying(50),
    "NextBy"     character varying(200),
    "ApplyBy"    character varying(50),
    "ApplyTime"  date,
    "VerifyBy"   character varying(50),
    "VerifyTime" date,
    "VerifyNote" text,
    PRIMARY KEY("Id")
);

CREATE TABLE "SysFlowLog" (
    "Id"          character varying(50)   NOT NULL,
    "CreateBy"    character varying(50)   NOT NULL,
    "CreateTime"  date                    NOT NULL,
    "ModifyBy"    character varying(50),
    "ModifyTime"  date,
    "Version"     integer                 NOT NULL,
    "Extension"   text,
    "AppId"       character varying(50)   NOT NULL,
    "CompNo"      character varying(50)   NOT NULL,
    "BizId"       character varying(50)   NOT NULL,
    "StepName"    character varying(50)   NOT NULL,
    "ExecuteBy"   character varying(50)   NOT NULL,
    "ExecuteTime" date                    NOT NULL,
    "Result"      character varying(50)   NOT NULL,
    "Note"        text,
    PRIMARY KEY("Id")
);

CREATE TABLE "SysWeixin" (
    "Id"          character varying(50)   NOT NULL,
    "CreateBy"    character varying(50)   NOT NULL,
    "CreateTime"  date                    NOT NULL,
    "ModifyBy"    character varying(50),
    "ModifyTime"  date,
    "Version"     integer                 NOT NULL,
    "Extension"   text,
    "AppId"       character varying(50)   NOT NULL,
    "CompNo"      character varying(50)   NOT NULL,
    "MPAppId"     character varying(50),
    "UserId"      character varying(50),
    "OpenId"      character varying(50),
    "UnionId"     character varying(50),
    "NickName"    character varying(50),
    "Sex"         character varying(50),
    "Country"     character varying(50),
    "Province"    character varying(50),
    "City"        character varying(50),
    "HeadImgUrl"  character varying(500),
    "Privilege"   text,
    "Note"        text,
    PRIMARY KEY("Id")
);