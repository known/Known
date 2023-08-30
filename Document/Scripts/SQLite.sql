CREATE TABLE [APrototype] (
    [Id]         varchar(50)  NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50) NOT NULL,
    [CreateTime] datetime     NOT NULL,
    [ModifyBy]   nvarchar(50) NULL,
    [ModifyTime] datetime     NULL,
    [Version]    int          NOT NULL,
    [Extension]  ntext        NULL,
    [AppId]      varchar(50)  NULL,
    [CompNo]     varchar(50)  NOT NULL,
    [Type]       varchar(50)  NULL,
    [HeadId]     varchar(50)  NULL,
    [Json]       ntext        NULL
);

CREATE TABLE [SysModule] (
    [Id]          varchar(50)  NOT NULL PRIMARY KEY,
    [CreateBy]    nvarchar(50) NOT NULL,
    [CreateTime]  datetime     NOT NULL,
    [ModifyBy]    nvarchar(50) NULL,
    [ModifyTime]  datetime     NULL,
    [Version]     int          NOT NULL,
    [Extension]   ntext        NULL,
    [AppId]       varchar(50)  NULL,
    [CompNo]      varchar(50)  NOT NULL,
    [ParentId]    varchar(50)  NULL,
    [Code]        varchar(50)  NULL,
    [Name]        nvarchar(50) NOT NULL,
    [Icon]        varchar(50)  NULL,
    [Description] varchar(200) NULL,
    [Target]      varchar(50)  NULL,
    [Sort]        int          NOT NULL,
    [Enabled]     varchar(50)  NOT NULL,
    [ButtonData]  ntext        NULL,
    [ActionData]  ntext        NULL,
    [ColumnData]  ntext        NULL,
    [Note]        ntext        NULL
);

CREATE TABLE [SysConfig] (
    [AppId]       varchar(50)    NOT NULL,
    [ConfigKey]   varchar(50)    NOT NULL,
    [ConfigValue] ntext          NOT NULL,
    CONSTRAINT [PK_SysConfig] PRIMARY KEY ([AppId] ASC,[ConfigKey] ASC)
);

CREATE TABLE [SysSetting] (
    [Id]         varchar(50)   NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50)  NOT NULL,
    [CreateTime] datetime      NOT NULL,
    [ModifyBy]   nvarchar(50)  NULL,
    [ModifyTime] datetime      NULL,
    [Version]    int           NOT NULL,
    [Extension]  ntext         NULL,
    [AppId]      varchar(50)   NOT NULL,
    [CompNo]     varchar(50)   NOT NULL,
    [BizType]    nvarchar(50)  NOT NULL,
    [BizName]    nvarchar(250) NULL,
    [BizData]    ntext         NULL
);

CREATE TABLE [SysLog] (
    [Id]         varchar(50)  NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50) NOT NULL,
    [CreateTime] datetime     NOT NULL,
    [ModifyBy]   nvarchar(50) NULL,
    [ModifyTime] datetime     NULL,
    [Version]    int          NOT NULL,
    [Extension]  ntext        NULL,
    [AppId]      varchar(50)  NOT NULL,
    [CompNo]     varchar(50)  NOT NULL,
    [Type]       nvarchar(50) NULL,
    [Target]     nvarchar(50) NULL,
    [Content]    ntext        NULL
);

CREATE TABLE [SysFile] (
    [Id]         varchar(50)   NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50)  NOT NULL,
    [CreateTime] datetime      NOT NULL,
    [ModifyBy]   nvarchar(50)  NULL,
    [ModifyTime] datetime      NULL,
    [Version]    int           NOT NULL,
    [Extension]  ntext         NULL,
    [AppId]      varchar(50)   NOT NULL,
    [CompNo]     varchar(50)   NOT NULL,
    [Category1]  nvarchar(50)  NOT NULL,
    [Category2]  nvarchar(50)  NULL,
    [Name]       nvarchar(250) NOT NULL,
    [Type]       nvarchar(50)  NULL,
    [Path]       nvarchar(500) NOT NULL,
    [Size]       int           NOT NULL,
    [SourceName] nvarchar(250) NOT NULL,
    [ExtName]    varchar(50)   NOT NULL,
    [Note]       nvarchar(500) NULL,
    [BizId]      varchar(50)   NULL,
    [ThumbPath]  nvarchar(500) NULL
);

