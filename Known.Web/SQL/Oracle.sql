create table SysLog (
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    int            not null,
    Extension  varchar2(4000) null,
    CompNo     varchar2(50)   not null,
    Type       varchar2(50)   null,
    Target     varchar2(50)   null,
    Content    varchar2(4000) null
);
alter table SysLog add constraint PK_SysLog primary key (Id);

create table SysDictionary (
    Id varchar2(50) not null,
    CreateBy varchar2(50) not null,
    CreateTime date not null,
    ModifyBy varchar2(50) null,
    ModifyTime date null,
    Version int not null,
    Extension varchar2(4000) null,
    CompNo varchar2(50) not null,
    Category varchar2(50) null,
    CategoryName varchar2(50) null,
    Code varchar2(50) null,
    Name varchar2(50) null,
    Sort int not null,
    Enabled int not null,
    Note varchar2(4000) null
);
alter table SysDictionary add constraint PK_SysDictionary primary key (Id);

create table SysOrganization (
    Id varchar2(50) not null,
    CreateBy varchar2(50) not null,
    CreateTime date not null,
    ModifyBy varchar2(50) null,
    ModifyTime date null,
    Version int not null,
    Extension varchar2(4000) null,
    CompNo varchar2(50) not null,
    ParentId varchar2(50) null,
    Code varchar2(50) null,
    Name varchar2(50) not null,
    ManagerId varchar2(50) null,
    Note varchar2(4000) null
);
alter table SysOrganization add constraint PK_SysOrganization primary key (Id);

create table SysRole (
    Id varchar2(50) not null,
    CreateBy varchar2(50) not null,
    CreateTime date not null,
    ModifyBy varchar2(50) null,
    ModifyTime date null,
    Version int not null,
    Extension varchar2(4000) null,
    CompNo varchar2(50) not null,
    Name varchar2(50) not null,
    Enabled int not null,
    Note varchar2(4000) null
);
alter table SysRole add constraint PK_SysRole primary key (Id);

create table SysRoleModule (
    RoleId varchar2(50) not null,
    ModuleId varchar2(50) not null
);
alter table SysRoleModule add constraint PK_SysRoleModule primary key (RoleId,ModuleId);

create table SysUser (
    Id varchar2(50) not null,
    CreateBy varchar2(50) not null,
    CreateTime date not null,
    ModifyBy varchar2(50) null,
    ModifyTime date null,
    Version int not null,
    Extension varchar2(4000) null,
    CompNo varchar2(50) not null,
    OrgNo varchar2(50) not null,
    UserName varchar2(50) not null,
    Password varchar2(50) not null,
    Name varchar2(50) not null,
    EnglishName varchar2(50) null,
    Gender varchar2(50) null,
    Phone varchar2(50) null,
    Mobile varchar2(50) null,
    Email varchar2(50) null,
    Enabled int not null,
    Note varchar2(4000) null,
    FirstLoginTime date null,
    FirstLoginIP varchar2(50) null,
    LastLoginTime date null,
    LastLoginIP varchar2(50) null
);
alter table SysUser add constraint PK_SysUser primary key (Id);

insert into SysUser(Id,CreateBy,CreateTime,Version,CompNo,OrgNo,UserName,Password,Name,EnglishName,Enabled)
values('System','System',sysdate,1,'known','known','System','c4ca4238a0b923820dcc509a6f75849b','超级管理员','System',1);
insert into SysUser(Id,CreateBy,CreateTime,Version,CompNo,OrgNo,UserName,Password,Name,EnglishName,Enabled)
values('101ffa5246714083967622761898ea6e','System',sysdate,1,'known','known','admin','c4ca4238a0b923820dcc509a6f75849b','管理员','Administrator',1);

create table SysUserModule (
    UserId varchar2(50) not null,
    ModuleId varchar2(50) not null
);
alter table SysUserModule add constraint PK_SysUserModule primary key (UserId,ModuleId);

create table SysUserRole (
    UserId varchar2(50) not null,
    RoleId varchar2(50) not null
);
alter table SysUserRole add constraint PK_SysUserRole primary key (UserId,RoleId);