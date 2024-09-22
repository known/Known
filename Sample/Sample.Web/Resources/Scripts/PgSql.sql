CREATE TABLE TbApply (
    Id          character varying(50)  NOT NULL,
    CreateBy    character varying(50)  NOT NULL,
    CreateTime  timestamp without time zone NOT NULL,
    ModifyBy    character varying(50),
    ModifyTime  timestamp without time zone,
    Version     integer                NOT NULL,
    Extension   text,
    AppId       character varying(50),
    CompNo      character varying(50)  NOT NULL,
    BizType     character varying(50)  NOT NULL,
    BizNo       character varying(50)  NOT NULL,
    BizTitle    character varying(100) NOT NULL,
    BizContent  text,
    BizFile     character varying(250),
    PRIMARY KEY (Id)
);

CREATE TABLE TbApplyList (
    Id         character varying(50)  NOT NULL,
    CreateBy   character varying(50)  NOT NULL,
    CreateTime timestamp without time zone NOT NULL,
    ModifyBy   character varying(50),
    ModifyTime timestamp without time zone,
    Version    integer                NOT NULL,
    Extension  text,
    AppId      character varying(50)  NOT NULL,
    CompNo     character varying(50)  NOT NULL,
    HeadId     character varying(50)  NOT NULL,
    Item       character varying(100),
    Note       text,
    PRIMARY KEY (Id)
);