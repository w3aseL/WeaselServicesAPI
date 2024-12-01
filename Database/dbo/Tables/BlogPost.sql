CREATE TABLE [dbo].[BlogPost] (
    [BlogPostId]          INT            IDENTITY (1, 1) NOT NULL,
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
    [AuthorId]        INT            NULL,
    [IsPublished]     BIT            NOT NULL   DEFAULT (0),
    [DatePublished]   DATETIME       NULL,

    CONSTRAINT [PK_BlogPost] PRIMARY KEY CLUSTERED ([BlogPostId] ASC),
    CONSTRAINT [FK_BlogPost_Author] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[BlogAuthor] ([Id])
)
GO

CREATE INDEX IDX_BlogPost_Author ON [BlogPost]([AuthorId])
GO
