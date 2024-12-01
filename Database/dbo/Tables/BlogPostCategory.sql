CREATE TABLE [dbo].[BlogPostCategory]
(
	[BlogPostId] INT NOT NULL,
	[BlogCategoryId] INT NOT NULL,

	CONSTRAINT [PK_BlogPostCategory] PRIMARY KEY ([BlogPostId], [BlogCategoryId]),
	CONSTRAINT [FK_BlogPostCategory_BlogPost] FOREIGN KEY ([BlogPostId]) REFERENCES [BlogPost] ([BlogPostId]),
	CONSTRAINT [FK_BlogPostCategory_BlogCategory] FOREIGN KEY ([BlogCategoryId]) REFERENCES [BlogCategory] ([CategoryId])
)
GO