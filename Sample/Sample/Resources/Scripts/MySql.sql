create table `TbApply` (
    `Id`          varchar(50)   not null,
    `CreateBy`    varchar(50)   not null,
    `CreateTime`  datetime      not null,
    `ModifyBy`    varchar(50)   null,
    `ModifyTime`  datetime      null,
    `Version`     int           not null,
    `Extension`   text          null,
    `AppId`       varchar(50)   null,
    `CompNo`      varchar(50)   not null,
    `BizType`     varchar(50)   not null,
    `BizNo`       varchar(50)   not null,
    `BizTitle`    varchar(100)  not null,
    `BizContent`  text          null,
    `BizFile`     varchar(250)  null,
    PRIMARY KEY (`Id`)
);

create table `TbApplyList` (
    `Id`         varchar(50)   not null,
    `CreateBy`   varchar(50)   not null,
    `CreateTime` datetime      not null,
    `ModifyBy`   varchar(50)   null,
    `ModifyTime` datetime      null,
    `Version`    int           not null,
    `Extension`  text          null,
    `AppId`      varchar(50)   not null,
    `CompNo`     varchar(50)   not null,
    `HeadId`     varchar(50)   not null,
    `Item`       varchar(100)  null,
    `Note`       text          null,
    PRIMARY KEY (`Id`)
);