CREATE TABLE `TbApply` (
    `Id`          VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`    VarChar(50)  NOT NULL,
    `CreateTime`  DateTime     NOT NULL,
    `ModifyBy`    VarChar(50)  NULL,
    `ModifyTime`  DateTime     NULL,
    `Version`     Long         NOT NULL,
    `Extension`   LongText     NULL,
    `AppId`       VarChar(50)  NULL,
    `CompNo`      VarChar(50)  NOT NULL,
    `BizType`     VarChar(50)  NOT NULL,
    `BizNo`       VarChar(50)  NOT NULL,
    `BizTitle`    VarChar(100) NOT NULL,
    `BizContent`  LongText     NULL,
    `BizFile`     VarChar(250) NULL
)
GO

CREATE TABLE `TbApplyList` (
    `Id`         VarChar(50)  NOT NULL PRIMARY KEY,
    `CreateBy`   VarChar(50)  NOT NULL,
    `CreateTime` DateTime     NOT NULL,
    `ModifyBy`   VarChar(50)  NULL,
    `ModifyTime` DateTime     NULL,
    `Version`    Long         NOT NULL,
    `Extension`  LongText     NULL,
    `AppId`      VarChar(50)  NOT NULL,
    `CompNo`     VarChar(50)  NOT NULL,
    `HeadId`     VarChar(50)  NOT NULL,
    `Item`       VarChar(100) NULL,
    `Note`       LongText     NULL
)
GO