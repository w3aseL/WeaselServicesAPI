CREATE TABLE [dbo].[BlogPost] (
    [BlogId]          INT            IDENTITY (1, 1) NOT NULL,
    [BlogTitle]       VARCHAR (1024) COLLATE Latin1_General_BIN2  ENCRYPTED WITH (
     COLUMN_ENCRYPTION_KEY = [CMKBlogKey],
     ALGORITHM = N'AEAD_AES_256_CBC_HMAC_SHA_256',
     ENCRYPTION_TYPE = DETERMINISTIC
    ) NULL,
    [BlogDescription] VARCHAR (MAX)  COLLATE Latin1_General_BIN2  ENCRYPTED WITH (
     COLUMN_ENCRYPTION_KEY = [CMKBlogKey],
     ALGORITHM = N'AEAD_AES_256_CBC_HMAC_SHA_256',
     ENCRYPTION_TYPE = DETERMINISTIC
    ) NULL,
    [BlogContent]     VARCHAR (MAX)  COLLATE Latin1_General_BIN2  ENCRYPTED WITH (
     COLUMN_ENCRYPTION_KEY = [CMKBlogKey],
     ALGORITHM = N'AEAD_AES_256_CBC_HMAC_SHA_256',
     ENCRYPTION_TYPE = DETERMINISTIC
    ) NULL,
    [DateCreated]     DATETIME       NOT NULL,
    [LastModified]    DATETIME       NULL,
    [CategoryId]      INT            NULL,
    [AuthorId]       INT             NULL,

    PRIMARY KEY CLUSTERED ([BlogId] ASC),
    CONSTRAINT [FK_Post_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[BlogCategory] ([CategoryId]),
    CONSTRAINT [FK_Post_Author] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[BlogAuthor] ([Id])
)
GO

CREATE INDEX IDX_Post_Category ON [BlogPost]([CategoryId])
GO

CREATE INDEX IDX_Post_Author ON [BlogPost]([AuthorId])
GO
