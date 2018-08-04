create table [dbo].[t_plt_applications]
(
	[id] varchar(50) not null primary key,
	[create_by] varchar(50) not null,
	[create_time] datetime not null,
	[modify_by] varchar(50) null,
	[modify_time] datetime null,
    [extension] nvarchar(max) null,
	[code] varchar(50) not null,
	[name] nvarchar(50) not null,
    [description] nvarchar(500) null
);

create table [dbo].[t_plt_modules]
(
	[id] varchar(50) not null primary key,
	[create_by] varchar(50) not null,
	[create_time] datetime not null,
	[modify_by] varchar(50) null,
	[modify_time] datetime null,
    [extension] nvarchar(max) null,
	[parent_id] varchar(50) not null,
	[code] varchar(50) not null,
	[name] nvarchar(50) not null,
    [description] nvarchar(500) null,
	[view_type] int not null,
	[url] varchar(200) null,
	[icon] varchar(50) not null,
	[sort] int not null,
	[enabled] int not null default 1,
	[button_json] nvarchar(max) null,
	[field_json] nvarchar(max) null
);

insert into t_plt_modules(id,create_by,create_time,parent_id,code,name,view_type,icon,sort,enabled) 
values('7d2121622047444794a88912225293cb','admin',getdate(),'0','System','系统管理',0,'fa-desktop',1,1);
insert into t_plt_modules(id,create_by,create_time,parent_id,code,name,view_type,icon,sort,enabled) 
values('0e11a49bd678410f972bfa5fab5694ab','admin',getdate(),'7d2121622047444794a88912225293cb','Module','模块管理',2,'fa-cubes',1,1);