CREATE TABLE [SysTask] (
    [Id]         varchar(50)    NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50)   NOT NULL,
    [CreateTime] datetime       NOT NULL,
    [ModifyBy]   nvarchar(50)   NULL,
    [ModifyTime] datetime       NULL,
    [Version]    int            NOT NULL,
    [Extension]  ntext          NULL,
    [AppId]      varchar(50)    NOT NULL,
    [CompNo]     varchar(50)    NOT NULL,
    [BizId]      varchar(50)    NOT NULL,
    [Type]       nvarchar(50)   NOT NULL,
    [Name]       nvarchar(50)   NOT NULL,
    [Target]     nvarchar(200)  NOT NULL,
    [Status]     nvarchar(50)   NOT NULL,
    [BeginTime]  datetime       NULL,
    [EndTime]    datetime       NULL,
    [Note]       ntext          NULL
);

CREATE TABLE [SysDictionary] (
    [Id]           varchar(50)   NOT NULL PRIMARY KEY,
    [CreateBy]     nvarchar(50)  NOT NULL,
    [CreateTime]   datetime      NOT NULL,
    [ModifyBy]     nvarchar(50)  NULL,
    [ModifyTime]   datetime      NULL,
    [Version]      int           NOT NULL,
    [Extension]    ntext         NULL,
    [AppId]        varchar(50)   NOT NULL,
    [CompNo]       varchar(50)   NOT NULL,
    [Category]     nvarchar(50)  NULL,
    [CategoryName] nvarchar(50)  NULL,
    [Code]         nvarchar(100) NULL,
    [Name]         nvarchar(250) NULL,
    [Sort]         int           NOT NULL,
    [Enabled]      varchar(50)   NOT NULL,
    [Note]         ntext         NULL,
    [Child]        ntext         NULL
);

CREATE TABLE [SysTenant] (
    [Id]         varchar(50)      NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50)     NOT NULL,
    [CreateTime] datetime         NOT NULL,
    [ModifyBy]   nvarchar(50)     NULL,
    [ModifyTime] datetime         NULL,
    [Version]    int              NOT NULL,
    [Extension]  ntext            NULL,
    [AppId]      varchar(50)      NOT NULL,
    [CompNo]     varchar(50)      NOT NULL,
    [Code]       nvarchar(50)     NOT NULL,
    [Name]       nvarchar(50)     NOT NULL,
    [Enabled]    varchar(50)      NOT NULL,
    [UserCount]  int              NOT NULL,
    [BillCount]  int              NOT NULL,
    [Note]       ntext            NULL
);

CREATE TABLE [SysCompany] (
    [Id]          varchar(50)   NOT NULL PRIMARY KEY,
    [CreateBy]    nvarchar(50)  NOT NULL,
    [CreateTime]  datetime      NOT NULL,
    [ModifyBy]    nvarchar(50)  NULL,
    [ModifyTime]  datetime      NULL,
    [Version]     int           NOT NULL,
    [Extension]   ntext         NULL,
    [AppId]       varchar(50)   NOT NULL,
    [CompNo]      varchar(50)   NOT NULL,
    [Code]        nvarchar(50)  NULL,
    [Name]        nvarchar(250) NULL,
    [NameEn]      nvarchar(250) NULL,
    [SccNo]       varchar(18)   NULL,
    [Industry]    nvarchar(50)  NULL,
    [Region]      nvarchar(50)  NULL,
    [Address]     nvarchar(500) NULL,
    [AddressEn]   nvarchar(500) NULL,
    [Contact]     nvarchar(50)  NULL,
    [Phone]       nvarchar(50)  NULL,
    [Note]        ntext         NULL,
    [SystemData]  ntext         NULL,
    [CompanyData] ntext         NULL
);

