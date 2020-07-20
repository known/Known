create table `SysLog` (
    `Id`         varchar(50) not null,
    `CreateBy`   varchar(50) not null,
    `CreateTime` datetime    not null,
    `ModifyBy`   varchar(50) null,
    `ModifyTime` datetime    null,
    `Version`    int         not null,
    `Extension`  text        null,
    `CompNo`     varchar(50) not null,
    `Type`       varchar(50) null,
    `Target`     varchar(50) null,
    `Content`    text        null,
    PRIMARY KEY (`Id`)
);

create table `SysDictionary` (
    `Id` varchar(50) not null,
    `CreateBy` varchar(50) not null,
    `CreateTime` datetime not null,
    `ModifyBy` varchar(50) null,
    `ModifyTime` datetime null,
    `Version` int not null,
    `Extension` text null,
    `CompNo` varchar(50) not null,
    `Category` varchar(50) null,
    `CategoryName` varchar(50) null,
    `Code` varchar(50) null,
    `Name` varchar(50) null,
    `Sort` int not null,
    `Enabled` int not null,
    `Note` text null,
    PRIMARY KEY (`Id`)
);

create table `SysOrganization` (
    `Id` varchar(50) not null,
    `CreateBy` varchar(50) not null,
    `CreateTime` datetime not null,
    `ModifyBy` varchar(50) null,
    `ModifyTime` datetime null,
    `Version` int not null,
    `Extension` text null,
    `CompNo` varchar(50) not null,
    `ParentId` varchar(50) null,
    `Code` varchar(50) null,
    `Name` varchar(50) not null,
    `ManagerId` varchar(50) null,
    `Note` text null,
    PRIMARY KEY (`Id`)
);

create table `SysRole` (
    `Id` varchar(50) not null,
    `CreateBy` varchar(50) not null,
    `CreateTime` datetime not null,
    `ModifyBy` varchar(50) null,
    `ModifyTime` datetime null,
    `Version` int not null,
    `Extension` text null,
    `CompNo` varchar(50) not null,
    `Name` varchar(50) not null,
    `Enabled` int not null,
    `Note` text null,
    PRIMARY KEY (`Id`)
);

create table `SysRoleModule` (
    `RoleId` varchar(50) not null,
    `ModuleId` varchar(50) not null,
    PRIMARY KEY (`RoleId`,`ModuleId`)
);

create table `SysUser` (
    `Id` varchar(50) not null,
    `CreateBy` varchar(50) not null,
    `CreateTime` datetime not null,
    `ModifyBy` varchar(50) null,
    `ModifyTime` datetime null,
    `Version` int not null,
    `Extension` text null,
    `CompNo` varchar(50) not null,
    `OrgNo` varchar(50) not null,
    `UserName` varchar(50) not null,
    `Password` varchar(50) not null,
    `Name` varchar(50) not null,
    `EnglishName` varchar(50) null,
    `Gender` varchar(50) null,
    `Phone` varchar(50) null,
    `Mobile` varchar(50) null,
    `Email` varchar(50) null,
    `Enabled` int not null,
    `Note` text null,
    `FirstLoginTime` datetime null,
    `FirstLoginIP` varchar(50) null,
    `LastLoginTime` datetime null,
    `LastLoginIP` varchar(50) null,
    PRIMARY KEY (`Id`)
);

insert into SysUser(Id,CreateBy,CreateTime,Version,CompNo,OrgNo,UserName,Password,Name,EnglishName,Enabled)
values('System','System',sysdate(),1,'known','known','System','c4ca4238a0b923820dcc509a6f75849b','超级管理员','System',1);
insert into SysUser(Id,CreateBy,CreateTime,Version,CompNo,OrgNo,UserName,Password,Name,EnglishName,Enabled)
values('101ffa5246714083967622761898ea6e','System',sysdate(),1,'known','known','admin','c4ca4238a0b923820dcc509a6f75849b','管理员','Administrator',1);

create table `SysUserModule` (
    `UserId` varchar(50) not null,
    `ModuleId` varchar(50) not null,
    PRIMARY KEY (`UserId`,`ModuleId`)
);

create table `SysUserRole` (
    `UserId` varchar(50) not null,
    `RoleId` varchar(50) not null,
    PRIMARY KEY (`UserId`,`RoleId`)
);