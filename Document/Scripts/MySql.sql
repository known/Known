create table `APrototype` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  null,
    `CompNo`     varchar(50)  not null,
    `Type`       varchar(50)  null,
    `HeadId`     varchar(50)  null,
    `Json`       text         null,
    PRIMARY KEY (`Id`)
);

create table `SysModule` (
    `Id`          varchar(50)  not null,
    `CreateBy`    varchar(50)  not null,
    `CreateTime`  datetime     not null,
    `ModifyBy`    varchar(50)  null,
    `ModifyTime`  datetime     null,
    `Version`     int          not null,
    `Extension`   text         null,
    `AppId`       varchar(50)  null,
    `CompNo`      varchar(50)  not null,
    `ParentId`    varchar(50)  null,
    `Code`        varchar(50)  null,
    `Name`        varchar(50)  not null,
    `Icon`        varchar(50)  null,
    `Description` varchar(200) null,
    `Target`      varchar(250) null,
    `Sort`        int          not null,
    `Enabled`     varchar(50)  not null,
    `ButtonData`  text         null,
    `ActionData`  text         null,
    `ColumnData`  text         null,
    `Note`        text         null,
    PRIMARY KEY (`Id`)
);

create table `SysConfig` (
    `AppId`       varchar(50)   not null,
    `ConfigKey`   varchar(50)   not null,
    `ConfigValue` varchar(4000) not null,
    PRIMARY KEY(`AppId`,`ConfigKey`)
);

create table `SysSetting` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  not null,
    `CompNo`     varchar(50)  not null,
    `BizType`    varchar(50)  not null,
    `BizName`    varchar(250) null,
    `BizData`    text         null,
    PRIMARY KEY (`Id`)
);

create table `SysLog` (
    `Id`         varchar(50) not null,
    `CreateBy`   varchar(50) not null,
    `CreateTime` datetime    not null,
    `ModifyBy`   varchar(50) null,
    `ModifyTime` datetime    null,
    `Version`    int         not null,
    `Extension`  text        null,
    `AppId`      varchar(50) not null,
    `CompNo`     varchar(50) not null,
    `Type`       varchar(50) null,
    `Target`     varchar(50) null,
    `Content`    text        null,
    PRIMARY KEY (`Id`)
);

create table `SysFile` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  not null,
    `CompNo`     varchar(50)  not null,
    `Category1`  varchar(50)  not null,
    `Category2`  varchar(50)  null,
    `Name`       varchar(250) not null,
    `Type`       varchar(50)  null,
    `Path`       varchar(500) not null,
    `Size`       int          not null,
    `SourceName` varchar(250)  not null,
    `ExtName`    varchar(50)  not null,
    `Note`       varchar(500) null,
    `BizId`      varchar(50)  null,
    `ThumbPath`  varchar(500) null,
    PRIMARY KEY(`Id`)
);

create table `SysTask` (
    `Id`         varchar(50)   not null,
    `CreateBy`   varchar(50)   not null,
    `CreateTime` datetime      not null,
    `ModifyBy`   varchar(50)   null,
    `ModifyTime` datetime      null,
    `Version`    int           not null,
    `Extension`  text          null,
    `AppId`      varchar(50)   not null,
    `CompNo`     varchar(50)   not null,
    `BizId`      varchar(50)   not null,
    `Type`       varchar(50)   not null,
    `Name`       varchar(50)   not null,
    `Target`     varchar(200)  not null,
    `Status`     varchar(50)   not null,
    `BeginTime`  datetime      null,
    `EndTime`    datetime      null,
    `Note`       varchar(4000) null,
    PRIMARY KEY(`Id`)
);

create table `SysDictionary` (
    `Id`           varchar(50)   not null,
    `CreateBy`     varchar(50)   not null,
    `CreateTime`   datetime      not null,
    `ModifyBy`     varchar(50)   null,
    `ModifyTime`   datetime      null,
    `Version`      int           not null,
    `Extension`    text          null,
    `AppId`        varchar(50)   not null,
    `CompNo`       varchar(50)   not null,
    `Category`     varchar(50)   null,
    `CategoryName` varchar(50)   null,
    `Code`         varchar(100)  null,
    `Name`         varchar(250)  null,
    `Sort`         int           not null,
    `Enabled`      varchar(50)   not null,
    `Note`         text          null,
    `Child`        text          null,
    PRIMARY KEY (`Id`)
);

