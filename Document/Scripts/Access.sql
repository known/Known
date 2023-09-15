CREATE TABLE `APrototype` (
    `Id`         VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)  NOT NULL,
    `CreateTime` DateTime     NOT NULL,
    `ModifyBy`   VarChar(50)  NULL,
    `ModifyTime` DateTime     NULL,
    `Version`    Long         NOT NULL,
    `Extension`  LongText     NULL,
    `AppId`      VarChar(50)  NULL,
    `CompNo`     VarChar(50)  NOT NULL,
    `Type`       VarChar(50)  NULL,
    `HeadId`     VarChar(50)  NULL,
    `Json`       LongText     NULL
)
GO

CREATE TABLE `SysModule` (
    `Id`          VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`    VarChar(50)  NOT NULL,
    `CreateTime`  DateTime     NOT NULL,
    `ModifyBy`    VarChar(50)  NULL,
    `ModifyTime`  DateTime     NULL,
    `Version`     Long         NOT NULL,
    `Extension`   LongText     NULL,
    `AppId`       VarChar(50)  NULL,
    `CompNo`      VarChar(50)  NOT NULL,
    `ParentId`    VarChar(50)  NULL,
    `Code`        VarChar(50)  NULL,
    `Name`        VarChar(50)  NOT NULL,
    `Icon`        VarChar(50)  NULL,
    `Description` VarChar(200) NULL,
    `Target`      VarChar(50)  NULL,
    `Sort`        Long         NOT NULL,
    `Enabled`     VarChar(50)  NOT NULL,
    `ButtonData`  LongText     NULL,
    `ActionData`  LongText     NULL,
    `ColumnData`  LongText     NULL,
    `Note`        LongText     NULL
)
GO

CREATE TABLE `SysConfig` (
    `AppId`       VarChar(50)    NOT NULL,
    `ConfigKey`   VarChar(50)    NOT NULL,
    `ConfigValue` LongText       NOT NULL,
    CONSTRAINT `PK_SysConfig` PRIMARY KEY (`AppId`, `ConfigKey`)
)
GO

CREATE TABLE `SysSetting` (
    `Id`         VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)  NOT NULL,
    `CreateTime` DateTime     NOT NULL,
    `ModifyBy`   VarChar(50)  NULL,
    `ModifyTime` DateTime     NULL,
    `Version`    Long         NOT NULL,
    `Extension`  LongText     NULL,
    `AppId`      VarChar(50)  NOT NULL,
    `CompNo`     VarChar(50)  NOT NULL,
    `BizType`    VarChar(50)  NOT NULL,
    `BizName`    VarChar(250) NULL,
    `BizData`    LongText     NULL
)
GO

CREATE TABLE `SysLog` (
    `Id`         VarChar(50) NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50) NOT NULL,
    `CreateTime` DateTime    NOT NULL,
    `ModifyBy`   VarChar(50) NULL,
    `ModifyTime` DateTime    NULL,
    `Version`    Long        NOT NULL,
    `Extension`  LongText    NULL,
    `AppId`      VarChar(50) NOT NULL,
    `CompNo`     VarChar(50) NOT NULL,
    `Type`       VarChar(50) NULL,
    `Target`     VarChar(50) NULL,
    `Content`    LongText    NULL
)
GO

CREATE TABLE `SysFile` (
    `Id`         VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)  NOT NULL,
    `CreateTime` DateTime     NOT NULL,
    `ModifyBy`   VarChar(50)  NULL,
    `ModifyTime` DateTime     NULL,
    `Version`    Long         NOT NULL,
    `Extension`  LongText     NULL,
    `AppId`      VarChar(50)  NOT NULL,
    `CompNo`     VarChar(50)  NOT NULL,
    `Category1`  VarChar(50)  NOT NULL,
    `Category2`  VarChar(50)  NULL,
    `Name`       VarChar(250) NOT NULL,
    `Type`       VarChar(50)  NULL,
    `Path`       VarChar(250) NOT NULL,
    `Size`       Long         NOT NULL,
    `SourceName` VarChar(250) NOT NULL,
    `ExtName`    VarChar(50)  NOT NULL,
    `Note`       LongText     NULL,
    `BizId`      VarChar(50)  NULL,
    `ThumbPath`  VarChar(250) NULL
)
GO

CREATE TABLE `SysTask` (
    `Id`         VarChar(50)   NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)   NOT NULL,
    `CreateTime` DateTime      NOT NULL,
    `ModifyBy`   VarChar(50)   NULL,
    `ModifyTime` DateTime      NULL,
    `Version`    Long          NOT NULL,
    `Extension`  LongText      NULL,
    `AppId`      VarChar(50)   NOT NULL,
    `CompNo`     VarChar(50)   NOT NULL,
    `BizId`      VarChar(50)   NOT NULL,
    `Type`       VarChar(50)   NOT NULL,
    `Name`       VarChar(50)   NOT NULL,
    `Target`     VarChar(200)  NOT NULL,
    `Status`     VarChar(50)   NOT NULL,
    `BeginTime`  DateTime      NULL,
    `EndTime`    DateTime      NULL,
    `Note`       LongText      NULL
)
GO

CREATE TABLE `SysDictionary` (
    `Id`           VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`     VarChar(50)  NOT NULL,
    `CreateTime`   DateTime     NOT NULL,
    `ModifyBy`     VarChar(50)  NULL,
    `ModifyTime`   DateTime     NULL,
    `Version`      Long         NOT NULL,
    `Extension`    LongText     NULL,
    `AppId`        VarChar(50)  NOT NULL,
    `CompNo`       VarChar(50)  NOT NULL,
    `Category`     VarChar(50)  NULL,
    `CategoryName` VarChar(50)  NULL,
    `Code`         VarChar(100) NULL,
    `Name`         VarChar(250) NULL,
    `Sort`         Long         NOT NULL,
    `Enabled`      VarChar(50)  NOT NULL,
    `Note`         LongText     NULL,
    `Child`        LongText     NULL
)
GO

