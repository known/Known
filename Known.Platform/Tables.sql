create table [dbo].[t_plt_applications]
(
	[id] varchar(50) not null primary key, 
	[create_by] varchar(50) not null, 
	[create_time] DATETIME not null, 
	[modify_by] varchar(50) null, 
	[modify_time] DATETIME null,
    [extension] varchar(max) null, 
	[name] varchar(50) not null,
    [description] varchar(500) null
)