CREATE TABLE [SysOrganization] (
    [Id]         varchar(50)  NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50) NOT NULL,
    [CreateTime] datetime     NOT NULL,
    [ModifyBy]   nvarchar(50) NULL,
    [ModifyTime] datetime     NULL,
    [Version]    int          NOT NULL,
    [Extension]  ntext        NULL,
    [AppId]      varchar(50)  NOT NULL,
    [CompNo]     varchar(50)  NOT NULL,
    [ParentId]   varchar(50)  NULL,
    [Code]       nvarchar(50) NULL,
    [Name]       nvarchar(50) NOT NULL,
    [ManagerId]  varchar(50)  NULL,
    [Note]       ntext        NULL
);

CREATE TABLE [SysRole] (
    [Id]         varchar(50)  NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50) NOT NULL,
    [CreateTime] datetime     NOT NULL,
    [ModifyBy]   nvarchar(50) NULL,
    [ModifyTime] datetime     NULL,
    [Version]    int          NOT NULL,
    [Extension]  ntext        NULL,
    [AppId]      varchar(50)  NOT NULL,
    [CompNo]     varchar(50)  NOT NULL,
    [Name]       nvarchar(50) NOT NULL,
    [Enabled]    varchar(50)  NOT NULL,
    [Note]       ntext        NULL
);

CREATE TABLE [SysRoleModule] (
    [RoleId]   varchar(50) NOT NULL,
    [ModuleId] varchar(50) NOT NULL,
    CONSTRAINT [PK_SysRoleModule] PRIMARY KEY ([RoleId] ASC,[ModuleId] ASC)
);

CREATE TABLE [SysUser] (
    [Id]             varchar(50)   NOT NULL PRIMARY KEY,
    [CreateBy]       nvarchar(50)  NOT NULL,
    [CreateTime]     datetime      NOT NULL,
    [ModifyBy]       nvarchar(50)  NULL,
    [ModifyTime]     datetime      NULL,
    [Version]        int           NOT NULL,
    [Extension]      ntext         NULL,
    [AppId]          varchar(50)   NOT NULL,
    [CompNo]         varchar(50)   NOT NULL,
    [OrgNo]          varchar(50)   NOT NULL,
    [UserName]       varchar(50)   NOT NULL,
    [Password]       varchar(50)   NOT NULL,
    [Name]           nvarchar(50)  NOT NULL,
    [EnglishName]    varchar(50)   NULL,
    [Gender]         nvarchar(50)  NULL,
    [Phone]          varchar(50)   NULL,
    [Mobile]         varchar(50)   NULL,
    [Email]          varchar(50)   NULL,
    [Enabled]        varchar(50)   NOT NULL,
    [Note]           ntext         NULL,
    [FirstLoginTime] datetime      NULL,
    [FirstLoginIP]   varchar(50)   NULL,
    [LastLoginTime]  datetime      NULL,
    [LastLoginIP]    varchar(50)   NULL,
    [Type]           nvarchar(50)  NULL,
    [Role]           nvarchar(500) NULL,
    [Data]           ntext         NULL
);

CREATE TABLE [SysUserModule] (
    [UserId]   varchar(50) NOT NULL,
    [ModuleId] varchar(50) NOT NULL,
    CONSTRAINT [PK_SysUserModule] PRIMARY KEY ([UserId] ASC,[ModuleId] ASC)
);

CREATE TABLE [SysUserRole] (
    [UserId] varchar(50) NOT NULL,
    [RoleId] varchar(50) NOT NULL,
    CONSTRAINT [PK_SysUserRole] PRIMARY KEY ([UserId] ASC,[RoleId] ASC)
);

CREATE TABLE [SysNotice] (
    [Id]          varchar(50)    NOT NULL PRIMARY KEY,
    [CreateBy]    nvarchar(50)   NOT NULL,
    [CreateTime]  datetime       NOT NULL,
    [ModifyBy]    nvarchar(50)   NULL,
    [ModifyTime]  datetime       NULL,
    [Version]     int            NOT NULL,
    [Extension]   ntext          NULL,
    [AppId]       varchar(50)    NOT NULL,
    [CompNo]      varchar(50)    NOT NULL,
    [Status]      nvarchar(50)   NOT NULL,
    [Title]       nvarchar(50)   NOT NULL,
    [Content]     nvarchar(4000) NULL,
    [PublishBy]   nvarchar(50)   NULL,
    [PublishTime] datetime       NULL
);

