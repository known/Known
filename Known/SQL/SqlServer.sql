CREATE TABLE APrototype (
    [Id]         [varchar](50)  not null,
    [CreateBy]   [nvarchar](50) not null,
    [CreateTime] [datetime]     not null,
    [ModifyBy]   [nvarchar](50) null,
    [ModifyTime] [datetime]     null,
    [Version]    [int]          not null,
    [Extension]  [ntext]        null,
    [AppId]      [varchar](50)  null,
    [CompNo]     [varchar](50)  not null,
    [Type]       [varchar](50)  null,
    [HeadId]     [varchar](50)  null,
    [Json]       [ntext]        null,
    CONSTRAINT [PK_APrototype] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE SysModule (
    [Id]         [varchar](50)  not null,
    [CreateBy]   [nvarchar](50) not null,
    [CreateTime] [datetime]     not null,
    [ModifyBy]   [nvarchar](50) null,
    [ModifyTime] [datetime]     null,
    [Version]    [int]          not null,
    [Extension]  [ntext]        null,
    [AppId]      [varchar](50)  null,
    [CompNo]     [varchar](50)  not null,
    [ParentId]   [varchar](50)  null,
    [Type]       [nvarchar](50) not null,
    [Code]       [varchar](50)  null,
    [Name]       [nvarchar](50) not null,
    [Icon]       [varchar](50)  null,
    [Url]        [varchar](200) null,
    [Target]     [varchar](50)  null,
    [Sort]       [int]          not null,
    [Enabled]    [int]          not null,
    [Note]       [ntext]        null,
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
    [Name]       [nvarchar](50)  NOT NULL,
    [Type]       [nvarchar](50)  NULL,
    [Path]       [nvarchar](250) NOT NULL,
    [Size]       [int]           NOT NULL,
    [SourceName] [nvarchar](50)  NOT NULL,
    [ExtName]    [varchar](50)   NOT NULL,
    [Note]       [nvarchar](500) NULL,
    [BizId]      [varchar](50)   NULL,
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
    [Note]       [nvarchar](4000) NULL,
    CONSTRAINT [PK_SysTask] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysDictionary] (
    [Id]           [varchar](50)  NOT NULL,
    [CreateBy]     [nvarchar](50) NOT NULL,
    [CreateTime]   [datetime]     NOT NULL,
    [ModifyBy]     [nvarchar](50) NULL,
    [ModifyTime]   [datetime]     NULL,
    [Version]      [int]          NOT NULL,
    [Extension]    [ntext]        NULL,
    [AppId]        [varchar](50)  NOT NULL,
    [CompNo]       [varchar](50)  NOT NULL,
    [Category]     [nvarchar](50) NULL,
    [CategoryName] [nvarchar](50) NULL,
    [Code]         [nvarchar](50) NULL,
    [Name]         [nvarchar](50) NULL,
    [Sort]         [int]          NOT NULL,
    [Enabled]      [int]          NOT NULL,
    [Note]         [ntext]        NULL,
    CONSTRAINT [PK_SysDictionary] PRIMARY KEY ([Id] ASC)
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
    [Enabled]    [int]          NOT NULL,
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
    [Enabled]        [int]           NOT NULL,
    [Note]           [ntext]         NULL,
    [FirstLoginTime] [datetime]      NULL,
    [FirstLoginIP]   [varchar](50)   NULL,
    [LastLoginTime]  [datetime]      NULL,
    [LastLoginIP]    [varchar](50)   NULL,
    [Role]           [nvarchar](500) NULL,
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

CREATE TABLE [SysUserMessage] (
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
    [MsgBy]      [varchar](50)    NOT NULL,
    [MsgByName]  [nvarchar](50)   NOT NULL,
    [MsgLevel]   [nvarchar](50)   NOT NULL,
    [Category]   [nvarchar](50)   NULL,
    [Subject]    [nvarchar](250)  NOT NULL,
    [Content]    [nvarchar](4000) NOT NULL,
    [FilePath]   [nvarchar](500)  NULL,
    [IsHtml]     [varchar](50)    NOT NULL,
    [Status]     [nvarchar](50)   NOT NULL,
    [BizId]      [varchar](50)    NULL,
    CONSTRAINT [PK_SysUserMessage] PRIMARY KEY ([Id] ASC)
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
    [CurrById]   [varchar](200)  NOT NULL,
    [CurrBy]     [nvarchar](100) NOT NULL,
    [PrevStep]   [nvarchar](50)  NULL,
    [PrevById]   [varchar](200)  NULL,
    [PrevBy]     [nvarchar](100) NULL,
    [NextStep]   [nvarchar](50)  NULL,
    [NextById]   [varchar](200)  NULL,
    [NextBy]     [nvarchar](100) NULL,
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
    [ExecuteById] [varchar](50)    NOT NULL,
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