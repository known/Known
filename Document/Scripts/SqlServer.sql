CREATE TABLE APrototype (
    [Id]         [varchar](50)  NOT NULL,
    [CreateBy]   [nvarchar](50) NOT NULL,
    [CreateTime] [datetime]     NOT NULL,
    [ModifyBy]   [nvarchar](50) NULL,
    [ModifyTime] [datetime]     NULL,
    [Version]    [int]          NOT NULL,
    [Extension]  [ntext]        NULL,
    [AppId]      [varchar](50)  NULL,
    [CompNo]     [varchar](50)  NOT NULL,
    [Type]       [varchar](50)  NULL,
    [HeadId]     [varchar](50)  NULL,
    [Json]       [ntext]        NULL,
    CONSTRAINT [PK_APrototype] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE SysModule (
    [Id]          [varchar](50)  NOT NULL,
    [CreateBy]    [nvarchar](50) NOT NULL,
    [CreateTime]  [datetime]     NOT NULL,
    [ModifyBy]    [nvarchar](50) NULL,
    [ModifyTime]  [datetime]     NULL,
    [Version]     [int]          NOT NULL,
    [Extension]   [ntext]        NULL,
    [AppId]       [varchar](50)  NULL,
    [CompNo]      [varchar](50)  NOT NULL,
    [ParentId]    [varchar](50)  NULL,
    [Code]        [varchar](50)  NULL,
    [Name]        [nvarchar](50) NOT NULL,
    [Icon]        [varchar](50)  NULL,
    [Description] [varchar](200) NULL,
    [Target]      [varchar](50)  NULL,
    [Sort]        [int]          NOT NULL,
    [Enabled]     [varchar](50)  NOT NULL,
    [ButtonData]  [ntext]        NULL,
    [ActionData]  [ntext]        NULL,
    [ColumnData]  [ntext]        NULL,
    [Note]        [ntext]        NULL,
    CONSTRAINT [PK_SysModule] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE [SysConfig] (
    [AppId]       [varchar](50)    NOT NULL,
    [ConfigKey]   [varchar](50)    NOT NULL,
    [ConfigValue] [ntext]          NOT NULL,
    CONSTRAINT [PK_SysConfig] PRIMARY KEY ([AppId] ASC,[ConfigKey] ASC)
) 
GO

CREATE TABLE [SysSetting] (
    [Id]         [varchar](50)   NOT NULL,
    [CreateBy]   [nvarchar](50)  NOT NULL,
    [CreateTime] [datetime]      NOT NULL,
    [ModifyBy]   [nvarchar](50)  NULL,
    [ModifyTime] [datetime]      NULL,
    [Version]    [int]           NOT NULL,
    [Extension]  [ntext]         NULL,
    [AppId]      [varchar](50)   NOT NULL,
    [CompNo]     [varchar](50)   NOT NULL,
    [BizType]    [nvarchar](50)  NOT NULL,
    [BizName]    [nvarchar](250) NULL,
    [BizData]    [ntext]         NULL,
    CONSTRAINT [PK_SysSetting] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysLog] (
    [Id]         [varchar](50)  NOT NULL,
    [CreateBy]   [nvarchar](50) NOT NULL,
    [CreateTime] [datetime]     NOT NULL,
    [ModifyBy]   [nvarchar](50) NULL,
    [ModifyTime] [datetime]     NULL,
    [Version]    [int]          NOT NULL,
    [Extension]  [ntext]        NULL,
    [AppId]      [varchar](50)  NOT NULL,
    [CompNo]     [varchar](50)  NOT NULL,
    [Type]       [nvarchar](50) NULL,
    [Target]     [nvarchar](50) NULL,
    [Content]    [ntext]        NULL,
    CONSTRAINT [PK_SysLog] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysFile] (
    [Id]         [varchar](50)   NOT NULL,
    [CreateBy]   [nvarchar](50)  NOT NULL,
    [CreateTime] [datetime]      NOT NULL,
    [ModifyBy]   [nvarchar](50)  NULL,
    [ModifyTime] [datetime]      NULL,
    [Version]    [int]           NOT NULL,
    [Extension]  [ntext]         NULL,
    [AppId]      [varchar](50)   NOT NULL,
    [CompNo]     [varchar](50)   NOT NULL,
    [Category1]  [nvarchar](50)  NOT NULL,
    [Category2]  [nvarchar](50)  NULL,
    [Name]       [nvarchar](250) NOT NULL,
    [Type]       [nvarchar](50)  NULL,
    [Path]       [nvarchar](500) NOT NULL,
    [Size]       [int]           NOT NULL,
    [SourceName] [nvarchar](250) NOT NULL,
    [ExtName]    [varchar](50)   NOT NULL,
    [Note]       [nvarchar](500) NULL,
    [BizId]      [varchar](50)   NULL,
    [ThumbPath]  [nvarchar](500) NULL,
    CONSTRAINT [PK_SysFile] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysTask] (
    [Id]         [varchar](50)    NOT NULL,
    [CreateBy]   [nvarchar](50)   NOT NULL,
    [CreateTime] [datetime]       NOT NULL,
    [ModifyBy]   [nvarchar](50)   NULL,
    [ModifyTime] [datetime]       NULL,
    [Version]    [int]            NOT NULL,
    [Extension]  [ntext]          NULL,
    [AppId]      [varchar](50)    NOT NULL,
    [CompNo]     [varchar](50)    NOT NULL,
    [BizId]      [varchar](50)    NOT NULL,
    [Type]       [nvarchar](50)   NOT NULL,
    [Name]       [nvarchar](50)   NOT NULL,
    [Target]     [nvarchar](200)  NOT NULL,
    [Status]     [nvarchar](50)   NOT NULL,
    [BeginTime]  [datetime]       NULL,
    [EndTime]    [datetime]       NULL,
    [Note]       [ntext]          NULL,
    CONSTRAINT [PK_SysTask] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysDictionary] (
    [Id]           [varchar](50)   NOT NULL,
    [CreateBy]     [nvarchar](50)  NOT NULL,
    [CreateTime]   [datetime]      NOT NULL,
    [ModifyBy]     [nvarchar](50)  NULL,
    [ModifyTime]   [datetime]      NULL,
    [Version]      [int]           NOT NULL,
    [Extension]    [ntext]         NULL,
    [AppId]        [varchar](50)   NOT NULL,
    [CompNo]       [varchar](50)   NOT NULL,
    [Category]     [nvarchar](50)  NULL,
    [CategoryName] [nvarchar](50)  NULL,
    [Code]         [nvarchar](100) NULL,
    [Name]         [nvarchar](250) NULL,
    [Sort]         [int]           NOT NULL,
    [Enabled]      [varchar](50)   NOT NULL,
    [Note]         [ntext]         NULL,
    [Child]        [ntext]         NULL,
    CONSTRAINT [PK_SysDictionary] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysTenant] (
    [Id]         [varchar](50)    NOT NULL,
    [CreateBy]   [nvarchar](50)   NOT NULL,
    [CreateTime] [datetime]       NOT NULL,
    [ModifyBy]   [nvarchar](50)   NULL,
    [ModifyTime] [datetime]       NULL,
    [Version]    [int]            NOT NULL,
    [Extension]  [ntext]          NULL,
    [AppId]      [varchar](50)    NOT NULL,
    [CompNo]     [varchar](50)    NOT NULL,
    [Code]       [nvarchar](50)   NOT NULL,
    [Name]       [nvarchar](50)   NOT NULL,
    [Enabled]    [varchar](50)    NOT NULL,
    [UserCount]  [int]            NOT NULL,
    [BillCount]  [int]            NOT NULL,
    [Note]       [ntext]          NULL,
    CONSTRAINT [PK_SysTenant] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysCompany] (
    [Id]          [varchar](50)    NOT NULL,
    [CreateBy]    [nvarchar](50)   NOT NULL,
    [CreateTime]  [datetime]       NOT NULL,
    [ModifyBy]    [nvarchar](50)   NULL,
    [ModifyTime]  [datetime]       NULL,
    [Version]     [int]            NOT NULL,
    [Extension]   [ntext]          NULL,
    [AppId]       [varchar](50)    NOT NULL,
    [CompNo]      [varchar](50)    NOT NULL,
    [Code]        [nvarchar](50)   NULL,
    [Name]        [nvarchar](250)  NULL,
    [NameEn]      [nvarchar](250)  NULL,
    [SccNo]       [varchar](18)    NULL,
    [Industry]    [nvarchar](50)   NULL,
    [Region]      [nvarchar](50)   NULL,
    [Address]     [nvarchar](500)  NULL,
    [AddressEn]   [nvarchar](500)  NULL,
    [Contact]     [nvarchar](50)   NULL,
    [Phone]       [nvarchar](50)   NULL,
    [Note]        [ntext]          NULL,
    [SystemData]  [ntext]          NULL,
    [CompanyData] [ntext]          NULL,
    CONSTRAINT [PK_SysCompany] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysOrganization] (
    [Id]         [varchar](50)  NOT NULL,
    [CreateBy]   [nvarchar](50) NOT NULL,
    [CreateTime] [datetime]     NOT NULL,
    [ModifyBy]   [nvarchar](50) NULL,
    [ModifyTime] [datetime]     NULL,
    [Version]    [int]          NOT NULL,
    [Extension]  [ntext]        NULL,
    [AppId]      [varchar](50)  NOT NULL,
    [CompNo]     [varchar](50)  NOT NULL,
    [ParentId]   [varchar](50)  NULL,
    [Code]       [nvarchar](50) NULL,
    [Name]       [nvarchar](50) NOT NULL,
    [ManagerId]  [varchar](50)  NULL,
    [Note]       [ntext]        NULL,
    CONSTRAINT [PK_SysOrganization] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE [SysRole] (
    [Id]         [varchar](50)  NOT NULL,
    [CreateBy]   [nvarchar](50) NOT NULL,
    [CreateTime] [datetime]     NOT NULL,
    [ModifyBy]   [nvarchar](50) NULL,
    [ModifyTime] [datetime]     NULL,
    [Version]    [int]          NOT NULL,
    [Extension]  [ntext]        NULL,
    [AppId]      [varchar](50)  NOT NULL,
    [CompNo]     [varchar](50)  NOT NULL,
    [Name]       [nvarchar](50) NOT NULL,
    [Enabled]    [varchar](50)  NOT NULL,
    [Note]       [ntext]        NULL,
    CONSTRAINT [PK_SysRole] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysRoleModule] (
    [RoleId]   [varchar](50) NOT NULL,
    [ModuleId] [varchar](50) NOT NULL,
    CONSTRAINT [PK_SysRoleModule] PRIMARY KEY ([RoleId] ASC,[ModuleId] ASC)
) 
GO

CREATE TABLE [SysUser] (
    [Id]             [varchar](50)   NOT NULL,
    [CreateBy]       [nvarchar](50)  NOT NULL,
    [CreateTime]     [datetime]      NOT NULL,
    [ModifyBy]       [nvarchar](50)  NULL,
    [ModifyTime]     [datetime]      NULL,
    [Version]        [int]           NOT NULL,
    [Extension]      [ntext]         NULL,
    [AppId]          [varchar](50)   NOT NULL,
    [CompNo]         [varchar](50)   NOT NULL,
    [OrgNo]          [varchar](50)   NOT NULL,
    [UserName]       [varchar](50)   NOT NULL,
    [Password]       [varchar](50)   NOT NULL,
    [Name]           [nvarchar](50)  NOT NULL,
    [EnglishName]    [varchar](50)   NULL,
    [Gender]         [nvarchar](50)  NULL,
    [Phone]          [varchar](50)   NULL,
    [Mobile]         [varchar](50)   NULL,
    [Email]          [varchar](50)   NULL,
    [Enabled]        [varchar](50)   NOT NULL,
    [Note]           [ntext]         NULL,
    [FirstLoginTime] [datetime]      NULL,
    [FirstLoginIP]   [varchar](50)   NULL,
    [LastLoginTime]  [datetime]      NULL,
    [LastLoginIP]    [varchar](50)   NULL,
    [Type]           [nvarchar](50)  NULL,
    [Role]           [nvarchar](500) NULL,
    [Data]           [ntext]         NULL,
    CONSTRAINT [PK_SysUser] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysUserModule] (
    [UserId]   [varchar](50) NOT NULL,
    [ModuleId] [varchar](50) NOT NULL,
    CONSTRAINT [PK_SysUserModule] PRIMARY KEY ([UserId] ASC,[ModuleId] ASC)
) 
GO

CREATE TABLE [SysUserRole] (
    [UserId] [varchar](50) NOT NULL,
    [RoleId] [varchar](50) NOT NULL,
    CONSTRAINT [PK_SysUserRole] PRIMARY KEY ([UserId] ASC,[RoleId] ASC)
) 
GO

CREATE TABLE [SysNotice] (
    [Id]          [varchar](50)    NOT NULL,
    [CreateBy]    [nvarchar](50)   NOT NULL,
    [CreateTime]  [datetime]       NOT NULL,
    [ModifyBy]    [nvarchar](50)   NULL,
    [ModifyTime]  [datetime]       NULL,
    [Version]     [int]            NOT NULL,
    [Extension]   [ntext]          NULL,
    [AppId]       [varchar](50)    NOT NULL,
    [CompNo]      [varchar](50)    NOT NULL,
    [Status]      [nvarchar](50)   NOT NULL,
    [Title]       [nvarchar](50)   NOT NULL,
    [Content]     [nvarchar](4000) NULL,
    [PublishBy]   [nvarchar](50)   NULL,
    [PublishTime] [datetime]       NULL,
    CONSTRAINT [PK_SysNotice] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysUserLink] (
    [Id]         [varchar](50)  NOT NULL,
    [CreateBy]   [nvarchar](50) NOT NULL,
    [CreateTime] [datetime]     NOT NULL,
    [ModifyBy]   [nvarchar](50) NULL,
    [ModifyTime] [datetime]     NULL,
    [Version]    [int]          NOT NULL,
    [Extension]  [ntext]        NULL,
    [AppId]      [varchar](50)  NOT NULL,
    [CompNo]     [varchar](50)  NOT NULL,
    [UserName]   [varchar](50)  NOT NULL,
    [Type]       [varchar](50)  NOT NULL,
    [Name]       [nvarchar](50) NOT NULL,
    [Address]    [varchar](200) NOT NULL,
    CONSTRAINT [PK_SysUserLink] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysMessage] (
    [Id]         [varchar](50)    NOT NULL,
    [CreateBy]   [nvarchar](50)   NOT NULL,
    [CreateTime] [datetime]       NOT NULL,
    [ModifyBy]   [nvarchar](50)   NULL,
    [ModifyTime] [datetime]       NULL,
    [Version]    [int]            NOT NULL,
    [Extension]  [ntext]          NULL,
    [AppId]      [varchar](50)    NOT NULL,
    [CompNo]     [varchar](50)    NOT NULL,
    [UserId]     [varchar](50)    NOT NULL,
    [Type]       [nvarchar](50)   NOT NULL,
    [MsgBy]      [nvarchar](50)   NOT NULL,
    [MsgLevel]   [nvarchar](50)   NOT NULL,
    [Category]   [nvarchar](50)   NULL,
    [Subject]    [nvarchar](250)  NOT NULL,
    [Content]    [nvarchar](4000) NOT NULL,
    [FilePath]   [nvarchar](500)  NULL,
    [IsHtml]     [varchar](50)    NOT NULL,
    [Status]     [nvarchar](50)   NOT NULL,
    [BizId]      [varchar](50)    NULL,
    CONSTRAINT [PK_SysMessage] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysFlow] (
    [Id]         [varchar](50)   NOT NULL,
    [CreateBy]   [nvarchar](50)  NOT NULL,
    [CreateTime] [datetime]      NOT NULL,
    [ModifyBy]   [nvarchar](50)  NULL,
    [ModifyTime] [datetime]      NULL,
    [Version]    [int]           NOT NULL,
    [Extension]  [ntext]         NULL,
    [AppId]      [varchar](50)   NOT NULL,
    [CompNo]     [varchar](50)   NOT NULL,
    [FlowCode]   [varchar](50)   NOT NULL,
    [FlowName]   [nvarchar](50)  NOT NULL,
    [FlowStatus] [nvarchar](50)  NOT NULL,
    [BizId]      [varchar](50)   NOT NULL,
    [BizName]    [nvarchar](200) NOT NULL,
    [BizUrl]     [varchar](200)  NOT NULL,
    [BizStatus]  [nvarchar](50)  NOT NULL,
    [CurrStep]   [nvarchar](50)  NOT NULL,
    [CurrBy]     [nvarchar](200) NOT NULL,
    [PrevStep]   [nvarchar](50)  NULL,
    [PrevBy]     [nvarchar](200) NULL,
    [NextStep]   [nvarchar](50)  NULL,
    [NextBy]     [nvarchar](200) NULL,
    [ApplyBy]    [nvarchar](50)  NULL,
    [ApplyTime]  [datetime]      NULL,
    [VerifyBy]   [nvarchar](50)  NULL,
    [VerifyTime] [datetime]      NULL,
    [VerifyNote] [nvarchar](500) NULL,
    CONSTRAINT [PK_SysFlow] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysFlowLog] (
    [Id]          [varchar](50)    NOT NULL,
    [CreateBy]    [nvarchar](50)   NOT NULL,
    [CreateTime]  [datetime]       NOT NULL,
    [ModifyBy]    [nvarchar](50)   NULL,
    [ModifyTime]  [datetime]       NULL,
    [Version]     [int]            NOT NULL,
    [Extension]   [ntext]          NULL,
    [AppId]       [varchar](50)    NOT NULL,
    [CompNo]      [varchar](50)    NOT NULL,
    [BizId]       [varchar](50)    NOT NULL,
    [StepName]    [nvarchar](50)   NOT NULL,
    [ExecuteBy]   [nvarchar](50)   NOT NULL,
    [ExecuteTime] [datetime]       NOT NULL,
    [Result]      [nvarchar](50)   NOT NULL,
    [Note]        [nvarchar](1000) NULL,
    CONSTRAINT [PK_SysFlowLog] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysFlowStep] (
    [Id]            [varchar](50)    NOT NULL,
    [CreateBy]      [nvarchar](50)   NOT NULL,
    [CreateTime]    [datetime]       NOT NULL,
    [ModifyBy]      [nvarchar](50)   NULL,
    [ModifyTime]    [datetime]       NULL,
    [Version]       [int]            NOT NULL,
    [Extension]     [ntext]          NULL,
    [AppId]         [varchar](50)    NOT NULL,
    [CompNo]        [varchar](50)    NOT NULL,
    [FlowCode]      [varchar](50)    NOT NULL,
    [FlowName]      [nvarchar](50)   NOT NULL,
    [StepCode]      [varchar](50)    NOT NULL,
    [StepName]      [nvarchar](50)   NOT NULL,
    [StepType]      [nvarchar](50)   NOT NULL,
    [OperateBy]     [varchar](50)    NULL,
    [OperateByName] [nvarchar](50)   NULL,
    [Note]          [nvarchar](500)  NULL,
    [X]             [int]            NULL,
    [Y]             [int]            NULL,
    [IsRound]       [int]            NULL,
    [Arrows]        [nvarchar](4000) NULL,
    CONSTRAINT [PK_SysFlowStep] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysNoRule] (
    [Id]          [varchar](50)    NOT NULL,
    [CreateBy]    [nvarchar](50)   NOT NULL,
    [CreateTime]  [datetime]       NOT NULL,
    [ModifyBy]    [nvarchar](50)   NULL,
    [ModifyTime]  [datetime]       NULL,
    [Version]     [int]            NOT NULL,
    [Extension]   [ntext]          NULL,
    [AppId]       [varchar](50)    NOT NULL,
    [CompNo]      [varchar](50)    NOT NULL,
    [Code]        [varchar](50)    NOT NULL,
    [Name]        [nvarchar](50)   NOT NULL,
    [Description] [nvarchar](500)  NULL,
    [RuleData]    [nvarchar](4000) NULL,
    CONSTRAINT [PK_SysNoRule] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysNoRuleData] (
    [AppId]  [varchar](50)  NOT NULL,
    [CompNo] [varchar](50)  NOT NULL,
    [RuleId] [varchar](50)  NOT NULL,
    [RuleNo] [nvarchar](50) NOT NULL
) 
GO