CREATE TABLE [SysUserLink] (
    [Id]         varchar(50)  NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50) NOT NULL,
    [CreateTime] datetime     NOT NULL,
    [ModifyBy]   nvarchar(50) NULL,
    [ModifyTime] datetime     NULL,
    [Version]    int          NOT NULL,
    [Extension]  ntext        NULL,
    [AppId]      varchar(50)  NOT NULL,
    [CompNo]     varchar(50)  NOT NULL,
    [UserName]   varchar(50)  NOT NULL,
    [Type]       varchar(50)  NOT NULL,
    [Name]       nvarchar(50) NOT NULL,
    [Address]    varchar(200) NOT NULL
);

CREATE TABLE [SysMessage] (
    [Id]         varchar(50)    NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50)   NOT NULL,
    [CreateTime] datetime       NOT NULL,
    [ModifyBy]   nvarchar(50)   NULL,
    [ModifyTime] datetime       NULL,
    [Version]    int            NOT NULL,
    [Extension]  ntext          NULL,
    [AppId]      varchar(50)    NOT NULL,
    [CompNo]     varchar(50)    NOT NULL,
    [UserId]     varchar(50)    NOT NULL,
    [Type]       nvarchar(50)   NOT NULL,
    [MsgBy]      nvarchar(50)   NOT NULL,
    [MsgLevel]   nvarchar(50)   NOT NULL,
    [Category]   nvarchar(50)   NULL,
    [Subject]    nvarchar(250)  NOT NULL,
    [Content]    nvarchar(4000) NOT NULL,
    [FilePath]   nvarchar(500)  NULL,
    [IsHtml]     varchar(50)    NOT NULL,
    [Status]     nvarchar(50)   NOT NULL,
    [BizId]      varchar(50)    NULL
);

CREATE TABLE [SysFlow] (
    [Id]         varchar(50)   NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50)  NOT NULL,
    [CreateTime] datetime      NOT NULL,
    [ModifyBy]   nvarchar(50)  NULL,
    [ModifyTime] datetime      NULL,
    [Version]    int           NOT NULL,
    [Extension]  ntext         NULL,
    [AppId]      varchar(50)   NOT NULL,
    [CompNo]     varchar(50)   NOT NULL,
    [FlowCode]   varchar(50)   NOT NULL,
    [FlowName]   nvarchar(50)  NOT NULL,
    [FlowStatus] nvarchar(50)  NOT NULL,
    [BizId]      varchar(50)   NOT NULL,
    [BizName]    nvarchar(200) NOT NULL,
    [BizUrl]     varchar(200)  NOT NULL,
    [BizStatus]  nvarchar(50)  NOT NULL,
    [CurrStep]   nvarchar(50)  NOT NULL,
    [CurrBy]     nvarchar(200) NOT NULL,
    [PrevStep]   nvarchar(50)  NULL,
    [PrevBy]     nvarchar(200) NULL,
    [NextStep]   nvarchar(50)  NULL,
    [NextBy]     nvarchar(200) NULL,
    [ApplyBy]    nvarchar(50)  NULL,
    [ApplyTime]  datetime      NULL,
    [VerifyBy]   nvarchar(50)  NULL,
    [VerifyTime] datetime      NULL,
    [VerifyNote] nvarchar(500) NULL
);

CREATE TABLE [SysFlowLog] (
    [Id]          varchar(50)    NOT NULL PRIMARY KEY,
    [CreateBy]    nvarchar(50)   NOT NULL,
    [CreateTime]  datetime       NOT NULL,
    [ModifyBy]    nvarchar(50)   NULL,
    [ModifyTime]  datetime       NULL,
    [Version]     int            NOT NULL,
    [Extension]   ntext          NULL,
    [AppId]       varchar(50)    NOT NULL,
    [CompNo]      varchar(50)    NOT NULL,
    [BizId]       varchar(50)    NOT NULL,
    [StepName]    nvarchar(50)   NOT NULL,
    [ExecuteBy]   nvarchar(50)   NOT NULL,
    [ExecuteTime] datetime       NOT NULL,
    [Result]      nvarchar(50)   NOT NULL,
    [Note]        nvarchar(1000) NULL
);

