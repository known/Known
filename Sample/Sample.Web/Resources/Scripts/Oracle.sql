create table TbApply (
    Id          varchar2(50)   not null,
    CreateBy    varchar2(50)   not null,
    CreateTime  date           not null,
    ModifyBy    varchar2(50)   null,
    ModifyTime  date           null,
    Version     number(8)      not null,
    Extension   varchar2(4000) null,
    AppId       varchar2(50)   null,
    CompNo      varchar2(50)   not null,
    BizType     varchar2(50)   not null,
    BizNo       varchar2(50)   not null,
    BizTitle    varchar2(100)  not null,
    BizContent  varchar2(4000) null,
    BizFile     varchar2(250)  null
);
alter table TbApply add constraint PK_TbApply primary key (Id);

create table TbApplyList (
    Id         varchar2(50)   not null,
    CreateBy   varchar2(50)   not null,
    CreateTime date           not null,
    ModifyBy   varchar2(50)   null,
    ModifyTime date           null,
    Version    number(8)      not null,
    Extension  varchar2(4000) null,
    AppId      varchar2(50)   not null,
    CompNo     varchar2(50)   not null,
    HeadId     varchar2(50)   not null,
    Item       varchar2(100)  null,
    Note       varchar2(4000) null
);
alter table TbApplyList add constraint PK_TbApplyList primary key (Id);