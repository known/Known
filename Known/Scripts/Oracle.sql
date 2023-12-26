create table SysModule (
    Id          varchar2(50)   not null,
    CreateBy    varchar2(50)   not null,
    CreateTime  date           not null,
    ModifyBy    varchar2(50)   null,
    ModifyTime  date           null,
    Version     number(8)      not null,
    Extension   varchar2(4000) null,
    AppId       varchar2(50)   null,
    CompNo      varchar2(50)   not null,
    ParentId    varchar2(50)   null,
    Code        varchar2(50)   null,
    Name        varchar2(50)   not null,
    Icon        varchar2(50)   null,
    Description varchar2(200)  null,
    Target      varchar2(50)   null,
    Sort        number(8)      not null,
    Enabled     varchar2(50)   not null,
    EntityData  varchar2(4000)  null,
    PageData    varchar2(4000) null,
    FormData    varchar2(4000) null,
    Note        varchar2(4000) null
);
alter table SysModule add constraint PK_SysModule primary key (Id);

create table SysConfig(
    AppId       varchar2(50)   not null,
    ConfigKey   varchar2(50)   not null,
    ConfigValue varchar2(4000) not null
);
alter table SysConfig add constraint PK_SysConfig primary key(AppId,ConfigKey);

create table SysSetting (
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   not null,
    CompNo     varchar2(50)   not null,
    BizType    varchar2(50)   not null,
    BizName    varchar2(250)  null,
    BizData    varchar2(4000) null
);
alter table SysSetting add constraint PK_SysSetting primary key (Id);

create table SysLog (
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   not null,
    CompNo     varchar2(50)   not null,
    Type       varchar2(50)   null,
    Target     varchar2(50)   null,
    Content    varchar2(4000) null
);
alter table SysLog add constraint PK_SysLog primary key (Id);

create table SysFile(
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   not null,
    CompNo     varchar2(50)   not null,
    Category1  varchar2(50)   not null,
    Category2  varchar2(50)   null,
    Name       varchar2(250)  not null,
    Type       varchar2(50)   null,
    Path       varchar2(500)  not null,
    Size       number(8)      not null,
    SourceName varchar2(250)  not null,
    ExtName    varchar2(50)   not null,
    Note       varchar2(500)  null,
    BizId      varchar2(50)   null,
    ThumbPath  varchar2(500)  null
);
alter table SysFile add constraint PK_SysFile primary key(Id);

create table SysTask(
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   not null,
    CompNo     varchar2(50)   not null,
    BizId      varchar2(50)   not null,
    Type       varchar2(50)   not null,
    Name       varchar2(50)   not null,
    Target     varchar2(200)  not null,
    Status     varchar2(50)   not null,
    BeginTime  date           null,
    EndTime    date           null,
    Note       varchar2(4000) null
);
alter table SysTask add constraint PK_SysTask primary key(Id);

create table SysDictionary (
    Id           varchar2(50)    not null,
    CreateBy     varchar2(50)    not null,
    CreateTime   date            not null,
    ModifyBy     varchar2(50)    null,
    ModifyTime   date            null,
    Version      number(8)       not null,
    Extension    varchar2(4000)  null,
    AppId        varchar2(50)    not null,
    CompNo       varchar2(50)    not null,
    Category     varchar2(50)    null,
    CategoryName varchar2(50)    null,
    Code         varchar2(100)   null,
    Name         varchar2(250)   null,
    Sort         number(8)       not null,
    Enabled      varchar2(50)    not null,
    Note         varchar2(4000)  null,
    Child        varchar2(4000)  null
);
alter table SysDictionary add constraint PK_SysDictionary primary key (Id);

create table SysCompany(
    Id          varchar2(50)     not null,
    CreateBy    varchar2(50)     not null,
    CreateTime  date             not null,
    ModifyBy    varchar2(50)     null,
    ModifyTime  date             null,
    Version     number(8)        not null,
    Extension   varchar2(4000)   null,
    AppId       varchar2(50)     not null,
    CompNo      varchar2(50)     not null,
    Code        varchar2(50)     null,
    Name        varchar2(250)    null,
    NameEn      varchar2(250)    null,
    SccNo       varchar2(18)     null,
    Industry    varchar2(50)     null,
    Region      varchar2(50)     null,
    Address     varchar2(500)    null,
    AddressEn   varchar2(500)    null,
    Contact     varchar2(50)     null,
    Phone       varchar2(50)     null,
    Note        varchar2(4000)   null,
    SystemData  varchar2(4000)   null,
    CompanyData varchar2(4000)   null
);
alter table SysCompany add constraint PK_SysCompany primary key(Id);

create table SysOrganization (
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   not null,
    CompNo     varchar2(50)   not null,
    ParentId   varchar2(50)   null,
    Code       varchar2(50)   null,
    Name       varchar2(50)   not null,
    ManagerId  varchar2(50)   null,
    Note       varchar2(4000) null
);
alter table SysOrganization add constraint PK_SysOrganization primary key (Id);

create table SysRole (
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   not null,
    CompNo     varchar2(50)   not null,
    Name       varchar2(50)   not null,
    Enabled    varchar2(50)   not null,
    Note       varchar2(4000) null
);
alter table SysRole add constraint PK_SysRole primary key (Id);

create table SysRoleModule (
    RoleId   varchar2(50) not null,
    ModuleId varchar2(50) not null
);
alter table SysRoleModule add constraint PK_SysRoleModule primary key (RoleId,ModuleId);

