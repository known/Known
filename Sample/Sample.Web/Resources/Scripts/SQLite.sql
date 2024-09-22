CREATE TABLE [TbApply] (
    [Id]          varchar(50)   NOT NULL PRIMARY KEY,
    [CreateBy]    nvarchar(50)  NOT NULL,
    [CreateTime]  datetime      NOT NULL,
    [ModifyBy]    nvarchar(50)  NULL,
    [ModifyTime]  datetime      NULL,
    [Version]     int           NOT NULL,
    [Extension]   ntext         NULL,
    [AppId]       varchar(50)   NULL,
    [CompNo]      varchar(50)   NOT NULL,
    [BizType]     varchar(50)   NOT NULL,
    [BizNo]       varchar(50)   NOT NULL,
    [BizTitle]    nvarchar(100) NOT NULL,
    [BizContent]  ntext         NULL,
    [BizFile]     varchar(250)  NULL
);

CREATE TABLE [TbApplyList] (
    [Id]         varchar(50)   NOT NULL PRIMARY KEY,
    [CreateBy]   nvarchar(50)  NOT NULL,
    [CreateTime] datetime      NOT NULL,
    [ModifyBy]   nvarchar(50)  NULL,
    [ModifyTime] datetime      NULL,
    [Version]    int           NOT NULL,
    [Extension]  ntext         NULL,
    [AppId]      varchar(50)   NOT NULL,
    [CompNo]     varchar(50)   NOT NULL,
    [HeadId]     varchar(50)   NOT NULL,
    [Item]       nvarchar(100) NULL,
    [Note]       ntext         NULL
);