CREATE TABLE [SysFlowStep] (
    [Id]            varchar(50)    NOT NULL PRIMARY KEY,
    [CreateBy]      nvarchar(50)   NOT NULL,
    [CreateTime]    datetime       NOT NULL,
    [ModifyBy]      nvarchar(50)   NULL,
    [ModifyTime]    datetime       NULL,
    [Version]       int            NOT NULL,
    [Extension]     ntext          NULL,
    [AppId]         varchar(50)    NOT NULL,
    [CompNo]        varchar(50)    NOT NULL,
    [FlowCode]      varchar(50)    NOT NULL,
    [FlowName]      nvarchar(50)   NOT NULL,
    [StepCode]      varchar(50)    NOT NULL,
    [StepName]      nvarchar(50)   NOT NULL,
    [StepType]      nvarchar(50)   NOT NULL,
    [OperateBy]     varchar(50)    NULL,
    [OperateByName] nvarchar(50)   NULL,
    [Note]          nvarchar(500)  NULL,
    [X]             int            NULL,
    [Y]             int            NULL,
    [IsRound]       int            NULL,
    [Arrows]        nvarchar(4000) NULL
);

CREATE TABLE [SysNoRule] (
    [Id]          varchar(50)    NOT NULL PRIMARY KEY,
    [CreateBy]    nvarchar(50)   NOT NULL,
    [CreateTime]  datetime       NOT NULL,
    [ModifyBy]    nvarchar(50)   NULL,
    [ModifyTime]  datetime       NULL,
    [Version]     int            NOT NULL,
    [Extension]   ntext          NULL,
    [AppId]       varchar(50)    NOT NULL,
    [CompNo]      varchar(50)    NOT NULL,
    [Code]        varchar(50)    NOT NULL,
    [Name]        nvarchar(50)   NOT NULL,
    [Description] nvarchar(500)  NULL,
    [RuleData]    nvarchar(4000) NULL
);

CREATE TABLE [SysNoRuleData] (
    [AppId]  varchar(50)  NOT NULL,
    [CompNo] varchar(50)  NOT NULL,
    [RuleId] varchar(50)  NOT NULL,
    [RuleNo] nvarchar(50) NOT NULL
);

INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Sort],[Enabled]) VALUES ('aec2815390ab4af9943a051b6d67052b','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','0','BaseData','基础数据','fa fa-database',1,'True');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ButtonData],[ColumnData]) VALUES ('93afcd01f640476bb35daec485cb0b70','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','aec2815390ab4af9943a051b6d67052b','CompanyForm','企业信息','fa fa-id-card-o','维护企业基本资料。','Known.Test.Pages.CompanyForm',1,'True','修改','[]');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ButtonData],[ActionData],[ColumnData]) VALUES ('d5ca63b57fb343c4992139fcf6215a01','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','aec2815390ab4af9943a051b6d67052b','SysOrgList','组织架构','fa fa-sitemap','维护企业组织架构信息。','Known.Razor.Pages.SysOrgList',2,'True','新增,批量删除','编辑,删除','[{"Id":"Code","Name":"编码","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ButtonData],[ActionData],[ColumnData]) VALUES ('832cc5034c4c45d9ba7c6fd3f7cb5ade','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','aec2815390ab4af9943a051b6d67052b','SysDicList','数据字典','fa fa-list-ul','维护系统所需的下拉框数据源。','Known.Razor.Pages.SysDicList',3,'True','新增,批量删除,导入','编辑,删除','[{"Id":"Category","Name":"类别","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Code","Name":"代码","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Sort","Name":"顺序","Type":1,"Align":1,"Width":60,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Sort],[Enabled]) VALUES ('c92e432ac89d4e668cf243fa9fededa6','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','0','System','系统管理','fa fa-cogs',2,'True');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ColumnData]) VALUES ('12d3be601e804695a97530de6c4bea15','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysSystem','关于系统','fa fa-info-circle','显示系统版本及产品授权信息。','Known.Razor.Pages.SysSystem',1,'True','[]');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ButtonData],[ActionData],[ColumnData]) VALUES ('2944632b28844a54aad68c0f584febe2','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysTenantList','租户管理','fa fa-address-card-o','维护平台租户信息。','Known.Razor.Pages.SysTenantList',2,'True','新增','查看,编辑','[{"Id":"Code","Name":"账号","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"UserCount","Name":"用户数量","Type":1,"Align":2,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"BillCount","Name":"单据数量","Type":1,"Align":2,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateBy","Name":"创建人","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"创建时间","Type":4,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ButtonData],[ActionData],[ColumnData]) VALUES ('5c9eb1bdf02b49afa1ac90e7ae01b090','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysRoleList','角色管理','fa fa-users','维护系统用户角色及其菜单权限信息。','Known.Razor.Pages.SysRoleList',3,'True','新增,批量删除','编辑,删除','[{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":150,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ButtonData],[ActionData],[ColumnData]) VALUES ('44856b50a77d4639b3298c66202abb6e','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysUserList','用户管理','fa fa-user','维护系统用户账号及角色信息。','Known.Razor.Pages.SysUserList',4,'True','新增,批量删除,重置密码,启用,禁用','编辑,删除','[{"Id":"UserName","Name":"用户名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"姓名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Gender","Name":"性别","Type":0,"Align":1,"Width":80,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Phone","Name":"固定电话","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Mobile","Name":"移动电话","Type":0,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Email","Name":"电子邮件","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Role","Name":"角色","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ColumnData]) VALUES ('63ed9926f3fd4fe4a99a337d0f76fd3e','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysLogList','系统日志','fa fa-clock-o','查询系统用户操作日志信息。','Known.Razor.Pages.SysLogList',5,'True','[{"Id":"CreateBy","Name":"操作人","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"操作时间","Type":4,"Align":1,"Width":120,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Type","Name":"操作类型","Type":0,"Align":1,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Target","Name":"操作对象","Type":0,"Align":0,"Width":200,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Content","Name":"操作内容","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ColumnData]) VALUES ('7214a7db358d4e968890fbd8573b49d3','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysFileList','系统附件','fa fa-file-o','查询系统所有附件信息。','Known.Razor.Pages.SysFileList',6,'True','[{"Id":"Category1","Name":"一级分类","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Category2","Name":"二级分类","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"文件名称","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Type","Name":"文件类型","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Size","Name":"文件大小","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"SourceName","Name":"原文件名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"ExtName","Name":"扩展名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateBy","Name":"创建人","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"创建时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ColumnData]) VALUES ('91024299564b4b45a8d2de2d527028d4','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysTaskList','后台任务','fa fa-tasks','查询系统所有定时任务运行情况。','Known.Razor.Pages.SysTaskList',7,'True','[{"Id":"Type","Name":"类型","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Target","Name":"执行目标","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Status","Name":"执行状态","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"BeginTime","Name":"开始时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"EndTime","Name":"结束时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateBy","Name":"创建人","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"创建时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO [SysModule] ([Id],[CreateBy],[CreateTime],[ModifyBy],[ModifyTime],[Version],[AppId],[CompNo],[ParentId],[Code],[Name],[Icon],[Description],[Target],[Sort],[Enabled],[ButtonData],[ActionData],[ColumnData]) VALUES ('d96d0db0b0694a48ab61ba00643a7884','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysModuleList','模块管理','fa fa-cubes','维护系统菜单按钮及列表栏位信息。','Known.Razor.Pages.SysModuleList',8,'True','新增,复制,批量删除,移动','编辑,删除,上移,下移','[{"Id":"Code","Name":"代码","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Description","Name":"描述","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Target","Name":"目标","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Sort","Name":"顺序","Type":1,"Align":1,"Width":80,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"可用","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