create table SysUser (
    Id             varchar2(50)   not null,
    CreateBy       varchar2(50)   not null,
    CreateTime     date           not null,
    ModifyBy       varchar2(50)   null,
    ModifyTime     date           null,
    Version        number(8)      not null,
    Extension      varchar2(4000) null,
    AppId          varchar2(50)   not null,
    CompNo         varchar2(50)   not null,
    OrgNo          varchar2(50)   not null,
    UserName       varchar2(50)   not null,
    Password       varchar2(50)   not null,
    Name           varchar2(50)   not null,
    EnglishName    varchar2(50)   null,
    Gender         varchar2(50)   null,
    Phone          varchar2(50)   null,
    Mobile         varchar2(50)   null,
    Email          varchar2(50)   null,
    Enabled        varchar2(50)   not null,
    Note           varchar2(4000) null,
    FirstLoginTime date           null,
    FirstLoginIP   varchar2(50)   null,
    LastLoginTime  date           null,
    LastLoginIP    varchar2(50)   null,
    Type           varchar2(50)   null,
    Role           varchar2(500)  null,
    Data           varchar2(4000) null
);
alter table SysUser add constraint PK_SysUser primary key (Id);

create table SysUserRole (
    UserId varchar2(50) not null,
    RoleId varchar2(50) not null
);
alter table SysUserRole add constraint PK_SysUserRole primary key (UserId,RoleId);

create table SysNotice(
    Id          varchar2(50)   not null,
    CreateBy    varchar2(50)   not null,
    CreateTime  date           not null,
    ModifyBy    varchar2(50)   null,
    ModifyTime  date           null,
    Version     number(8)      not null,
    Extension   varchar2(4000) null,
    AppId       varchar2(50)   not null,
    CompNo      varchar2(50)   not null,
    Status      varchar2(50)   not null,
    Title       varchar2(50)   not null,
    Content     varchar2(4000) null,
    PublishBy   varchar2(50)   null,
    PublishTime date           null
);
alter table SysNotice add constraint PK_SysNotice primary key(Id);

create table SysMessage(
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   not null,
    CompNo     varchar2(50)   not null,
    UserId     varchar2(50)   not null,
    Type       varchar2(50)   not null,
    MsgBy      varchar2(50)   not null,
    MsgLevel   varchar2(50)   not null,
    Category   varchar2(50)   null,
    Subject    varchar2(250)  not null,
    Content    varchar2(4000) not null,
    FilePath   varchar2(500)  null,
    IsHtml     varchar2(50)   not null,
    Status     varchar2(50)   not null,
    BizId      varchar2(50)   null
);
alter table SysMessage add constraint PK_SysMessage primary key(Id);

create table SysFlow(
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   not null,
    CompNo     varchar2(50)   not null,
    FlowCode   varchar2(50)   not null,
    FlowName   varchar2(50)   not null,
    FlowStatus varchar2(50)   not null,
    BizId      varchar2(50)   not null,
    BizName    varchar2(200)  not null,
    BizUrl     varchar2(200)  not null,
    BizStatus  varchar2(50)   not null,
    CurrStep   varchar2(50)   not null,
    CurrBy     varchar2(200)  not null,
    PrevStep   varchar2(50)   null,
    PrevBy     varchar2(200)  null,
    NextStep   varchar2(50)   null,
    NextBy     varchar2(200)  null,
    ApplyBy    varchar2(50)   null,
    ApplyTime  date           null,
    VerifyBy   varchar2(50)   null,
    VerifyTime date           null,
    VerifyNote varchar2(500)  null
);
alter table SysFlow add constraint PK_SysFlow primary key(Id);

create table SysFlowLog(
    Id          varchar2(50)   not null,
    CreateBy    varchar2(50)   not null,
    CreateTime  date           not null,
    ModifyBy    varchar2(50)   null,
    ModifyTime  date           null,
    Version     number(8)      not null,
    Extension   varchar2(4000) null,
    AppId       varchar2(50)   not null,
    CompNo      varchar2(50)   not null,
    BizId       varchar2(50)   not null,
    StepName    varchar2(50)   not null,
    ExecuteBy   varchar2(50)   not null,
    ExecuteTime date           not null,
    Result      varchar2(50)   not null,
    Note        varchar2(1000) null
);
alter table SysFlowLog add constraint PK_SysFlowLog primary key(Id);

create table SysFlowStep(
    Id            varchar2(50)   not null,
    CreateBy      varchar2(50)   not null,
    CreateTime    date           not null,
    ModifyBy      varchar2(50)   null,
    ModifyTime    date           null,
    Version       number(8)      not null,
    Extension     varchar2(4000) null,
    AppId         varchar2(50)   not null,
    CompNo        varchar2(50)   not null,
    FlowCode      varchar2(50)   not null,
    FlowName      varchar2(50)   not null,
    StepCode      varchar2(50)   not null,
    StepName      varchar2(50)   not null,
    StepType      varchar2(50)   not null,
    OperateBy     varchar2(50)   null,
    OperateByName varchar2(50)   null,
    Note          varchar2(500)  null,
    X             number(8)      null,
    Y             number(8)      null,
    IsRound       number(8)      null,
    Arrows        varchar2(4000) null
);
alter table SysFlowStep add constraint PK_SysFlowStep primary key(Id);