CREATE TABLE `SysTenant` (
    `Id`         VarChar(50)      NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)      NOT NULL,
    `CreateTime` DateTime         NOT NULL,
    `ModifyBy`   VarChar(50)      NULL,
    `ModifyTime` DateTime         NULL,
    `Version`    Long             NOT NULL,
    `Extension`  LongText         NULL,
    `AppId`      VarChar(50)      NOT NULL,
    `CompNo`     VarChar(50)      NOT NULL,
    `Code`       VarChar(50)      NOT NULL,
    `Name`       VarChar(50)      NOT NULL,
    `Enabled`    VarChar(50)      NOT NULL,
    `OperateBy`  VarChar(250)     NULL,
    `UserCount`  Long             NOT NULL,
    `BillCount`  Long             NOT NULL,
    `Note`       LongText         NULL
)
GO

CREATE TABLE `SysCompany` (
    `Id`          VarChar(50)      NOT NULL PRIMARY KEY,
    `CreateBy`    VarChar(50)      NOT NULL,
    `CreateTime`  DateTime         NOT NULL,
    `ModifyBy`    VarChar(50)      NULL,
    `ModifyTime`  DateTime         NULL,
    `Version`     Long             NOT NULL,
    `Extension`   LongText         NULL,
    `AppId`       VarChar(50)      NOT NULL,
    `CompNo`      VarChar(50)      NOT NULL,
    `Code`        VarChar(50)      NULL,
    `Name`        VarChar(250)     NULL,
    `NameEn`      VarChar(250)     NULL,
    `SccNo`       VarChar(18)      NULL,
    `Industry`    VarChar(50)      NULL,
    `Region`      VarChar(50)      NULL,
    `Address`     VarChar(250)     NULL,
    `AddressEn`   VarChar(250)     NULL,
    `Contact`     VarChar(50)      NULL,
    `Phone`       VarChar(50)      NULL,
    `Note`        LongText         NULL,
    `SystemData`  LongText         NULL,
    `CompanyData` LongText         NULL
)
GO

CREATE TABLE `SysOrganization` (
    `Id`         VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)  NOT NULL,
    `CreateTime` DateTime     NOT NULL,
    `ModifyBy`   VarChar(50)  NULL,
    `ModifyTime` DateTime     NULL,
    `Version`    Long         NOT NULL,
    `Extension`  LongText     NULL,
    `AppId`      VarChar(50)  NOT NULL,
    `CompNo`     VarChar(50)  NOT NULL,
    `ParentId`   VarChar(50)  NULL,
    `Code`       VarChar(50)  NULL,
    `Name`       VarChar(50)  NOT NULL,
    `ManagerId`  VarChar(50)  NULL,
    `Note`       LongText     NULL
)
GO

CREATE TABLE `SysRole` (
    `Id`         VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)  NOT NULL,
    `CreateTime` DateTime     NOT NULL,
    `ModifyBy`   VarChar(50)  NULL,
    `ModifyTime` DateTime     NULL,
    `Version`    Long         NOT NULL,
    `Extension`  LongText     NULL,
    `AppId`      VarChar(50)  NOT NULL,
    `CompNo`     VarChar(50)  NOT NULL,
    `Name`       VarChar(50)  NOT NULL,
    `Enabled`    VarChar(50)  NOT NULL,
    `Note`       LongText     NULL
)
GO

