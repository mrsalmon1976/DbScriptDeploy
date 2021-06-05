-- Identity table scripts: https://gist.github.com/akatakritos/96b0c3136f8498246fa810d393927f04


CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	ClaimType text NULL,
	ClaimValue text NULL,
	RoleId text NOT NULL
);
CREATE TABLE IF NOT EXISTS AspNetRoles (
	Id text PRIMARY KEY NOT NULL,
	ConcurrencyStamp text NULL,
	Name text NULL,
	NormalizedName text NULL
);
CREATE TABLE IF NOT EXISTS AspNetUserClaims (
	Id text PRIMARY KEY NOT NULL,
	ClaimType text NULL,
	ClaimValue text NULL,
	UserId text NOT NULL
);
CREATE TABLE IF NOT EXISTS AspNetUserLogins (
	LoginProvider text NOT NULL,
	ProviderKey text NOT NULL,
	ProviderDisplayName text NULL,
	UserId text NOT NULL,
	PRIMARY KEY (LoginProvider, ProviderKey)
);

CREATE TABLE IF NOT EXISTS AspNetUserRoles (
	UserId text NOT NULL,
	RoleId text NOT NULL,
	PRIMARY KEY (UserId, RoleId)
);
-- Identity Users
CREATE TABLE IF NOT EXISTS AspNetUsers (
	Id text PRIMARY KEY,
	AccessFailedCount INTEGER NOT NULL,
	ConcurrencyStamp text NULL,
	Email text NULL,
	EmailConfirmed bit NOT NULL,
	LockoutEnabled bit NOT NULL,
	LockoutEnd text NULL,
	NormalizedEmail text NULL,
	NormalizedUserName text NULL,
	PasswordHash text NULL,
	PhoneNumber text NULL,
	PhoneNumberConfirmed bit NOT NULL,
	SecurityStamp text NULL,
	TwoFactorEnabled bit NOT NULL,
	UserName text NULL
);
CREATE TABLE IF NOT EXISTS AspNetUserTokens (
	UserId text NOT NULL,
	LoginProvider text NOT NULL,
	Name text NOT NULL,
	Value text NULL,
	PRIMARY KEY (UserId, LoginProvider, Name)
);
