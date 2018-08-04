create table [dbo].[t_plt_applications]
(
	[id] varchar(50) not null primary key,
	[create_by] varchar(50) not null,
	[create_time] datetime not null,
	[modify_by] varchar(50) null,
	[modify_time] datetime null,
    [extension] varchar(max) null,
	[code] varchar(50) not null,
	[name] varchar(50) not null,
    [description] varchar(500) null
);

create table [dbo].[t_plt_modules]
(
	[id] varchar(50) not null primary key,
	[create_by] varchar(50) not null,
	[create_time] datetime not null,
	[modify_by] varchar(50) null,
	[modify_time] datetime null,
    [extension] varchar(max) null,
	[parent_id] varchar(50) not null,
	[code] varchar(50) not null,
	[name] varchar(50) not null,
    [description] varchar(500) null,
	[view_type] int not null,
	[url] varchar(200) null,
	[icon] varchar(50) not null,
	[sort] int not null,
	[enabled] int not null default 1,
	[button_json] varchar(max) null,
	[field_json] varchar(max) null
);