create table APrototype (
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   null,
    CompNo     varchar2(50)   not null,
    Type       varchar2(50)   null,
    HeadId     varchar2(50)   null,
    Json       varchar2(4000) null
);
alter table APrototype add constraint PK_APrototype primary key (Id);

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
    ButtonData  varchar2(4000) null,
    ActionData  varchar2(4000) null,
    ColumnData  varchar2(4000) null,
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

create table SysTenant(
    Id         varchar2(50)     not null,
    CreateBy   varchar2(50)     not null,
    CreateTime date             not null,
    ModifyBy   varchar2(50)     null,
    ModifyTime date             null,
    Version    number(8)        not null,
    Extension  varchar2(4000)   null,
    AppId      varchar2(50)     not null,
    CompNo     varchar2(50)     not null,
    Code       varchar2(50)     not null,
    Name       varchar2(50)     not null,
    Enabled    varchar2(50)     not null,
    UserCount  number(8)        not null,
    BillCount  number(8)        not null,
    Note       varchar2(4000)   null
);
alter table SysTenant add constraint PK_SysTenant primary key(Id);

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

create table SysUserModule (
    UserId   varchar2(50) not null,
    ModuleId varchar2(50) not null
);
alter table SysUserModule add constraint PK_SysUserModule primary key (UserId,ModuleId);

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

create table SysUserLink(
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   not null,
    CompNo     varchar2(50)   not null,
    UserName   varchar2(50)   not null,
    Type       varchar2(50)   not null,
    Name       varchar2(50)   not null,
    Address    varchar2(200)  not null,
);
alter table SysUserLink add constraint PK_SysUserLink primary key(Id);

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

create table SysNoRule(
    Id          varchar2(50)   not null,
    CreateBy    varchar2(50)   not null,
    CreateTime  date           not null,
    ModifyBy    varchar2(50)   null,
    ModifyTime  date           null,
    Version     number(8)      not null,
    Extension   varchar2(4000) null,
    AppId       varchar2(50)   not null,
    CompNo      varchar2(50)   not null,
    Code        varchar2(50)   not null,
    Name        varchar2(50)   not null,
    Description varchar2(500)  null,
    RuleData    varchar2(4000) null
);
alter table SysNoRule add constraint PK_SysNoRule primary key(Id);

create table SysNoRuleData(
    AppId  varchar2(50) not null,
    CompNo varchar2(50) not null,
    RuleId varchar2(50) not null,
    RuleNo varchar2(50) not null
);

INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Sort,Enabled) VALUES ('aec2815390ab4af9943a051b6d67052b','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','0','BaseData','基础数据','fa fa-database',1,'True');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ButtonData,ColumnData) VALUES ('93afcd01f640476bb35daec485cb0b70','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','aec2815390ab4af9943a051b6d67052b','CompanyForm','企业信息','fa fa-id-card-o','维护企业基本资料。','Known.Test.Pages.CompanyForm',1,'True','修改','[]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ColumnData) VALUES ('12d3be601e804695a97530de6c4bea15','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysSystem','关于系统','fa fa-info-circle','显示系统版本及产品授权信息。','Known.Razor.Pages.SysSystem',1,'True','[]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Sort,Enabled) VALUES ('c92e432ac89d4e668cf243fa9fededa6','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','0','System','系统管理','fa fa-cogs',2,'True');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ButtonData,ActionData,ColumnData) VALUES ('d5ca63b57fb343c4992139fcf6215a01','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','aec2815390ab4af9943a051b6d67052b','SysOrgList','组织架构','fa fa-sitemap','维护企业组织架构信息。','Known.Razor.Pages.SysOrgList',2,'True','新增,批量删除','编辑,删除','[{"Id":"Code","Name":"编码","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ButtonData,ActionData,ColumnData) VALUES ('2944632b28844a54aad68c0f584febe2','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysTenantList','租户管理','fa fa-address-card-o','维护平台租户信息。','Known.Razor.Pages.SysTenantList',2,'True','新增','查看,编辑','[{"Id":"Code","Name":"账号","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"UserCount","Name":"用户数量","Type":1,"Align":2,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"BillCount","Name":"单据数量","Type":1,"Align":2,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateBy","Name":"创建人","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"创建时间","Type":4,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ButtonData,ActionData,ColumnData) VALUES ('832cc5034c4c45d9ba7c6fd3f7cb5ade','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','aec2815390ab4af9943a051b6d67052b','SysDicList','数据字典','fa fa-list-ul','维护系统所需的下拉框数据源。','Known.Razor.Pages.SysDicList',3,'True','新增,批量删除,导入','编辑,删除','[{"Id":"Category","Name":"类别","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Code","Name":"代码","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Sort","Name":"顺序","Type":1,"Align":1,"Width":60,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ButtonData,ActionData,ColumnData) VALUES ('5c9eb1bdf02b49afa1ac90e7ae01b090','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysRoleList','角色管理','fa fa-users','维护系统用户角色及其菜单权限信息。','Known.Razor.Pages.SysRoleList',3,'True','新增,批量删除','编辑,删除','[{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":150,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ColumnData) VALUES ('c73e7816bb38404194728fda02e98c98','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','0','DevSample','开发示例','fa fa-code','演示框架各组件示例','Known.Test.Pages.Samples.DevSample',3,'True','[]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ButtonData,ActionData,ColumnData) VALUES ('44856b50a77d4639b3298c66202abb6e','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysUserList','用户管理','fa fa-user','维护系统用户账号及角色信息。','Known.Razor.Pages.SysUserList',4,'True','新增,批量删除,重置密码,启用,禁用','编辑,删除','[{"Id":"UserName","Name":"用户名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"姓名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Gender","Name":"性别","Type":0,"Align":1,"Width":80,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Phone","Name":"固定电话","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Mobile","Name":"移动电话","Type":0,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Email","Name":"电子邮件","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Role","Name":"角色","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ColumnData) VALUES ('63ed9926f3fd4fe4a99a337d0f76fd3e','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysLogList','系统日志','fa fa-clock-o','查询系统用户操作日志信息。','Known.Razor.Pages.SysLogList',5,'True','[{"Id":"CreateBy","Name":"操作人","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"操作时间","Type":4,"Align":1,"Width":120,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Type","Name":"操作类型","Type":0,"Align":1,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Target","Name":"操作对象","Type":0,"Align":0,"Width":200,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Content","Name":"操作内容","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ColumnData) VALUES ('7214a7db358d4e968890fbd8573b49d3','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysFileList','系统附件','fa fa-file-o','查询系统所有附件信息。','Known.Razor.Pages.SysFileList',6,'True','[{"Id":"Category1","Name":"一级分类","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Category2","Name":"二级分类","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"文件名称","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Type","Name":"文件类型","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Size","Name":"文件大小","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"SourceName","Name":"原文件名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"ExtName","Name":"扩展名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateBy","Name":"创建人","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"创建时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ColumnData) VALUES ('91024299564b4b45a8d2de2d527028d4','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysTaskList','后台任务','fa fa-tasks','查询系统所有定时任务运行情况。','Known.Razor.Pages.SysTaskList',7,'True','[{"Id":"Type","Name":"类型","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Target","Name":"执行目标","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Status","Name":"执行状态","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"BeginTime","Name":"开始时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"EndTime","Name":"结束时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateBy","Name":"创建人","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"创建时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO SysModule (Id,CreateBy,CreateTime,ModifyBy,ModifyTime,Version,AppId,CompNo,ParentId,Code,Name,Icon,Description,Target,Sort,Enabled,ButtonData,ActionData,ColumnData) VALUES ('d96d0db0b0694a48ab61ba00643a7884','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysModuleList','模块管理','fa fa-cubes','维护系统菜单按钮及列表栏位信息。','Known.Razor.Pages.SysModuleList',8,'True','新增,复制,批量删除,移动','编辑,删除,上移,下移','[{"Id":"Code","Name":"代码","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Description","Name":"描述","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Target","Name":"目标","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Sort","Name":"顺序","Type":1,"Align":1,"Width":80,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"可用","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
