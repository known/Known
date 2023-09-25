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