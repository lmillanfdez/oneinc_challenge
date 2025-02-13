CREATE TABLE [Challenge].[Users]
(
	[Id] uniqueidentifier primary key,
	[FirstName] nvarchar(128) not null,
	[LastName] nvarchar(128) null,
	[Email] nvarchar(450) unique not null,
	[DateOfBirth] datetime not null,
	[PhoneNumber] bigint not null
)