CREATE TABLE `SysRoleModule` (
    `RoleId`   VarChar(50) NOT NULL,
    `ModuleId` VarChar(50) NOT NULL,
    CONSTRAINT `PK_SysRoleModule` PRIMARY KEY (`RoleId`,`ModuleId`)
)
GO

CREATE TABLE `SysUser` (
    `Id`               VarChar(50)   NOT NULL PRIMARY KEY,
    `CreateBy`         VarChar(50)   NOT NULL,
    `CreateTime`       DateTime      NOT NULL,
    `ModifyBy`         VarChar(50)   NULL,
    `ModifyTime`       DateTime      NULL,
    `Version`          Long          NOT NULL,
    `Extension`        LongText      NULL,
    `AppId`            VarChar(50)   NOT NULL,
    `CompNo`           VarChar(50)   NOT NULL,
    `OrgNo`            VarChar(50)   NOT NULL,
    `UserName`         VarChar(50)   NOT NULL,
    `Password`         VarChar(50)   NOT NULL,
    `Name`             VarChar(50)   NOT NULL,
    `EnglishName`      VarChar(50)   NULL,
    `Gender`           VarChar(50)   NULL,
    `Phone`            VarChar(50)   NULL,
    `Mobile`           VarChar(50)   NULL,
    `Email`            VarChar(50)   NULL,
    `Enabled`          VarChar(50)   NOT NULL,
    `Note`             LongText      NULL,
    `FirstLoginTime`   DateTime      NULL,
    `FirstLoginIP`     VarChar(50)   NULL,
    `LastLoginTime`    DateTime      NULL,
    `LastLoginIP`      VarChar(50)   NULL,
    `Type`             VarChar(50)   NULL,
    `Role`             LongText      NULL,
    `Data`             LongText      NULL
)
GO

CREATE TABLE `SysUserModule` (
    `UserId`   VarChar(50) NOT NULL,
    `ModuleId` VarChar(50) NOT NULL,
    CONSTRAINT `PK_SysUserModule` PRIMARY KEY (`UserId`,`ModuleId`)
)
GO

CREATE TABLE `SysUserRole` (
    `UserId` VarChar(50) NOT NULL,
    `RoleId` VarChar(50) NOT NULL,
    CONSTRAINT `PK_SysUserRole` PRIMARY KEY (`UserId`,`RoleId`)
)
GO

CREATE TABLE `SysNotice` (
    `Id`          VarChar(50)   NOT NULL PRIMARY KEY,
    `CreateBy`    VarChar(50)   NOT NULL,
    `CreateTime`  DateTime      NOT NULL,
    `ModifyBy`    VarChar(50)   NULL,
    `ModifyTime`  DateTime      NULL,
    `Version`     Long          NOT NULL,
    `Extension`   LongText      NULL,
    `AppId`       VarChar(50)   NOT NULL,
    `CompNo`      VarChar(50)   NOT NULL,
    `Status`      VarChar(50)   NOT NULL,
    `Title`       VarChar(50)   NOT NULL,
    `Content`     LongText      NULL,
    `PublishBy`   VarChar(50)   NULL,
    `PublishTime` DateTime      NULL
)
GO

CREATE TABLE `SysUserLink` (
    `Id`         VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)  NOT NULL,
    `CreateTime` DateTime     NOT NULL,
    `ModifyBy`   VarChar(50)  NULL,
    `ModifyTime` DateTime     NULL,
    `Version`    Long         NOT NULL,
    `Extension`  LongText     NULL,
    `AppId`      VarChar(50)  NOT NULL,
    `CompNo`     VarChar(50)  NOT NULL,
    `UserName`   VarChar(50)  NOT NULL,
    `Type`       VarChar(50)  NOT NULL,
    `Name`       VarChar(50)  NOT NULL,
    `Address`    VarChar(200) NOT NULL
)
GO

CREATE TABLE `SysMessage` (
    `Id`         VarChar(50)   NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)   NOT NULL,
    `CreateTime` DateTime      NOT NULL,
    `ModifyBy`   VarChar(50)   NULL,
    `ModifyTime` DateTime      NULL,
    `Version`    Long          NOT NULL,
    `Extension`  LongText      NULL,
    `AppId`      VarChar(50)   NOT NULL,
    `CompNo`     VarChar(50)   NOT NULL,
    `UserId`     VarChar(50)   NOT NULL,
    `Type`       VarChar(50)   NOT NULL,
    `MsgBy`      VarChar(50)   NOT NULL,
    `MsgLevel`   VarChar(50)   NOT NULL,
    `Category`   VarChar(50)   NULL,
    `Subject`    VarChar(250)  NOT NULL,
    `Content`    LongText      NOT NULL,
    `FilePath`   LongText      NULL,
    `IsHtml`     VarChar(50)   NOT NULL,
    `Status`     VarChar(50)   NOT NULL,
    `BizId`      VarChar(50)   NULL
)
GO