create table `SysTenant` (
    `Id`         varchar(50)      not null,
    `CreateBy`   varchar(50)      not null,
    `CreateTime` datetime         not null,
    `ModifyBy`   varchar(50)      null,
    `ModifyTime` datetime         null,
    `Version`    int              not null,
    `Extension`  text             null,
    `AppId`      varchar(50)      not null,
    `CompNo`     varchar(50)      not null,
    `Code`       varchar(50)      not null,
    `Name`       varchar(50)      not null,
    `Enabled`    varchar(50)      not null,
    `OperateBy`  varchar(250)     null,
    `UserCount`  int              not null,
    `BillCount`  int              not null,
    `Note`       text             null,
    PRIMARY KEY(`Id`)
);

create table `SysCompany` (
    `Id`          varchar(50)  not null,
    `CreateBy`    varchar(50)  not null,
    `CreateTime`  datetime     not null,
    `ModifyBy`    varchar(50)  null,
    `ModifyTime`  datetime     null,
    `Version`     int          not null,
    `Extension`   text         null,
    `AppId`       varchar(50)  not null,
    `CompNo`      varchar(50)  not null,
    `Code`        varchar(50)  null,
    `Name`        varchar(250) null,
    `NameEn`      varchar(250) null,
    `SccNo`       varchar(18)  null,
    `Industry`    varchar(50)  null,
    `Region`      varchar(50)  null,
    `Address`     varchar(500) null,
    `AddressEn`   varchar(500) null,
    `Contact`     varchar(50)  null,
    `Phone`       varchar(50)  null,
    `Note`        text         null,
    `SystemData`  text         null,
    `CompanyData` text         null,
    PRIMARY KEY(`Id`)
);

create table `SysOrganization` (
    `Id`         varchar(50) not null,
    `CreateBy`   varchar(50) not null,
    `CreateTime` datetime    not null,
    `ModifyBy`   varchar(50) null,
    `ModifyTime` datetime    null,
    `Version`    int         not null,
    `Extension`  text        null,
    `AppId`      varchar(50) not null,
    `CompNo`     varchar(50) not null,
    `ParentId`   varchar(50) null,
    `Code`       varchar(50) null,
    `Name`       varchar(50) not null,
    `ManagerId`  varchar(50) null,
    `Note`       text        null,
    PRIMARY KEY (`Id`)
);

create table `SysRole` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  not null,
    `CompNo`     varchar(50)  not null,
    `Name`       varchar(50)  not null,
    `Enabled`    varchar(50)  not null,
    `Note`       text         null,
    PRIMARY KEY (`Id`)
);

create table `SysRoleModule` (
    `RoleId`   varchar(50) not null,
    `ModuleId` varchar(50) not null,
    PRIMARY KEY (`RoleId`,`ModuleId`)
);

create table `SysUser` (
    `Id`             varchar(50)  not null,
    `CreateBy`       varchar(50)  not null,
    `CreateTime`     datetime     not null,
    `ModifyBy`       varchar(50)  null,
    `ModifyTime`     datetime     null,
    `Version`        int          not null,
    `Extension`      text         null,
    `AppId`          varchar(50)  not null,
    `CompNo`         varchar(50)  not null,
    `OrgNo`          varchar(50)  not null,
    `UserName`       varchar(50)  not null,
    `Password`       varchar(50)  not null,
    `Name`           varchar(50)  not null,
    `EnglishName`    varchar(50)  null,
    `Gender`         varchar(50)  null,
    `Phone`          varchar(50)  null,
    `Mobile`         varchar(50)  null,
    `Email`          varchar(50)  null,
    `Enabled`        varchar(50)  not null,
    `Note`           text         null,
    `FirstLoginTime` datetime     null,
    `FirstLoginIP`   varchar(50)  null,
    `LastLoginTime`  datetime     null,
    `LastLoginIP`    varchar(50)  null,
    `Type`           varchar(50)  null,
    `Role`           varchar(500) null,
    `Data`           text         null,
    PRIMARY KEY (`Id`)
);

