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
	[version] varchar(50) not null,
    [description] nvarchar(500) null
);

insert into t_plt_applications(id,create_by,create_time,code,name,version)
values('kms','admin',getdate(),'KMS','Known管理系统','1.0');

create table [dbo].[t_plt_users]
(
	[id] varchar(50) not null primary key,
	[create_by] varchar(50) not null,
	[create_time] datetime not null,
	[modify_by] varchar(50) null,
	[modify_time] datetime null,
    [extension] nvarchar(max) null,
	[app_id] varchar(50) not null,
	[company_id] varchar(50) not null,
	[department_id] varchar(50) not null,
	[user_name] varchar(50) not null,
	[password] varchar(50) not null,
	[name] nvarchar(50) null,
    [email] varchar(50) null,
	[mobile] varchar(50) null,
	[phone] varchar(50) null,
	[token] varchar(50) null,
	[first_login_time] datetime null,
	[last_login_time] datetime null,
	[settings_data] nvarchar(max) null
);

insert into t_plt_users(id,create_by,create_time,app_id,company_id,department_id,user_name,password,name)
values('485d3b1c4cfb4597b03df31bc934aad5','admin',getdate(),'kms','0','0','admin','c4ca4238a0b923820dcc509a6f75849b','管理员');

create table [dbo].[t_plt_modules]
(
	[id] varchar(50) not null primary key,
	[create_by] varchar(50) not null,
	[create_time] datetime not null,
	[modify_by] varchar(50) null,
	[modify_time] datetime null,
    [extension] nvarchar(max) null,
	[app_id] varchar(50) not null,
	[parent_id] varchar(50) not null,
	[code] varchar(50) not null,
	[name] nvarchar(50) not null,
    [description] nvarchar(500) null,
	[view_type] int not null,
	[url] varchar(200) null,
	[icon] varchar(50) not null,
	[sort] int not null,
	[enabled] int not null default 1,
	[button_data] nvarchar(max) null,
	[field_data] nvarchar(max) null
);

insert into t_plt_modules(id,create_by,create_time,app_id,parent_id,code,name,view_type,icon,sort,enabled) 
values('7d2121622047444794a88912225293cb','admin',getdate(),'kms','0','System','系统管理',0,'fa-desktop',1,1);
insert into t_plt_modules(id,create_by,create_time,app_id,parent_id,code,name,view_type,icon,sort,enabled) 
values('0e11a49bd678410f972bfa5fab5694ab','admin',getdate(),'kms','7d2121622047444794a88912225293cb','Module','模块管理',4,'fa-cubes',1,1);