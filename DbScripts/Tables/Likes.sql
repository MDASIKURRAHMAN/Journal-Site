use eJournalDb;

--Likes Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Likes' and xtype='U')
CREATE TABLE "Likes"(
    "LikeId" BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    "UserId" BIGINT NOT NULL,
    "CommentId" BIGINT NULL,
    "BlogId" BIGINT NULL
);