create table `SysUserModule` (
    `UserId`   varchar(50) not null,
    `ModuleId` varchar(50) not null,
    PRIMARY KEY (`UserId`,`ModuleId`)
);

create table `SysUserRole` (
    `UserId` varchar(50) not null,
    `RoleId` varchar(50) not null,
    PRIMARY KEY (`UserId`,`RoleId`)
);

create table `SysNotice` (
    `Id`          varchar(50)   not null,
    `CreateBy`    varchar(50)   not null,
    `CreateTime`  datetime      not null,
    `ModifyBy`    varchar(50)   null,
    `ModifyTime`  datetime      null,
    `Version`     int           not null,
    `Extension`   text          null,
    `AppId`       varchar(50)   not null,
    `CompNo`      varchar(50)   not null,
    `Status`      varchar(50)   not null,
    `Title`       varchar(50)   not null,
    `Content`     varchar(4000) null,
    `PublishBy`   varchar(50)   null,
    `PublishTime` datetime      null,
    PRIMARY KEY(`Id`)
);

create table `SysUserLink` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  not null,
    `CompNo`     varchar(50)  not null,
    `UserName`   varchar(50)  not null,
    `Type`       varchar(50)  not null,
    `Name`       varchar(50)  not null,
    `Address`    varchar(200) not null,
    PRIMARY KEY(`Id`)
);

create table `SysMessage` (
    `Id`         varchar(50)   not null,
    `CreateBy`   varchar(50)   not null,
    `CreateTime` datetime      not null,
    `ModifyBy`   varchar(50)   null,
    `ModifyTime` datetime      null,
    `Version`    int           not null,
    `Extension`  text          null,
    `AppId`      varchar(50)   not null,
    `CompNo`     varchar(50)   not null,
    `UserId`     varchar(50)   not null,
    `Type`       varchar(50)   not null,
    `MsgBy`      varchar(50)   not null,
    `MsgLevel`   varchar(50)   not null,
    `Category`   varchar(50)   null,
    `Subject`    varchar(250)  not null,
    `Content`    varchar(4000) not null,
    `FilePath`   varchar(500)  null,
    `IsHtml`     varchar(50)   not null,
    `Status`     varchar(50)   not null,
    `BizId`      varchar(50)   null,
    PRIMARY KEY(`Id`)
);

create table `SysFlow` (
    `Id`         varchar(50)  not null,
    `CreateBy`   varchar(50)  not null,
    `CreateTime` datetime     not null,
    `ModifyBy`   varchar(50)  null,
    `ModifyTime` datetime     null,
    `Version`    int          not null,
    `Extension`  text         null,
    `AppId`      varchar(50)  not null,
    `CompNo`     varchar(50)  not null,
    `FlowCode`   varchar(50)  not null,
    `FlowName`   varchar(50)  not null,
    `FlowStatus` varchar(50)  not null,
    `BizId`      varchar(50)  not null,
    `BizName`    varchar(200) not null,
    `BizUrl`     varchar(200) not null,
    `BizStatus`  varchar(50)  not null,
    `CurrStep`   varchar(50)  not null,
    `CurrBy`     varchar(200) not null,
    `PrevStep`   varchar(50)  null,
    `PrevBy`     varchar(200) null,
    `NextStep`   varchar(50)  null,
    `NextBy`     varchar(200) null,
    `ApplyBy`    varChar(50)  null,
    `ApplyTime`  datetime     null,
    `VerifyBy`   varchar(50)  null,
    `VerifyTime` datetime     null,
    `VerifyNote` varchar(500) null,
    PRIMARY KEY(`Id`)
);

create table `SysFlowLog` (
    `Id`          varchar(50)   not null,
    `CreateBy`    varchar(50)   not null,
    `CreateTime`  datetime      not null,
    `ModifyBy`    varchar(50)   null,
    `ModifyTime`  datetime      null,
    `Version`     int           not null,
    `Extension`   text          null,
    `AppId`       varchar(50)   not null,
    `CompNo`      varchar(50)   not null,
    `BizId`       varchar(50)   not null,
    `StepName`    varchar(50)   not null,
    `ExecuteBy`   varchar(50)   not null,
    `ExecuteTime` datetime      not null,
    `Result`      varchar(50)   not null,
    `Note`        varchar(1000) null,
    PRIMARY KEY(`Id`)
);

