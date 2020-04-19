CREATE TABLE IF NOT EXISTS Users (
	Id UNIQUEIDENTIFIER PRIMARY KEY,
	UserName text NOT NULL,
	Password text NOT NULL,
	Role text NULL
);

--CREATE TABLE IF NOT EXISTS Projects (
--	Id INTEGER PRIMARY KEY,
--	Name text NOT NULL,
--	CreateDate text NOT NULL
--);

