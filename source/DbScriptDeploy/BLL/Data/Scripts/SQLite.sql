﻿CREATE TABLE IF NOT EXISTS Designation (
	Id UNIQUEIDENTIFIER PRIMARY KEY,
	Name text NOT NULL,
	CreateDate text NOT NULL
);

CREATE TABLE IF NOT EXISTS User (
	Id UNIQUEIDENTIFIER PRIMARY KEY,
	UserName text NOT NULL,
	Password text NOT NULL,
	PasswordExpired bit NOT NULL,
	CreateDate text NOT NULL
);

CREATE TABLE IF NOT EXISTS Project (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Name text NOT NULL,
	CreateDate text NOT NULL
);

CREATE TABLE IF NOT EXISTS UserClaim (
	Id UNIQUEIDENTIFIER PRIMARY KEY,
	UserId UNIQUEIDENTIFIER NOT NULL,
	Name text NOT NULL,
	ProjectId INTEGER NULL,
	FOREIGN KEY (UserId) REFERENCES User (Id),
	FOREIGN KEY (ProjectId) REFERENCES Project (Id)
);

CREATE TABLE IF NOT EXISTS Environment (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	ProjectId INTEGER NULL,
	Name text NOT NULL,
	DbType text NOT NULL,
	HostName text NOT NULL,
	Port numeric NOT NULL,
	UserName text NOT NULL,
	Password text NOT NULL,
	DisplayOrder numeric NOT NULL,
	DesignationId uniqueidentifier NOT NULL,
	FOREIGN KEY (ProjectId) REFERENCES Project (Id),
	FOREIGN KEY (DesignationId) REFERENCES Designation (Id)
);