create table `SysFlowStep` (
    `Id`            varchar(50)   not null,
    `CreateBy`      varchar(50)   not null,
    `CreateTime`    datetime      not null,
    `ModifyBy`      varchar(50)   null,
    `ModifyTime`    datetime      null,
    `Version`       int           not null,
    `Extension`     text          null,
    `AppId`         varchar(50)   not null,
    `CompNo`        varchar(50)   not null,
    `FlowCode`      varchar(50)   not null,
    `FlowName`      varchar(50)   not null,
    `StepCode`      varchar(50)   not null,
    `StepName`      varchar(50)   not null,
    `StepType`      varchar(50)   not null,
    `OperateBy`     varchar(50)   null,
    `OperateByName` varchar(50)   null,
    `Note`          varchar(500)  null,
    `X`             int           null,
    `Y`             int           null,
    `IsRound`       int           null,
    `Arrows`        varchar(4000) null,
    PRIMARY KEY(`Id`)
);

create table `SysNoRule` (
    `Id`          varchar(50)   not null,
    `CreateBy`    varchar(50)   not null,
    `CreateTime`  datetime      not null,
    `ModifyBy`    varchar(50)   null,
    `ModifyTime`  datetime      null,
    `Version`     int           not null,
    `Extension`   text          null,
    `AppId`       varchar(50)   not null,
    `CompNo`      varchar(50)   not null,
    `Code`        varchar(50)   not null,
    `Name`        varchar(50)   not null,
    `Description` varchar(500)  null,
    `RuleData`    varchar(4000) null,
    PRIMARY KEY(`Id`)
);

create table `SysNoRuleData` (
    `AppId`      varchar(50) not null,
    `CompNo`     varchar(50) not null,
    `RuleId`     varchar(50) not null,
    `RuleNo`     varchar(50) not null
);

INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Sort`,`Enabled`) VALUES ('aec2815390ab4af9943a051b6d67052b','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','0','BaseData','基础数据','fa fa-database',1,'True');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ButtonData`,`ColumnData`) VALUES ('93afcd01f640476bb35daec485cb0b70','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','aec2815390ab4af9943a051b6d67052b','CompanyForm','企业信息','fa fa-id-card-o','维护企业基本资料。','Known.Test.Pages.CompanyForm',1,'True','修改','[]');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ButtonData`,`ActionData`,`ColumnData`) VALUES ('d5ca63b57fb343c4992139fcf6215a01','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','aec2815390ab4af9943a051b6d67052b','SysOrgList','组织架构','fa fa-sitemap','维护企业组织架构信息。','Known.Razor.Pages.SysOrgList',2,'True','新增,批量删除','编辑,删除','[{"Id":"Code","Name":"编码","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ButtonData`,`ActionData`,`ColumnData`) VALUES ('832cc5034c4c45d9ba7c6fd3f7cb5ade','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','aec2815390ab4af9943a051b6d67052b','SysDicList','数据字典','fa fa-list-ul','维护系统所需的下拉框数据源。','Known.Razor.Pages.SysDicList',3,'True','新增,批量删除,导入','编辑,删除','[{"Id":"Category","Name":"类别","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Code","Name":"代码","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Sort","Name":"顺序","Type":1,"Align":1,"Width":60,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Sort`,`Enabled`) VALUES ('c92e432ac89d4e668cf243fa9fededa6','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','0','System','系统管理','fa fa-cogs',2,'True');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ColumnData`) VALUES ('12d3be601e804695a97530de6c4bea15','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysSystem','关于系统','fa fa-info-circle','显示系统版本及产品授权信息。','Known.Razor.Pages.SysSystem',1,'True','[]');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ButtonData`,`ActionData`,`ColumnData`) VALUES ('2944632b28844a54aad68c0f584febe2','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysTenantList','租户管理','fa fa-address-card-o','维护平台租户信息。','Known.Razor.Pages.SysTenantList',2,'True','新增','查看,编辑','[{"Id":"Code","Name":"账号","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"UserCount","Name":"用户数量","Type":1,"Align":2,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"BillCount","Name":"单据数量","Type":1,"Align":2,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateBy","Name":"创建人","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"创建时间","Type":4,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ButtonData`,`ActionData`,`ColumnData`) VALUES ('5c9eb1bdf02b49afa1ac90e7ae01b090','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysRoleList','角色管理','fa fa-users','维护系统用户角色及其菜单权限信息。','Known.Razor.Pages.SysRoleList',3,'True','新增,批量删除','编辑,删除','[{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":150,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ButtonData`,`ActionData`,`ColumnData`) VALUES ('44856b50a77d4639b3298c66202abb6e','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysUserList','用户管理','fa fa-user','维护系统用户账号及角色信息。','Known.Razor.Pages.SysUserList',4,'True','新增,批量删除,重置密码,启用,禁用','编辑,删除','[{"Id":"UserName","Name":"用户名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"姓名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Gender","Name":"性别","Type":0,"Align":1,"Width":80,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Phone","Name":"固定电话","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Mobile","Name":"移动电话","Type":0,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Email","Name":"电子邮件","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"状态","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Role","Name":"角色","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ColumnData`) VALUES ('63ed9926f3fd4fe4a99a337d0f76fd3e','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysLogList','系统日志','fa fa-clock-o','查询系统用户操作日志信息。','Known.Razor.Pages.SysLogList',5,'True','[{"Id":"CreateBy","Name":"操作人","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"操作时间","Type":4,"Align":1,"Width":120,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Type","Name":"操作类型","Type":0,"Align":1,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Target","Name":"操作对象","Type":0,"Align":0,"Width":200,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Content","Name":"操作内容","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ColumnData`) VALUES ('7214a7db358d4e968890fbd8573b49d3','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysFileList','系统附件','fa fa-file-o','查询系统所有附件信息。','Known.Razor.Pages.SysFileList',6,'True','[{"Id":"Category1","Name":"一级分类","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Category2","Name":"二级分类","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"文件名称","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Type","Name":"文件类型","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Size","Name":"文件大小","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"SourceName","Name":"原文件名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"ExtName","Name":"扩展名","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateBy","Name":"创建人","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"创建时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ColumnData`) VALUES ('91024299564b4b45a8d2de2d527028d4','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysTaskList','后台任务','fa fa-tasks','查询系统所有定时任务运行情况。','Known.Razor.Pages.SysTaskList',7,'True','[{"Id":"Type","Name":"类型","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Target","Name":"执行目标","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Status","Name":"执行状态","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":true,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"BeginTime","Name":"开始时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"EndTime","Name":"结束时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateBy","Name":"创建人","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"CreateTime","Name":"创建时间","Type":3,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsAdvQuery":true,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
INSERT INTO `SysModule` (`Id`,`CreateBy`,`CreateTime`,`ModifyBy`,`ModifyTime`,`Version`,`AppId`,`CompNo`,`ParentId`,`Code`,`Name`,`Icon`,`Description`,`Target`,`Sort`,`Enabled`,`ButtonData`,`ActionData`,`ColumnData`) VALUES ('d96d0db0b0694a48ab61ba00643a7884','admin','2023-05-20 08:08:08.6859586','admin','2023-05-20 08:08:08.6859586',1,'KIMS','puman','c92e432ac89d4e668cf243fa9fededa6','SysModuleList','模块管理','fa fa-cubes','维护系统菜单按钮及列表栏位信息。','Known.Razor.Pages.SysModuleList',8,'True','新增,复制,批量删除,移动','编辑,删除,上移,下移','[{"Id":"Code","Name":"代码","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Name","Name":"名称","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Description","Name":"描述","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Target","Name":"目标","Type":0,"Align":0,"Width":100,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Sort","Name":"顺序","Type":1,"Align":1,"Width":80,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Enabled","Name":"可用","Type":2,"Align":1,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false},{"Id":"Note","Name":"备注","Type":0,"Align":0,"Width":0,"Sort":0,"IsQuery":false,"IsSum":false,"IsSort":true,"IsVisible":true,"IsFixed":false}]');
