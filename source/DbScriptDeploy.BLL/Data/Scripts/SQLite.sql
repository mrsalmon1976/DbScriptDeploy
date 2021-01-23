-- Designation table stores the possible types of environment, e.g. Development, Staging, Production
-- Primary key is an integer which we seed at a random number
CREATE TABLE IF NOT EXISTS Designation (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Name text NOT NULL,
	CreateDate text NOT NULL
);
INSERT INTO sqlite_sequence (name, seq) 
	SELECT 'Designation', 10000 
	WHERE NOT EXISTS (SELECT 1 FROM sqlite_sequence WHERE name = 'Designation');

-- User table stores users - primary key has to be text to store Guid values (makes it easier working 
-- with Nancy)
CREATE TABLE IF NOT EXISTS User (
	Id UNIQUEIDENTIFIER PRIMARY KEY,
	UserName text NOT NULL,
	Password text NOT NULL,
	PasswordExpired bit NOT NULL,
	CreateDate text NOT NULL
);

-- The Project table stores projects, not suprisingly
CREATE TABLE IF NOT EXISTS Project (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Name text NOT NULL,
	CreateDate text NOT NULL
);
INSERT INTO sqlite_sequence (name, seq) 
	SELECT 'Project', 20000 
	WHERE NOT EXISTS (SELECT 1 FROM sqlite_sequence WHERE name = 'Project');

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
	Host text NOT NULL,
	Port numeric NOT NULL,
	Database text NOT NULL,
	UserName text NOT NULL,
	Password text NOT NULL,
	DisplayOrder numeric NOT NULL,
	DesignationId INTEGER NOT NULL,
	CreateDate text NOT NULL,
	FOREIGN KEY (ProjectId) REFERENCES Project (Id),
	FOREIGN KEY (DesignationId) REFERENCES Designation (Id)
);
INSERT INTO sqlite_sequence (name, seq) 
	SELECT 'Environment', 30000
	WHERE NOT EXISTS (SELECT 1 FROM sqlite_sequence WHERE name = 'Environment');

CREATE TABLE IF NOT EXISTS Script (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	ProjectId INTEGER NULL,
	Name text NOT NULL,
	ScriptUp text NOT NULL,
	ScriptDown text NULL,
	CreateDate text NOT NULL,
	FOREIGN KEY (ProjectId) REFERENCES Project (Id)
);
INSERT INTO sqlite_sequence (name, seq) 
	SELECT 'Script', 40000
	WHERE NOT EXISTS (SELECT 1 FROM sqlite_sequence WHERE name = 'Script');

CREATE TABLE IF NOT EXISTS ScriptTag (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	ScriptId INTEGER NULL,
	Tag text NOT NULL,
	CreateDate text NOT NULL,
	FOREIGN KEY (ScriptId) REFERENCES Script (Id)
);
INSERT INTO sqlite_sequence (name, seq) 
	SELECT 'ScriptTag', 50000
	WHERE NOT EXISTS (SELECT 1 FROM sqlite_sequence WHERE name = 'ScriptTag');
