CREATE TABLE [SysLog] (
    [Id]         [varchar](50)  NOT NULL,
    [CreateBy]   [nvarchar](50) NOT NULL,
    [CreateTime] [datetime]     NOT NULL,
    [ModifyBy]   [nvarchar](50) NULL,
    [ModifyTime] [datetime]     NULL,
    [Version]    [int]          NOT NULL,
    [Extension]  [ntext]        NULL,
    [CompNo]     [varchar](50)  NOT NULL,
    [Type]       [varchar](50)  NULL,
    [Target]     [varchar](50)  NULL,
    [Content]    [ntext]        NULL,
    CONSTRAINT [PK_SysLog] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysDictionary] (
    [Id] [varchar](50) NOT NULL,
    [CreateBy] [nvarchar](50) NOT NULL,
    [CreateTime] [datetime] NOT NULL,
    [ModifyBy] [nvarchar](50) NULL,
    [ModifyTime] [datetime] NULL,
    [Version] [int] NOT NULL,
    [Extension] [ntext] NULL,
    [CompNo] [varchar](50) NOT NULL,
    [Category] [varchar](50) NULL,
    [CategoryName] [nvarchar](50) NULL,
    [Code] [varchar](50) NULL,
    [Name] [nvarchar](50) NULL,
    [Sort] [int] NOT NULL,
    [Enabled] [int] NOT NULL,
    [Note] [ntext] NULL,
    CONSTRAINT [PK_SysDictionary] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysOrganization] (
    [Id] [varchar](50) NOT NULL,
    [CreateBy] [nvarchar](50) NOT NULL,
    [CreateTime] [datetime] NOT NULL,
    [ModifyBy] [nvarchar](50) NULL,
    [ModifyTime] [datetime] NULL,
    [Version] [int] NOT NULL,
    [Extension] [ntext] NULL,
    [CompNo] [varchar](50) NOT NULL,
    [ParentId] [varchar](50) NULL,
    [Code] [varchar](50) NULL,
    [Name] [nvarchar](50) NOT NULL,
    [ManagerId] [varchar](50) NULL,
    [Note] [ntext] NULL,
    CONSTRAINT [PK_SysOrganization] PRIMARY KEY ([Id] ASC)
)
GO

CREATE TABLE [SysRole] (
    [Id] [varchar](50) NOT NULL,
    [CreateBy] [nvarchar](50) NOT NULL,
    [CreateTime] [datetime] NOT NULL,
    [ModifyBy] [nvarchar](50) NULL,
    [ModifyTime] [datetime] NULL,
    [Version] [int] NOT NULL,
    [Extension] [ntext] NULL,
    [CompNo] [varchar](50) NOT NULL,
    [Name] [nvarchar](50) NOT NULL,
    [Enabled] [int] NOT NULL,
    [Note] [ntext] NULL,
    CONSTRAINT [PK_SysRole] PRIMARY KEY ([Id] ASC)
) 
GO

CREATE TABLE [SysRoleModule] (
    [RoleId] [varchar](50) NOT NULL,
    [ModuleId] [varchar](50) NOT NULL,
    CONSTRAINT [PK_SysRoleModule] PRIMARY KEY ([RoleId] ASC,[ModuleId] ASC)
) 
GO

CREATE TABLE [SysUser] (
    [Id] [varchar](50) NOT NULL,
    [CreateBy] [nvarchar](50) NOT NULL,
    [CreateTime] [datetime] NOT NULL,
    [ModifyBy] [nvarchar](50) NULL,
    [ModifyTime] [datetime] NULL,
    [Version] [int] NOT NULL,
    [Extension] [ntext] NULL,
    [CompNo] [varchar](50) NOT NULL,
    [OrgNo] [varchar](50) NOT NULL,
    [UserName] [varchar](50) NOT NULL,
    [Password] [varchar](50) NOT NULL,
    [Name] [nvarchar](50) NOT NULL,
    [EnglishName] [varchar](50) NULL,
    [Gender] [varchar](50) NULL,
    [Phone] [varchar](50) NULL,
    [Mobile] [varchar](50) NULL,
    [Email] [varchar](50) NULL,
    [Enabled] [int] NOT NULL,
    [Note] [ntext] NULL,
    [FirstLoginTime] [datetime] NULL,
    [FirstLoginIP] [varchar](50) NULL,
    [LastLoginTime] [datetime] NULL,
    [LastLoginIP] [varchar](50) NULL,
    CONSTRAINT [PK_SysUser] PRIMARY KEY ([Id] ASC)
) 
GO
insert into SysUser(Id,CreateBy,CreateTime,Version,CompNo,OrgNo,UserName,Password,Name,EnglishName,Enabled)
values('System','System','2020-04-01',1,'known','known','System','c4ca4238a0b923820dcc509a6f75849b','超级管理员','System',1)
GO
insert into SysUser(Id,CreateBy,CreateTime,Version,CompNo,OrgNo,UserName,Password,Name,EnglishName,Enabled)
values('101ffa5246714083967622761898ea6e','System','2020-04-01',1,'known','known','admin','c4ca4238a0b923820dcc509a6f75849b','管理员','Administrator',1)
GO

CREATE TABLE [SysUserModule] (
    [UserId] [varchar](50) NOT NULL,
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