CREATE TABLE `SysFlow` (
    `Id`         VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)  NOT NULL,
    `CreateTime` DateTime     NOT NULL,
    `ModifyBy`   VarChar(50)  NULL,
    `ModifyTime` DateTime     NULL,
    `Version`    Long         NOT NULL,
    `Extension`  LongText     NULL,
    `AppId`      VarChar(50)  NOT NULL,
    `CompNo`     VarChar(50)  NOT NULL,
    `FlowCode`   VarChar(50)  NOT NULL,
    `FlowName`   VarChar(50)  NOT NULL,
    `FlowStatus` VarChar(50)  NOT NULL,
    `BizId`      VarChar(50)  NOT NULL,
    `BizName`    VarChar(200) NOT NULL,
    `BizUrl`     VarChar(200) NOT NULL,
    `BizStatus`  VarChar(50)  NOT NULL,
    `CurrStep`   VarChar(50)  NOT NULL,
    `CurrBy`     VarChar(200) NOT NULL,
    `PrevStep`   VarChar(50)  NULL,
    `PrevBy`     VarChar(200) NULL,
    `NextStep`   VarChar(50)  NULL,
    `NextBy`     VarChar(200) NULL,
    `ApplyBy`    VarChar(50)  NULL,
    `ApplyTime`  DateTime     NULL,
    `VerifyBy`   VarChar(50)  NULL,
    `VerifyTime` DateTime     NULL,
    `VerifyNote` LongText     NULL
)
GO

CREATE TABLE `SysFlowLog` (
    `Id`          VarChar(50)   NOT NULL PRIMARY KEY,
    `CreateBy`    VarChar(50)   NOT NULL,
    `CreateTime`  DateTime      NOT NULL,
    `ModifyBy`    VarChar(50)   NULL,
    `ModifyTime`  DateTime      NULL,
    `Version`     Long          NOT NULL,
    `Extension`   LongText      NULL,
    `AppId`       VarChar(50)   NOT NULL,
    `CompNo`      VarChar(50)   NOT NULL,
    `BizId`       VarChar(50)   NOT NULL,
    `StepName`    VarChar(50)   NOT NULL,
    `ExecuteBy`   VarChar(50)   NOT NULL,
    `ExecuteTime` DateTime      NOT NULL,
    `Result`      VarChar(50)   NOT NULL,
    `Note`        LongText      NULL
)
GO

CREATE TABLE `SysFlowStep` (
    `Id`            VarChar(50)   NOT NULL PRIMARY KEY,
    `CreateBy`      VarChar(50)   NOT NULL,
    `CreateTime`    DateTime      NOT NULL,
    `ModifyBy`      VarChar(50)   NULL,
    `ModifyTime`    DateTime      NULL,
    `Version`       Long          NOT NULL,
    `Extension`     LongText      NULL,
    `AppId`         VarChar(50)   NOT NULL,
    `CompNo`        VarChar(50)   NOT NULL,
    `FlowCode`      VarChar(50)   NOT NULL,
    `FlowName`      VarChar(50)   NOT NULL,
    `StepCode`      VarChar(50)   NOT NULL,
    `StepName`      VarChar(50)   NOT NULL,
    `StepType`      VarChar(50)   NOT NULL,
    `OperateBy`     VarChar(50)   NULL,
    `OperateByName` VarChar(50)   NULL,
    `Note`          LongText      NULL,
    `X`             Long          NULL,
    `Y`             Long          NULL,
    `IsRound`       Long          NULL,
    `Arrows`        LongText      NULL
)
GO

CREATE TABLE `SysNoRule` (
    `Id`          VarChar(50)   NOT NULL PRIMARY KEY,
    `CreateBy`    VarChar(50)   NOT NULL,
    `CreateTime`  DateTime      NOT NULL,
    `ModifyBy`    VarChar(50)   NULL,
    `ModifyTime`  DateTime      NULL,
    `Version`     Long          NOT NULL,
    `Extension`   LongText      NULL,
    `AppId`       VarChar(50)   NOT NULL,
    `CompNo`      VarChar(50)   NOT NULL,
    `Code`        VarChar(50)   NOT NULL,
    `Name`        VarChar(50)   NOT NULL,
    `Description` LongText      NULL,
    `RuleData`    LongText      NULL
)
GO

CREATE TABLE `SysNoRuleData` (
    `AppId`  VarChar(50)  NOT NULL,
    `CompNo` VarChar(50)  NOT NULL,
    `RuleId` VarChar(50)  NOT NULL,
    `RuleNo` VarChar(50)  NOT NULL
)
GO

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
